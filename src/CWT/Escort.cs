using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;


namespace TheEscort{
    public partial class Escort{
        //public readonly SlugcatStats.Name name;
        //public String EsbuildName;
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
        public int spriteQueue;
        public LightSource hypeLight;
        public LightSource hypeSurround;
        public Color hypeColor;
        public bool secretRGB;
        private float secretTick;
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

        // Build stuff
        public bool Brawler;
        public bool BrawWall;
        public bool BrawShankMode;
        public bool BrawShankSpearTumbler;
        public Vector2 BrawShankDir;
        public Stack<Weapon> BrawMeleeWeapon;
        public int BrawThrowUsed;
        public int BrawThrowGrab;
        public int BrawRevertWall;
        public Stack<Spear> BrawWallSpear;
        public bool Deflector;
        public int DeflAmpTimer;
        public bool DeflTrampoline;
        public int DeflSFXcd;
        public int DeflSlideCom;
        public bool DeflSlideKick;
        public bool Escapist;
        public int EscDangerExtend;
        public Creature.Grasp EscDangerGrasp;
        public int EscUnGraspTime;
        public int EscUnGraspLimit;
        public int EscUnGraspCD;
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
      
        public Escort(Player player){
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
            this.spriteQueue = -1;
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

            // Build specific
            this.Brawler = false;
            this.BrawShankMode = false;
            this.BrawWall = false;
            this.BrawRevertWall = -1;
            this.BrawWallSpear = new Stack<Spear>(1);
            this.BrawShankDir = new Vector2();
            this.BrawMeleeWeapon = new Stack<Weapon>(1);
            this.BrawShankSpearTumbler = false;
            this.BrawThrowGrab = -1;
            this.BrawThrowUsed = -1;

            this.Deflector = false;
            this.DeflAmpTimer = 0;
            this.DeflTrampoline = false;
            this.DeflSFXcd = 0;
            this.DeflSlideCom = 0;
            this.DeflSlideKick = false;

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
        }


        public void Escat_setSFX_roller(SoundID sound){
            try{
                this.Rollin = new ChunkDynamicSoundLoop(SFXChunk);
                this.Rollin.sound = sound;
            } catch (Exception err){
                Debug.Log("Something went horribly wrong when setting up the rolling sound!");
                Debug.LogException(err);
            }
        }

        public void Escat_setSFX_lizgrab(SoundID sound){
            try{
                this.LizGet = new ChunkDynamicSoundLoop(SFXChunk);
                this.LizGet.sound = sound;
            } catch (Exception err){
                Debug.Log("Something went horribly wrong when setting up the lizard grab sound!");
                Debug.LogException(err);
            }
        }

        public void Escat_setLight_hype(Player self, Color c, float alpha=0f){
            try{
                if (this.hypeLight == null){
                    this.hypeLight = new LightSource(self.mainBodyChunk.pos, environmentalLight: true, c, self);
                    this.hypeLight.submersible = true;
                    this.hypeLight.noGameplayImpact = true;
                    //this.hypeLight.requireUpKeep = true;
                    this.hypeLight.setRad = 50f;
                    this.hypeLight.setAlpha = alpha;
                    this.hypeLight.flat = true;
                }
                else{
                    Debug.Log("-> Esclas: Hypelight Rebuild!");
                    this.hypeLight.Destroy();
                    this.hypeLight = null;
                    this.hypeLight = new LightSource(self.mainBodyChunk.pos, environmentalLight: true, c, self);
                    this.hypeLight.submersible = true;
                    this.hypeLight.noGameplayImpact = true;
                    //this.hypeLight.requireUpKeep = true;
                    this.hypeLight.setRad = 50f;
                    this.hypeLight.setAlpha = alpha;
                    this.hypeLight.flat = true;
                }
                if (this.hypeSurround == null){
                    this.hypeSurround = new LightSource(self.bodyChunks[0].pos, environmentalLight: false, c, self);
                    this.hypeSurround.submersible = true;
                    this.hypeSurround.setRad = 120f;
                    this.hypeSurround.setAlpha = alpha * 5f;
                }
                else {
                    Debug.Log("-> Esclas: Hypesurround Rebuild!");
                    this.hypeSurround.Destroy();
                    this.hypeSurround = null;
                    this.hypeSurround = new LightSource(self.bodyChunks[0].pos, environmentalLight: false, c, self);
                    this.hypeSurround.submersible = true;
                    this.hypeSurround.setRad = 120f;
                    this.hypeSurround.setAlpha = alpha * 5f;
                }
                self.room.AddObject(this.hypeLight);
                self.room.AddObject(this.hypeSurround);
            } catch (Exception e){
                Debug.Log("Something went horribly wrong when setting up the hyped light!");
                Debug.LogException(e);
            }
        }

        public void Escat_setIndex_sprite_cue(int cue){
            if (this.spriteQueue == -1){
                this.spriteQueue = cue;
            }
            else{
                Debug.Log("Cue is already set for sprites!");
            }
        }

        public Color Escat_runit_thru_RGB(Color c, float progression=1f){
            if (this.secretRGB){
                this.hypeColor = new HSLColor(Mathf.InverseLerp(0f, 959f, secretTick), 1f, (progression > 5f? 0.67f : 0.5f)).rgb;
                if (secretTick >= 959f){
                    secretTick = 0;
                } else {
                    secretTick += progression;
                }
                return hypeColor;
            }
            return c;
        }

    }
}
