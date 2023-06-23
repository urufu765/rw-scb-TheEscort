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
                                "hype" => new HypeRing(self, traction, new Color(0.85f, 0.85f, 0.85f), new Color(0.58f, 0.55f, 0.57f), new Color(0.85f, 0.85f, 0.85f)),
                                _ => new GenericRing(self, traction, ringColor: new Color(0.85f, 0.85f, 0.85f), beyondLimitColor: new Color(0.58f, 0.55f, 0.57f))
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
        public readonly FSprite progressBacking;
        public readonly Trackrr<float> tracked;
        public Color ringColor;
        public Color beyondLimitColor;
        public Vector2 lastPos;
        public FoodMeter foodmeter;
        public float flashColor;

        public GenericRing(HUD.HUD hud, Trackrr<float> tracked, Color ringColor = default, Color beyondLimitColor = default) : base(hud)
        {
            this.tracked = tracked;
            this.pos = new Vector2(40f, 40f);
            this.ringColor = ringColor;
            this.beyondLimitColor = beyondLimitColor;
            this.progressSprite = new FSprite("Futile_White")
            {
                x = pos.x,
                y = pos.y,
                scale = 3.1f,
                color = this.ringColor,
                shader = hud.rainWorld.Shaders["HoldButtonCircle"]
            };
            this.progressBacking = new FSprite("Futile_White")
            {
                x = pos.x,
                y = pos.y,
                scale = 3.1f,
                color = this.beyondLimitColor,
                shader = hud.rainWorld.Shaders["HoldButtonCircle"]
            };
            hud.fContainers[1].AddChild(progressBacking);
            hud.fContainers[1].AddChild(progressSprite);
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
            progressBacking.color = Color.Lerp(ringColor, beyondLimitColor, flashColor);
            progressSprite.x = DrawPos(timeStacker).x;
            progressSprite.y = DrawPos(timeStacker).y;
            progressSprite.alpha = Mathf.InverseLerp(0f, tracked.Max, Mathf.Min(tracked.Value, tracked.Limit));
            progressSprite.color = ringColor;

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
            if (flashColor < 1f){
                flashColor += 1f/20f;
            } else {
                flashColor = 0;
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
        private readonly Color spriteColor;

        public HypeRing(HUD.HUD hud, Trackrr<float> tracked, Color ringColor = default, Color beyondLimitColor = default, Color spriteColor = default) : base(hud, tracked, ringColor, beyondLimitColor)
        {
            this.spriteColor = spriteColor;
            this.progressGlow = new FSprite("Futile_White")
            {
                x = pos.x,
                y = pos.y,
                scale = 6,
                shader = hud.rainWorld.Shaders["FlatLight"]
            };
            this.normalSprite = new FSprite("FriendA"){
                x = pos.x,
                y = pos.y,
                alpha = 1,
                scale = 1,
                color = spriteColor
            };
            this.hypedSprite = new FSprite("FriendB"){
                x = pos.x,
                y = pos.y,
                alpha = 0,
                scale = 1,
                color = spriteColor
            };
            hud.fContainers[1].AddChild(progressGlow);
            hud.fContainers[1].AddChild(normalSprite);
            hud.fContainers[1].AddChild(hypedSprite);
        }


        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
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
}