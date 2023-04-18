using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;


namespace TheEscort{
    public class Escort{
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
        public bool RailBombJump;
        public bool Speedster;
        public int SpeSpeedin;
        public bool SpeDashNCrash;
        public Queue<SpeedTrail> SpeTrail;
        public SpeedTrail SpeTrailCam;
        public int SpeTrailTick;
        public bool SpeSecretSpeed;
        public int SpeExtraSpe;
        public Color SpeColor;
        public int SpeBonk;

        public class SpeedTrail : ISingleCameraDrawable{
            private Vector2 pos;
            private RoomCamera camera;
            public PlayerGraphics playerGraphics;
            private int lifeTime;
            private int maxLife;
            //private RoomCamera.SpriteLeaser pTrail;
            private FSprite[] pSprite;
            private bool[] wasVisible;
            private float[] preAlpha;
            //private Color[] preColor;
            //private float[] preScale;
            private float[] preScaleX;
            private float[] preScaleY;
            private Vector2[] prePos;
            private Color color1;
            private Color color2;
            private Vector2 cPos;
            private Vector2 offset;
            public SpeedTrail(PlayerGraphics pg, RoomCamera.SpriteLeaser s, Color color, Color bonusColor, int life=20){
                this.lifeTime = life;
                this.maxLife = life;
                this.playerGraphics = pg;
                this.color1 = color;
                this.color2 = bonusColor;
                this.camera = pg.owner.room.game.cameras[0];
                this.pSprite = new FSprite[s.sprites.Length];
                this.wasVisible = new bool[s.sprites.Length];
                this.preAlpha = new float[s.sprites.Length];
                //this.preColor = new Color[s.sprites.Length];
                this.preScaleX = new float[s.sprites.Length];
                this.preScaleY = new float[s.sprites.Length];
                this.prePos = new Vector2[s.sprites.Length];
                for (int i = 0; i < s.sprites.Length; i++){
                    //this.pSprite[i] = Clone(s.sprites[i]);
                    this.pSprite[i] = new FSprite(s.sprites[i].element);
                    this.pSprite[i].SetPosition(s.sprites[i].GetPosition());
                    this.pSprite[i].scaleX = s.sprites[i].scaleX;
                    this.pSprite[i].scaleY = s.sprites[i].scaleY;
                    this.preScaleX[i] = s.sprites[i].scaleX;
                    this.preScaleY[i] = s.sprites[i].scaleY;
                    this.pSprite[i].rotation = s.sprites[i].rotation;
                    this.pSprite[i].shader = camera.game.rainWorld.Shaders["Basic"];
                    /*
                    this.wasVisible[i] = s.sprites[i].isVisible;
                    */
                    if (s.sprites[i].element == Futile.atlasManager.GetElementWithName("Futile_White") || s.sprites[i].element == Futile.atlasManager.GetElementWithName("pixel")){
                        this.wasVisible[i] = false;
                    }
                    else {
                        this.wasVisible[i] = s.sprites[i].isVisible;
                    }
                    //this.preScale[i] = s.sprites[i].scale;
                    this.prePos[i] = s.sprites[i].GetPosition();
                    this.pos = s.sprites[1].GetPosition();
                    this.preAlpha[i] = s.sprites[i].alpha;
                    //this.preColor[i] = color;
                };
                this.cPos = this.camera.pos;
                this.offset = new Vector2();
                this.camera.AddSingleCameraDrawable(this);
                foreach(FSprite f in pSprite){
                    camera.ReturnFContainer("Background").AddChild(f);
                }
            }
            /*
            public FSprite Clone(FSprite f){
                using (MemoryStream stream = new MemoryStream()){
                    XmlSerializer spriter = new XmlSerializer(f.GetType());
                    spriter.Serialize(stream, f);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (FSprite) spriter.Deserialize(stream);
                }
            }*/

            public void Update(){
                if (camera.AboutToSwitchRoom){
                    this.KillTrail();
                    return;
                }
                for(int j = 0; j < pSprite.Length; j++){
                    if (lifeTime > maxLife - (int)(maxLife / 10)){
                        pSprite[j].isVisible = false;
                    }
                    else{
                        pSprite[j].isVisible = wasVisible[j];
                        //pSprite[j].color = Color.Lerp(Color.black, preColor[j], Mathf.InverseLerp(0, maxLife - (int)(maxLife / 10), lifeTime));
                        pSprite[j].color = Color.Lerp(color2, color1, Mathf.InverseLerp(0, maxLife - (int)(maxLife / 10), lifeTime));
                        pSprite[j].alpha = preAlpha[j];
                        //pSprite[j]._concatenatedAlpha = Mathf.InverseLerp(0, 40, lifeTime);
                        pSprite[j].scaleX = preScaleX[j] * Mathf.InverseLerp(0, maxLife, lifeTime);
                        pSprite[j].scaleY = preScaleY[j] * Mathf.InverseLerp(0, maxLife - (int)(maxLife / 10), lifeTime);
                        pSprite[j].SetPosition(new Vector2(
                            Mathf.Lerp(pos.x, prePos[j].x, Mathf.InverseLerp(0, maxLife + (int)(maxLife / 10), lifeTime)),
                            Mathf.Lerp(pos.y, prePos[j].y, Mathf.InverseLerp(0, maxLife + (int)(maxLife / 10), lifeTime))
                            ) - this.offset);
                    }
                }
                if (lifeTime > 0){
                    lifeTime--;
                }
            }

            public void KillTrail(){
                foreach(FSprite f in pSprite){
                    camera.ReturnFContainer("Background").RemoveChild(f);
                }
                this.pSprite = null;
                this.wasVisible = null;
                this.preAlpha = null;
                //this.preColor = null;
                this.camera.RemoveSingleCameraDrawable(this);
            }

            public void Draw(RoomCamera rCam, float timeStacker, Vector2 camPos){
                this.offset = new Vector2(camPos.x - this.cPos.x, camPos.y - this.cPos.y);
            }
        }
        
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
            this.voidRetailiate = 40;


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
            this.RailBombJump = false;

            this.Speedster = false;
            this.SpeSpeedin = 0;
            this.SpeExtraSpe = 0;
            this.SpeDashNCrash = false;
            this.SpeSecretSpeed = false;
            if (this.SpeTrail == null){
                this.SpeTrail = new Queue<SpeedTrail>();
            }
            this.SpeTrailTick = 0;
            this.SpeColor = new Color(0.76f, 0.78f, 0f);
            this.SpeBonk = 0;
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

        public void Escat_addTrail(PlayerGraphics pg, RoomCamera.SpriteLeaser s, int life, int trailCount=10){
            if (this.SpeTrail.Count >= trailCount){
                SpeedTrail trail = this.SpeTrail.Dequeue();
                trail.KillTrail();
            }
            this.SpeTrail.Enqueue(new SpeedTrail(pg, s, (this.SpeSecretSpeed? Color.white : this.hypeColor), (this.SpeSecretSpeed? this.hypeColor : Color.black), life));
        }

        public void Escat_showTrail(RoomCamera rCam, float timeStacker, Vector2 camPos){
            foreach(SpeedTrail trail in this.SpeTrail){
                if (trail.playerGraphics != null && trail.playerGraphics.owner != null){
                    trail.Update();
                } else {
                    trail.KillTrail();
                }
            }
        }
    }
}
