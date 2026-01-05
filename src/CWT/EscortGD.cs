using System;
using System.Collections.Generic;
using UnityEngine;
using R = UnityEngine.Random;
using static TheEscort.Eshelp;
using IL.MoreSlugcats;
using RWCustom;

namespace TheEscort
{
    public enum Charger
    {
        Normal,
        Ground,
        ZeroG,
        Gate,
        Shelter,
        Oracle,
        VoidSwim
    }
    public partial class Escort
    {
        public Color GildedColor;  // Color of Gilded
        public bool GildFloatState;  // Whether Gilded is levitating
        //public int GildFloatFloat;  OLD OUTDATED VARIABLE
        public int GildMoonJump;  // Gilded additional air time when jumping
        public int GildMoonJumpMax = 20;  // Limit to for how many frames Gilded is allowed to moonjump (may allow change later)
        public bool GildCrush;  // Stomp in action
        public int GildCrushTime;  // Delay before stomp happens
        public bool GildCrushReady;  // Whether Gilded can stomp (stops them from stomping midair)
        public int GildPower;  // Gilded's current power
        public int GildPowerMax;  // Gilded's max power (changes depending on malnourished state) (may allow players to customize max power)
        public int GildStartPower;  // Gilded's power when an ability that requires power is used (remains stationary to allow the current power to change values while not affecting calculations)
        public int GildLevitateLimit;  // Gilded's limit as to how long they can levitate. Changes depending on max karma (becoming limitless (whatever current power is at) on 10)
        public bool GildLockRecharge;  // Stops Gilded's power from going up
        public bool GildCancel;  // Indicates Gilded's "stop & refund" state
        public int GildReservePower;  // How much power is deducted from current power (live transfer) so that the exact amount can be refunded if cancelled
        public int GildRequiredPower;  // How much power is required to use a certain ability
        public int GildPowerUsage;  // How much power is used per tick
        public int GildWantToThrow;  // Grasp that Gilded wants to throw with
        public bool GildClearReserve;  // Indicates the end of ability use, clearing the temporary reserve to commit to spending the power
        public int GildInstaCreate;  // If positive, object that Gilded was crafting is insta-completed
        public int GildCrushCooldown;  // Stops players from spamming stomp
        public int GildLevitateCooldown;  // Stops levitate from being annoying when it decides to activate/deactivate repeatedly
        public bool GildAlsoPop;  // Lets Gilded craft another object in a different hand without needing to let go of the craft button
        public int GildJetPackVFX;  // Gilded jetpack clouds delay
        public int GildPowerPipsIndex;  // Index which Gilded's power pip sprites are stored
        public int GildPowerPipsMax;  // Max number of power pip sprites
        public bool GildOverpowered;  // Grants Gilded bonus and enhanced abilities
        public bool GildNeedsToReset;  // Resets relevant Gilded variables (that aren't init-only)
        /// <summary>
        /// For even slower charging, 1 full fine = 1 GildPower. Quite janky how it's done, but it's to keep maximum legacy compatibility
        /// </summary>
        public float GildFinePower;
        /// <summary>
        /// Determines how much Gilded's power meter fills up
        /// </summary>
        public Charger GildRechargeMode;

        private int _gildCheckRechargeTypeLater;

        /// <summary>
        /// Do the expensive room check once every 2 seconds
        /// </summary>
        public bool GildExpensiveCheck
        {
            get
            {
                if (_gildCheckRechargeTypeLater > 80) _gildCheckRechargeTypeLater = 0;
                return _gildCheckRechargeTypeLater++ == 1;
            }
        }

        public string GildLastRoomChecked;
        public Charger GildRoomCheck;

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
            GildPowerMax = (int)(Plugin.ins.config.cfgGildedMaxPower.Value * (self.Malnourished ? 0.625 : 1));
            GildPowerPipsMax = self.Malnourished ? 10 : 16;
            GildPowerPipsIndex = -1;
            GildLastRoomChecked = "";
            GildRechargeMode = Charger.Normal;
            GildRoomCheck = Charger.Normal;
            Escat_GD_reset_vars();
        }

        public void Escat_GD_reset_vars(bool fromDeath = false)
        {
            GildPower = GildStartPower = fromDeath ? 0 : (int)(GildPowerMax * 0.3f);
            GildMoonJump = GildReservePower = GildRequiredPower = GildPowerUsage = GildInstaCreate = GildCrushTime = 0;
            GildCancel = GildLockRecharge = GildNeedsToReset = false;
        }

        public void Escat_float_state(Player self, bool status = true)
        {
            if (status)
            {
                GildFloatState = true;
                self.wantToJump = 0;
                self.room?.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Cap_Bump_Vengeance, SFXChunk, false, 0.72f, 6.5f);
                for (int i = 0; i < 7; i++)
                {
                    self.room?.AddObject(new WaterDrip(self.bodyChunks[1].pos, RWCustom.Custom.DegToVec(R.value * 360) * Mathf.Lerp(4, 20, R.value), false));
                }
            }
            else
            {
                self.room?.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Cap_Bump_Vengeance, SFXChunk, loop: false, 0.72f, 5.5f);
                GildFloatState = false;
            }
        }

        public float Escat_GD_GetRechargeRate()
        {
            float percent = (float)GildPower / GildPowerMax;
            return (GildRechargeMode, percent) switch
            {
                (Charger.VoidSwim, _) => 0f,
                (Charger.Oracle, < 0.3f) => Custom.LerpMap(percent, 0, 0.3f, 2f, 1),
                (Charger.Oracle, _) => Custom.LerpMap(percent, 0.3f, 0.9f, 1f, 0),
                (Charger.Shelter, > 0.75f) => 0f,
                (Charger.Shelter, _) => 0.1f,
                (Charger.Gate, > 0.9f) => 0f,
                (Charger.Gate, _) => Custom.LerpMap(percent, 0, 0.7f, 5f, 1),
                (Charger.ZeroG, _) => Custom.LerpMap(percent, 0, 0.8f, 2f, 0.25f),
                (Charger.Ground, < 0.3f) => 2f,
                _ => 1f
            };
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