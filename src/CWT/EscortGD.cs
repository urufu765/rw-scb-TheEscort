using System;
using System.Collections.Generic;
using UnityEngine;
using R = UnityEngine.Random;
using static TheEscort.Eshelp;
using IL.MoreSlugcats;

namespace TheEscort
{
    public partial class Escort
    {
        /// <summary>
        /// Colour of Gilded
        /// </summary>
        public Color GildedColor;
        /// <summary>
        /// Whether Gilded is levitating
        /// </summary>
        public bool GildFloatState;
        /// <summary>
        /// Gilded's additional air time when jumping
        /// </summary>
        public int GildMoonJump;
        /// <summary>
        /// Limit to for how many frames Gilded is allowed to moonjump (may allow change later)
        /// </summary>
        public int GildMoonJumpMax = 20;
        /// <summary>
        /// Stomp in action
        /// </summary>
        public bool GildCrush;
        /// <summary>
        /// Delay before stomp happens
        /// </summary>
        public int GildCrushTime;
        /// <summary>
        /// Whether Gilded can stomp (stops them from stomping midair)
        /// </summary>
        public bool GildCrushReady;
        /// <summary>
        /// Gilded's current power
        /// </summary>
        public int GildPower;
        /// <summary>
        /// Gilded's max power (changes depending on malnourished state) (may allow players to customize max power)
        /// </summary>
        public int GildPowerMax;
        /// <summary>
        /// Gilded's power when an ability that requires power is used (remains stationary to allow the current power to change values while not affecting calculations)
        /// </summary>
        public int GildStartPower;
        /// <summary>
        /// Gilded's limit as to how long they can levitate. Changes depending on max karma (becoming limitless (whatever current power is at) on 10)
        /// </summary>
        public int GildLevitateLimit;
        /// <summary>
        /// Stops Gilded's power from going up
        /// </summary>
        public bool GildLockRecharge;
        /// <summary>
        /// Indicates Gilded's "stop & refund" state
        /// </summary>
        public bool GildCancel;
        /// <summary>
        /// How much power is deducted from current power (live transfer) so that the exact amount can be refunded if cancelled
        /// </summary>
        public int GildReservePower;
        /// <summary>
        /// How much power is required to use a certain ability
        /// </summary>
        public int GildRequiredPower;
        /// <summary>
        /// How much power is used per tick
        /// </summary>
        public int GildPowerUsage;
        /// <summary>
        /// Grasp that Gilded wants to throw with
        /// </summary>
        public int GildWantToThrow;
        /// <summary>
        /// Indicates the end of ability use, clearing the temporary reserve to commit to spending the power
        /// </summary>
        public bool GildClearReserve;
        /// <summary>
        /// If positive, object that Gilded was crafting is insta-completed
        /// </summary>
        public int GildInstaCreate;
        /// <summary>
        /// Stops players from spamming stomp
        /// </summary>
        public int GildCrushCooldown;
        /// <summary>
        /// Stops levitate from being annoying when it decides to activate/deactivate repeatedly
        /// </summary>
        public int GildLevitateCooldown;
        /// <summary>
        /// Lets Gilded craft another object in a different hand without needing to let go of the craft button
        /// </summary>
        public bool GildAlsoPop;
        /// <summary>
        /// Gilded jetpack clouds delay
        /// </summary>
        public int GildJetPackVFX;
        /// <summary>
        /// Index which Gilded's power pip sprites are stored
        /// </summary>
        public int GildPowerPipsIndex;
        /// <summary>
        /// Max number of power pip sprites
        /// </summary>
        public int GildPowerPipsMax;
        /// <summary>
        /// Grants Gilded bonus and enhanced abilities
        /// </summary>
        public bool GildOverpowered;
        /// <summary>
        /// Resets relevant Gilded variables (that aren't init-only)
        /// </summary>
        public bool GildNeedsToReset;

        // Power check
        public const int GildCheckCraftFirebomb = 800;
        public const int GildCheckCraftFirespear = 2000;
        public const int GildCheckCraftSingularity = 400;

        // Power usage (per tick)
        public const int GildUseLevitate = 4;
        public const int GildUseCraftFirebomb = 10;
        public const int GildUseCraftFirespear = 15;
        public const int GildUseCraftSingularity = 10;
        
        public void EscortGD(Player self)
        {
            Gilded = false;
            GildedColor = new Color(0.122f, 0.176f, 0.28f);
            GildFloatState = false;
            GildLevitateLimit = 120;
            GildPowerMax = (int)(Plugin.ins.config.cfgGildedMaxPower.Value * (self.Malnourished? 0.625 : 1));
            GildPowerPipsMax = self.Malnourished? 10: 16;
            GildPowerPipsIndex = -1;
            Escat_GD_reset_vars();
        }

        public void Escat_GD_reset_vars(bool fromDeath = false)
        {
            GildPower = GildStartPower = fromDeath? 0 : (int)(GildPowerMax * 0.3f);
            GildMoonJump = GildReservePower = GildRequiredPower = GildPowerUsage = GildInstaCreate = GildCrushTime = 0;
            GildCancel = GildLockRecharge = GildNeedsToReset = false;
        }

        public void Escat_float_state(Player self, bool status = true){
            if (status){
                GildFloatState = true;
                self.wantToJump = 0;
                self.room?.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Cap_Bump_Vengeance, SFXChunk, false, 0.72f, 6.5f);
                for (int i = 0; i < 7; i++){
                    self.room?.AddObject(new WaterDrip(self.bodyChunks[1].pos, RWCustom.Custom.DegToVec(R.value * 360) * Mathf.Lerp(4, 20, R.value), false));
                }
            }
            else{
                self.room?.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Cap_Bump_Vengeance, SFXChunk, loop: false, 0.72f, 5.5f);
                GildFloatState = false;
            }
        }

        #if false
        public void Escat_RGB_firespear()
        {
            try
            {
                for (int i = 0; i < GildRainbowFirespear.Count; i++)
                {
                    if (GildRainbowFirespear[i] is null) 
                    {
                        GildRainbowFirespear.RemoveAt(i);
                        continue;
                    }
                    GildRainbowFirespear[i].abstractSpear.hue += 1/360;
                    if (GildRainbowFirespear[i].abstractSpear.hue >= 360) GildRainbowFirespear[i].abstractSpear.hue = 1/360;
                }
            }
            catch (NullReferenceException nre)
            {
                Ebug(nre, "Null exception when doing RGB spears?!");
            }
            catch (Exception err)
            {
                Ebug(err, "Generic exception when doing RGB spears.");
            }
        }
        #endif
    }
}