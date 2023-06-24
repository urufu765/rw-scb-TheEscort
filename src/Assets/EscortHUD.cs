using System;
using UnityEngine;
using HUD;
using System.CodeDom;
using System.Collections.Generic;
using static TheEscort.Eshelp;

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
        if (self.owner is Player p){
            for (int i = 0; i < cam.room.game.session.Players.Count; i++){
                if (cam.room.game.session.Players[i].realizedCreature is Player player && Plugin.eCon.TryGetValue(player, out Escort e)){
                    Ebug(p, "Found player! Applying hud", ignoreRepetition: true);
                    foreach(Trackrr<float> traction in e.floatTrackers){
                        self.AddPart(
                            traction.trackerName switch {
                                "hype" => new HypeRing(self, traction),
                                "railgunnerUse" => new RailRing(self, traction),
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
    /// Generic progression ring (meter)
    /// </summary>
    public class GenericRing : HudPart
    {
        public Vector2 pos;
        public readonly FSprite progressSprite;
        public readonly FSprite progressSprite2;
        public readonly FSprite progressBacking;
        public readonly FSprite progressBacking2;
        public readonly Trackrr<float> tracked;
        public Vector2 lastPos;
        public FoodMeter foodmeter;
        public float flashColor;
        public bool staticFlash;

        public GenericRing(HUD.HUD hud, Trackrr<float> tracked, bool staticFlash = false) : base(hud)
        {
            this.tracked = tracked;
            this.staticFlash = staticFlash;
            this.pos = new Vector2(40f, 40f);

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

            pos = DrawPos(timeStacker);
            pos.x = 60f + 80f * tracked.playerNumber;
            if (this.foodmeter is not null) pos.y = this.foodmeter.pos.y + 80f;
            progressBacking.x = DrawPos(timeStacker).x;
            progressBacking.y = DrawPos(timeStacker).y;
            progressBacking.alpha = Mathf.InverseLerp(0f, tracked.Max, tracked.Value);
            progressBacking.color = Color.Lerp(tracked.trackerColor, Color.black, flashColor / 2f);
            progressBacking2.x = DrawPos(timeStacker).x;
            progressBacking2.y = DrawPos(timeStacker).y;
            progressBacking2.alpha = Mathf.InverseLerp(0f, tracked.Max, tracked.Value);
            progressBacking2.color = Color.Lerp(tracked.trackerColor, Color.black, flashColor / 2f);
            progressSprite.x = DrawPos(timeStacker).x;
            progressSprite.y = DrawPos(timeStacker).y;
            progressSprite.alpha = Mathf.InverseLerp(0f, tracked.Max, Mathf.Min(tracked.Value, tracked.Limit));
            progressSprite.color = tracked.trackerColor;
            progressSprite2.x = DrawPos(timeStacker).x;
            progressSprite2.y = DrawPos(timeStacker).y;
            progressSprite2.alpha = Mathf.InverseLerp(0f, tracked.Max, Mathf.Min(tracked.Value, tracked.Limit));
            progressSprite2.color = tracked.trackerColor;

        }

		public Vector2 DrawPos(float timeStacker)
		{
			return Vector2.Lerp(this.lastPos, this.pos, timeStacker);
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
        private readonly FSprite hypedSprite;

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
            this.normalSprite = new FSprite("FriendA"){
                x = pos.x,
                y = pos.y,
                alpha = 1,
                scale = 1,
            };
            this.hypedSprite = new FSprite("FriendB"){
                x = pos.x,
                y = pos.y,
                alpha = 0,
                scale = 1,
            };
            hud.fContainers[1].AddChild(progressGlow);
            hud.fContainers[1].AddChild(normalSprite);
            hud.fContainers[1].AddChild(hypedSprite);
        }


        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            normalSprite.color = tracked.trackerColor * Mathf.Lerp(0.4f, 0.9f, Mathf.InverseLerp(0, tracked.Limit, tracked.Value));
            hypedSprite.color = Color.Lerp(tracked.trackerColor, tracked.trackerColor * 0.7f, flashColor);
            progressGlow.x = DrawPos(timeStacker).x;
            progressGlow.y = DrawPos(timeStacker).y;
            progressGlow.alpha = 0.15f;
            normalSprite.x = DrawPos(timeStacker).x;
            normalSprite.y = DrawPos(timeStacker).y;
            hypedSprite.x = DrawPos(timeStacker).x;
            hypedSprite.y = DrawPos(timeStacker).y;
            if (tracked.Value > tracked.Limit){
                normalSprite.alpha = 0;
                hypedSprite.alpha = 1;
            }
            else
            {
                normalSprite.alpha = 1;
                hypedSprite.alpha = 0;
            }
        }
    }

    public class RailRing : GenericRing
    {
        public RailRing(HUD.HUD hud, Trackrr<float> tracked) : base(hud, tracked)
        {
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            progressBacking.color = Color.Lerp(tracked.effectColor, Color.black, flashColor / 2f);
            progressBacking2.color = Color.Lerp(tracked.effectColor, Color.black, flashColor / 2f);
        }
    }
}