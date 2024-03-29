using System;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using static TheEscort.Eshelp;


namespace TheEscort
{
    public partial class Escort
    {
        public string Eskie = "Escort";
        public bool isDefault;
        public int DropKickCD;
        public float RollinCount;
        public int parrySlideLean;
        public int parryAirLean;
        public int iFrames;
        public bool Cometted;
        public int CometFrames;
        public BodyChunk SFXChunk;
        public DynamicSoundLoop Rollin;
        public DynamicSoundLoop LizGet;
        public bool LizardDunk;
        public int LizDunkLean;
        public int LizGoForWalk;
        public int LizGrabCount;
        public bool ParrySuccess;
        public bool ElectroParry;
        public int mainSpriteIndex;
        public int mainSprites;
        public LightSource hypeLight;
        public LightSource hypeSurround;
        public Color hypeColor;
        public bool secretRGB;
        private float rgbTick;
        public float smoothTrans;
        public bool tossEscort;
        public bool dualWield;
        public int superWallFlip;
        public bool easyMode;
        public bool easyKick;
        public int consoleTick;
        public bool savingThrowed;
        public bool lenientSlide;
        public int voidRetailiate;
        public int spearLaunched;
        public bool slideStunOnCD;
        public bool poleDance;
        public bool kickFlip;
        public float hipScaleX;
        public float hipScaleY;
        public bool isChunko;
        public float originalMass;
        public bool slideFromSpear;
        public int playerKillCount;
        public int lizzieVengenceClock;
        public int lizzieVengenceTick;
        public int lizzieVengenceCount;
        public int lizzieVengenceTolerance;
        public List<AbstractCreature> vengefulLizards;
        public int vengefulLizardsCount;
        public WorldCoordinate lizzieDestination;
        public List<Trackrr<float>> floatTrackers;
        public List<Trackrr<int>> intTrackers;
        public float acidSwim;
        public int acidRepetitionGuard;
        public int shelterSaveComplete;
        public bool verticalPoleToggle;
        public bool verticalPoleTech;
        public int verticalPoleFail;
        public bool overrideSprite;
        public float viscoDance;
        public Color viscoColor;
        public bool escortArena;
        public Stack<Vulture> offendingKingTusk;
        public int offendingKTtusk;
        public int offendingRemoval;
        public int tryFindingPup;
        public bool expeditionSpawnPup;
        public bool cheatedSpawnPup;
        public Player SocksAliveAndHappy
        {
            get 
            {
                if (socksAbstract?.realizedCreature is not null && socksAbstract.state is not null && socksAbstract.state.alive)
                {
                    return socksAbstract.realizedCreature as Player;
                }
                return null;
            }
        }
        public AbstractCreature socksAbstract;

        public bool CustomKeybindEnabled {get; private set;}
        public KeyCode CustomKeybind {get; private set;}
        public const int SocksID = 3118;
        public const int DemonSocksID = 4118;
        public const int SpeedSocksID = 765;
        public int PupCampaignID;

        // Build stuff
        public bool Brawler;
        public bool BrawWall;
        public bool BrawShankMode;
        public bool BrawShank;
        public bool BrawPunch;
        public bool BrawShankSpearTumbler;
        public Vector2 BrawShankDir;
        public Stack<Weapon> BrawMeleeWeapon;
        public int BrawThrowUsed;
        public int BrawThrowGrab;
        public int BrawRevertWall;
        public Stack<Spear> BrawWallSpear;
        public string BrawLastWeapon;
        public int BrawSetCooldown;
        public bool Deflector;
        public int DeflAmpTimer;
        public bool DeflTrampoline;
        public int DeflSFXcd;
        public int DeflSlideCom;
        public bool DeflSlideKick;
        public int DeflPowah;
        private float _deflperma;
        public float DeflPerma
        {
            get
            {
                if (Plugin.ins.config.cfgDeflecterSharedPool.Value && !escortArena)
                {
                    return Plugin.DeflSharedPerma;
                }
                return _deflperma;
            }
            set
            {
                if (Plugin.ins.config.cfgDeflecterSharedPool.Value && !escortArena)
                {
                    Plugin.DeflSharedPerma = value;
                }
                else
                {
                    _deflperma = value;
                }
            }
        }
        public float DeflDamageMult 
        { 
            get 
            {
                return DeflPowah switch
                {
                    3 => 1000000f,
                    2 => 7f,
                    1 => 3f,
                    _ => 0.5f
                };
            }
        }
        public bool Escapist;
        public int EscDangerExtend;
        public Creature.Grasp EscDangerGrasp;
        public int EscUnGraspTime;
        public int EscUnGraspLimit;
        public int EscUnGraspCD;
        public bool NewEscapist;
        public bool Railgunner;
        public int RailGaussed;
        public Creature RailThrower;
        public bool RailDoubleSpear;
        public bool RailDoubleRock;
        public bool RailDoubleLilly;
        public bool RailDoubleBomb;
        public bool RailFirstWeaped;
        public Vector2 RailFirstWeaper;
        public int RailWeaping;
        public int RailgunCD;
        public int RailgunUse;
        public int RailgunLimit;
        public bool RailIReady;
        public bool RailBombJump;
        public bool Gilded;

        public Escort(Player player)
        {
            /*
            if (ExtEnumBase.TryParse(typeof(SlugcatStats.Name), "EscortMe", true, out var r)){
                name = r as SlugcatStats.Name;
            }*/
            this.DropKickCD = 0;
            this.iFrames = 0;
            this.parrySlideLean = 0;
            this.parryAirLean = 0;
            this.Cometted = false;
            this.CometFrames = 0;
            this.RollinCount = 0f;
            this.LizardDunk = false;
            this.LizDunkLean = 0;
            this.LizGoForWalk = 0;
            this.LizGrabCount = 0;
            this.ParrySuccess = false;
            this.ElectroParry = false;
            this.SFXChunk = player.bodyChunks[0];
            this.mainSpriteIndex = -1;
            this.mainSprites = 2;
            this.hypeColor = new Color(0.796f, 0.549f, 0.27843f);
            this.Escat_setLight_hype(player, this.hypeColor);
            this.secretRGB = false;
            this.smoothTrans = 0f;
            this.tossEscort = true;
            this.dualWield = true;
            this.superWallFlip = 0;
            this.easyMode = false;
            this.easyKick = false;
            this.consoleTick = 0;
            this.savingThrowed = false;
            this.lenientSlide = false;
            this.voidRetailiate = 0;
            this.spearLaunched = 0;
            this.slideStunOnCD = false;
            this.poleDance = false;
            this.kickFlip = false;
            this.hipScaleX = 1f;
            this.hipScaleY = 1f;
            this.isChunko = false;
            this.originalMass = 0f;
            this.slideFromSpear = false;
            this.lizzieVengenceClock = 400;
            this.lizzieVengenceTick = 40;
            this.vengefulLizards = new List<AbstractCreature>(30);
            this.lizzieVengenceTolerance = UnityEngine.Random.Range(3, 7);
            //this.lizzieVengenceTolerance = 1;
            this.lizzieDestination = player.abstractCreature.pos;
            this.floatTrackers = new List<Trackrr<float>>();
            this.intTrackers = new List<Trackrr<int>>();
            this.isDefault = false;
            this.acidSwim = 0.4f;
            this.viscoColor = Color.black;
            this.CustomKeybindEnabled = Plugin.ins.config.cfgCustomBinds[player.playerState.playerNumber].Value && Plugin.ins.config.cfgBindKeys[player.playerState.playerNumber].Value is not KeyCode.None;
            if (CustomKeybindEnabled)
            {
                CustomKeybind = Plugin.ins.config.cfgBindKeys[player.playerState.playerNumber].Value;
            }
            this.offendingKingTusk = new(1);
            this.offendingKTtusk = -1;
            this.offendingRemoval = 0;
            this.tryFindingPup = 80;

            // Build specific
            this.Brawler = false;
            this.BrawShankMode = false;
            this.BrawPunch = false;
            this.BrawWall = false;
            this.BrawRevertWall = -1;
            this.BrawWallSpear = new Stack<Spear>(1);
            this.BrawShankDir = new Vector2();
            this.BrawMeleeWeapon = new Stack<Weapon>(1);
            this.BrawShankSpearTumbler = false;
            this.BrawThrowGrab = -1;
            this.BrawThrowUsed = -1;
            this.BrawLastWeapon = "";
            this.BrawSetCooldown = 20;

            this.Deflector = false;
            this.DeflAmpTimer = 0;
            this.DeflTrampoline = false;
            this.DeflSFXcd = 0;
            this.DeflSlideCom = 0;
            this.DeflSlideKick = false;
            this.DeflPowah = 0;
            this.DeflPerma = 0f;

            this.Escapist = false;
            this.EscDangerExtend = 0;
            this.EscDangerGrasp = null;
            this.EscUnGraspTime = 0;
            this.EscUnGraspLimit = 0;
            this.EscUnGraspCD = 0;

            this.Railgunner = false;
            this.RailGaussed = 0;
            this.RailThrower = player;
            this.RailDoubleSpear = false;
            this.RailDoubleRock = false;
            this.RailDoubleLilly = false;
            this.RailDoubleBomb = false;
            this.RailFirstWeaped = false;
            this.RailFirstWeaper = new Vector2();
            this.RailWeaping = 0;
            this.RailgunCD = 0;
            this.RailgunUse = 0;
            this.RailgunLimit = 10;
            this.RailIReady = false;
            this.RailBombJump = false;

            EscortSS();
            EscortGD(player);
            EscortNE();
        }


        public void Escat_setSFX_roller(SoundID sound)
        {
            try
            {
                this.Rollin = new ChunkDynamicSoundLoop(SFXChunk)
                {
                    sound = sound,
                    Volume = 0
                };
            }
            catch (Exception err)
            {
                Debug.Log("-> Escwt: Something went horribly wrong when setting up the rolling sound!");
                Debug.LogException(err);
            }
        }

        public void Escat_setSFX_lizgrab(SoundID sound)
        {
            try
            {
                this.LizGet = new ChunkDynamicSoundLoop(SFXChunk)
                {
                    sound = sound
                };
            }
            catch (Exception err)
            {
                Debug.Log("-> Escwt: Something went horribly wrong when setting up the lizard grab sound!");
                Debug.LogException(err);
            }
        }

        public void Escat_setLight_hype(Player self, Color c, float alpha = 0f)
        {
            try
            {
                if (this.hypeLight == null)
                {
                    this.hypeLight = new LightSource(self.mainBodyChunk.pos, environmentalLight: true, c, self)
                    {
                        submersible = true,
                        noGameplayImpact = true,
                        //this.hypeLight.requireUpKeep = true;
                        setRad = 50f,
                        setAlpha = alpha,
                        flat = true
                    };
                }
                else
                {
                    Debug.Log("-> Escwt: Hypelight Rebuild!");
                    this.hypeLight.Destroy();
                    this.hypeLight = null;
                    this.hypeLight = new LightSource(self.mainBodyChunk.pos, environmentalLight: true, c, self)
                    {
                        submersible = true,
                        noGameplayImpact = true,
                        //this.hypeLight.requireUpKeep = true;
                        setRad = 50f,
                        setAlpha = alpha,
                        flat = true
                    };
                }
                if (this.hypeSurround == null)
                {
                    this.hypeSurround = new LightSource(self.bodyChunks[0].pos, environmentalLight: false, c, self)
                    {
                        submersible = true,
                        setRad = 120f,
                        setAlpha = alpha * 5f
                    };
                }
                else
                {
                    Debug.Log("-> Escwt: Hypesurround Rebuild!");
                    this.hypeSurround.Destroy();
                    this.hypeSurround = null;
                    this.hypeSurround = new LightSource(self.bodyChunks[0].pos, environmentalLight: false, c, self)
                    {
                        submersible = true,
                        setRad = 120f,
                        setAlpha = alpha * 5f
                    };
                }
                self.room.AddObject(this.hypeLight);
                self.room.AddObject(this.hypeSurround);
            }
            catch (Exception e)
            {
                Debug.Log("-> Escwt: Something went horribly wrong when setting up the hyped light!");
                Debug.LogException(e);
            }
        }

        public void Escat_setIndex_sprite_cue(ref int cue, int index)
        {
            if (cue == -1)
            {
                cue = index;
                Ebug("Cue set to " + cue);
            }
            else
            {
                Ebug("Cue is already set for sprites! " + cue);
            }
        }

        public Color Escat_runit_thru_RGB(Color c, float progression = 1f)
        {
            if (this.secretRGB)
            {
                this.hypeColor = Eshelp_cycle_dat_RGB(ref rgbTick, lightness: (progression > 6f ? 0.67f : 0.5f), increment: progression);
                return hypeColor;
            }
            return c;
        }


        public void Escat_ping_lizards(Player self)
        {
            if (self == null)
            {
                return;
            }
            if (self != null && self.room != null && self.room.game != null && self.room.game.paused)
            {
                return;
            }
            if (this.lizzieVengenceCount < this.lizzieVengenceTolerance || self.dead)
            {
                return;
            }
            if (this.vengefulLizards.Count == 0)
            {
                Ebug(self, "the lizards want VENGENCE!");
                Escat_begin_pursuit(self);
                return;
            }
            if (this.vengefulLizards.Count < this.vengefulLizardsCount)
            {
                Ebug(self, "more lizards want VENGENCE!");
                Escat_replace_dead(self);
                return;
            }
            try
            {
                for (int j = 0; j < this.vengefulLizards.Count; j++)
                {
                    if (this.vengefulLizards[j].slatedForDeletion || this.vengefulLizards[j] == null)
                    {
                        this.vengefulLizards[j] = null;
                        this.vengefulLizards.Remove(this.vengefulLizards[j]);
                    }
                }
            }
            catch (Exception err)
            {
                Ebug(err, "Something went wrong while removing lizards from list!");
            }
            int i = 0;
            // Track
            foreach (AbstractCreature veggie in this.vengefulLizards)
            {
                i++;
                if (!(self != null && self.room != null && self.room.game != null && self.abstractCreature != null && veggie != null))
                {
                    return;
                }
                if (veggie.abstractAI == null || self.room.abstractRoom.shelter || self.room.abstractRoom.gate)
                {
                    break;
                }
                bool overrideTick = false;
                if (veggie.pos.room == self.abstractCreature.pos.room)
                {
                    overrideTick = true;
                }
                if (overrideTick)
                {  // Tick 10 times faster when creature is in the same room as player
                    if (i != this.lizzieVengenceTick) continue;  // Ticks once per second
                }
                else
                {
                    if (i != this.lizzieVengenceClock) continue;  // Ticks once per 10 seconds
                }
                try
                {  // Track player's room
                    if (veggie.abstractAI.destination.room != self.abstractCreature.pos.room)
                    {
                        Ebug(self, "Hunting lizard destination changed! From: " + veggie.abstractAI.destination.ResolveRoomName() + " to " + self.abstractCreature.pos.ResolveRoomName());
                        veggie.abstractAI.SetDestination(self.abstractCreature.pos);
                        //veggie.abstractAI.followCreature = self.abstractCreature;
                    }
                }
                catch (Exception err)
                {
                    Ebug(err, "Couldn't set destination!", asregular: true);
                }
                try
                {  // Be aggressive towards Escort
                    if (veggie.abstractAI.RealAI != null)
                    {
                        foreach (Tracker.CreatureRepresentation tracked in veggie.abstractAI.RealAI.tracker.creatures)
                        {
                            if (tracked == null || tracked.representedCreature == null)
                            {
                                continue;
                            }
                            if (tracked.representedCreature != self.abstractCreature)
                            {
                                veggie.abstractAI.RealAI.tracker.ForgetCreature(tracked.representedCreature);
                            }
                            else
                            {
                                veggie.abstractAI.RealAI.agressionTracker.SetAnger(tracked, 10f, 10f);
                            }
                        }
                        veggie.abstractAI.RealAI.tracker.SeeCreature(self.abstractCreature);
                    }
                }
                catch (Exception err)
                {
                    Ebug(err, "Couldn't track Escort!", asregular: true);
                }
            }
        }

        public void Escat_begin_pursuit(Player self)
        {
            if (self != null && self.room != null && self.room.game != null && self.room.game.world != null)
            {
                this.vengefulLizardsCount = UnityEngine.Random.Range(12, vengefulLizards.Count);
                for (int i = 0; i < this.vengefulLizardsCount; i++)
                {
                    this.vengefulLizards.Add(new AbstractCreature(self.room.game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.CyanLizard), null, Escat_get_cornerable_rooms(self), self.room.game.GetNewID()));
                    this.vengefulLizards[i].saveCreature = false;
                    this.vengefulLizards[i].ignoreCycle = true;
                    this.vengefulLizards[i].voidCreature = true;
                    self.room.abstractRoom.AddEntity(this.vengefulLizards[i]);
                }
            }
        }

        public void Escat_replace_dead(Player self)
        {
            if (self != null && self.room != null && self.room.game != null && self.room.game.world != null)
            {
                AbstractCreature ac = new(self.room.game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.CyanLizard), null, Escat_get_cornerable_rooms(self), self.room.game.GetNewID())
                {
                    saveCreature = false,
                    ignoreCycle = true,
                    voidCreature = true
                };

                this.vengefulLizards.Add(ac);
                self.room.abstractRoom.AddEntity(ac);
            }
        }

        public WorldCoordinate Escat_get_cornerable_rooms(Player self)
        {
            WorldCoordinate coordi = self.room.abstractRoom.RandomNodeInRoom();
            List<int> l = new();
            for (int i = 0; i < self.room.game.world.NumberOfRooms; i++)
            {
                AbstractRoom ar = self.room.game.world.GetAbstractRoom(self.room.game.world.firstRoomIndex + i);
                if (ar != null && !ar.shelter && !ar.gate && ar.name != self.room.abstractRoom.name && ar.NodesRelevantToCreature(StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.CyanLizard)) > 0)
                {
                    l.Add(i);
                }
            }
            if (l.Count > 0)
            {
                return self.room.game.world.GetAbstractRoom(self.room.game.world.firstRoomIndex + l[UnityEngine.Random.Range(0, l.Count)]).RandomNodeInRoom();
                //return self.room.game.world.GetAbstractRoom(self.room.game.world.firstRoomIndex + l[UnityEngine.Random.Range(0, l.Count)]).RandomNodeInRoom();
            }
            return coordi;
        }

        public void Escat_Add_Ring_Trackers(Player self)
        {
            int n = 0;
            if (self.playerState?.playerNumber is not null)
            {
                n = self.playerState.playerNumber;
            }
            string hypeSprite = "escort_hud_default";
            if (Brawler)
            {
                hypeSprite = Plugin.ins.config.cfgSFX.Value? "escort_hud_brawler_alt" : "escort_hud_brawler";
                floatTrackers.Add(new ETrackrr.BrawlerMeleeTraction(n, 1, self, this));
            }
            if (Deflector)
            {
                hypeSprite = "escort_hud_deflector";
                floatTrackers.Add(new ETrackrr.DeflectorEmpowerTraction(n, 1, this));
                floatTrackers.Add(new ETrackrr.DeflectorPermaDamage(n, 2, this));
            }
            if (Escapist)
            {
                hypeSprite = "escort_hud_escapist";
                floatTrackers.Add(new ETrackrr.EscapistUngraspTraction(n, 1, this));
            }
            if (Railgunner)
            {
                hypeSprite = "escort_hud_railgunner";
                floatTrackers.Add(new ETrackrr.RailgunnerCDTraction(n, 1, self, this));
                floatTrackers.Add(new ETrackrr.RailgunnerUsageTraction(n, 2, this));
            }
            if (Speedster)
            {
                hypeSprite = "escort_hud_speedster";
                if (!SpeOldSpeed)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        floatTrackers.Add(new ETrackrr.SpeedsterTraction(n, i, this, i));
                    }
                }
                else 
                {
                    floatTrackers.Add(new ETrackrr.SpeedsterOldTraction(n, 1, this));
                    floatTrackers.Add(new ETrackrr.SpeedsterOldTraction(n, 2, this, true));
                }
            }
            if (Gilded)
            {
                hypeSprite = "escort_hud_gilded";
                floatTrackers.Add(new ETrackrr.GildedPoweredTraction(n, 1, this));
            }
            if (NewEscapist)
            {
                hypeSprite = "escort_hud_escapist";
                floatTrackers.Add(new ETrackrr.NewEscapistTraction(n, 1, this));
            }
            this.floatTrackers.Add(new ETrackrr.HypeTraction(n, 0, Plugin.ins.config.cfgHypeRequirement.Value, self, this, hypeSprite));
            this.floatTrackers.Add(new ETrackrr.DamageProtectionTraction(n, 0, self, this));
            this.floatTrackers.Add(new ETrackrr.SwimTracker(n, 0, self, this));
        }

        public void Escat_Draw_Ring_Trackers(float timeStacker)
        {
            foreach(Trackrr<float> t in this.floatTrackers)
            {
                t.DrawTracker(timeStacker);
            }
            foreach(Trackrr<int> t2 in this.intTrackers)
            {
                t2.DrawTracker(timeStacker);
            }
        }


        public void Escat_Update_Ring_Trackers()
        {
            foreach(Trackrr<float> t in this.floatTrackers)
            {
                t.UpdateTracker();
            }
            foreach(Trackrr<int> t2 in this.intTrackers)
            {
                t2.UpdateTracker();
            }
        }
    }
}
