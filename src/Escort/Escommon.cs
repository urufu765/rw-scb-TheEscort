using BepInEx;
using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RWCustom;
using System;
using System.Collections.Generic;
using UnityEngine;
using static TheEscort.Eshelp;

namespace TheEscort;
/// <summary>
/// For methods specific to Escort that are used by all builds
/// </summary>
partial class Plugin : BaseUnityPlugin
{

    /// <summary>
    /// Only used for wall longpounce.
    /// 0.3.1: And now for Unstable's jump
    /// </summary>
    public static void Escort_MovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
    {
        orig(self, eu);
        try
        {
            if (Escort_IsNull(self.slugcatStats.name))
            {
                return;
            }
        }
        catch (Exception err)
        {
            Ebug(self, err);
            return;
        }
        if (!eCon.TryGetValue(self, out Escort e))
        {
            return;
        }
        if (e.Unstable) Esclass_US_MovementUpdate(self, ref e);
        if (!ins.Esconfig_WallJumps(self))
        {
            return;
        }
        if (self.bodyMode == Player.BodyModeIndex.WallClimb && self.bodyChunks[0].ContactPoint.x != 0 && self.bodyChunks[1].ContactPoint.x != 0)
        {
            String msg = "Nothing New";
            self.canWallJump = 0;
            if (self.input[0].jmp)
            {
                msg = "Is touching the jump button";
                if (self.superLaunchJump < 20)
                {
                    self.superLaunchJump += 2;
                    if (self.Adrenaline == 1f && self.superLaunchJump < 6)
                    {
                        self.superLaunchJump = 6;
                    }
                }
                else
                {
                    self.killSuperLaunchJumpCounter = 15;
                }
            }

            if (!self.input[0].jmp && self.input[1].jmp)
            {
                msg = "Lets go of the jump button";
                self.wantToJump = 1;
            }
            if (e.consoleTick == 0)
            {
                Ebug(self, msg, LogLevel.DEBUG);
            }
        }
    }


    /// <summary>
    /// Only used for wall longpounce.
    /// </summary>
    public static void Escort_checkInput(On.Player.orig_checkInput orig, Player self)
    {
        try
        {
            if (Escort_IsNull(self.slugcatStats.name))
            {
                orig(self);
                return;
            }
        }
        catch (Exception err)
        {
            orig(self);
            Ebug(self, err);
            return;
        }
        if (!ins.Esconfig_WallJumps(self))
        {
            orig(self);
            return;
        }

        //Ebug(self, "CheckInput Triggered!");
        int previously = self.input[0].x;
        orig(self);

        // Undoes the input cancellation
        if (self.bodyMode == Player.BodyModeIndex.WallClimb && self.superLaunchJump > 5 && self.input[0].jmp && self.input[1].jmp && self.input[0].y < 1)
        {
            if (self.input[0].x == 0)
            {
                self.input[0].x = previously;
            }
        }
    }

    /// <summary>
    /// Immitates a wall jump but not really
    /// </summary>
    public static void Escort_FakeWallJump(Player self, int direction = -2, float boostUp = 18f, float yankUp = 10f, float boostLR = 5f)
    {
        self.bodyChunks[0].vel.y = boostUp;
        self.bodyChunks[1].vel.y = boostUp - 1f;
        self.bodyChunks[0].pos.y += yankUp;
        self.bodyChunks[1].pos.y += yankUp;
        if (direction != -2)
        {
            self.bodyChunks[0].vel.x = boostLR * direction;
            self.bodyChunks[1].vel.x = (boostLR - 1) * direction;
        }
    }


    /// <summary>
    /// Implement a different type of dropkick
    /// </summary>
    public static void Escort_TossObject(On.Player.orig_TossObject orig, Player self, int grasp, bool eu)
    {
        orig(self, grasp, eu);
        try
        {
            if (Escort_IsNull(self.slugcatStats.name))
            {
                return;
            }
            if (!BodySlam.TryGet(self, out var bodySlam) ||
                !eCon.TryGetValue(self, out Escort e))
            {
                return;
            }

            Ebug(self, "Toss Object Triggered!");
            if (self.grasps[grasp].grabbed is Lizard lizzie && !lizzie.dead)
            {
                if (ins.Esconfig_SFX(self) && e.LizGet != null)
                {
                    e.LizGet.Volume = 0f;
                }
                if (self.bodyMode == Player.BodyModeIndex.Default && (!e.Brawler || e.BrawThrowGrab == 0))
                {
                    self.animation = Player.AnimationIndex.RocketJump;
                    self.bodyChunks[1].vel.x += self.ThrowDirection;
                }
            }
        }
        catch (Exception err)
        {
            Ebug(self, err);
            return;
        }
    }


    /// <summary>
    /// Contrary to the name, prevents Escort from dying on certain conditions.
    /// </summary>
    public static void Escort_Die(On.Player.orig_Die orig, Player self)
    {
        try
        {
            if (Escort_IsNull(self.slugcatStats.name))
            {
                orig(self);
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e))
            {
                orig(self);
                return;
            }

            Ebug(self, "Die Triggered!");
            // Acid water survival
            if (e.Railgunner) e.RailTargetAcquired = null;

            if (self.Submersion > 0.2f && self.room?.waterObject is not null && self.room.waterObject.WaterIsLethal && self.aerobicLevel < 1f)
            {
                Ebug(self, "Escort survives swimming in acid!");
                self.dead = false;
                if (e.Gilded) Esclass_GD_Die(ref e);
                if (self.lavaContactCount > 0)
                {
                    self.lavaContactCount = self.aerobicLevel switch
                    {
                        > 0.9f => 3,
                        > 0.7f => 2,
                        > 0.4f => 1,
                        _ => 0
                    };
                }
                if (e.acidRepetitionGuard == 0)
                {
                    self.aerobicLevel += e.acidSwim;
                    e.acidRepetitionGuard += 5;
                }
                return;
            }

            if (!e.ParrySuccess && e.iFrames == 0 && !self.dead)
            {
                orig(self);
                if (self.dead && ins.Esconfig_SFX(self) && self.room != null)
                {
                    self.room.PlaySound(Escort_SFX_Death, e.SFXChunk);
                    //self.room.PlayCustomSound("escort_failure", self.mainBodyChunk.pos, 0.7f, 1f);
                }
                Ebug(self, "Failure.", LogLevel.INFO);
                return;
            }
            else if (e.iFrames > 0)
            {
                self.dead = false;
                Ebug(self, "Player didn't die? Due to Iframes", LogLevel.DEBUG);
                e.ParrySuccess = false;
                return;
            }
            else if (e.ParrySuccess)
            {
                Ebug(self, "Player attempted to cheat death with the ParrySuccess card. Failed.");
            }

            orig(self);
        }
        catch (Exception err)
        {
            Ebug(self, err, "Something happened while trying to die!");
            orig(self);
        }
    }

    /// <summary>
    /// Makes Escort eat held items twice as fast
    /// </summary>
    /// <param name="orig">Original function call (pass the method along)</param>
    /// <param name="self">Player instance</param>
    /// <param name="eu">Even Updates</param>
    public static void Escort_Eated(On.Player.orig_BiteEdibleObject orig, Player self, bool eu)
    {
        try
        {
            if (Escort_IsNull(self.slugcatStats.name))
            {
                orig(self, eu);
                return;
            }
            for (int a = 0; a < 2; a++)
            {
                if (self.grasps[a] != null && self.grasps[a].grabbed is IPlayerEdible ipe && ipe.Edible)
                {
                    if (ipe.BitesLeft > 1)
                    {
                        if (self.grasps[a].grabbed is Fly)
                        {
                            for (int b = 0; b < ipe.BitesLeft; b++)
                            {
                                ipe.BitByPlayer(self.grasps[a], eu);
                            }
                        }
                        else
                        {
                            ipe.BitByPlayer(self.grasps[a], eu);
                        }
                    }
                    break;
                }
            }
            orig(self, eu);
        }
        catch (Exception err)
        {
            Ebug(self, err, "Error when eated!");
            orig(self, eu);
            return;
        }
    }

    public static void Escort_ExplosionKnockbackResist(ILContext il)
    {
        ILCursor tomato = new(il);
        int localLoc = int.MinValue;
        int collisionLoc = int.MinValue;
        int objectLoc = int.MinValue;
        try
        {
            // Finds the local variable locations for the j and k in room.physicalObjects[j][k]
            tomato.GotoNext(MoveType.After,
                getThis => getThis.MatchLdloc(out collisionLoc),
                findThis => findThis.MatchLdelemRef(),
                thenGet => thenGet.MatchLdloc(out objectLoc)
            );
            // Then sets the cursor where the force is reduced, right before the Artificer check
            tomato.GotoNext(MoveType.After,
                matchThis => matchThis.MatchLdfld<Explosion>(nameof(Explosion.force)),
                thenMatch => thenMatch.MatchStloc(out localLoc)  // Local variable location for force
            );
            Ebug("Matched point of explosive interest", LogLevel.MESSAGE, ignoreRepetition: true);
        }
        catch (Exception err)
        {
            Ebug(err, "Issue finding the explosion update's PoI");
            throw new Exception("Failure in matching when trying to do explosion resist!", err);
        }

        if (localLoc == int.MinValue || collisionLoc == int.MinValue || objectLoc == int.MinValue)
        {
            Ebug("Failed to find local variable locations! Stopping hook here.", LogLevel.ERR);
            throw new Exception("Failure to find the locations of vital local variables when trying to do explosion resist!");
        }

        // Reduce the knockback force
        try
        {
            tomato.Emit(OpCodes.Ldarg, 0);  // Explosion instance
            tomato.Emit(OpCodes.Ldloc, localLoc);  // Get local variable
            tomato.Emit(OpCodes.Ldloc, collisionLoc);  // j index
            tomato.Emit(OpCodes.Ldloc, objectLoc);  // k index
            tomato.EmitDelegate(Escort_ExplosionKnockbackReduction);  // Function call
            tomato.Emit(OpCodes.Stloc, localLoc);  // Set local variable
        }
        catch (Exception err)
        {
            Ebug(err, "Failed to inject knockback resist into the spot");
            throw new Exception("Failure in method injection when trying to do knockback reduction!", err);
        }

        ILLabel lable = null;

        try
        {
            // Finds the check for minStun (so we can skip it later)
            tomato.GotoNext(MoveType.After,
                findThis => findThis.MatchLdfld<Explosion>(nameof(Explosion.minStun)),
                thenFind => thenFind.MatchLdcR4(0.0f),
                thenGet => thenGet.MatchBleUn(out lable)  // Get the label location so we could skip to this
            );
            Ebug("Matched point of explosive interest 2", LogLevel.MESSAGE, ignoreRepetition: true);
        }
        catch (Exception err)
        {
            Ebug(err, "Issue finding the explosion update's PoI2");
            throw new Exception("Failure in matching when trying to do explosion resist's stun negation!", err);
        }

        if (lable is null)
        {
            Ebug("Failed to find branch label to skip stun for! Stopping hook here.", LogLevel.ERR);
            throw new Exception("Failure to find the label of branch when trying to do explosion resist's stun negation!");
        }

        // Turn off the stun
        try
        {
            tomato.Emit(OpCodes.Ldarg, 0);  // Explosion instance
            tomato.Emit(OpCodes.Ldloc, collisionLoc);  // j index
            tomato.Emit(OpCodes.Ldloc, objectLoc);  // k index
            tomato.EmitDelegate(Escort_ExplosionStunNegate);  // Function call
            tomato.Emit(OpCodes.Brtrue_S, lable);  // Goto branch if false
        }
        catch (Exception err)
        {
            Ebug(err, "Failed to inject stun negation into the spot");
            throw new Exception("Failure in method injection when trying to do stun negation!", err);
        }

        int localLoc2 = int.MinValue;
        try
        {
            // Finds the next time Explosion.force is used, to reduce this one too (for graphics)
            tomato.GotoNext(MoveType.After,
                matchThis => matchThis.MatchLdfld<Explosion>(nameof(Explosion.force)),
                thenMatch => thenMatch.MatchStloc(out localLoc2)  // Local variable location for force 2
            );
            Ebug("Matched point of explosive interest 3", LogLevel.MESSAGE, ignoreRepetition: true);
        }
        catch (Exception err)
        {
            Ebug(err, "Issue finding the explosion update's PoI 3");
            throw new Exception("Failure in matching when trying to do knockback resist again!", err);
        }

        if (localLoc2 == int.MinValue)
        {
            Ebug("Failed to find local variable location again! Stopping hook here.", LogLevel.ERR);
            throw new Exception("Failure to find the locations of vital local variable when trying to do explosion resist again!");
        }

        // Reduce the knockback force AGAIN
        try
        {
            tomato.Emit(OpCodes.Ldarg, 0);  // Explosion instance
            tomato.Emit(OpCodes.Ldloc, localLoc2);  // Get local variable 2
            tomato.Emit(OpCodes.Ldloc, collisionLoc);  // j index
            tomato.Emit(OpCodes.Ldloc, objectLoc);  // k index
            tomato.EmitDelegate(Escort_ExplosionKnockbackReduction);  // Function call
            tomato.Emit(OpCodes.Stloc, localLoc2);  // Set local variable 2
        }
        catch (Exception err)
        {
            Ebug(err, "Failed to inject knockback resist again into the spot");
            throw new Exception("Failure in method injection when trying to do knockback reduction again!", err);
        }
    }

    /// <summary>
    /// Negates the stun portion
    /// </summary>
    /// <param name="self">Explosion instance</param>
    /// <param name="collisionIndex">Collision layer index</param>
    /// <param name="objectIndex">Object in layer index</param>
    /// <returns>true if negates</returns>
    public static bool Escort_ExplosionStunNegate(Explosion self, int collisionIndex, int objectIndex)
    {
        if (self?.room?.physicalObjects?[collisionIndex]?[objectIndex] is Player p && 
            eCon.TryGetValue(p, out Escort e))
        {
            if (e.iFrames > 0 || Eshelp_ParryCondition(p))
            {
                return true;
            }

            if (e.Railgunner)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Reduces the explosion knockback effect
    /// </summary>
    /// <param name="self">Explosion instance</param>
    /// <param name="frc">original force</param>
    /// <param name="collisionIndex">Collision layer index</param>
    /// <param name="objectIndex">Object in layer index</param>
    /// <returns>modified force</returns>
    public static float Escort_ExplosionKnockbackReduction(Explosion self, float frc, int collisionIndex, int objectIndex)
    {
        if (
            self?.room?.physicalObjects?[collisionIndex]?[objectIndex] is Player p && 
            eCon.TryGetValue(p, out Escort e))
        {
            if (e.Railgunner)
            {
                frc /= 2;
                if (e.RailBombJump)
                {
                    frc /= 2;
                }
            }

            if (e.Deflector && (e.iFrames > 0 || Eshelp_ParryCondition(p)))
            {
                frc /= 5;
            }
        }
        return frc;
    }


    public static bool Escort_CorridorThrowDir(Player self, out IntVector2 throwDir)
    {
        throwDir = new();

        if (self.bodyMode == Player.BodyModeIndex.CorridorClimb && self.IsTileSolid(0, 1, 0) && self.IsTileSolid(0, -1, 0))
        {
            throwDir = new IntVector2((int)(self.mainBodyChunk.Rotation.x * 2), (int)(self.mainBodyChunk.Rotation.y * 2));
            return true;
        }
        return false;
    }
}
