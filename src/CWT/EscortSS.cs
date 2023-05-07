using System.Collections.Generic;
using UnityEngine;


namespace TheEscort
{
    public partial class Escort
    {
        public bool Speedster;
        public int SpeSpeedin;
        public bool SpeDashNCrash;
        public Queue<SpeedTrail> SpeTrail;
        public int SpeTrailTick;
        public bool SpeSecretSpeed;
        public int SpeExtraSpe;
        public Color SpeColor;
        public int SpeBonk;
        public float SpeBuildup;
        public int SpeGear;
        public int SpeCharge;
        public float SpeGain;
        public bool SpeOldSpeed;

        public void EscortSS(bool useOld = false)
        {
            this.Speedster = false;
            this.SpeSpeedin = 0;
            this.SpeExtraSpe = 0;
            this.SpeDashNCrash = false;
            this.SpeSecretSpeed = false;
            this.SpeTrail ??= new Queue<SpeedTrail>();
            this.SpeTrailTick = 0;
            this.SpeColor = new Color(0.76f, 0.78f, 0f);
            this.SpeBonk = 0;
            this.SpeBuildup = 0f;
            this.SpeGear = 0;
            this.SpeCharge = 0;
            this.SpeGain = -1f;
            this.SpeOldSpeed = useOld;
        }

        public class SpeedTrail : ISingleCameraDrawable
        {
            private Vector2 pos;
            private readonly RoomCamera camera;  // Could make this switch cameras if using splitscreen
            public PlayerGraphics playerGraphics;
            private int lifeTime;
            private readonly int maxLife;
            //private RoomCamera.SpriteLeaser pTrail;
            private FSprite[] pSprite;
            private bool[] wasVisible;
            private float[] preAlpha;
            //private Color[] preColor;
            //private float[] preScale;
            private readonly float[] preScaleX;
            private readonly float[] preScaleY;
            private readonly Vector2[] prePos;
            private Color color1;
            private Color color2;
            private Vector2 cPos;
            private Vector2 offset;
            public bool killed = false;
            public SpeedTrail(PlayerGraphics pg, RoomCamera.SpriteLeaser s, Color color, Color bonusColor, int life = 20)
            {
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
                for (int i = 0; i < s.sprites.Length; i++)
                {
                    //this.pSprite[i] = Clone(s.sprites[i]);
                    if (i == 0)
                    {
                        this.pSprite[0] = new FSprite("BodyA");
                    }
                    else if (i == 1)
                    {
                        this.pSprite[1] = new FSprite("HipsA");
                    }
                    else
                    {
                        this.pSprite[i] = new FSprite(s.sprites[i].element);
                    }
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
                    if (s.sprites[i].element == Futile.atlasManager.GetElementWithName("Futile_White") || s.sprites[i].element == Futile.atlasManager.GetElementWithName("pixel") || i == 2)
                    {
                        this.wasVisible[i] = false;
                    }
                    else
                    {
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
                foreach (FSprite f in pSprite)
                {
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

            public void Update()
            {
                for (int j = 0; j < pSprite.Length; j++)
                {
                    if (lifeTime > maxLife - maxLife / 10)
                    {
                        pSprite[j].isVisible = false;
                    }
                    else
                    {
                        pSprite[j].isVisible = wasVisible[j];
                        //pSprite[j].color = Color.Lerp(Color.black, preColor[j], Mathf.InverseLerp(0, maxLife - (int)(maxLife / 10), lifeTime));
                        pSprite[j].color = Color.Lerp(color2, color1, Mathf.InverseLerp(0, maxLife - maxLife / 10, lifeTime));
                        pSprite[j].alpha = preAlpha[j];
                        //pSprite[j]._concatenatedAlpha = Mathf.InverseLerp(0, 40, lifeTime);
                        pSprite[j].scaleX = preScaleX[j] * Mathf.InverseLerp(0, maxLife, lifeTime);
                        pSprite[j].scaleY = preScaleY[j] * Mathf.InverseLerp(0, maxLife - maxLife / 10, lifeTime);
                        pSprite[j].SetPosition(new Vector2(
                            Mathf.Lerp(pos.x, prePos[j].x, Mathf.InverseLerp(0, maxLife + maxLife / 10, lifeTime)),
                            Mathf.Lerp(pos.y, prePos[j].y, Mathf.InverseLerp(0, maxLife + maxLife / 10, lifeTime))
                            ) - this.offset);
                    }
                }
            }

            public void KillTrail()
            {
                this.killed = true;
                foreach (FSprite f in pSprite)
                {
                    //camera.ReturnFContainer("Background").RemoveChild(f);
                    f.RemoveFromContainer();
                }
                this.pSprite = null;
                this.wasVisible = null;
                this.preAlpha = null;
                //this.preColor = null;
                this.camera.RemoveSingleCameraDrawable(this);
            }

            public void Draw(RoomCamera rCam, float timeStacker, Vector2 camPos)
            {
                this.offset = new Vector2(camPos.x - this.cPos.x, camPos.y - this.cPos.y);
                if (this.lifeTime > 0)
                {
                    this.lifeTime--;
                }
                else if (!this.killed)
                {
                    KillTrail();
                }
            }
        }
        public void Escat_addTrail(PlayerGraphics pg, RoomCamera.SpriteLeaser s, int life, int trailCount = 10)
        {
            if (this.SpeTrail.Count >= trailCount)
            {
                SpeedTrail trail = this.SpeTrail.Dequeue();
                if (!trail.killed)
                {
                    trail.KillTrail();
                }
            }
            if (this.SpeOldSpeed)
            {
                this.SpeTrail.Enqueue(new SpeedTrail(pg, s, (this.SpeSecretSpeed ? Color.white : this.hypeColor), (this.SpeSecretSpeed ? this.hypeColor : Color.black), life));
            }
            else
            {
                this.SpeTrail.Enqueue(new SpeedTrail(pg, s, Color.Lerp(this.hypeColor, Color.white, this.SpeGear * 0.33f), Color.Lerp(Color.black, this.hypeColor, this.SpeGear * 0.33f), life));
            }
        }

        public void Escat_showTrail()
        {
            foreach (SpeedTrail trail in this.SpeTrail)
            {
                if (trail.killed)
                {
                    continue;
                }
                if (trail.playerGraphics != null && trail.playerGraphics.owner != null)
                {
                    trail.Update();
                }
                else if (!trail.killed)
                {
                    trail.KillTrail();
                }
            }
        }
    }
}