using BepInEx;
using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static TheEscort.Eshelp;
using static RWCustom.Custom;

namespace TheEscort
{
    partial class Plugin : BaseUnityPlugin
    {
        public static Dictionary<int, string> selectionable = new() { 
            {  0, rainWorld.inGameTranslator.Translate("Default") }, 
            { -1, rainWorld.inGameTranslator.Translate("Brawler") }, 
            { -2, rainWorld.inGameTranslator.Translate("Deflector") }, 
            { -3, rainWorld.inGameTranslator.Translate("Escapist") }, 
            { -4, rainWorld.inGameTranslator.Translate("Railgunner") }, 
            { -5, rainWorld.inGameTranslator.Translate("Speedster") }, 
            { -6, rainWorld.inGameTranslator.Translate("Gilded") } 
        };
        public static UIelementWrapper[] hackyWrapper;
        public static UIelementWrapper[] fairlyIllegalWrapper;
        //public static UIelementWrapper[] wackyWrapper;
        //public static UIelementWrapper[] somewhatIllegalWrapper;
        private static float[] escortRGBTick = new float[4];
        private static Color[] escortRGBStore = new Color[4];

#region Escort Graphics
        private void Escort_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam)
        {
            ins.L().Set();
            orig(self, s, rCam);
            try
            {
                if (!(self != null && self.player != null))
                {
                    return;
                }
                ins.L().Set("Null Check");
                if (self.player.slugcatStats.name != EscortMe)
                {
                    return;
                }
                ins.L().Set("Escort Check");
                if (!eCon.TryGetValue(self.player, out Escort e))
                {
                    return;
                }
                ins.L().Set("Escort/Socks CWT Access");
                e.Escat_setIndex_sprite_cue(ref e.mainSpriteIndex, s.sprites.Length);
                Array.Resize(ref s.sprites, s.sprites.Length + e.mainSprites);
                // Store the end index of the sprites so none are overwritten!
                s.sprites[e.mainSpriteIndex] = new FSprite("escortHeadT");
                s.sprites[e.mainSpriteIndex + 1] = new FSprite("escortHipT");

                if (e.Gilded) Esclass_GD_InitiateSprites(self, s, rCam, ref e);

                // When builds have custom sprites, do an if condition and add accordingly
                for (int i = e.mainSpriteIndex; i < s.sprites.Length; i++)
                {
                    if (s.sprites[e.mainSpriteIndex] == null)
                    {
                        Ebug(self.player, "Oh geez. No sprites?", 0);
                    }
                }
                ins.L().Set("Successful Spriting Check");
                self.AddToContainer(s, rCam, null);
            }
            catch (Exception err)
            {
                Ebug(self.player, err, "Something went wrong when initiating sprites!");
                return;
            }
        }

        private void Escort_ApplyPalette(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, s, rCam, palette);
            try
            {
                if (!(self != null && self.player != null))
                {
                    return;
                }
                if (self.player.slugcatStats.name.value != "EscortMe")
                {
                    return;
                }
                if (!eCon.TryGetValue(self.player, out Escort e))
                {
                    return;
                }

                if (e.mainSpriteIndex == -1)
                {
                    return;
                }
                if (e.mainSpriteIndex + 2 == s.sprites.Length && (s.sprites[e.mainSpriteIndex] == null || s.sprites[e.mainSpriteIndex + 1] == null))
                {
                    Ebug(self.player, "Oh dear. Null sprites!!", 0);
                    return;
                }
                Color c = new(0.796f, 0.549f, 0.27843f);
                // Applying colors?
                if (s.sprites.Length > e.mainSpriteIndex)
                {
                    //Ebug(self.player, "Gone in", 2);
                    if (ModManager.CoopAvailable && self.useJollyColor)
                    {
                        //Ebug(self.player, "Jollymachine", 2);
                        c = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                        //Ebug(self.player, "R: " + c.r + " G: " + c.g + " B: " + c.b);
                        //Ebug(self.player, "Jollymachine end", 2);
                    }
                    else if (PlayerGraphics.CustomColorsEnabled())
                    {
                        Ebug(self.player, "Custom color go brr", 2);
                        c = PlayerGraphics.CustomColorSafety(2);
                        Ebug(self.player, "Custom color end", 2);
                    }
                    else
                    {
                        //==Ebug(self.player, "Arenasession or Singleplayer", 2);

                        if (rCam.room.game.IsArenaSession && !rCam.room.game.setupValues.arenaDefaultColors)
                        {
                            switch (self.player.playerState.playerNumber)
                            {
                                case 0:
                                    if (rCam.room.game.IsArenaSession && rCam.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType != MoreSlugcats.MoreSlugcatsEnums.GameTypeID.Challenge && !nonArena)
                                    {
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
                if (c.r <= 0.02f && c.g <= 0.02f && c.b <= 0.02f)
                {
                    e.secretRGB = true;
                }
                for (int i = e.mainSpriteIndex; i < s.sprites.Length; i++)
                {
                    s.sprites[i].color = e.Escat_runit_thru_RGB(c);
                }
                if (!e.secretRGB)
                {
                    e.hypeColor = c;
                }
                if (e.hypeLight == null || e.hypeSurround == null)
                {
                    e.Escat_setLight_hype(self.player, e.hypeColor);
                }
                else
                {
                    e.hypeLight.color = c;
                    e.hypeSurround.color = c;
                }
            }
            catch (Exception err)
            {
                Ebug(self.player, err, "Something went wrong when coloring in the palette!");
                return;
            }
        }

        private void Escort_AddGFXContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, FContainer newContainer)
        {
            orig(self, s, rCam, newContainer);
            try
            {
                if (!(self != null && self.player != null))
                {
                    return;
                }
                if (self.player.slugcatStats.name.value != "EscortMe")
                {
                    return;
                }
                if (!eCon.TryGetValue(self.player, out Escort e))
                {
                    return;
                }
                if (e.mainSpriteIndex == -1)
                {
                    return;
                }
                if (e.mainSpriteIndex + 2 == s.sprites.Length && (s.sprites[e.mainSpriteIndex] == null || s.sprites[e.mainSpriteIndex + 1] == null))
                {
                    Ebug(self.player, "Oh shoot. Where sprites?", 0);
                    return;
                }
                if (e.mainSpriteIndex < s.sprites.Length)
                {
                    rCam.ReturnFContainer("Foreground").RemoveChild(s.sprites[e.mainSpriteIndex]);
                    rCam.ReturnFContainer("Foreground").RemoveChild(s.sprites[e.mainSpriteIndex + 1]);
                    Ebug(self.player, "Removal success.", 1);
                    rCam.ReturnFContainer("Midground").AddChild(s.sprites[e.mainSpriteIndex]);
                    rCam.ReturnFContainer("Midground").AddChild(s.sprites[e.mainSpriteIndex + 1]);
                    Ebug(self.player, "Addition success.", 1);
                    s.sprites[e.mainSpriteIndex].MoveBehindOtherNode(s.sprites[9]);
                    s.sprites[e.mainSpriteIndex + 1].MoveBehindOtherNode(s.sprites[3]);
                    //Ebug(self.player, "Restructure success.", 1);
                }
                if (e.Gilded) Esclass_GD_AddTaCantaina(self, s, rCam, ref e);
                /*
                foreach (EscortHUD.Traction traction in e.ringTrackers)
                {
                    if (rCam is null){
                        Ebug("Rcam is null!");
                        break;
                    }
                    else if (rCam.hud is null)
                    {
                        Ebug("HUD is null!");
                        break;
                    }
                    else if (rCam.hud.parts is null)
                    {
                        Ebug("Parts is null!");
                        break;
                    }
                    rCam.hud.parts.Add(new EscortHUD.ProgressionRing(rCam.hud, traction));
                }*/
            }
            catch (Exception err)
            {
                Ebug(self.player, err, "Something went wrong when adding graphics to container!");
                return;
            }
        }

        private void Escort_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser s, RoomCamera rCam, float t, Vector2 camP)
        {
            try
            {
                if (!(self != null && self.player != null))
                {
                    orig(self, s, rCam, t, camP);
                    return;
                }
                if (self.player.slugcatStats.name.value != "EscortMe")
                {
                    orig(self, s, rCam, t, camP);
                    return;
                }
                if (!headDraw.TryGet(self.player, out var hD) || !bodyDraw.TryGet(self.player, out var bD) || !eCon.TryGetValue(self.player, out Escort e))
                {
                    orig(self, s, rCam, t, camP);
                    return;
                }
                if (e.mainSpriteIndex == -1)
                {
                    orig(self, s, rCam, t, camP);
                    return;
                }
                if (e.mainSpriteIndex + 2 == s.sprites.Length && (s.sprites[e.mainSpriteIndex] == null || s.sprites[e.mainSpriteIndex + 1] == null))
                {
                    orig(self, s, rCam, t, camP);
                    Ebug(self.player, "Oh crap. Sprites? Hello?!", 0);
                    return;
                }
                if (s.sprites.Length > 9 && s.sprites[1] != null)
                {
                    e.hipScaleX = s.sprites[1].scaleX;
                    e.hipScaleY = s.sprites[1].scaleY;
                }
                else
                {
                    e.hipScaleX = bD[0];
                    e.hipScaleY = bD[0];
                }
                orig(self, s, rCam, t, camP);
                if (s.sprites.Length > e.mainSpriteIndex)
                {
                    // Hypelevel visual fx
                    try
                    {
                        if (self.player != null && Esconfig_Hypable(self.player))
                        {
                            float alphya = 1f;
                            if (hypeRequirement > self.player.aerobicLevel)
                            {
                                alphya = Mathf.Lerp(self.player.dead ? 0f : 0.57f, 1f, Mathf.InverseLerp(0f, hypeRequirement, self.player.aerobicLevel));
                            }
                            for (int a = e.mainSpriteIndex; a < s.sprites.Length; a++)
                            {
                                s.sprites[a].alpha = alphya;
                                s.sprites[a].color = e.Escat_runit_thru_RGB(e.hypeColor, hypeRequirement < self.player.aerobicLevel ? 8f : Mathf.Lerp(1f, 4f, Mathf.InverseLerp(0f, hypeRequirement, self.player.aerobicLevel))) * Mathf.Lerp(1f, 1.8f, Mathf.InverseLerp(0f, 15f, e.smoothTrans));
                            }
                            s.sprites[e.mainSpriteIndex + 1].scaleX = e.hipScaleX;
                            s.sprites[e.mainSpriteIndex + 1].scaleY = e.hipScaleY;
                            if (e.hypeLight != null && e.hypeSurround != null)
                            {
                                float alpine = 0f;
                                float alpite = 0f;

                                if (hypeRequirement > self.player.aerobicLevel)
                                {
                                    alpine = Mathf.Lerp(0f, 0.08f, Mathf.InverseLerp(0f, hypeRequirement, self.player.aerobicLevel));
                                    alpite = Mathf.Lerp(0f, 0.2f, Mathf.InverseLerp(0f, hypeRequirement, self.player.aerobicLevel));
                                }
                                else
                                {
                                    alpine = 0.1f;
                                    alpite = 0.3f;
                                }
                                e.hypeLight.stayAlive = true;
                                e.hypeSurround.stayAlive = true;
                                e.hypeLight.setPos = self.player.mainBodyChunk.pos;
                                e.hypeSurround.setPos = self.player.bodyChunks[0].pos;
                                e.hypeLight.setAlpha = alpine;
                                e.hypeSurround.setAlpha = alpite;
                                if (e.secretRGB)
                                {
                                    e.hypeLight.color = e.hypeColor * Mathf.Lerp(1f, 2f, Mathf.InverseLerp(0f, 15f, e.smoothTrans));
                                    e.hypeSurround.color = e.hypeColor * Mathf.Lerp(1f, 2f, Mathf.InverseLerp(0f, 15f, e.smoothTrans));
                                }
                            }
                            else
                            {
                                e.Escat_setLight_hype(self.player, e.hypeColor);
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        Ebug(self.player, err, "something went wrong when altering alpha");
                    }
                    s.sprites[e.mainSpriteIndex].rotation = s.sprites[9].rotation;
                    s.sprites[e.mainSpriteIndex + 1].rotation = s.sprites[1].rotation;
                    s.sprites[e.mainSpriteIndex].scaleX = hD[0];
                    //s.sprites[e.spriteQueue + 1].scaleX = bD[0];
                    if (self.player.animation == Player.AnimationIndex.Flip || self.player.animation == Player.AnimationIndex.Roll)
                    {
                        Vector2 vectoria = RWCustom.Custom.DegToVec(s.sprites[9].rotation) * hD[1];
                        Vector2 vectorib = RWCustom.Custom.DegToVec(s.sprites[1].rotation) * bD[1];
                        s.sprites[e.mainSpriteIndex].x = s.sprites[9].x + vectoria.x;
                        s.sprites[e.mainSpriteIndex].y = s.sprites[9].y + vectoria.y;
                        s.sprites[e.mainSpriteIndex + 1].x = s.sprites[1].x + vectorib.x;
                        s.sprites[e.mainSpriteIndex + 1].y = s.sprites[1].y + vectorib.y;
                    }
                    else
                    {
                        s.sprites[e.mainSpriteIndex].x = s.sprites[9].x + hD[2];
                        s.sprites[e.mainSpriteIndex].y = s.sprites[9].y + hD[3];
                        s.sprites[e.mainSpriteIndex + 1].x = s.sprites[1].x + bD[2];
                        s.sprites[e.mainSpriteIndex + 1].y = s.sprites[1].y + bD[3];
                    }
                }
                if (e.Speedster) Esclass_SS_DrawSprites(self, s, rCam, t, camP, ref e);
                if (e.Gilded) Esclass_GD_DrawPipSprites(self, s, rCam, t, camP, ref e);
                e.Escat_Update_Ring_Trackers(t);
            }
            catch (Exception err)
            {
                orig(self, s, rCam, t, camP);
                Ebug(self.player, err, "Something happened while trying to draw sprites!");
            }
        }

        private Color Escort_Colorz(On.PlayerGraphics.orig_DefaultSlugcatColor orig, SlugcatStats.Name i)
        {
            try {
                if (i == EscortMe){
                    return new Color(0.255f, 0.412f, 0.882f);
                }
                return orig(i);
            } catch (NullReferenceException nre) {
                Ebug(nre, "Null found when setting default scug flavor!");
                return orig(i);
            } catch (Exception err) {
                Ebug(err, "Generic error found when setting scug flavor!");
                return orig(i);
            }
        }


#endregion

#region Escort Jolly UI
        private static bool Escort_Jolly_Sprite(On.JollyCoop.JollyMenu.SymbolButtonTogglePupButton.orig_HasUniqueSprite orig, JollyCoop.JollyMenu.SymbolButtonTogglePupButton self)
        {
            Ebug("Woof woof");
            if (self.symbolNameOff.Contains("escortme")) return true;
            return orig(self);
        }

        private static string Escort_Jolly_Name(On.JollyCoop.JollyMenu.JollyPlayerSelector.orig_GetPupButtonOffName orig, JollyCoop.JollyMenu.JollyPlayerSelector self)
        {
            SlugcatStats.Name playerClass = self.JollyOptions(self.index).playerClass;
            if (playerClass != null && playerClass.value.Equals("EscortMe"))
            {
                return "escortme_pup_off";
            }
            return orig(self);
        }

        private static Color Escort_Please_Just_Kill_Me(On.PlayerGraphics.orig_JollyUniqueColorMenu orig, SlugcatStats.Name slugName, SlugcatStats.Name reference, int playerNumber)
        {
            if ((RWCustom.Custom.rainWorld.options.jollyColorMode == Options.JollyColorMode.DEFAULT || (playerNumber == 0 && RWCustom.Custom.rainWorld.options.jollyColorMode == Options.JollyColorMode.AUTO)) && slugName == EscortMe)
            {
                return new Color(0.796f, 0.549f, 0.27843f);
            }
            else
            {
               return orig(slugName, reference, playerNumber);
            }
            /*
            if ((RWCustom.Custom.rainWorld.options.jollyColorMode == Options.JollyColorMode.DEFAULT || (playerNumber == 0 && RWCustom.Custom.rainWorld.options.jollyColorMode == Options.JollyColorMode.AUTO)) && slugName == EscortMe)
            {
                escortRGBStore[playerNumber] = new Color(0.796f, 0.549f, 0.27843f);
            }
            else
            {
                escortRGBStore[playerNumber] = orig(slugName, reference, playerNumber);
            }
            return escortRGBStore[playerNumber];*/
        }

        private static void Escort_RGBRGBRGB_GoesBrr(On.JollyCoop.JollyMenu.SymbolButtonTogglePupButton.orig_Update orig, JollyCoop.JollyMenu.SymbolButtonTogglePupButton self)
        {
            orig(self);
            if (self.HasUniqueSprite() && self.symbolNameOff == "escortme_pup_off")
            {
                string[] signalTexts = self.signalText.Split('_');
                if (!int.TryParse("" + signalTexts[signalTexts.Length - 2], out int index))
                {
                    Ebug("RGB Failed! Try parsed: ");
                    Ebug(signalTexts);
                    return;
                }
                if (escortRGBStore[index].r < 0.05f &&
                    escortRGBStore[index].g < 0.05f &&
                    escortRGBStore[index].b < 0.05f
                ){
                    self.uniqueSymbol.color = Eshelp_cycle_dat_RGB(ref escortRGBTick[index]);
                    //Ebug("GUESS WHAT? RGB!");
                    return;
                }
                //Ebug("Not RGB state!");
            }
        }

        private static void EscortBuildSelectFromJollyMenu(On.JollyCoop.JollyMenu.JollySlidingMenu.orig_ctor orig, JollyCoop.JollyMenu.JollySlidingMenu self, JollyCoop.JollyMenu.JollySetupDialog menu, MenuObject owner, Vector2 pos)
        {
            orig(self, menu, owner, pos);
            // I'm not calculating this crap.
            int num = 100;
            float num2 = (1024f - num * 4f) / 5f;
            float num3 = 171f;
            Vector2 vector = new(num3 + num2, 0f);
            Vector2 vector2 = vector + new Vector2(0f, menu.manager.rainWorld.screenSize.y * 0.55f) + new Vector2(0f, -106.5f);

            // Creates custom buttons that are independent from Configurables such that they don't have to have conflicts and such. It's janky but it's also flexible and very compatible. The button and the remix option can share a configurable!
            ins.config.jollyEscortBuilds = new OpSimpleButton[4];
            ins.config.jollyEscortEasies = new OpSimpleButton[4];
            hackyWrapper = new UIelementWrapper[4];  // UIElementwrapper to make it work with jolly coop menu lol
            fairlyIllegalWrapper = new UIelementWrapper[4];

            for (int i = 0; i < 4; i++)
            {
                ins.config.jollyEscortBuilds[i] = new OpSimpleButton(  // Set up build switching button
                    vector2, new Vector2(100f, 30f)
                )
                {
                    text = selectionable[ins.config.cfgBuild[i].Value],
                    /*
                    text = i switch
                    {
                        0 => selectionable[ins.config.cfgBuildP1.Value],
                        1 => selectionable[ins.config.cfgBuildP2.Value],
                        2 => selectionable[ins.config.cfgBuildP3.Value],
                        _ => selectionable[ins.config.cfgBuildP4.Value]
                    },
                    */
                    description = "Change Escort's Build, which affects how they play significantly! You can also set these values in the Remix Settings!",
                    greyedOut = i >= menu.manager.rainWorld.options.JollyPlayerCount

                };
                ins.config.jollyEscortBuilds[i].OnClick += Eshelp_Set_Jolly_To_Remix;
                ins.config.jollyEscortEasies[i] = new OpSimpleButton(  // Set up easier mode toggle button
                    vector2 + new Vector2(105.5f, 0f), new Vector2(30f, 30f)
                )
                {
                    text = ins.config.cfgEasy[i].Value? "X" : "",
                    /*
                    text = i switch
                    {
                        0 => ins.config.cfgEasyP1.Value ? "X" : "",
                        1 => ins.config.cfgEasyP2.Value ? "X" : "",
                        2 => ins.config.cfgEasyP3.Value ? "X" : "",
                        _ => ins.config.cfgEasyP4.Value ? "X" : ""
                    },
                    */
                    description = "Easier Mode: While midair and moving, press Jump + Grab to do a dropkick!",
                    colorEdge = ins.config.easyColor,
                    greyedOut = i >= menu.manager.rainWorld.options.JollyPlayerCount
                };
                ins.config.jollyEscortEasies[i].OnClick += Eshelp_Set_Jolly_To_Easier_Remix;
                fairlyIllegalWrapper[i] = new UIelementWrapper(menu.tabWrapper, ins.config.jollyEscortEasies[i]);
                self.subObjects.Add(fairlyIllegalWrapper[i]);
                hackyWrapper[i] = new UIelementWrapper(menu.tabWrapper, ins.config.jollyEscortBuilds[i]);
                vector2 += new Vector2(num2 + num, 0f);
                self.subObjects.Add(hackyWrapper[i]);
            }
            self.UpdatePlayerSlideSelectable(menu.manager.rainWorld.options.JollyPlayerCount - 1);
        }

        private static void EscortPleaseSaveTheGoddamnConfigs(On.JollyCoop.JollyMenu.JollySetupDialog.orig_RequestClose orig, JollyCoop.JollyMenu.JollySetupDialog self)
        {
            try{
                if (!self.closing){
                    ins.config._SaveConfigFile();
                    
                    Ebug("Saving configs!");
                }
            } catch (NullReferenceException nre){
                Ebug(nre, "EscortsavingJOLLYCONFIGS: OH CRAP A NULL EXCEPTOIN");
            } catch (Exception err){
                Ebug(err, "EscortsavingJOLLYCONFIGS: Oh god an different error help me");
            } finally {
                orig(self);
            }
        }


        private static void Eshelp_Set_Jolly_To_Easier_Remix(UIfocusable trigger)
        {
            int index = -1;
            for (int i = 0; i < ins.config.jollyEscortEasies.Length; i++)
            {
                if (ins.config.jollyEscortEasies[i].bumpBehav == trigger.bumpBehav)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                Ebug("Couldn't find button!");
                return;
            }
            ins.config.cfgEasy[index].Value = !ins.config.cfgEasy[index].Value;
            ins.config.jollyEscortEasies[index].text = ins.config.cfgEasy[index].Value? "X" : "";
            /*
            switch (index)
            {
                case 0:
                    ins.config.cfgEasyP1.Value = !ins.config.cfgEasyP1.Value;
                    ins.config.jollyEscortEasies[index].text = ins.config.cfgEasyP1._typedValue ? "X" : "";
                    break;
                case 1:
                    ins.config.cfgEasyP2.Value = !ins.config.cfgEasyP2.Value;
                    ins.config.jollyEscortEasies[index].text = ins.config.cfgEasyP2._typedValue ? "X" : "";
                    break;
                case 2:
                    ins.config.cfgEasyP3.Value = !ins.config.cfgEasyP3.Value;
                    ins.config.jollyEscortEasies[index].text = ins.config.cfgEasyP3._typedValue ? "X" : "";
                    break;
                case 3:
                    ins.config.cfgEasyP4.Value = !ins.config.cfgEasyP4.Value;
                    ins.config.jollyEscortEasies[index].text = ins.config.cfgEasyP4._typedValue ? "X" : "";
                    break;
            }*/
        }

        private static void EscortGrayedOutLikeAnIdiot(On.JollyCoop.JollyMenu.JollySlidingMenu.orig_UpdatePlayerSlideSelectable orig, JollyCoop.JollyMenu.JollySlidingMenu self, int pIndex)
        {
            orig(self, pIndex);
            if (!(ins != null && ins.config != null && ins.config.jollyEscortBuilds != null && ins.config.jollyEscortBuilds.Length > 0))
            {
                return;
            }
            for (int j = 0; j < ins.config.jollyEscortBuilds.Length; j++)
            {
                ins.config.jollyEscortBuilds[j].greyedOut = false;
                ins.config.jollyEscortEasies[j].greyedOut = false;
            }
            for (int i = 3; i > pIndex; i--)
            {
                ins.config.jollyEscortBuilds[i].greyedOut = true;
                ins.config.jollyEscortEasies[i].greyedOut = true;
            }
        }


        private static void Eshelp_Set_Jolly_To_Remix(UIfocusable trigger)
        {
            int index = -1;
            for (int i = 0; i < ins.config.jollyEscortBuilds.Length; i++)
            {
                if (ins.config.jollyEscortBuilds[i].bumpBehav == trigger.bumpBehav)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                Ebug("Couldn't find button!");
                return;
            }
            if (ins.config.cfgBuild[index].Value - 1 < ins.config.buildDiv) ins.config.cfgBuild[index].Value = 0;
            else ins.config.cfgBuild[index].Value--;
            ins.config.jollyEscortBuilds[index].text = selectionable[ins.config.cfgBuild[index].Value];
            /*
            switch (index)
            {
                case 0:
                    if (ins.config.cfgBuildP1.Value - 1 < ins.config.buildDiv) ins.config.cfgBuildP1.Value = 0;
                    else ins.config.cfgBuildP1.Value--;
                    //ins.config.cfgBuildP1.BoxedValue = ins.config.cfgBuildP1.Value;
                    //ValueConverter.ConvertToString(ins.config.cfgBuildP1.Value, ins.config.cfgBuildP1.settingType);
                    ins.config.jollyEscortBuilds[0].text = selectionable[ins.config.cfgBuildP1.Value];
                    break;
                case 1:
                    if (ins.config.cfgBuildP2.Value - 1 < ins.config.buildDiv) ins.config.cfgBuildP2.Value = 0;
                    else ins.config.cfgBuildP2.Value--;
                    ins.config.jollyEscortBuilds[1].text = selectionable[ins.config.cfgBuildP2.Value];
                    break;
                case 2:
                    if (ins.config.cfgBuildP3.Value - 1 < ins.config.buildDiv) ins.config.cfgBuildP3.Value = 0;
                    else ins.config.cfgBuildP3.Value--;
                    ins.config.jollyEscortBuilds[2].text = selectionable[ins.config.cfgBuildP3.Value];
                    break;
                case 3:
                    if (ins.config.cfgBuildP4.Value - 1 < ins.config.buildDiv) ins.config.cfgBuildP4.Value = 0;
                    else ins.config.cfgBuildP4.Value--;
                    ins.config.jollyEscortBuilds[3].text = selectionable[ins.config.cfgBuildP4.Value];
                    break;
            }*/
        }

        private static void EscortHideShowBuildCopium(On.JollyCoop.JollyMenu.JollyPlayerSelector.orig_Update orig, JollyCoop.JollyMenu.JollyPlayerSelector self)
        {
            orig(self);
            if (ins.config.jollyEscortBuilds.Length > 0)
            {
                if (self.slugName == EscortMe) { 
                    ins.config.jollyEscortBuilds[self.index].Reactivate(); 
                    ins.config.jollyEscortEasies[self.index].Reactivate(); 
                } else { 
                    ins.config.jollyEscortBuilds[self.index].Deactivate(); 
                    ins.config.jollyEscortEasies[self.index].Deactivate(); 
                }
            }
        }
#endregion

/*
#region Arena Mode UI
        private static void Escort_Arena_Class_Changer(On.Menu.MultiplayerMenu.orig_InitiateGameTypeSpecificButtons orig, Menu.MultiplayerMenu self)
        {
            orig(self);
            ins.config.arenaEscortBuilds = new OpSimpleButton[4];
            //ins.config.arenaEscortEasies = new OpSimpleButton[4];
            wackyWrapper = new UIelementWrapper[4];  // UIElementwrapper to make it work with jolly coop menu lol
            somewhatIllegalWrapper = new UIelementWrapper[4];
            if (self.currentGameType == ArenaSetup.GameTypeID.Sandbox || self.currentGameType == ArenaSetup.GameTypeID.Competitive){
                for (int i = 0; i < 4; i++){
                    ins.config.arenaEscortBuilds[i] = new(
                        new Vector2(580f + (float)i * 120f, 500f) + new Vector2(106f, -60f) - new Vector2((120f - 120f) * (float)self.playerClassButtons.Length, 0f), new Vector2(100f, 30f)
                    ){
                        text = i switch
                        {
                            0 => selectionable[ins.config.cfgBuildP1.Value],
                            1 => selectionable[ins.config.cfgBuildP2.Value],
                            2 => selectionable[ins.config.cfgBuildP3.Value],
                            _ => selectionable[ins.config.cfgBuildP4.Value]
                        },
                        description = "Change Escort's Build, which affects how they play significantly! You can also set these values in the Remix Settings!"
                    };
                    ins.config.arenaEscortBuilds[i].OnClick += Eshelp_Set_Arena_To_Remix;
                    wackyWrapper[i] = new UIelementWrapper(self.);
                    self.pages[0].subObjects.Add(wackyWrapper[i]);
                }
            }
        }

        private static void Eshelp_Set_Arena_To_Remix(UIfocusable trigger)
        {
            int index = -1;
            for (int i = 0; i < ins.config.arenaEscortBuilds.Length; i++)
            {
                if (ins.config.arenaEscortBuilds[i].bumpBehav == trigger.bumpBehav)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                Ebug("Couldn't find button!");
                return;
            }
            switch (index)
            {
                case 0:
                    if (ins.config.cfgBuildP1.Value - 1 < ins.config.buildDiv) ins.config.cfgBuildP1.Value = 0;
                    else ins.config.cfgBuildP1.Value--;
                    ins.config.arenaEscortBuilds[0].text = selectionable[ins.config.cfgBuildP1.Value];
                    break;
                case 1:
                    if (ins.config.cfgBuildP2.Value - 1 < ins.config.buildDiv) ins.config.cfgBuildP2.Value = 0;
                    else ins.config.cfgBuildP2.Value--;
                    ins.config.arenaEscortBuilds[1].text = selectionable[ins.config.cfgBuildP2.Value];
                    break;
                case 2:
                    if (ins.config.cfgBuildP3.Value - 1 < ins.config.buildDiv) ins.config.cfgBuildP3.Value = 0;
                    else ins.config.cfgBuildP3.Value--;
                    ins.config.arenaEscortBuilds[2].text = selectionable[ins.config.cfgBuildP3.Value];
                    break;
                case 3:
                    if (ins.config.cfgBuildP4.Value - 1 < ins.config.buildDiv) ins.config.cfgBuildP4.Value = 0;
                    else ins.config.cfgBuildP4.Value--;
                    ins.config.arenaEscortBuilds[3].text = selectionable[ins.config.cfgBuildP4.Value];
                    break;
            }
        }

#endregion
*/
    }
}

