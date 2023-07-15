using System;
using UnityEngine;
using HUD;
using System.CodeDom;
using System.Collections.Generic;
using static TheEscort.Eshelp;
using System.Diagnostics;

namespace TheEscort;

public static class EscortHUD
{
    public static void Attach()
    {
        On.HUD.HUD.InitSinglePlayerHud += Escort_HUD;
    }


    private static void Escort_HUD(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);
        if (Plugin.ins.config.cfgShowHud.Value == Plugin.ins.config.hudShowOptions[0].name) return;
        if (self.owner is Player p){
            for (int i = 0; i < cam.room.game.session.Players.Count; i++){
                if (cam.room.game.session.Players[i].realizedCreature is Player player && Plugin.eCon.TryGetValue(player, out Escort e)){
                    Ebug(p, "Found player! Applying hud", ignoreRepetition: true);
                    foreach(Trackrr<float> traction in e.floatTrackers){
                        self.AddPart(
                            traction.trackerName switch {
                                "parry" => new ParryShield(self, traction),
                                "hype" => new HypeRing(self, traction),
                                "railgunnerUse" => new RailRing(self, traction),
                                "speedster" => new SpeedRing(self, traction),
                                "deflectorPerma" => new DmgText(self, traction),
                                _ => new GenericRing(self, traction)
                            }
                        );
                    }
                }
                else {
                    Ebug(p, "No HUD for you!", ignoreRepetition: true);
                }
            }
        }
    }

    /// <summary>
    /// The base for the RingMeter
    /// </summary>
    public class RingMeter : HudPart
    {
        public Vector2 pos;
        public Vector2 lastPos;
        public FoodMeter foodmeter;

        public RingMeter(HUD.HUD hud) : base(hud) { 
            this.pos = new Vector2(40f, 40f);
        }

		public Vector2 DrawPos(float timeStacker)
		{
			return Vector2.Lerp(this.lastPos, this.pos, timeStacker);
		}

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            pos = DrawPos(timeStacker);
            if (this.foodmeter is not null) pos.y = this.foodmeter.pos.y + 80f;
        }

        public override void Update()
        {
            base.Update();
            if (foodmeter == null)
            {
                for (int i = 0; i < hud.parts.Count; i++)
                {
                    if (hud.parts[i] is FoodMeter)
                    {
                        foodmeter = hud.parts[i] as FoodMeter;
                    }
                }
            }
            lastPos = pos;
        }
    }

    /// <summary>
    /// Generic progression float ring (meter)
    /// </summary>
    public class GenericRing : RingMeter
    {
        public readonly FSprite progressSprite;
        public readonly FSprite progressSprite2;
        public readonly FSprite progressBacking;
        public readonly FSprite progressBacking2;
        public readonly Trackrr<float> tracked;
        public float flashColor;
        public bool staticFlash;

        public GenericRing(HUD.HUD hud, Trackrr<float> tracked, bool staticFlash = false) : base(hud)
        {
            this.tracked = tracked;
            this.staticFlash = staticFlash;

            this.progressSprite = new FSprite("Futile_White")
            {
                x = pos.x,
                y = pos.y,
                scale = 2.4f + 1f * tracked.trackerNumber,
                shader = hud.rainWorld.Shaders["HoldButtonCircle"]
            };
            this.progressSprite2 = new FSprite("Futile_White")
            {
                x = pos.x,
                y = pos.y,
                scale = 2.4f + Mathf.Max(0.25f - 0.02f * Mathf.Pow(tracked.trackerNumber, 1.5f), 0f) + 1f * tracked.trackerNumber,
                shader = hud.rainWorld.Shaders["HoldButtonCircle"]
            };
            this.progressBacking = new FSprite("Futile_White")
            {
                x = pos.x,
                y = pos.y,
                scale = 2.4f + 1f * tracked.trackerNumber,
                shader = hud.rainWorld.Shaders["HoldButtonCircle"]
            };
            this.progressBacking2 = new FSprite("Futile_White")
            {
                x = pos.x,
                y = pos.y,
                scale = 2.4f + Mathf.Max(0.25f - 0.02f * Mathf.Pow(tracked.trackerNumber, 1.5f), 0f) + 1f * tracked.trackerNumber,
                shader = hud.rainWorld.Shaders["HoldButtonCircle"]
            };
            hud.fContainers[1].AddChild(progressBacking);
            hud.fContainers[1].AddChild(progressBacking2);
            hud.fContainers[1].AddChild(progressSprite);
            hud.fContainers[1].AddChild(progressSprite2);
        }


        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);

            pos.x = 60f + 80f * tracked.playerNumber;
            progressBacking.x = DrawPos(timeStacker).x;
            progressBacking.y = DrawPos(timeStacker).y;
            progressBacking.alpha = Mathf.InverseLerp(0f, tracked.Max, tracked.Value);
            progressBacking.color = Color.Lerp(tracked.effectColor, Color.black, flashColor / 2f);
            progressBacking2.x = DrawPos(timeStacker).x;
            progressBacking2.y = DrawPos(timeStacker).y;
            progressBacking2.alpha = Mathf.InverseLerp(0f, tracked.Max, tracked.Value);
            progressBacking2.color = Color.Lerp(tracked.effectColor, Color.black, flashColor / 2f);
            progressSprite.x = DrawPos(timeStacker).x;
            progressSprite.y = DrawPos(timeStacker).y;
            progressSprite.alpha = Mathf.InverseLerp(0f, tracked.Max, Mathf.Min(tracked.Value, tracked.Limit));
            progressSprite.color = tracked.trackerColor;
            progressSprite2.x = DrawPos(timeStacker).x;
            progressSprite2.y = DrawPos(timeStacker).y;
            progressSprite2.alpha = Mathf.InverseLerp(0f, tracked.Max, Mathf.Min(tracked.Value, tracked.Limit));
            progressSprite2.color = tracked.trackerColor;

        }



        public override void Update()
        {
            base.Update();
            if (staticFlash) flashColor = 0.6f;
            else
            {
                if (flashColor < 1f){
                    flashColor += 1f/20f;
                } else {
                    flashColor = 0;
                }
            }
        }

    }


    /// <summary>
    /// Ring specifically for displaying hyped
    /// </summary>
    public class HypeRing : GenericRing
    {
        private readonly FSprite progressGlow;
        private readonly FSprite normalSprite;

        public HypeRing(HUD.HUD hud, Trackrr<float> tracked) : base(hud, tracked)
        {
            this.progressGlow = new FSprite("Futile_White")
            {
                x = pos.x,
                y = pos.y,
                scale = 6,
                color = tracked.trackerColor,
                shader = hud.rainWorld.Shaders["FlatLight"]
            };
            this.normalSprite = new FSprite(tracked.centerSprite){
                x = pos.x,
                y = pos.y,
                alpha = 1,
                scale = 1,
            };
            hud.fContainers[1].AddChild(progressGlow);
            hud.fContainers[1].AddChild(normalSprite);
        }


        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            normalSprite.color = tracked.Value > tracked.Limit? Color.Lerp(tracked.trackerColor, tracked.trackerColor * 0.7f, flashColor) : tracked.trackerColor * Mathf.Lerp(0.55f, 0.9f, Mathf.InverseLerp(0, tracked.Limit, tracked.Value));
            progressGlow.x = DrawPos(timeStacker).x;
            progressGlow.y = DrawPos(timeStacker).y;
            progressGlow.alpha = 0.15f;
            normalSprite.x = DrawPos(timeStacker).x;
            normalSprite.y = DrawPos(timeStacker).y;
            progressBacking.color = Color.Lerp(tracked.effectColor, tracked.trackerColor, flashColor);
            progressBacking2.color = Color.Lerp(tracked.effectColor, tracked.trackerColor, flashColor);
        }
    }

    /// <summary>
    /// Ring specifically for Railgunner's using a solid bar
    /// </summary>
    public class RailRing : GenericRing
    {
        public RailRing(HUD.HUD hud, Trackrr<float> tracked) : base(hud, tracked)
        {
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            progressBacking.color = Color.Lerp(tracked.effectColor, tracked.trackerColor, flashColor);
            progressBacking2.color = Color.Lerp(tracked.effectColor, tracked.trackerColor, flashColor);
        }
    }

    /// <summary>
    /// Ring for tracking the speedster's charge.
    /// </summary>
    public class SpeedRing : RingMeter
    {
        private readonly Trackrr<float> tracked;
        private readonly FSprite progressSprite;
        private readonly FSprite progressBacking;
        private float pulseColor;

        public SpeedRing(HUD.HUD hud, Trackrr<float> tracked) : base(hud)
        {
            this.tracked = tracked;
            this.progressSprite = new FSprite("Futile_White")
            {
                x = pos.x,
                y = pos.y,
                scale = 2.9f + 0.4f * tracked.trackerNumber,
                shader = hud.rainWorld.Shaders["HoldButtonCircle"]
            };
            this.progressBacking = new FSprite("Futile_White")
            {
                x = pos.x,
                y = pos.y,
                scale = 2.9f + 0.4f * tracked.trackerNumber,
                shader = hud.rainWorld.Shaders["HoldButtonCircle"]
            };
            hud.fContainers[1].AddChild(progressBacking);
            hud.fContainers[1].AddChild(progressSprite);
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            pos.x = 60f + 80f * tracked.playerNumber;
            progressBacking.x = DrawPos(timeStacker).x;
            progressBacking.y = DrawPos(timeStacker).y;
            progressBacking.alpha = Mathf.InverseLerp(0f, tracked.Max, tracked.Value);
            progressBacking.color = Color.Lerp(tracked.trackerColor, tracked.effectColor, Mathf.InverseLerp(-1, 1, Mathf.Sin(Mathf.PI * pulseColor)));
            progressSprite.x = DrawPos(timeStacker).x;
            progressSprite.y = DrawPos(timeStacker).y;
            progressSprite.alpha = Mathf.InverseLerp(0f, tracked.Max, Mathf.Min(tracked.Value, tracked.Limit));
            progressSprite.color = tracked.trackerColor;
        }


        public override void Update()
        {
            base.Update();

            if (pulseColor < 1f){
                pulseColor += 1f/40f;
            } else {
                pulseColor = 0;
            }
        }
    }

    public class ParryShield : RingMeter
    {
        private readonly Trackrr<float> tracked;
        private readonly FSprite normalSprite;

        public ParryShield(HUD.HUD hud, Trackrr<float> trackrr) : base(hud)
        {
            this.tracked = trackrr;
            this.normalSprite = new FSprite(trackrr.centerSprite){
                x = pos.x,
                y = pos.y,
                alpha = 1,
                scale = 1,
            };
            hud.fContainers[1].AddChild(normalSprite);
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            pos.x = 60f + 80f * tracked.playerNumber;
            normalSprite.x = base.DrawPos(timeStacker).x;
            normalSprite.y = base.DrawPos(timeStacker).y;
            normalSprite.color = Color.Lerp(tracked.trackerColor, tracked.effectColor, tracked.Limit);
            normalSprite.alpha = Mathf.InverseLerp(0, 5, tracked.Value);
            normalSprite.scale = 1 + 0.07f * tracked.Max;
        }
    }

    public class DmgText : RingMeter
    {
        private readonly Trackrr<float> tracked;
        private readonly FLabel damage;
        private readonly FLabel damageBacking;
        private readonly FSprite glow;
        public DmgText(HUD.HUD hud, Trackrr<float> trackrr) : base(hud)
        {
            this.tracked = trackrr;
            damage = new FLabel(RWCustom.Custom.GetDisplayFont(), "x" + 1 + trackrr.Limit)
            {
                x = pos.x,
                y = pos.y + 40,
                color = tracked.trackerColor
            };
            damageBacking = new FLabel(RWCustom.Custom.GetDisplayFont(), "x" + 1 + trackrr.Limit)
            {
                x = pos.x,
                y = pos.y + 40,
                alpha = 0.7f,
                color = Color.black
            };
            this.glow = new FSprite("Futile_White")
            {
                x = pos.x,
                y = pos.y + 40,
                scaleX = 6,
                scaleY = 1,
                alpha = 0.5f,
                color = tracked.trackerColor,
                shader = hud.rainWorld.Shaders["FlatLight"]
            };

            hud.fContainers[1].AddChild(glow);
            hud.fContainers[1].AddChild(damageBacking);
            hud.fContainers[1].AddChild(damage);
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            string multiplier = (tracked.Max + tracked.Limit).ToString();
            if (!multiplier.Contains(".")) multiplier += ".";
            while (multiplier.Length < 4 || multiplier[multiplier.Length - 4] != '.')
            {
                multiplier += "0";
            }
            damage.text = "x" + (tracked.Max == 69? "∞.000" : multiplier);
            damage.x = DrawPos(timeStacker).x;
            damage.y = DrawPos(timeStacker).y + 40;
            damage.color = tracked.trackerColor;
            damage.scale = Mathf.Lerp(0.75f, 1f, tracked.Value);
            damageBacking.text = "x" + (tracked.Max == 69? "∞.000" : multiplier);
            damageBacking.x = DrawPos(timeStacker).x + 2;
            damageBacking.y = DrawPos(timeStacker).y + 38;
            damageBacking.scale = Mathf.Lerp(0.75f, 1f, tracked.Value);
            glow.x = DrawPos(timeStacker).x;
            glow.y = DrawPos(timeStacker).y + 40;
            glow.color = tracked.effectColor;
            glow.scaleY = Mathf.Lerp(1, 3, tracked.Value);
        }
    }
}