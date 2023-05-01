using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using System.Runtime.CompilerServices;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using static TheEscort.Eshelp;
using RWCustom;

namespace TheEscort
{
    /// <summary>
    /// For methods specific to Escort that are used by all builds
    /// </summary>
    partial class Plugin : BaseUnityPlugin
    {
        private void Escort_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam)
        {
            ins.L().set();
            orig(self, s, rCam);
            try{
                if (!(self != null && self.player != null)){
                    return;
                }
                ins.L().set("Null Check");
                if (self.player.slugcatStats.name != EscortMe){
                    return;
                }
                ins.L().set("Escort Check");
                if (!eCon.TryGetValue(self.player, out Escort e)){
                    return;
                }
                ins.L().set("Escort/Socks CWT Access");
                e.Escat_setIndex_sprite_cue(s.sprites.Length);
                Array.Resize(ref s.sprites, s.sprites.Length + e.customSprites);
                // Store the end index of the sprites so none are overwritten!
                s.sprites[e.spriteQueue] = new FSprite("escortHeadT");
                s.sprites[e.spriteQueue + 1] = new FSprite("escortHipT");
                
                // When builds have custom sprites, do an if condition and add accordingly
                for (int i = e.spriteQueue; i < e.spriteQueue + e.customSprites; i++){
                    if (s.sprites[e.spriteQueue] == null){
                        Ebug(self.player, "Oh geez. No sprites?", 0);
                    }
                }

                ins.L().set("Successful Spriting Check");
                self.AddToContainer(s, rCam, null);
            } catch(Exception err){
                Ebug(self.player, err, "Something went wrong when initiating sprites!");
                return;
            }
        }

        private void Escort_ApplyPalette(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, RoomPalette palette){
            orig(self, s, rCam, palette);
            try{
                if (!(self != null && self.player != null)){
                    return;
                }
                if (self.player.slugcatStats.name.value != "EscortMe"){
                    return;
                }
                if (!eCon.TryGetValue(self.player, out Escort e)){
                    return;
                }

                if (e.spriteQueue == -1){
                    return;
                }
                if (e.spriteQueue + 2 == s.sprites.Length && (s.sprites[e.spriteQueue] == null || s.sprites[e.spriteQueue + 1] == null)){
                    Ebug(self.player, "Oh dear. Null sprites!!", 0);
                    return;
                }
                Color c = new Color(0.796f, 0.549f, 0.27843f);
                // Applying colors?
                if (s.sprites.Length > e.spriteQueue){
                    //Ebug(self.player, "Gone in", 2);
                    if (ModManager.CoopAvailable && self.useJollyColor){
                        //Ebug(self.player, "Jollymachine", 2);
                        c = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                        //Ebug(self.player, "R: " + c.r + " G: " + c.g + " B: " + c.b);
                        //Ebug(self.player, "Jollymachine end", 2);
                    }
                    else if (PlayerGraphics.CustomColorsEnabled()){
                        Ebug(self.player, "Custom color go brr", 2);
                        c = PlayerGraphics.CustomColorSafety(2);
                        Ebug(self.player, "Custom color end", 2);
                    }
                    else{
                        //==Ebug(self.player, "Arenasession or Singleplayer", 2);

                        if (rCam.room.game.IsArenaSession && !rCam.room.game.setupValues.arenaDefaultColors){
                            switch(self.player.playerState.playerNumber){
                                case 0:
                                    if (rCam.room.game.IsArenaSession && rCam.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType != MoreSlugcats.MoreSlugcatsEnums.GameTypeID.Challenge && !nonArena){
                                        c = new Color(0.2f, 0.6f, 0.77f);
                                    }
                                    break;
                                case 1:
                                    c = new Color(0.26f, 0.68f, 0.21f);
                                    break;
                                case 2:
                                    c = new Color(0.55f, 0.11f, 0.55f);
                                    break;
                                case 3:
                                    c = new Color(0.91f, 0.7f, 0.9f);
                                    break;
                            }
                        }
                        Ebug(self.player, "Arena/Single end.", 2);
                    }
                }
                if (c.r <= 0.02f && c.g <= 0.02f && c.b <= 0.02f){
                    e.secretRGB = true;
                }
                for (int i = e.spriteQueue; i < s.sprites.Length; i++){
                    s.sprites[i].color = e.Escat_runit_thru_RGB(c);
                }
                if (!e.secretRGB){
                    e.hypeColor = c;
                }
                if (e.hypeLight == null || e.hypeSurround == null){
                    e.Escat_setLight_hype(self.player, e.hypeColor);
                }
                else{
                    e.hypeLight.color = c;
                    e.hypeSurround.color = c;
                }
            } catch(Exception err){
                Ebug(self.player, err, "Something went wrong when coloring in the palette!");
                return;
            }
        }

        private void Escort_AddGFXContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, FContainer newContainer){
            orig(self, s, rCam, newContainer);
            try{
                if (!(self != null && self.player != null)){
                    return;
                }
                if (self.player.slugcatStats.name.value != "EscortMe"){
                    return;
                }
                if (!eCon.TryGetValue(self.player, out Escort e)){
                    return;
                }
                if (e.spriteQueue == -1){
                    return;
                }
                if (e.spriteQueue + 2 == s.sprites.Length && (s.sprites[e.spriteQueue] == null || s.sprites[e.spriteQueue + 1] == null)){
                    Ebug(self.player, "Oh shoot. Where sprites?", 0);
                    return;
                }
                if (e.spriteQueue < s.sprites.Length){
                    rCam.ReturnFContainer("Foreground").RemoveChild(s.sprites[e.spriteQueue]);
                    rCam.ReturnFContainer("Foreground").RemoveChild(s.sprites[e.spriteQueue + 1]);
                    Ebug(self.player, "Removal success.", 1);
                    rCam.ReturnFContainer("Midground").AddChild(s.sprites[e.spriteQueue]);
                    rCam.ReturnFContainer("Midground").AddChild(s.sprites[e.spriteQueue + 1]);
                    Ebug(self.player, "Addition success.", 1);
                    s.sprites[e.spriteQueue].MoveBehindOtherNode(s.sprites[9]);
                    s.sprites[e.spriteQueue + 1].MoveBehindOtherNode(s.sprites[3]);
                    //Ebug(self.player, "Restructure success.", 1);
                }
            } catch(Exception err){
                Ebug(self.player, err, "Something went wrong when adding graphics to container!");
                return;
            }
        }

        private void Escort_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, float t, Vector2 camP){
            try{
                if (!(self != null && self.player != null)){
                    orig(self, s, rCam, t, camP);
                    return;
                }
                if (self.player.slugcatStats.name.value != "EscortMe"){
                    orig(self, s, rCam, t, camP);
                    return;
                }
                if (!headDraw.TryGet(self.player, out var hD) || !bodyDraw.TryGet(self.player, out var bD) || !eCon.TryGetValue(self.player, out Escort e)){
                    orig(self, s, rCam, t, camP);
                    return;
                }
                if (e.spriteQueue == -1){
                    orig(self, s, rCam, t, camP);
                    return;
                }
                if (e.spriteQueue + 2 == s.sprites.Length && (s.sprites[e.spriteQueue] == null || s.sprites[e.spriteQueue + 1] == null)){
                    orig(self, s, rCam, t, camP);
                    Ebug(self.player, "Oh crap. Sprites? Hello?!", 0);
                    return;
                }
                if (s.sprites.Length > 9 && s.sprites[1] != null){
                    e.hipScaleX = s.sprites[1].scaleX;
                    e.hipScaleY = s.sprites[1].scaleY;
                } else {
                    e.hipScaleX = bD[0];
                    e.hipScaleY = bD[0];
                }
                orig(self, s, rCam, t, camP);
                if (s.sprites.Length > e.spriteQueue){
                    // Hypelevel visual fx
                    try{
                        if (self.player != null && Esconfig_Hypable(self.player)){
                            float alphya = 1f;
                            if (requirement > self.player.aerobicLevel){
                                alphya = Mathf.Lerp((self.player.dead? 0f : 0.57f), 1f, Mathf.InverseLerp(0f, requirement, self.player.aerobicLevel));
                            }
                            for (int a = e.spriteQueue; a < s.sprites.Length; a++){
                                s.sprites[a].alpha = alphya;
                                s.sprites[a].color = e.Escat_runit_thru_RGB(e.hypeColor, (requirement < self.player.aerobicLevel? 8f : Mathf.Lerp(1f, 4f, Mathf.InverseLerp(0f, requirement, self.player.aerobicLevel)))) * Mathf.Lerp(1f, 1.8f, Mathf.InverseLerp(0f, 15f, e.smoothTrans));
                            }
                            s.sprites[e.spriteQueue + 1].scaleX = e.hipScaleX;
                            s.sprites[e.spriteQueue + 1].scaleY = e.hipScaleY;
                            if (e.hypeLight != null && e.hypeSurround != null){
                                float alpine = 0f;
                                float alpite = 0f;

                                if (requirement > self.player.aerobicLevel){
                                    alpine = Mathf.Lerp(0f, 0.08f, Mathf.InverseLerp(0f, requirement, self.player.aerobicLevel));
                                    alpite = Mathf.Lerp(0f, 0.2f, Mathf.InverseLerp(0f, requirement, self.player.aerobicLevel));
                                }
                                else {
                                    alpine = 0.1f;
                                    alpite = 0.3f;
                                }
                                e.hypeLight.stayAlive = true;
                                e.hypeSurround.stayAlive = true;
                                e.hypeLight.setPos = self.player.mainBodyChunk.pos;
                                e.hypeSurround.setPos = self.player.bodyChunks[0].pos;
                                e.hypeLight.setAlpha = alpine;
                                e.hypeSurround.setAlpha = alpite;
                                if (e.secretRGB){
                                    e.hypeLight.color = e.hypeColor * Mathf.Lerp(1f, 2f, Mathf.InverseLerp(0f, 15f, e.smoothTrans));
                                    e.hypeSurround.color = e.hypeColor * Mathf.Lerp(1f, 2f, Mathf.InverseLerp(0f, 15f, e.smoothTrans));
                                }
                            }
                            else {
                                e.Escat_setLight_hype(self.player, e.hypeColor);
                            }
                        }
                    } catch (Exception err){
                        Ebug(self.player, err, "something went wrong when altering alpha");
                    }
                    s.sprites[e.spriteQueue].rotation = s.sprites[9].rotation;
                    s.sprites[e.spriteQueue + 1].rotation = s.sprites[1].rotation;
                    s.sprites[e.spriteQueue].scaleX = hD[0];
                    //s.sprites[e.spriteQueue + 1].scaleX = bD[0];
                    if (self.player.animation == Player.AnimationIndex.Flip || self.player.animation == Player.AnimationIndex.Roll){
                        Vector2 vectoria = RWCustom.Custom.DegToVec(s.sprites[9].rotation) * hD[1];
                        Vector2 vectorib = RWCustom.Custom.DegToVec(s.sprites[1].rotation) * bD[1];
                        s.sprites[e.spriteQueue].x = s.sprites[9].x + vectoria.x;
                        s.sprites[e.spriteQueue].y = s.sprites[9].y + vectoria.y;
                        s.sprites[e.spriteQueue + 1].x = s.sprites[1].x + vectorib.x;
                        s.sprites[e.spriteQueue + 1].y = s.sprites[1].y + vectorib.y;
                    } else {
                        s.sprites[e.spriteQueue].x = s.sprites[9].x + hD[2];
                        s.sprites[e.spriteQueue].y = s.sprites[9].y + hD[3];
                        s.sprites[e.spriteQueue + 1].x = s.sprites[1].x + bD[2];
                        s.sprites[e.spriteQueue + 1].y = s.sprites[1].y + bD[3];
                    }
                }
                if (e.Speedster) Esclass_SS_DrawSprites(self, s, rCam, t, camP, ref e);
            } catch (Exception err){
                orig(self, s, rCam, t, camP);
                Ebug(self.player, err, "Something happened while trying to draw sprites!");
            }
        }




        /// <summary>
        /// Only used for wall longpounce.
        /// </summary>
        private void Escort_MovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            orig(self, eu);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
            if (!Esconfig_WallJumps(self)){
                return;
            }
            if (!eCon.TryGetValue(self, out Escort e)){
                return;
            }
            if (self.bodyMode == Player.BodyModeIndex.WallClimb && self.bodyChunks[0].ContactPoint.x != 0 && self.bodyChunks[1].ContactPoint.x != 0){
                String msg = "Nothing New";
                self.canWallJump = 0;
                if (self.input[0].jmp){
                    msg = "Is touching the jump button";
                    if (self.superLaunchJump < 20){
                        self.superLaunchJump += 2;
                        if (self.Adrenaline == 1f && self.superLaunchJump < 6){
                            self.superLaunchJump = 6;
                        }
                    } else {
                        self.killSuperLaunchJumpCounter = 15;
                    }
                }

                if (!self.input[0].jmp && self.input[1].jmp){
                    msg = "Lets go of the jump button";
                    self.wantToJump = 1;
                }
                if(e.consoleTick == 0){
                    Ebug(self, msg, 2);
                }
            }
        }


        /// <summary>
        /// Only used for wall longpounce.
        /// </summary>
        private void Escort_checkInput(On.Player.orig_checkInput orig, Player self)
        {
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self);
                    return;
                }
            } catch (Exception err){
                orig(self);
                Ebug(self, err);
                return;
            }
            if(!Esconfig_WallJumps(self)){
                orig(self);
                return;
            }

            //Ebug(self, "CheckInput Triggered!");
            int previously = self.input[0].x;
            orig(self);

            // Undoes the input cancellation
            if(self.bodyMode == Player.BodyModeIndex.WallClimb && self.superLaunchJump > 5 && self.input[0].jmp && self.input[1].jmp && self.input[0].y < 1){
                if (self.input[0].x == 0){
                    self.input[0].x = previously;
                }
            }
        }




        // Implement a different type of dropkick
        private void Escort_TossObject(On.Player.orig_TossObject orig, Player self, int grasp, bool eu){
            orig(self, grasp, eu);
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    return;
                }
                if (!BodySlam.TryGet(self, out var bodySlam) ||
                    !eCon.TryGetValue(self, out Escort e)){
                    return;
                }

                Ebug(self, "Toss Object Triggered!");
                if (self.grasps[grasp].grabbed is Lizard lizzie && !lizzie.dead){
                    if (Esconfig_SFX(self) && e.LizGet != null){
                        e.LizGet.Volume = 0f;
                    }
                    if (self.bodyMode == Player.BodyModeIndex.Default && (!e.Brawler || e.BrawThrowGrab == 0)){
                        self.animation = Player.AnimationIndex.RocketJump;
                        self.bodyChunks[1].vel.x += self.ThrowDirection;
                    }
                }
            } catch (Exception err){
                Ebug(self, err);
                return;
            }
        }


        private void Escort_Die(On.Player.orig_Die orig, Player self){
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self);
                    return;
                }
                if (!eCon.TryGetValue(self, out Escort e)){
                    orig(self);
                    return;
                }

                Ebug(self, "Die Triggered!");
                if (!e.ParrySuccess && e.iFrames == 0){
                    orig(self);
                    if (self.dead && Esconfig_SFX(self) && self.room != null){
                        self.room.PlaySound(Escort_SFX_Death, e.SFXChunk);
                        //self.room.PlayCustomSound("escort_failure", self.mainBodyChunk.pos, 0.7f, 1f);
                    }
                    Ebug(self, "Failure.", 1);
                } else {
                    self.dead = false;
                    Ebug(self, "Player didn't die?", 1);
                    e.ParrySuccess = false;
                }
            } catch (Exception err){
                Ebug(self, err, "Something happened while trying to die!");
                orig(self);
            }
        }

        private void Escort_Eated(On.Player.orig_BiteEdibleObject orig, Player self, bool eu)
        {
            try{
                if (self.slugcatStats.name.value != "EscortMe"){
                    orig(self, eu);
                    return;
                }
                for (int a = 0; a < 2; a++){
                    if (self.grasps[a] != null && self.grasps[a].grabbed is IPlayerEdible ipe && ipe.Edible){
                        if (ipe.BitesLeft > 1){
                            if (self.grasps[a].grabbed is Fly){
                                for (int b = 0; b < ipe.BitesLeft; b++){
                                    ipe.BitByPlayer(self.grasps[a], eu);
                                }
                            } else {
                                ipe.BitByPlayer(self.grasps[a], eu);
                            }
                        }
                        break;
                    }
                }
                orig(self, eu);
            } catch (Exception err){
                Ebug(self, err, "Error when eated!");
                orig(self, eu);
                return;
            }
        }


    }
}
