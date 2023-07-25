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
        On.HUD.HUD.InitSinglePlayerHud += Escort_Singleplayer_HUD;
        On.HUD.PlayerSpecificMultiplayerHud.ctor += Escort_Multiplayer_HUD;
    }

    private static void Escort_Multiplayer_HUD(On.HUD.PlayerSpecificMultiplayerHud.orig_ctor orig, PlayerSpecificMultiplayerHud self, HUD.HUD hud, ArenaGameSession session, AbstractCreature abstractPlayer)
    {
        orig(self, hud, session, abstractPlayer);
        try
        {
            if (Plugin.ins.config.cfgShowHud.Value == Plugin.ins.config.hudShowOptions[0].name) return;
            if (abstractPlayer.realizedCreature is Player p)
            {
                if (Plugin.eCon.TryGetValue(p, out Escort e))
                {
                    Ebug(p, "Found player! Applying hud", ignoreRepetition: true);
                    Vector2 location = p.playerState.playerNumber switch
                    {
                        3 => self.cornerPos + new Vector2(-100, -40),
                        2 => self.cornerPos + new Vector2(100, -40),
                        1 => self.cornerPos + new Vector2(100, 20),
                        _ => self.cornerPos + new Vector2(-100, 20)
                    };
                    hud.Escort_HUD(ref e, location);
                }
                else
                {
                    Ebug(p, "No HUD for you!", ignoreRepetition: true);
                }
            }
            else 
            {
                Ebug("Escort Hud not applicable", ignoreRepetition: true);
            }

        }
        catch (Exception err)
        {
            Ebug(err, "Couldn't init multiplayer hud!");
        }
    }

    private static void Escort_Singleplayer_HUD(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);
        try
        {
            if (Plugin.ins.config.cfgShowHud.Value == Plugin.ins.config.hudShowOptions[0].name) return;
            if (self.owner is Player)
            {
                for (int i = 0; i < cam.room.game.session.Players.Count; i++)
                {
                    if (cam.room.game.session.Players[i].realizedCreature is Player player)
                    {
                        if (Plugin.eCon.TryGetValue(player, out Escort e))
                        {
                            Ebug(player, "Found player! Applying hud", ignoreRepetition: true);
                            Vector2 o = Plugin.ins.config.cfgHudLocation.Value switch
                            {
                                "botmid" => new(self.rainWorld.options.ScreenSize.x / 2 - (120f - 80f * player.playerState.playerNumber), 20),
                                "leftstack" => new(60, 80 + 80f * player.playerState.playerNumber),
                                _ => new(60 + 80f * player.playerState.playerNumber, 80)
                            };
                            self.Escort_HUD(ref e, o, true);
                        }
                        else
                        {
                            Ebug(player, "No HUD for you!", ignoreRepetition: true);
                        }
                    }
                    else 
                    {
                        Ebug("Escort Hud not applicable", ignoreRepetition: true);
                    }
                }
            }
        }
        catch (Exception err)
        {
            Ebug(err, "Couldn't init singleplayer hud!");
        }
    }

    private static void Escort_HUD(this HUD.HUD self, ref Escort escort, Vector2 location, bool foodMeterAnchor = false)
    {
        foreach(Trackrr<float> traction in escort.floatTrackers){
            self.AddPart(
                traction.trackerName switch {
                    "parry" => new ParryShield(self, traction, location, foodMeterAnchor),
                    "hype" => new HypeRing(self, traction, location, foodMeterAnchor),
                    "swimming" => new SwimmingVisuals(self, traction, location, foodMeterAnchor),
                    "default" => new HypeRing(self, traction, location, foodMeterAnchor),
                    "brawler" => new BrawWeaponSprites(self, traction, location, foodMeterAnchor),
                    "deflector" => new DeflEmpowerSprites(self, traction, location, foodMeterAnchor),
                    "deflectorPerma" => new DmgText(self, traction, location, foodMeterAnchor),
                    "escapist" => new EscSprite(self, traction, location, foodMeterAnchor),
                    "railgunnerUse" => new RailRing(self, traction, location, foodMeterAnchor),
                    "speedster" => new SpeedRing(self, traction, location, foodMeterAnchor),
                    "speedsterOld" => new OldSpeedRing(self, traction, location, foodMeterAnchor),
                    "gilded" => new GildSprite(self, traction, location, foodMeterAnchor),
                    _ => new GenericRing(self, traction, location, foodMeterAnchor)
                }
            );
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
        public Vector2 offset;
        public bool foodmeterAnchor;

        public RingMeter(HUD.HUD hud, Vector2 offset, bool foodmeterAnchor) : base(hud) { 
            this.pos = new Vector2(40f, 40f);
            this.offset = offset;  // TODO: Make this changeable later
            this.foodmeterAnchor = foodmeterAnchor;
        }

		public Vector2 DrawPos(float timeStacker)
		{
			return Vector2.Lerp(this.lastPos, this.pos, timeStacker);
		}

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            pos = DrawPos(timeStacker);
            pos.x = offset.x;
            pos.y = offset.y;
            if (this.foodmeter is not null) pos.y += this.foodmeter.pos.y;
        }

        public override void Update()
        {
            base.Update();
            if (foodmeterAnchor && foodmeter == null)
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

        public GenericRing(HUD.HUD hud, Trackrr<float> tracked, Vector2 offset, bool foodmeterAnchor, bool staticFlash = false) : base(hud, offset, foodmeterAnchor)
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
            progressBacking.x = progressBacking2.x = progressSprite.x = progressSprite2.x = DrawPos(timeStacker).x;
            progressBacking.y = progressBacking2.y = progressSprite.y = progressSprite2.y = DrawPos(timeStacker).y;
            progressBacking.alpha = progressBacking2.alpha = Mathf.InverseLerp(0f, tracked.Max, tracked.Value);
            progressBacking.color = progressBacking2.color = Color.Lerp(tracked.effectColor, Color.black, flashColor / 2f);
            progressSprite.alpha = progressSprite2.alpha = Mathf.InverseLerp(0f, tracked.Max, Mathf.Min(tracked.Value, tracked.Limit));
            progressSprite.color = progressSprite2.color = tracked.trackerColor;
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

        public HypeRing(HUD.HUD hud, Trackrr<float> tracked, Vector2 offset, bool foodmeterAnchor) : base(hud, tracked, offset, foodmeterAnchor)
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
            normalSprite.color = tracked.Value > tracked.Limit? Color.Lerp(tracked.trackerColor, tracked.trackerColor * 0.7f, flashColor) : tracked.trackerColor * Mathf.Lerp(0.7f, 0.95f, Mathf.InverseLerp(0, tracked.Limit, tracked.Value));
            progressGlow.x = normalSprite.x = DrawPos(timeStacker).x;
            progressGlow.y = normalSprite.y = DrawPos(timeStacker).y;
            progressGlow.alpha = 0.2f;
            normalSprite.alpha = tracked.overridden? 0f : 1f;
            progressBacking.color = progressBacking2.color = Color.Lerp(tracked.effectColor, tracked.trackerColor, flashColor);
        }
    }

    /// <summary>
    /// Ring specifically for Railgunner's using a solid bar
    /// </summary>
    public class RailRing : GenericRing
    {
        private readonly FSprite[] sprites;

        public RailRing(HUD.HUD hud, Trackrr<float> tracked, Vector2 offset, bool foodmeterAnchor) : base(hud, tracked, offset, foodmeterAnchor)
        {
            sprites = new FSprite[4];
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = new FSprite(i switch
                {
                    3 => "escort_hud_raildspear",
                    2 => "escort_hud_raildrock",
                    1 => "escort_hud_raildlily",
                    _ => "escort_hud_raildbomb"
                })
                {
                    x = pos.x,
                    y = pos.y,
                    alpha = 0,
                    scale = 1,
                };
                hud.fContainers[1].AddChild(sprites[i]);
            }
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            progressBacking.color = progressBacking2.color = Color.Lerp(tracked.effectColor, tracked.trackerColor, flashColor);
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].x = DrawPos(timeStacker).x;
                sprites[i].y = DrawPos(timeStacker).y;
                sprites[i].alpha = i == tracked.spriteNumber? 1 : 0;
                sprites[i].color = tracked.Value > tracked.Limit? Color.Lerp(tracked.effectColor, tracked.trackerColor, flashColor) : tracked.trackerColor;
            }
        }
    }

    /// <summary>
    /// Ring that uses the generic ring while adding custom sprites
    /// </summary>
    public class BrawWeaponSprites : GenericRing
    {
        private readonly FSprite[] brawlSprites;
        public BrawWeaponSprites(HUD.HUD hud, Trackrr<float> tracked, Vector2 offset, bool foodmeterAnchor) : base(hud, tracked, offset, foodmeterAnchor)
        {
            brawlSprites = new FSprite[3];
            for (int i = 0; i < brawlSprites.Length; i++)
            {
                brawlSprites[i] = new FSprite(i switch
                {
                    3 => "escort_hud_brawpowerpunch",  // Not implemented yet
                    2 => "escort_hud_brawpunch",
                    1 => "escort_hud_brawshank",
                    _ => "escort_hud_brawsupershank"
                })
                {
                    x = pos.x,
                    y = pos.y,
                    alpha = 0,
                    scale = 1,
                };
                hud.fContainers[1].AddChild(brawlSprites[i]);
            }
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            for (int i = 0; i < brawlSprites.Length; i++)
            {
                brawlSprites[i].x = DrawPos(timeStacker).x;
                brawlSprites[i].y = DrawPos(timeStacker).y;
                brawlSprites[i].alpha = i == tracked.spriteNumber? 1 : 0;
                brawlSprites[i].color = tracked.Limit == 0? Color.Lerp(tracked.effectColor, Color.black, flashColor / 2f) : tracked.trackerColor;
            }
        }
    }

    /// <summary>
    /// Ring that uses the generic ring while adding custom sprites
    /// </summary>
    public class DeflEmpowerSprites : GenericRing
    {
        private readonly FSprite[] sprites;
        public DeflEmpowerSprites(HUD.HUD hud, Trackrr<float> tracked, Vector2 offset, bool foodmeterAnchor) : base(hud, tracked, offset, foodmeterAnchor)
        {
            sprites = new FSprite[4];
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = new FSprite($"escort_hud_deflamp{i}")
                {
                    x = pos.x,
                    y = pos.y,
                    alpha = 0,
                    scale = 1,
                    color = tracked.trackerColor
                };
                hud.fContainers[1].AddChild(sprites[i]);
            }
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].x = DrawPos(timeStacker).x;
                sprites[i].y = DrawPos(timeStacker).y;
                sprites[i].alpha = i == tracked.spriteNumber? 1 : 0;
            }
        }
    }

    /// <summary>
    /// Ring that uses the generic ring while adding custom sprites
    /// </summary>
    public class EscSprite : GenericRing
    {
        private readonly FSprite sprite;

        public EscSprite(HUD.HUD hud, Trackrr<float> tracked, Vector2 offset, bool foodmeterAnchor) : base(hud, tracked, offset, foodmeterAnchor)
        {
            sprite = new FSprite("escort_hud_escshadow")
            {
                x = pos.x,
                y = pos.y,
                alpha = 0,
                scale = 1
            };
            hud.fContainers[1].AddChild(sprite);
        }


        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            sprite.x = DrawPos(timeStacker).x;
            sprite.y = DrawPos(timeStacker).y;
            sprite.alpha = tracked.overridden? 1 : 0;
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
        private readonly FSprite chargeSprite;
        private readonly FSprite gearSprite;
        private float pulseColor, gradientColor;

        public SpeedRing(HUD.HUD hud, Trackrr<float> tracked, Vector2 offset, bool foodmeterAnchor) : base(hud, offset, foodmeterAnchor)
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
            chargeSprite = new FSprite($"escort_hud_spegb{tracked.trackerNumber}")
            {
                x = pos.x,
                y = pos.y,
                alpha = 0,
                scale = 1,
            };
            gearSprite = new FSprite($"escort_hud_spega{tracked.trackerNumber}")
            {
                x = pos.x,
                y = pos.y,
                alpha = 0,
                scale = 1,
                color = tracked.trackerColor
            };
            gradientColor = (tracked.trackerNumber - 1) * 0.25f;
            hud.fContainers[1].AddChild(chargeSprite);
            hud.fContainers[1].AddChild(gearSprite);
            hud.fContainers[1].AddChild(progressBacking);
            hud.fContainers[1].AddChild(progressSprite);
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            progressBacking.x = progressSprite.x = chargeSprite.x = gearSprite.x = DrawPos(timeStacker).x;
            progressBacking.y = progressSprite.y = chargeSprite.y = gearSprite.y = DrawPos(timeStacker).y;
            progressBacking.alpha = Mathf.InverseLerp(0f, tracked.Max, tracked.Value);
            progressBacking.color = Color.Lerp(tracked.trackerColor, tracked.effectColor, Mathf.InverseLerp(-1, 1, Mathf.Sin(Mathf.PI * pulseColor)));
            progressSprite.alpha = Mathf.InverseLerp(0f, tracked.Max, Mathf.Min(tracked.Value, tracked.Limit));
            progressSprite.color = tracked.trackerColor;

            chargeSprite.alpha = Mathf.InverseLerp(0f, tracked.Max, tracked.Value);
            chargeSprite.color = tracked.Limit == 0? tracked.trackerColor : Color.Lerp(tracked.trackerColor, tracked.effectColor, Mathf.InverseLerp(-1, 1, Mathf.Sin(Mathf.PI * pulseColor)));
            gearSprite.alpha = tracked.spriteNumber >= tracked.trackerNumber? 1 : 0;
            gearSprite.color = Color.Lerp(tracked.trackerColor, tracked.effectColor, gradientColor);
        }


        public override void Update()
        {
            base.Update();

            if (pulseColor < 1f){
                pulseColor += 1f/80f;
            } else {
                pulseColor = 0;
            }

            if (gradientColor < 1f)
            {
                gradientColor += 1f/40f;
            }
            else
            {
                gradientColor = 0;
            }
        }
    }

    public class OldSpeedRing : GenericRing
    {
        private readonly FSprite[] sprites;
        private readonly float[] spriteGradient;

        public OldSpeedRing(HUD.HUD hud, Trackrr<float> tracked, Vector2 offset, bool foodmeterAnchor, bool staticFlash = false) : base(hud, tracked, offset, foodmeterAnchor, staticFlash)
        {
            sprites = new FSprite[4];
            spriteGradient = new float[4];
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = new FSprite("escort_hud_speg" + (tracked.trackerNumber == 1? "b" : "a") + (i + 1))
                {
                    x = pos.x,
                    y = pos.y,
                    alpha = 0,
                    scale = 1,
                    color = tracked.trackerColor
                };
                hud.fContainers[1].AddChild(sprites[i]);
                spriteGradient[i] = 0.25f * i;
            }
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].x = DrawPos(timeStacker).x;
                sprites[i].y = DrawPos(timeStacker).y;
                sprites[i].alpha = tracked.trackerNumber == tracked.spriteNumber? 1 : 0;
                sprites[i].color = Color.Lerp(tracked.effectColor, tracked.trackerColor, spriteGradient[i]);
            }
        }

        public override void Update()
        {
            base.Update();
            for (int i = 0; i < spriteGradient.Length; i++)
            {
                if (spriteGradient[i] < 1f){
                    spriteGradient[i] += 1f/40f;
                } else {
                    spriteGradient[i] = 0;
                }
            }
        }
    }

    public class GildSprite : GenericRing
    {
        private readonly FSprite[] sprites;

        public GildSprite(HUD.HUD hud, Trackrr<float> tracked, Vector2 offset, bool foodmeterAnchor) : base(hud, tracked, offset, foodmeterAnchor)
        {
            sprites = new FSprite[7];
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = new FSprite(i switch
                {
                    >= 4 => $"escort_hud_gildlevitate{i - 4}",
                    3 => "escort_hud_gildcraftdone",
                    _ => $"escort_hud_gildcraft{i}"
                })
                {
                    x = pos.x,
                    y = pos.y,
                    alpha = 0,
                    scale = 1,
                    color = tracked.trackerColor
                };
                hud.fContainers[1].AddChild(sprites[i]);
            }
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].x = DrawPos(timeStacker).x;
                sprites[i].y = DrawPos(timeStacker).y;
                sprites[i].alpha = (i == tracked.spriteNumber) && tracked.overridden? 1 : 0;
            }
        }
    }

    public class ParryShield : RingMeter
    {
        private readonly Trackrr<float> tracked;
        private readonly FSprite normalSprite;

        public ParryShield(HUD.HUD hud, Trackrr<float> trackrr, Vector2 offset, bool foodmeterAnchor) : base(hud, offset, foodmeterAnchor)
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
        private readonly String formatting;

        public DmgText(HUD.HUD hud, Trackrr<float> trackrr, Vector2 offset, bool foodmeterAnchor) : base(hud, offset, foodmeterAnchor)
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
            formatting = tracked.spriteNumber == 1? "###0.00" : "###0.000";
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            string multiplier = (tracked.Max + tracked.Limit).ToString(formatting);

            // string multiplier = (tracked.Max + tracked.Limit).ToString();
            
            // if (!multiplier.Contains(".")) multiplier += ".";
            // while (multiplier.Length < 4 || multiplier[multiplier.Length - 4] != '.')
            // {
            //     multiplier += "0";
            // }
            damage.text = "x" + (tracked.Max == 69f? $"INF{multiplier.Substring(multiplier.IndexOf('.'))}" : multiplier);
            damage.x = DrawPos(timeStacker).x;
            damage.y = DrawPos(timeStacker).y + 40;
            damage.color = tracked.trackerColor;
            damage.scale = Mathf.Lerp(0.75f, 1f, tracked.Value);
            damageBacking.text = "x" + (tracked.Max == 69f? $"INF{multiplier.Substring(multiplier.IndexOf('.'))}" : multiplier);
            damageBacking.x = DrawPos(timeStacker).x + 2;
            damageBacking.y = DrawPos(timeStacker).y + 38;
            damageBacking.scale = Mathf.Lerp(0.75f, 1f, tracked.Value);
            glow.x = DrawPos(timeStacker).x;
            glow.y = DrawPos(timeStacker).y + 40;
            glow.color = tracked.effectColor;
            glow.scaleY = Mathf.Lerp(1, 3, tracked.Value);
            
        }
    }

    public class SwimmingVisuals : RingMeter
    {
        private readonly Trackrr<float> tracked;
        private readonly FSprite[] shallowSprites, deepSprites;
        private readonly FSprite defaultSprite;
        public SwimmingVisuals(HUD.HUD hud, Trackrr<float> tracked, Vector2 offset, bool foodmeterAnchor) : base(hud, offset, foodmeterAnchor)
        {
            this.tracked = tracked;
            shallowSprites = new FSprite[12];
            deepSprites = new FSprite[12];
            for(int i = 0; i < 12; i++)
            {
                shallowSprites[i] = new FSprite($"escort_hud_swims{i}")
                {
                    x = pos.x,
                    y = pos.y,
                    alpha = 0
                };
                deepSprites[i] = new FSprite($"escort_hud_swimd{i}")
                {
                    x = pos.x,
                    y = pos.y,
                    alpha = 0
                };
                hud.fContainers[1].AddChild(shallowSprites[i]);
                hud.fContainers[1].AddChild(deepSprites[i]);
            }
            defaultSprite = new FSprite("escort_hud_defaultswimplus")
            {
                x = pos.x,
                y = pos.y,
                alpha = 0
            };
            hud.fContainers[1].AddChild(defaultSprite);
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);

            for (int i = 0; i < deepSprites.Length; i++)
            {
                deepSprites[i].x = shallowSprites[i].x = DrawPos(timeStacker).x;
                deepSprites[i].y = shallowSprites[i].y = DrawPos(timeStacker).y;
                deepSprites[i].color = shallowSprites[i].color = tracked.effectColor;
                deepSprites[i].alpha = tracked.spriteNumber == i? tracked.Value : 0;
                shallowSprites[i].alpha = tracked.spriteNumber == i? tracked.Limit : 0;
            }
            defaultSprite.x = DrawPos(timeStacker).x;
            defaultSprite.y = DrawPos(timeStacker).y;
            defaultSprite.color = tracked.trackerColor;
            defaultSprite.alpha = Mathf.Min(tracked.Max, tracked.Limit);
        }
    }
}