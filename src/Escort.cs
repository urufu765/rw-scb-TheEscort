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
        
        // Build stuff
        public bool Barbarian;
        public Rock[] BarRockBelt;
        public int BarRockI;
        public bool Deflector;
        public int DeflAmpTimer;
        public int DeflSFXcd;
        
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
            this.ParrySuccess = false;
            this.ElectroParry = false;
            this.SFXChunk = player.bodyChunks[0];
            this.spriteQueue = -1;
            this.hypeColor = new Color(0.796f, 0.549f, 0.27843f);
            this.Esclass_setLight_hype(player, this.hypeColor);
            this.secretRGB = false;
            this.smoothTrans = 0f;
            this.tossEscort = true;


            // Build specific
            this.Barbarian = false;
            this.BarRockBelt = new Rock[3];
            this.BarRockI = 0;

            this.Deflector = false;
            this.DeflAmpTimer = 0;
            this.DeflSFXcd = 0;
        }
        /*
        public static class Deflector{

        };
        */


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

        public bool Esclass_putaRockInit(Rock rock){
            try{
                if (this.BarRockBelt[this.BarRockI] == null && this.BarRockI < 3){
                    this.BarRockBelt[this.BarRockI] = rock;
                    this.BarRockBelt[this.BarRockI].mode = Weapon.Mode.OnBack;
                    this.BarRockI++;
                    return true;
                }
            } catch (Exception e){
                Debug.LogError("-> Errort: Couldn't do things with rock!");
                Debug.LogException(e);
            }
            return false;
        }
    }
}
