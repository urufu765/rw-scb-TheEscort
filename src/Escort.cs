using System;
using UnityEngine;



namespace TheEscort{
    public class Escort{
        //public readonly SlugcatStats.Name name;
        //public String EsbuildName;
        public int DropKickCD;
        public int CentiCD;
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
        
        // Build stuff
        public bool Brawler;
        public bool BrawShankMode;
        public Vector2 BrawShankDir;
        public bool Deflector;
        public int DeflAmpTimer;
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
        public bool Speedstar;
        public int SpeSpeedin;
        public bool SpeDashNCrash;
        

        
        public Escort(Player player){
            /*
            if (ExtEnumBase.TryParse(typeof(SlugcatStats.Name), "EscortMe", true, out var r)){
                name = r as SlugcatStats.Name;
            }*/
            
            this.DropKickCD = 0;
            this.CentiCD = 0;
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
            this.Esclass_setLight_hype(player, this.hypeColor);
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


            // Build specific
            this.Brawler = false;
            this.BrawShankMode = false;
            this.BrawShankDir = new Vector2();

            this.Deflector = false;
            this.DeflAmpTimer = 0;
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
            }


        public void Esclass_setSFX_roller(SoundID sound){
            try{
                this.Rollin = new ChunkDynamicSoundLoop(SFXChunk);
                this.Rollin.sound = sound;
            } catch (Exception err){
                Debug.Log("Something went horribly wrong when setting up the rolling sound!");
                Debug.LogException(err);
            }
        }

        public void Esclass_setSFX_lizgrab(SoundID sound){
            try{
                this.LizGet = new ChunkDynamicSoundLoop(SFXChunk);
                this.LizGet.sound = sound;
            } catch (Exception err){
                Debug.Log("Something went horribly wrong when setting up the lizard grab sound!");
                Debug.LogException(err);
            }
        }


        public void Esclass_setLight_hype(Player self, Color c, float alpha=0f){
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

        public void Esclass_setIndex_sprite_cue(int cue){
            if (this.spriteQueue == -1){
                this.spriteQueue = cue;
            }
            else{
                Debug.Log("Cue is already set for sprites!");
            }
        }

        public Color Esclass_runit_thru_RGB(Color c, float progression=1f){
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


        public bool Esclass_Railgunner_Death(Player self, Room room){
            if (this.RailgunUse >= this.RailgunLimit){
                Color c = new Color(0.5f, 0.85f, 0.78f);
                Vector2 v = Vector2.Lerp(self.firstChunk.pos, self.firstChunk.lastPos, 0.35f);
                room.AddObject(new SootMark(room, v, 120f, bigSprite:true));
                room.AddObject(new Explosion(room, self, v, 10, 50f, 60f, 3.5f, 10f, 0.4f, self, 0.7f, 2f, 1f));
                room.AddObject(new Explosion(room, self, v, 8, 500f, 60f, 0.02f, 360f, 0.4f, self, 0.01f, 40f, 1f));
                room.AddObject(new Explosion.ExplosionLight(v, 210f, 0.7f, 7, c));
                room.AddObject(new ShockWave(v, 500f, 0.05f, 6));
                for (int i = 0; i < 20; i++){
                    Vector2 v2 = RWCustom.Custom.RNV();
                    room.AddObject(new Spark(v + v2 * Mathf.Lerp(30f, 60f, UnityEngine.Random.value), v2 * Mathf.Lerp(7f, 38f, UnityEngine.Random.value) + RWCustom.Custom.RNV() * 20f * UnityEngine.Random.value, Color.Lerp(Color.white, c, UnityEngine.Random.value), null, 11, 33));
                    room.AddObject(new Explosion.FlashingSmoke(v + v2 * 40f * UnityEngine.Random.value, v2 * Mathf.Lerp(4f, 20f, Mathf.Pow(UnityEngine.Random.value, 2f)), 1f + 0.05f * UnityEngine.Random.value, Color.white, c, UnityEngine.Random.Range(3, 11)));
                }
                room.ScreenMovement(v, default(Vector2), 1.5f);
                room.PlaySound(SoundID.Bomb_Explode, this.SFXChunk, false, 0.90f, 0.24f);
                self.Die();
                return true;
            }
            return false;
        }
    }
}
