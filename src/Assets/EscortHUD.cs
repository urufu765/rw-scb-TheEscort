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
                        self.AddPart(new ProgressionRing(self, traction, ringColor: new Color(0.85f, 0.85f, 0.85f), beyondLimitColor: new Color(1f, 0.5f, 0.0f)));
                    }

                }
                else {
                    Ebug(p, "No HUD for you!", ignoreRepetition: true);
                }
            }
        }
    }



    public class Rings : HudPart
    {
        public Rings(HUD.HUD hud) : base(hud)
        {
        }

        public void AddRing(HudPart part)
        {
            hud.AddPart(part);
        }
    }

    public class ProgressionRing : HudPart
    {
        private Vector2 pos;
        private float mainXPos;
        private FSprite progressSprite;
        private FSprite progressBacking;
        private FSprite progressGlow;
        private readonly Trackrr<float> tracked;
        private Color ringColor;
        private Color beyondLimitColor;
        private Vector2 lastPos;
        private FoodMeter foodmeter;

        public ProgressionRing(HUD.HUD hud, Trackrr<float> tracked, Vector2 position = default, Color ringColor = default, Color beyondLimitColor = default) : base(hud)
        {
            this.tracked = tracked;
            this.pos = new Vector2(40f, 40f);
            this.mainXPos = position.x;
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
            this.progressGlow = new FSprite("Futile_White")
            {
                x = pos.x,
                y = pos.y,
                scale = 6,
                shader = hud.rainWorld.Shaders["FlatLight"]
            };
            hud.fContainers[1].AddChild(progressGlow);
            hud.fContainers[1].AddChild(progressBacking);
            hud.fContainers[1].AddChild(progressSprite);
        }


        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);

            pos = DrawPos(timeStacker);
            pos.x = 60f + 80f * tracked.playerNumber;
            if (this.foodmeter is not null) pos.y = this.foodmeter.pos.y + 80f;
            progressGlow.x = DrawPos(timeStacker).x;
            progressGlow.y = DrawPos(timeStacker).y;
            progressGlow.alpha = 0.1f;
            progressBacking.x = DrawPos(timeStacker).x;
            progressBacking.y = DrawPos(timeStacker).y;
            progressBacking.alpha = Mathf.InverseLerp(0f, tracked.Max, tracked.Value);
            progressBacking.color = Color.Lerp(Color.white, Color.Lerp(beyondLimitColor, Color.grey, timeStacker), 0.7f);
            progressSprite.x = DrawPos(timeStacker).x;
            progressSprite.y = DrawPos(timeStacker).y;
            progressSprite.alpha = Mathf.InverseLerp(0f, tracked.Max, Mathf.Min(tracked.Value, tracked.Limit));
            progressSprite.color = Color.Lerp(Color.white, ringColor, 0.7f);
            if (tracked.Value > 0.5f){
                Ebug("SCREAMING");
            }
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
        }

    }

}