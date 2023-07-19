using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace TheEscort
{
    public class SnapshotTrail : IDrawable
    {
        protected Vector2 pos, offset;
        protected int lifeTime;
        protected FSprite[] playerSprites;
        protected bool[] prevVisible;
        protected float[] prevAlpha;
        protected Vector2[] prevPos;
        protected Color color;
        public bool killed, inited, ready;


        public SnapshotTrail(int life = 39, Color color = default)
        {
            lifeTime = life;
            this.color = color;
        }


        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (!inited || ready) return;
            foreach (FSprite fSprite in playerSprites)
            {
                rCam.ReturnFContainer("Background").AddChild(fSprite);
            }
            ready = true;
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            throw new NotImplementedException();
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            offset = new Vector2(camPos.x - pos.x, camPos.y - pos.y);
            for (int j = 0; j < playerSprites.Length; j++)
            {
                playerSprites[j].isVisible = prevVisible[j];
                playerSprites[j].color = color;
                playerSprites[j].alpha = prevAlpha[j];
                playerSprites[j].SetPosition(prevPos[j] - offset);
            }
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            if (inited || ready) return;
            playerSprites = new FSprite[sLeaser.sprites.Length];
            prevVisible = new bool[sLeaser.sprites.Length];
            prevAlpha = new float[sLeaser.sprites.Length];
            prevPos = new Vector2[sLeaser.sprites.Length];

            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                playerSprites[i] = new FSprite(sLeaser.sprites[i].element);
                playerSprites[i].SetPosition(sLeaser.sprites[i].GetPosition());
                playerSprites[i].scaleX = sLeaser.sprites[i].scaleX;
                playerSprites[i].scaleY = sLeaser.sprites[i].scaleY;
                playerSprites[i].rotation = sLeaser.sprites[i].rotation;
                playerSprites[i].shader = rCam.game.rainWorld.Shaders["Basic"];
                if (
                    sLeaser.sprites[i].element == Futile.atlasManager.GetElementWithName("Futile_White") ||
                    sLeaser.sprites[i].element == Futile.atlasManager.GetElementWithName("pixel") ||
                    i == 2
                )
                {
                    playerSprites[i].isVisible = prevVisible[i] = false;
                }
                else
                {
                    playerSprites[i].isVisible = prevVisible[i] = sLeaser.sprites[i].isVisible;
                }
                prevAlpha[i] = sLeaser.sprites[i].alpha;
                prevPos[i] = sLeaser.sprites[i].GetPosition();
            }
            pos = rCam.pos;
            offset = new Vector2();
            inited = true;
        }

        public void Update()
        {
            if (lifeTime > 0)
            {
                lifeTime--;
            }
        }

        public void Draw(RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (killed || !inited || !ready) return;
            DrawSprites(null, rCam, timeStacker, camPos);
        }

        public void Kill()
        {
            if (killed) return;
            foreach(FSprite fSprite in playerSprites)
            {
                fSprite.RemoveFromContainer();
            }
            killed = true;
        }
    }

    public class SpeedsTrail : SnapshotTrail
    {
        private Vector2 centerPos;
        private float[] prevScaleX, prevScaleY;
        private int maxLife;
        private Color colorAlt;

        public SpeedsTrail(Color colorTwo, int life = 19, Color color = default) : base(life, color)
        {
            colorAlt = colorTwo;
            maxLife = life;
        }

        public new void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            for (int j = 0; j < playerSprites.Length; j++)
            {
                if (lifeTime > maxLife - maxLife / 10)
                {
                    playerSprites[j].isVisible = false;
                }
                else
                {
                    float skipOne = Mathf.InverseLerp(0, maxLife - maxLife / 10, lifeTime);
                    float lerped = Mathf.InverseLerp(0, maxLife, lifeTime);
                    float jumpOne = Mathf.InverseLerp(0, maxLife + maxLife / 10, lifeTime);
                    playerSprites[j].color = Color.Lerp(colorAlt, color, skipOne);
                    playerSprites[j].scaleX = prevScaleX[j] * lerped;
                    playerSprites[j].scaleY = prevScaleY[j] * skipOne;
                    playerSprites[j].SetPosition(
                        new Vector2(Mathf.Lerp(centerPos.x, prevPos[j].x, jumpOne), Mathf.Lerp(centerPos.y, prevPos[j].y, jumpOne)) - offset
                    );
                }
            }
        }

        public new void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            base.InitiateSprites(sLeaser, rCam);
            prevScaleX = new float[sLeaser.sprites.Length];
            prevScaleY = new float[sLeaser.sprites.Length];
            centerPos = sLeaser.sprites[1].GetPosition();
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                prevScaleX[i] = sLeaser.sprites[i].scaleX;
                prevScaleY[i] = sLeaser.sprites[i].scaleY;
            }
        }
    }

    public class LegacyTrail : ISingleCameraDrawable
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
        public LegacyTrail(PlayerGraphics pg, RoomCamera.SpriteLeaser s, Color color, Color bonusColor, int life = 20)
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

}