using BepInEx;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using SlugBase.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;

namespace TheEscort;

partial class Plugin : BaseUnityPlugin
{
    // Implement Bodyslam
    private void Escort_Collision(On.Player.orig_Collide orig, Player self, PhysicalObject otherObject, int myChunk, int otherChunk)
    {
        orig(self, otherObject, myChunk, otherChunk);
        try
        {
            if (Eshelp_IsMe(self.slugcatStats.name))
            {
                return;
            }
        }
        catch (Exception err)
        {
            Ebug(self, err);
            return;
        }
        if (
            !BodySlam.TryGet(self, out float[] bodySlam) ||
            !TrampOhLean.TryGet(self, out float bounce) ||
            !SlideLaunchMod.TryGet(self, out float[] slideMod) ||
            !brawlerSlideLaunchFac.TryGet(self, out float bSlideFac) ||
            !brawlerDKHypeDmg.TryGet(self, out float bDKHDmg) ||
            !deflectorSlideLaunchMod.TryGet(self, out float dSlideMod) ||
            !deflectorSlideDmg.TryGet(self, out float dSlideDmg) ||
            !deflectorSlideLaunchFac.TryGet(self, out float dSlideFac) ||
            !deflectorDKHypeDmg.TryGet(self, out float[] dDKHDmg) ||
            !escapistSlideLaunchMod.TryGet(self, out float eSlideMod) ||
            !escapistSlideLaunchFac.TryGet(self, out float eSlideFac) ||
            !escapistNoGrab.TryGet(self, out int[] esNoGrab) ||
            !Esconfig_HypeReq(self) ||
            !Esconfig_DKMulti(self) ||
            !eCon.TryGetValue(self, out Escort e)
            )
        {
            return;
        }

        //Ebug(self, "Collision Triggered!");
        try
        {
            if (e.consoleTick == 0 && false)
            {
                Ebug(self, "Escort collides!");
                Ebug(self, "Has physical object? " + otherObject != null);
                if (otherObject != null)
                {
                    Ebug(self, "What is it? " + otherObject.GetType());
                }
            }
        }
        catch (Exception err)
        {
            Ebug(self, err, "Error when printing collision stuff");
        }

        bool hypedMode = Esconfig_Hypable(self);

        // Reimplementing the elevator... the way it was in its glory days
        if (Esconfig_Elevator(self) && otherObject is Creature && self.animation == Player.AnimationIndex.None && self.bodyMode == Player.BodyModeIndex.Default && !(otherObject as Creature).dead)
        {
            self.jumpBoost += 4;
        }


        if (otherObject is Creature creature &&
            creature.abstractCreature.creatureTemplate.type != MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC && !(ModManager.CoopAvailable && otherObject is Player && !RWCustom.Custom.rainWorld.options.friendlyFire))
        {

            if (e.Escapist && self.aerobicLevel > 0.02f)
            {
                self.aerobicLevel -= 0.01f;
            }
            if (e.Speedster) Esclass_SS_Collision(self, creature, ref e);
            if (e.Gilded) Esclass_GD_Collision(self, creature, ref e);

            // Creature Trampoline (or if enabled Escort's Elevator)
            /*
            Creature Trampoline is not consistent and may get you killed if you try to take advantage of it. Thus the intended use is to bounce away from the creature when running by or away.
            */
            if ((self.animation == Player.AnimationIndex.Flip || (e.Escapist && self.animation == Player.AnimationIndex.None)) && self.bodyMode == Player.BodyModeIndex.Default && (!creature.dead || creature is Lizard))
            {
                self.slideCounter = 0;
                if (self.jumpBoost <= 0)
                {
                    self.jumpBoost = (self.animation == Player.AnimationIndex.None ? bounce * 1.5f : bounce);
                }
                if (e.Escapist && self.cantBeGrabbedCounter < esNoGrab[1] && self.animation == Player.AnimationIndex.Flip)
                {
                    self.cantBeGrabbedCounter += esNoGrab[0];
                }
                if (e.Deflector)
                {
                    try
                    {
                        if (self.mainBodyChunk.vel.y < 10f && self.bodyChunks[myChunk].pos.y > creature.bodyChunks[otherChunk].pos.y)
                        {
                            self.mainBodyChunk.vel.y = 16f;
                        }
                        e.DeflTrampoline = true;
                        self.Violence(null, null, self.mainBodyChunk, null, Creature.DamageType.Blunt, 0f, 0f);
                    }
                    catch (Exception err)
                    {
                        Ebug(self, err, "Hitting thyself failed!");
                    }
                }
            }

            int direction;

            // Stunslide/Parryslide (stun module)
            if (creature.abstractCreature.creatureTemplate.type != CreatureTemplate.Type.Fly && self.animation == Player.AnimationIndex.BellySlide)
            {
                try
                {

                    creature.SetKillTag(self.abstractCreature);

                    if (e.parrySlideLean <= 0)
                    {
                        e.parrySlideLean = 4;
                    }
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, e.SFXChunk);

                    float normSlideStun = hypedMode || e.Brawler ? bodySlam[1] * 1.5f : bodySlam[1];
                    if (hypedMode && self.aerobicLevel > hypeRequirement)
                    {
                        normSlideStun = bodySlam[1] * (e.Brawler ? 2f : 1.75f);
                    }
                    if (e.Gilded)
                    {
                        normSlideStun *= 0.75f;
                    }
                    float deflectorSlidingDamage = dSlideDmg;
                    if (e.DeflPowah == 2) deflectorSlidingDamage *= 1.4f;
                    if (e.DeflPowah == 3) deflectorSlidingDamage *= 2.4f;
                    creature.Violence(
                        self.mainBodyChunk, new Vector2(self.mainBodyChunk.vel.x / 4f, self.mainBodyChunk.vel.y / 4f),
                        creature.firstChunk, null, e.DeflAmpTimer > 0 ? Creature.DamageType.Stab : Creature.DamageType.Blunt,
                        e.DeflAmpTimer > 0 ? deflectorSlidingDamage : bodySlam[0], normSlideStun
                    );
                    /*
                    if (self.pickUpCandidate is Spear){  // Attempts to pickup spears (may pickup things higher in priority that are nearby)
                        self.PickupPressed();
                    }*/

                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 5f, 5.5f, 40f, new Color(0f, 0.35f, 1f, 0f)));

                    // Long belly slide slidestun. Usually caused by throwing a spear midair while hyped.
                    if (self.longBellySlide)
                    {
                        direction = self.rollDirection;
                        //self.WallJump(direction);
                        //Escort_FakeWallJump(self, direction, boostUp:slideMod[5], yankUp: slideMod[6], boostLR:slideMod[7]);
                        if (Esconfig_Spears(self))
                        {
                            float tossModifier = slideMod[0];
                            self.animation = Player.AnimationIndex.Roll;
                            if (e.Deflector)
                            {
                                tossModifier = dSlideMod;
                            }
                            else if (e.Escapist)
                            {
                                tossModifier = eSlideMod;
                                self.animation = Player.AnimationIndex.BellySlide;
                                self.longBellySlide = false;
                            }
                            //self.mainBodyChunk.vel.x *= tossModifier;
                            self.bodyChunks[1].vel.x = self.slideDirection * tossModifier - 1;
                            self.bodyChunks[0].vel.x = self.slideDirection * tossModifier;
                            Escort_FakeWallJump(self, boostUp:slideMod[4], yankUp:slideMod[5]);
                        }
                        else
                        {
                            self.animation = Player.AnimationIndex.BellySlide;
                            self.longBellySlide = false;
                        }
                        Ebug(self, "Greatdadstance stunslide!", 2);
                    }
                    else
                    {
                        direction = self.flipDirection;
                        if (e.Brawler)
                        {
                            self.mainBodyChunk.vel.x *= bSlideFac;
                        }
                        if (e.Deflector)
                        {
                            self.mainBodyChunk.vel *= dSlideFac;
                        }
                        if (e.Escapist)
                        {
                            self.mainBodyChunk.vel.y *= eSlideFac;
                        }
                        if (e.Gilded)
                        {
                            self.mainBodyChunk.vel.x *= 0.75f;
                        }
                        if (self.rocketJumpFromBellySlide)
                        {
                            Ebug(self, "No more no control!");
                            self.rocketJumpFromBellySlide = false;
                        }

                        self.WallJump(direction);
                        //Escort_FakeWallJump(self, boostUp: slideMod[4]);

                        //self.animation = Player.AnimationIndex.None;
                        self.jumpBoost += slideMod[3];
                        self.animation = Player.AnimationIndex.Flip;
                        Ebug(self, "Stunslided!", 2);
                    }
                }
                catch (Exception err)
                {
                    Ebug(self, err, "Error while Slidestunning!");
                }
            }

            // Dropkick
            else if (creature.abstractCreature.creatureTemplate.type != CreatureTemplate.Type.Fly && self.animation == Player.AnimationIndex.RocketJump)
            {
                try
                {

                    creature.SetKillTag(self.abstractCreature);

                    String message = e.easyKick ? "Easykicked!" : "Dropkicked!";

                    DKMultiplier *= creature.TotalMass * 0.66f;
                    float normSlamDamage = e.easyKick ? 0.001f : 0.05f;
                    if (e.DropKickCD == 0)
                    {
                        self.room.PlaySound(SoundID.Big_Needle_Worm_Impale_Terrain, self.mainBodyChunk, false, 1f, 0.65f);
                        normSlamDamage = hypedMode ? bodySlam[2] : bodySlam[2] + (e.Brawler ? 0.27f : 0.15f);
                        creature.LoseAllGrasps();
                        if (hypedMode && self.aerobicLevel > hypeRequirement) { normSlamDamage = bodySlam[2] * (e.Brawler ? bDKHDmg : 1.6f); }
                        if (e.Deflector)
                        {
                            normSlamDamage *= e.DeflDamageMult + e.DeflPerma;
                        }
                        if (e.Gilded)
                        {
                            normSlamDamage *= 0.75f;
                        }
                        message = "Powerdropkicked!";
                    }
                    else
                    {
                        self.room.PlaySound(SoundID.Big_Needle_Worm_Bounce_Terrain, self.mainBodyChunk, false, 1f, 0.9f);
                    }
                    Vector2 momentum = new(
                        self.mainBodyChunk.vel.x * DKMultiplier, 
                        self.mainBodyChunk.vel.y * DKMultiplier);
                    if (e.LizardDunk)
                    {
                        bool diffYDirection = Mathf.Sign(momentum.y) != Mathf.Sign(self.input[0].y) && self.input[0].y != 0;
                        if (e.isDefault)
                        {
                            momentum.y *= Mathf.Lerp(1, 0.15f, Mathf.Log(Mathf.Clamp(Mathf.Abs(momentum.y), 1, 20), 20)) * (diffYDirection? -1 : 1);
                            momentum.x = Mathf.Abs(momentum.y) * self.input[0].x;
                        }
                        else
                        {
                            momentum.y *= Mathf.Lerp(1, 0.15f, Mathf.Log(Mathf.Clamp(Mathf.Abs(momentum.y), 1, 15), 15));
                        }
                    }
                    if (e.Gilded)
                    {
                        momentum *= 0.5f;
                    }
                    creature.Violence(
                        self.mainBodyChunk, momentum,
                        creature.firstChunk, null, Creature.DamageType.Blunt,
                        normSlamDamage, (e.DeflAmpTimer > 0 ? bodySlam[1] : bodySlam[3])
                    );
                    Ebug(self, $"Dunk the lizard: {e.LizardDunk} at {momentum.x:###0.000}|{momentum.y:###0.000}", 2);
                    if (e.DropKickCD == 0)
                    {
                        e.LizardDunk = false;
                    }
                    if (e.DeflAmpTimer > 0)
                    {
                        e.DeflAmpTimer = 0;
                    }
                    e.DropKickCD = (e.easyKick ? 40 : 15);
                    //self.mainBodyChunk.vel = new Vector2((float) self.flipDirection * 24f, 14f) * num;
                    /*
                    if (self.pickUpCandidate is Spear){
                        self.PickupPressed();
                    }*/
                    direction = -self.flipDirection;
                    e.kickFlip = true;
                    self.WallJump(direction);
                    e.kickFlip = false;
                    self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 7f, 8f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    //self.animation = Player.AnimationIndex.None;
                    Ebug(self, message + " Dmg: " + normSlamDamage, 2);
                }
                catch (Exception err)
                {
                    Ebug(self, err, "Error when dropkicking!");
                }
                e.savingThrowed = false;
            }

            // Headbutt
            else if (e.CometFrames > 0 && !e.Cometted)
            {
                try
                {
                    creature.SetKillTag(self.abstractCreature);
                    creature.Violence(
                        self.bodyChunks[0], new Vector2(self.bodyChunks[0].vel.x * (DKMultiplier * 0.5f) * creature.TotalMass, self.bodyChunks[0].vel.y * (DKMultiplier * 0.5f) * creature.TotalMass),
                        creature.mainBodyChunk, null, Creature.DamageType.Blunt,
                        creature.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Fly? 1f: 0f, 15f
                    );
                    creature.firstChunk.vel.x = self.bodyChunks[0].vel.x * (DKMultiplier * 0.5f) * creature.TotalMass;
                    creature.firstChunk.vel.y = self.bodyChunks[0].vel.y * (DKMultiplier * 0.5f) * creature.TotalMass;
                    if (self != null && self.room != null)
                    {
                        self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2(0f, -self.bodyChunks[1].rad), 8, 7f, 7f, 8f, 40f, new Color(0f, 0.35f, 1f, 0f)));
                    }
                    Ebug(self, "Headbutted!", 2);
                    if (self.room != null)
                    {
                        if (Esconfig_SFX(self))
                        {
                            self.room.PlaySound(Escort_SFX_Boop, e.SFXChunk);
                        }
                        self.room.PlaySound(SoundID.Slugcat_Floor_Impact_Standard, e.SFXChunk, false, 0.75f, 1.3f);
                    }
                    e.Cometted = true;
                }
                catch (Exception err)
                {
                    Ebug(self, err, "Error when headbutting!");
                }
            }

        }
    }


#region Thrown

    private void Escort_RockThrow(On.Rock.orig_Thrown orig, Rock self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, RWCustom.IntVector2 throwDir, float frc, bool eu)
    {
        try
        {
            if (thrownBy == null)
            {
                orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                return;
            }
            if (thrownBy is Player p)
            {
                if (p.slugcatStats.name.value != "EscortMe")
                {
                    orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                    return;
                }
                if (!eCon.TryGetValue(p, out Escort e))
                {
                    orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
                    return;
                }
                if (e.Brawler)
                {
                    if (e.BrawPunch)
                    {
                        frc *= 0.25f;
                        Esclass_BL_RockThrow(self, p);
                    }
                }
                if (e.Escapist)
                {
                    frc *= 0.75f;
                }
                if (e.Railgunner) Esclass_RG_RockThrow(self, p, ref frc, ref e);
            }
            orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        }
        catch (Exception err)
        {
            Ebug(self.thrownBy as Player, err, "Error in Rockthrow!");
            orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
            return;
        }

    }



    // Implement unique spearskill
    private void Escort_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
    {
        orig(self, spear);
        try
        {
            if (Eshelp_IsMe(self.slugcatStats.name))
            {
                return;
            }
        }
        catch (Exception err)
        {
            Ebug(self, err);
            return;
        }
        if (
            !bonusSpear.TryGet(self, out float[] spearDmgBonuses) ||
            !Esconfig_HypeReq(self) ||
            !eCon.TryGetValue(self, out Escort e)
            )
        {
            return;
        }

        Ebug(self, "ThrownSpear Triggered!");
        float thrust = 7f;
        bool onPole = (self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam || self.bodyMode == Player.BodyModeIndex.ClimbIntoShortCut);
        bool doNotYeet = onPole || !Esconfig_Spears(self) || e.RailDoubleSpear;
        try
        {
            if (self.slugcatStats.throwingSkill == 0 && !e.Speedster)
            {
                spear.spearDamageBonus = Mathf.Max(1, spear.spearDamageBonus);
            }
            if (Esconfig_Hypable(self))
            {
                if (self.aerobicLevel > hypeRequirement)
                {
                    spear.throwModeFrames = -1;
                    if (!e.Railgunner)
                    {
                        spear.spearDamageBonus *= spearDmgBonuses[0];
                    }
                    if (self.canJump != 0 && !self.longBellySlide)
                    {
                        if (!doNotYeet)
                        {
                            self.rollCounter = 0;
                            if (self.input[0].jmp && self.input[0].thrw)
                            {
                                self.animation = Player.AnimationIndex.BellySlide;
                                e.slideFromSpear = true;
                                self.whiplashJump = true;
                                spear.firstChunk.vel.x *= 1.7f;
                                Ebug(self, "Spear Go!?", 2);
                            }
                            else
                            {
                                self.animation = Player.AnimationIndex.Roll;
                                self.standing = false;
                            }
                        }
                        thrust = 12f;
                    }
                    else
                    {
                        self.longBellySlide = true;
                        if (!doNotYeet)
                        {
                            self.exitBellySlideCounter = 0;
                            self.rollCounter = 0;
                            self.flipFromSlide = true;
                            self.animation = Player.AnimationIndex.BellySlide;
                            e.slideFromSpear = true;
                        }
                        thrust = 9f;
                    }
                }
                else
                {
                    if (!doNotYeet)
                    {
                        self.rollCounter = 0;
                        if (self.canJump != 0)
                        {
                            self.whiplashJump = true;
                            if (self.animation != Player.AnimationIndex.BellySlide && self.input[0].x != 0)
                            {
                                self.animation = Player.AnimationIndex.BellySlide;
                                e.slideFromSpear = true;
                            }
                            if (self.input[0].jmp && self.input[0].thrw)
                            {
                                spear.firstChunk.vel.x *= 1.6f;
                                Ebug(self, "Spear Go!", 2);
                            }
                        }
                        else
                        {
                            self.animation = Player.AnimationIndex.Flip;
                            self.standing = false;
                        }
                    }
                    spear.spearDamageBonus *= spearDmgBonuses[1];
                    thrust = 5f;
                }
            }
            else
            {
                spear.spearDamageBonus *= 1.25f;
            }
        }
        catch (Exception err)
        {
            Ebug(self, err, "Error while setting additional spear effects!");
        }
        if (e.Brawler) Esclass_BL_ThrownSpear(self, spear, ref e, ref thrust);
        if (e.Escapist) Esclass_EC_ThrownSpear(self, spear);
        if (e.Railgunner) Esclass_RG_ThrownSpear(self, spear, onPole, ref e, ref thrust);
        if (onPole && !e.Railgunner || self.bodyMode == Player.BodyModeIndex.Crawl)
        {
            thrust = 1f;
        }

        try
        {
            BodyChunk firstChunker = self.firstChunk;
            if ((self.room != null && self.room.gravity == 0f) || (Mathf.Abs(spear.firstChunk.vel.x) < 1f && Mathf.Abs(spear.firstChunk.vel.y) < 1f))
            {
                self.firstChunk.vel += spear.firstChunk.vel.normalized * Math.Abs(thrust);
            }
            else
            {
                if (Esconfig_Spears(self))
                {
                    self.rollDirection = (int)Mathf.Sign(spear.firstChunk.vel.x);
                }
                if (self.bodyMode == Player.BodyModeIndex.CorridorClimb)
                {
                    spear.throwDir = new IntVector2((int)(self.mainBodyChunk.Rotation.x * 2), (int)(self.mainBodyChunk.Rotation.y * 2));
                    if (spear.throwDir.y != 0)
                    {
                        spear.firstChunk.vel.y = Mathf.Abs(spear.firstChunk.vel.x) * spear.throwDir.y;
                        spear.firstChunk.vel.x = 0;
                        thrust *= 0.25f;
                    }
                    else
                    {
                        thrust *= 0.5f;
                    }
                }
                if (self.animation != Player.AnimationIndex.BellySlide)
                {
                    if (spear.throwDir.x == 0)
                    {
                        if (e.Railgunner)
                        {
                            if (spear.throwDir.y == 1)
                            {
                                self.firstChunk.vel.y += spear.firstChunk.vel.normalized.y * thrust * 0.4f;
                            }
                            else if (spear.throwDir.y == -1)
                            {
                                self.firstChunk.vel.y += spear.firstChunk.vel.normalized.y * thrust * 0.65f;
                            }
                            else
                            {
                                self.firstChunk.vel += spear.firstChunk.vel.normalized * thrust;
                            }
                        }
                        else
                        {
                            self.firstChunk.vel.y = firstChunker.vel.y + Mathf.Sign(spear.firstChunk.vel.y) * thrust;
                        }
                    }
                    else
                    {
                        self.firstChunk.vel.x = firstChunker.vel.x + Mathf.Sign(spear.firstChunk.vel.x) * thrust;
                    }
                }
            }
        }
        catch (Exception err)
        {
            Ebug(self, err, "Error while adjusting the player thrust");
        }
        Ebug(self, "Speartoss! Velocity [X,Y]: [" + spear.firstChunk.vel.x + "," + spear.firstChunk.vel.y + "] Damage: " + spear.spearDamageBonus, 2);
    }



#endregion



#region Hit
    private void Escort_BulletHit(ILContext il)
    {
        var c = new ILCursor(il);
        try
        {
            c.GotoNext(MoveType.After,
                i => i.MatchCallOrCallvirt<Creature>(nameof(Creature.Violence))
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Bullethit IL match 1 failed");
            throw new Exception("IL Match 1 Failed", ex);
        }
        try
        {
            c.GotoPrev(MoveType.After,
                i => i.MatchLdloc(1)
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Bullethit IL match 2 failed");
            throw new Exception("IL Match 2 Failed", ex);
        }
        Ebug("Bullethit Identified point of interest", 0, true);
        try
        {
            c.Emit(OpCodes.Ldarg, 0);
            c.Emit(OpCodes.Ldarg, 1);
        }
        catch (Exception ex)
        {
            Ebug(ex, "Bullethit IL emit failed");
            throw new Exception("IL Emit Failed", ex);
        }

        try
        {
            c.EmitDelegate(
                (float original, Bullet self, SharedPhysics.CollisionResult result) => {
                    if (self.thrownBy is Player player && result.obj is Creature creature && Eshelp_IsMe(player.slugcatStats.name, false) && eCon.TryGetValue(player, out Escort e))
                    {
                        if (e.Deflector && !creature.dead)
                        {
                            original *= e.DeflDamageMult + e.DeflPerma;
                            if (e.DeflPowah == 3) Esclass_DF_UltimatePower(player);
                            e.DeflPowah = 0;
                            Ebug(player, $"Death upon thee! Sponsored by Raid, Shadow Legs. Damage: {original}", 3, true);
                        }

                        if (e.NewEscapist && e.NEsVulnerable.Contains(creature))
                        {
                            original *= 5;
                            Ebug(player, $"Hurt that modafoka! Damage: {original}", 3, true);
                        }
                    }
                    return original;
                }
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Bullethit IL emite delegate failed");
            throw new Exception("IL EmitDelegate Failed", ex);
        }
    }


    private void Escort_LillyHit(ILContext il)
    {
        var c = new ILCursor(il);
        try
        {
            c.GotoNext(MoveType.After,
                i => i.MatchCallOrCallvirt<Creature>(nameof(Creature.Violence))
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Lillyhit IL match 1 failed");
            throw new Exception("IL Match 1 Failed", ex);
        }
        try
        {
            c.GotoPrev(MoveType.After,
                i => i.MatchLdfld<LillyPuck>(nameof(LillyPuck.spearDamageBonus))
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Lillyhit IL match 2 failed");
            throw new Exception("IL Match 2 Failed", ex);
        }
        Ebug("Lillyhit Identified point of interest", 0, true);
        try
        {
            c.Emit(OpCodes.Ldarg, 0);
            c.Emit(OpCodes.Ldarg, 1);
        }
        catch (Exception ex)
        {
            Ebug(ex, "Lillyhit IL emit failed");
            throw new Exception("IL Emit Failed", ex);
        }

        try
        {
            c.EmitDelegate(
                (float original, LillyPuck self, SharedPhysics.CollisionResult result) => {
                    if (self.thrownBy is Player player && result.obj is Creature creature && Eshelp_IsMe(player.slugcatStats.name, false) && eCon.TryGetValue(player, out Escort e))
                    {
                        if (e.Deflector && !creature.dead)
                        {
                            original *= e.DeflDamageMult + e.DeflPerma;
                            if (e.DeflPowah == 3) Esclass_DF_UltimatePower(player);
                            e.DeflPowah = 0;
                            Ebug(player, $"Death upon thee! Sponsored by Lillypuck. Damage: {original}", ignoreRepetition: true);
                        }

                        if (e.NewEscapist && e.NEsVulnerable.Contains(creature))
                        {
                            original *= 2.5f;
                            Ebug(player, $"Escapists hits harder! Damage: {original}", ignoreRepetition: true);
                            e.NEsResetCooldown = true;
                        }
                    }
                    return original;
                }
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Lillyhit IL emite delegate failed");
            throw new Exception("IL EmitDelegate Failed", ex);
        }
    }



    // Implement rock throws
    private bool Escort_RockHit(On.Rock.orig_HitSomething orig, Rock self, SharedPhysics.CollisionResult result, bool eu)
    {
        try
        {
            if (self.thrownBy is Player p)
            {
                if (Eshelp_IsMe(p.slugcatStats.name))
                {
                    return orig(self, result, eu);
                }
                if (!eCon.TryGetValue(p, out Escort e))
                {
                    return orig(self, result, eu);
                }
                //self.canBeHitByWeapons = true;
                if (result.obj == null)
                {
                    return false;
                }
                self.vibrate = 20;
                self.ChangeMode(Weapon.Mode.Free);
                bool issaPunch = false;
                if (result.obj is Creature c)
                {
                    float stunBonus = 60f;
                    if (ModManager.MMF && MoreSlugcats.MMF.cfgIncreaseStuns.Value && (c is Cicada || c is LanternMouse || (ModManager.MSC && c is MoreSlugcats.Yeek)))
                    {
                        stunBonus = 105f;
                    }
                    if (ModManager.MSC && self.room.game.IsArenaSession && self.room.game.GetArenaGameSession.chMeta != null)
                    {
                        stunBonus = 105f;
                    }
                    if (e.Brawler && e.BrawPunch && !(
                        c.dead || c.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Fly || c.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC || (ModManager.CoopAvailable && c is Player && !RWCustom.Custom.rainWorld.options.friendlyFire))
                    )
                    {
                        c.Violence(self.firstChunk, self.firstChunk.vel, result.chunk, result.onAppendagePos, Creature.DamageType.Blunt, 0.2f, 30f);
                        if (ModManager.MSC && c is Player pl)
                        {
                            pl.playerState.permanentDamageTracking += 0.2 / pl.Template.baseDamageResistance;
                            if (pl.playerState.permanentDamageTracking >= 1)
                            {
                                pl.Die();
                            }
                        }
                        if (self.room != null)
                        {
                            self.room.PlaySound(Escort_SFX_Impact, self.firstChunk, false, 0.85f, 1.4f);
                            self.room.PlaySound(SoundID.Rock_Hit_Creature, self.firstChunk, false, 0.7f, 0.9f);
                        }
                        self.vibrate = 0;
                        e.BrawThrowGrab = 0;
                        issaPunch = true;
                    }
                    else
                    {
                        float baseDamage = 0.02f;
                        if (e.Brawler)
                        {
                            stunBonus *= 1.5f;
                        }
                        if (e.Deflector && !c.dead)
                        {
                            baseDamage *= e.DeflDamageMult + e.DeflPerma;
                            if (e.DeflPowah == 3) Esclass_DF_UltimatePower(p);
                            e.DeflPowah = 0;
                        }
                        if (e.Escapist)
                        {
                            baseDamage = 0.1f;
                        }
                        if (e.Railgunner)
                        {
                            baseDamage = 0.2f;
                            if (e.RailDoubleRock)
                            {
                                baseDamage = 0.25f;
                            }
                        }
                        if (e.NewEscapist && e.NEsVulnerable.Contains(c))
                        {
                            baseDamage = 0.25f;
                            stunBonus *= 5;
                            if (c?.Template is not null && c.Template.baseStunResistance > 1)
                            {
                                stunBonus *= c.Template.baseStunResistance;
                            }
                            e.NEsResetCooldown = true;
                            Ebug(p, $"Hit a vulnerable with rock! Damage: {baseDamage}, Stun bonus: {stunBonus}", ignoreRepetition: true);
                        }
                        c.Violence(self.firstChunk, self.firstChunk.vel * (e.RailDoubleRock ? Math.Max(result.chunk.mass * 0.75f, self.firstChunk.mass) : self.firstChunk.mass), result.chunk, result.onAppendagePos, Creature.DamageType.Blunt, baseDamage, stunBonus);
                    }
                }
                else if (result.chunk != null)
                {
                    result.chunk.vel += self.firstChunk.vel * self.firstChunk.mass / result.chunk.mass;
                }
                else if (result.onAppendagePos != null)
                {
                    (result.obj as PhysicalObject.IHaveAppendages).ApplyForceOnAppendage(result.onAppendagePos, self.firstChunk.vel * self.firstChunk.mass);
                }
                self.firstChunk.vel = self.firstChunk.vel * -0.5f + RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(0.1f, 0.4f, UnityEngine.Random.value) * self.firstChunk.vel.magnitude;
                if (!issaPunch)
                {
                    self.room.PlaySound(SoundID.Rock_Hit_Creature, self.firstChunk);
                }
                if (result.chunk != null)
                {
                    self.room.AddObject(new ExplosionSpikes(self.room, result.chunk.pos + RWCustom.Custom.DirVec(result.chunk.pos, result.collisionPoint) * result.chunk.rad, 5, 2f, 4f, 4.5f, 30f, new Color(1f, 1f, 1f, 0.5f)));
                }
                self.SetRandomSpin();
                return true;
            }

            return orig(self, result, eu);
        }
        catch (Exception err)
        {
            Ebug(self.thrownBy as Player, err, "Exception in Rockhit!");
            return orig(self, result, eu);
        }
    }


    private void Escort_BombHit(ILContext il)
    {
        var c = new ILCursor(il);
        try
        {
            c.GotoNext(MoveType.After,
                i => i.MatchCallOrCallvirt<Creature>(nameof(Creature.Violence))
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Bombhit IL match 1 failed");
            throw new Exception("IL Match 1 Failed", ex);
        }
        try
        {
            c.GotoPrev(MoveType.After,
                i => i.MatchLdcR4(0.8f)
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Bombhit IL match 2 failed");
            throw new Exception("IL Match 2 Failed", ex);
        }
        Ebug("Bombhit Identified point of interest", 0, true);
        try
        {
            c.Emit(OpCodes.Ldarg, 0);
            c.Emit(OpCodes.Ldarg, 1);
        }
        catch (Exception ex)
        {
            Ebug(ex, "Bombhit IL emit failed");
            throw new Exception("IL Emit Failed", ex);
        }

        try
        {
            c.EmitDelegate(
                (float original, ScavengerBomb self, SharedPhysics.CollisionResult result) => {
                    if (self.thrownBy is Player player && result.obj is Creature creature && Eshelp_IsMe(player.slugcatStats.name, false) && eCon.TryGetValue(player, out Escort e))
                    {
                        if (e.Deflector && !creature.dead)
                        {
                            original *= e.DeflDamageMult + e.DeflPerma;
                            if (e.DeflPowah == 3) Esclass_DF_UltimatePower(player);
                            e.DeflPowah = 0;
                            Ebug(player, $"Death upon thee! Sponsored by Bomb. Damage: {original}", ignoreRepetition: true);
                        }

                        if (e.NewEscapist && e.NEsVulnerable.Contains(creature))
                        {
                            int stunner = 100;
                            if (creature?.Template is not null && creature.Template.baseStunResistance > 1)
                            {
                                stunner = (int)(stunner * creature.Template.baseStunResistance);
                            }
                            creature.Stun(stunner);
                            original *= 2;
                            e.NEsResetCooldown = true;
                            Ebug(player, $"Direct bomb hit! Damage: {original}, Stun: {stunner}", ignoreRepetition: true);
                        }
                    }
                    return original;
                }
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Bombhit IL emite delegate failed");
            throw new Exception("IL EmitDelegate Failed", ex);
        }
    }

    private void Escort_SpearHit(ILContext il)
    {
        var c = new ILCursor(il);
        try
        {
            c.GotoNext(MoveType.After,
                i => i.MatchCallOrCallvirt<Creature>(nameof(Creature.Violence))
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Spearhit IL match 1 failed");
            throw new Exception("IL Match 1 Failed", ex);
        }
        try
        {
            c.GotoPrev(MoveType.After,
                i => i.MatchLdfld<Spear>(nameof(Spear.spearDamageBonus))
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Spearhit IL match 2 failed");
            throw new Exception("IL Match 2 Failed", ex);
        }
        Ebug("Spearhit Identified point of interest", 0, true);
        try
        {
            c.Emit(OpCodes.Ldarg, 0);
            c.Emit(OpCodes.Ldarg, 1);
        }
        catch (Exception ex)
        {
            Ebug(ex, "Spearhit IL emit failed");
            throw new Exception("IL Emit Failed", ex);
        }

        try
        {
            c.EmitDelegate(
                (float original, Spear self, SharedPhysics.CollisionResult result) => {
                    if (self.thrownBy is Player player && result.obj is Creature creature && Eshelp_IsMe(player.slugcatStats.name, false) && eCon.TryGetValue(player, out Escort e))
                    {
                        if (e.Deflector)
                        {
                            original *= e.DeflDamageMult + e.DeflPerma;
                            if (e.DeflPowah == 3) Esclass_DF_UltimatePower(player);
                            e.DeflPowah = 0;
                            Ebug(player, $"Death upon thee! Sponsored by Spear. Damage: {original}", ignoreRepetition: true);
                        }

                        if (e.NewEscapist && e.NEsVulnerable.Contains(creature))
                        {
                            original *= 2;
                            e.NEsResetCooldown = true;
                            Ebug(player, $"Reset from spear! Damage: {original}", ignoreRepetition: true);
                        }
                    }
                    return original;
                }
            );
        }
        catch (Exception ex)
        {
            Ebug(ex, "Spearhit IL emite delegate failed");
            throw new Exception("IL EmitDelegate Failed", ex);
        }
    }


    private bool Escort_FlareHit(On.FlareBomb.orig_HitSomething orig, FlareBomb self, SharedPhysics.CollisionResult result, bool eu)
    {
        bool ending = orig(self, result, eu);
        if (ending && self.thrownBy is Player player && result.obj is Creature creature && Eshelp_IsMe(player.slugcatStats.name, false) && eCon.TryGetValue(player, out Escort e))
        {
            if (e.NewEscapist && e.NEsVulnerable.Contains(creature))
            {
                e.NEsResetCooldown = true;
                self.room?.AddObject(new Explosion(self.room, self, Vector2.Lerp(self.firstChunk.pos, self.firstChunk.lastPos, 0.35f), 4, 40f, 5f, 2.5f, 200f, 0.01f, self.thrownBy, 0.7f, 100f, 1f));
                Ebug(player, $"Flarebomb? More like EXPLOSIVE!", ignoreRepetition: true);
            }
        }
        return ending;
    }

#endregion




}
