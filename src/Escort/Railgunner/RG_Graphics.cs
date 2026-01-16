using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheEscort.Plugin;
using static TheEscort.Eshelp;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TheEscort.Railgunner;

public static class RG_Gfx
{
    /// <summary>
    /// Makes Railgunner point to whoever dares to enter her line of sight
    /// </summary>
    public static void DrawThings(PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, float t, Vector2 camP, ref Escort e)
    {
        try
        {
            if (e.RailLaserSightIndex > s.sprites.Length)
            {
                Ebug("Laser sprite index is higher than expected!", LogLevel.ERR);
                return;
            }
            // Higher charge = brighter laser
            // Laser blinks if creature is in line of fire
            // 
            if (s.sprites[e.RailLaserSightIndex] is CustomFSprite cs)
            {
                if (e.RailDouble is not DoubleUp.None && self?.player?.dead == false)
                {
                    cs.isVisible = true;
                    float intensityThickness = Mathf.Lerp(0.7f, 1f, (float)e.RailgunUse / e.RailgunLimit);
                    float intensityAlpha = Custom.LerpMap(e.RailLaserDimmer, 0, Escort.RailLaserDimmerDuration, 0.9f, 0.6f);
                    bool targetSpotted = false;
                    Vector2 headPos = Vector2.Lerp(self.head.lastPos, self.head.pos, t);
                    Vector2 bodyPos = Vector2.Lerp(self.player.bodyChunks[1].lastPos, self.player.bodyChunks[1].pos, t);
                    Vector2 neckPos = Vector2.Lerp(headPos, bodyPos, 0.14f);
                    // TODO: Give railgunner 8 dir shots


                    // // How the game does throw direction calculation
                    Vector2 throwDir = new(self.player.ThrowDirection, 0);
                    // if (self.player.animation == Player.AnimationIndex.Flip && self.player.input[0].x == 0)
                    // {
                    //     if (self.player.input[0].y < 0)
                    //     {
                    //         throwDir.x = 0;
                    //         throwDir.y = -1;
                    //     }
                    //     else if (ModManager.MMF && MMF.cfgUpwardsSpearThrow.Value && self.player.input[0].y > 0)
                    //     {
                    //         throwDir.x = 0;
                    //         throwDir.y = 1;
                    //     }
                    // }

                    // ZeroG direction handling
                    // if (self.player.bodyMode == Player.BodyModeIndex.ZeroG && ModManager.MMF && MMF.cfgUpwardsSpearThrow.Value)
                    // {
                    //     throwDir = self.player.input[0].y == 0 ? new(self.player.ThrowDirection, 0) : new(0, self.player.input[0].y);
                    // }

                    // 8 direction throw
                    if (
                        self.player.bodyMode == Player.BodyModeIndex.ZeroG ||
                        self.player.animation == Player.AnimationIndex.Flip ||
                        self.player.animation == Player.AnimationIndex.DeepSwim
                        )
                    {
                        throwDir = new(self.player.input[0].y == 0 ? self.player.ThrowDirection : self.player.input[0].x, self.player.input[0].y);
                    }

                    // Custom direction for corridors
                    if (Escort_CorridorThrowDir(self.player, out IntVector2 tDir))
                    {
                        throwDir = Custom.IntVector2ToVector2(tDir);
                    }

                    Vector2 corner = Custom.RectCollision(neckPos, neckPos + throwDir * 100000f, rCam.room.RoomRect.Grow(200f)).GetCorner(FloatRect.CornerLabel.D);
                    IntVector2? intVector = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(rCam.room, neckPos, corner);
                    if (intVector is not null)
                    {
                        corner = Custom.RectCollision(corner, neckPos, rCam.room.TileRect(intVector.Value)).GetCorner(FloatRect.CornerLabel.D);
                    }

                    if (e.RailTargetClock == 0)
                    {
                        e.RailTargetAcquired = Esclass_RG_Spotter(self, neckPos, throwDir, corner, camP);
                        //Ebug("Target checking! Found creature? " + (e.RailTargetAcquired is not null), ignoreRepetition: true);
                        e.RailTargetClock = Custom.rainWorld.options.quality switch
                        {
                            var a when a == Options.Quality.LOW => 29,
                            var a when a == Options.Quality.MEDIUM => 14,
                            var a when a == Options.Quality.HIGH => 4,
                            _ => 4
                        };
                    }

                    if (e.RailTargetAcquired is not null)
                    {
                        float newX = corner.x;
                        float newY = corner.y;
                        if (throwDir.x < 0)
                        {
                            newX = Mathf.Max(corner.x, e.RailTargetAcquired.pos.x);
                        }
                        else if (throwDir.x > 0)
                        {
                            newX = Mathf.Min(corner.x, e.RailTargetAcquired.pos.x);
                        }
                        if (throwDir.y < 0)
                        {
                            newY = Mathf.Max(corner.y, e.RailTargetAcquired.pos.y);
                        }
                        else if (throwDir.y > 0)
                        {
                            newY = Mathf.Min(corner.y, e.RailTargetAcquired.pos.y);
                        }
                        corner = new(newX, newY);

                        if (e.RailTargetAcquired.owner is Creature cr && !cr.dead)
                        {
                            targetSpotted = true;
                            e.RailLaserDimmer = Escort.RailLaserDimmerHoldDur;
                            intensityAlpha = 1;
                            intensityThickness -= 0.2f;
                        }
                    }

                    Color c = e.RailgunnerColor;
                    if (targetSpotted)
                    {
                        c = Color.Lerp(Color.red, e.RailLaserBlink ? e.RailLaserColor : e.RailgunnerColor, Mathf.InverseLerp(0, 10, e.RailLaserBlinkClock));
                    }
                    cs.verticeColors = [.. cs.verticeColors.Select(a => Custom.RGB2RGBA(c, intensityAlpha))];

                    // This does some space magic, based on Vulture laser thing
                    cs.MoveVertice(0, neckPos + throwDir * 4f + Custom.PerpendicularVector(throwDir) * intensityThickness - camP);
                    cs.MoveVertice(1, neckPos + throwDir * 4f - Custom.PerpendicularVector(throwDir) * intensityThickness - camP);
                    cs.MoveVertice(2, corner + Custom.PerpendicularVector(throwDir) * intensityThickness - camP);
                    cs.MoveVertice(3, corner - Custom.PerpendicularVector(throwDir) * intensityThickness - camP);
                }
                else
                {
                    cs.isVisible = false;
                }
            }
            else
            {
                Ebug("Unexpected failure to identify laser sprite!", LogLevel.WARN);
                return;
            }
        }
        catch (NullReferenceException nre)
        {
            Ebug(nre, "Null reference when drawing Railgunner sprites");
        }
        catch (IndexOutOfRangeException ioore)
        {
            Ebug(ioore, "Index out of bounds when drawing Railgunner sprites");
        }
        catch (Exception err)
        {
            Ebug(err, "Generic error when drawing Railgunner sprites");
        }
    }

    public static void UpdateGraphics(PlayerGraphics self, ref Escort e)
    {
        // Points the spears at the target when target acquired
        if (e.RailTargetAcquired?.owner is Creature c && !c.dead && e.RailDouble is not DoubleUp.None && self.hands is not null)
        {
            for (int i = 0; i < self.hands.Length; i++)
            {
                if (self.hands[i].reachingForObject) continue; // Don't override other things

                self.hands[i].reachingForObject = true;
                self.hands[i].absoluteHuntPos = e.RailTargetAcquired.pos;
            }
        }
    }


    public static Vector2 PointWeaponAt(On.Player.orig_GetHeldItemDirection orig, Player self, int hand)
    {
        if (self is null) return orig(self, hand);
        if (!eCon.TryGetValue(self, out Escort e)) return orig(self, hand);

        if (e.Railgunner && e.RailDouble is not DoubleUp.None && e.RailTargetAcquired?.owner is Creature c && !c.dead)
        {
            if (self.grasps[hand].grabbed is Spear s)
            {
                return Custom.DirVec(s.firstChunk.pos, e.RailTargetAcquired.pos);
            }

            else if (ModManager.MSC && self.grasps[hand].grabbed is LillyPuck l)
            {
                return Custom.DirVec(l.firstChunk.pos, e.RailTargetAcquired.pos);
            }
        }
        return orig(self, hand);
    }


    public static void InitiateSprites(PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, ref Escort e)
    {
        try
        {
            if (self is null || s?.sprites is null || rCam is null) return;
            Escort.Escat_setIndex_sprite_cue(ref e.RailLaserSightIndex, s.sprites.Length);
            Ebug("Set cue for Railgunner laser sprite");
            Array.Resize(ref s.sprites, s.sprites.Length + 1);
            s.sprites[e.RailLaserSightIndex] = new CustomFSprite("Futile_White")
            {
                shader = rCam.game.rainWorld.Shaders["HologramBehindTerrain"]
            };
        }
        catch (NullReferenceException nre)
        {
            Ebug(nre, "Null reference when initiating Railgunner laser!");
        }
        catch (IndexOutOfRangeException ioore)
        {
            Ebug(ioore, "Index out of bounds when initiating Railgunner laser!");
        }
        catch (Exception err)
        {
            Ebug(err, "Generic error when initiating Railgunner laser");
        }
    }

    public static BodyChunk Esclass_RG_Spotter(PlayerGraphics self, Vector2 origin, Vector2 direction, Vector2 corner, Vector2 camP)
    {
        BodyChunk target = null;
        // if (!railgunLaserPos.TryGet(self.player, out float[] lPos))
        // {
        //     Ebug("Failed to get slugbase feature for Railgunner laser", LogLevel.WARN);
        // }
        try
        {
            float minX = Mathf.Min(origin.x, corner.x) - 15;
            float maxX = Mathf.Max(origin.x, corner.x) + 15;
            float minY = Mathf.Min(origin.y, corner.y) - 15;
            float maxY = Mathf.Max(origin.y, corner.y) + 15;
            float minDist = int.MaxValue;
            // Ebug("Origin: [" + origin.x + ", " + origin.y + "] | Corner: [" + corner.x + ", " + corner.y + "]");
            // Ebug("Positions xy: " + minX + "-" + maxX + ", " + minY + "-" + maxY);
            if (self?.player?.room?.physicalObjects is null) return null;
            // List<BodyChunk> potentials = (
            //     from po in self.player.room.physicalObjects
            //     where po.Any(a => a is Creature)
            //     from co in po
            //     where co is Creature
            //     where co != self.player
            //     from bc in co.bodyChunks
            //     where bc.pos.y < maxY && bc.pos.y > minY && bc.pos.x < maxX && bc.pos.x > minX
            //     select bc
            // ).ToList();
            List<BodyChunk> potentials = self.player.room.physicalObjects
                .SelectMany(a => a)
                .OfType<Creature>()
                .Where(c => c != self.player)
                .SelectMany(c => c.bodyChunks)
                .Where(bc => bc.pos.y < maxY && bc.pos.y > minY && bc.pos.x < maxX && bc.pos.x > minX)
                .ToList();

            if (direction.x == 0 && direction.y != 0)  // Vertical
            {
                foreach (BodyChunk bc in potentials)
                {
                    if (Mathf.Abs(origin.y - bc.pos.y) < minDist)
                    {
                        minDist = Mathf.Abs(origin.y - bc.pos.y);
                        target = bc;
                    }
                }
            }
            else if (direction.x != 0 && direction.y == 0)  // Horizontal
            {
                foreach (BodyChunk bc in potentials)
                {
                    if (Mathf.Abs(origin.x - bc.pos.x) < minDist)
                    {
                        minDist = Mathf.Abs(origin.x - bc.pos.x);
                        target = bc;
                    }
                }
            }
            else  // Diagonal
            {
                foreach (BodyChunk bc in potentials)
                {
                    float why = Mathf.Abs(Mathf.Lerp(origin.y, corner.y, Mathf.InverseLerp(origin.x, corner.x, bc.pos.x)));
                    float compare = Mathf.Abs(bc.pos.y);
                    // Check if in diagonal line with padding
                    if (compare < why + 10 && compare > why - 10)
                    {
                        float distance = Custom.Dist(origin, bc.pos);
                        if (distance < minDist)  // Then check distance from player
                        {
                            minDist = distance;
                            target = bc;
                        }
                    }
                }
            }
            if (target is not null)
            {
                Ebug("Target is: " + target.owner.GetType().ToString());
            }

        }
        catch (Exception err)
        {
            Ebug(err, "FECK something happened with spotting a creature");
        }
        return target;
    }
}