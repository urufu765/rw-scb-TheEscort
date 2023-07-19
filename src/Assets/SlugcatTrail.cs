using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace TheEscort
{
    public class SlugcatTrail : ISingleCameraDrawable
    {
        protected Vector2 pos, offset;
        protected int lifeTime;
        protected FSprite[] playerSprites;
        protected bool[] prevVisible;
        protected float[] prevAlpha;
        protected Vector2[] prevPos;
        protected Color color;
        protected RoomCamera camera;
        public bool killed, inited, ready;

        public SlugcatTrail(RoomCamera camera, RoomCamera.SpriteLeaser sLeaser, Color color = default, int life = 39)
        {
            lifeTime = life;
            this.color = color;
            this.camera = camera;
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
                playerSprites[i].shader = camera.game.rainWorld.Shaders["Basic"];
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
            pos = camera.pos;
            offset = new Vector2();
            foreach (FSprite fSprite in playerSprites)
            {
                camera.ReturnFContainer("Background").AddChild(fSprite);
            }
            camera.AddSingleCameraDrawable(this);
        }

        public virtual void Draw(RoomCamera camera, float timeStacker, Vector2 camPos)
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

        public virtual void Update()
        {
            if (lifeTime > 0)
            {
                lifeTime--;
            }
        }

        public void Kill()
        {
            if (killed) return;
            foreach(FSprite fSprite in playerSprites)
            {
                fSprite.RemoveFromContainer();
            }
            camera.RemoveSingleCameraDrawable(this);
            killed = true;
        }
    }


    public class SpeedTrail : SlugcatTrail
    {
        private Vector2 centerPos;
        private float[] prevScaleX, prevScaleY;
        private readonly int maxLife;
        private Color bonusColor;

        public SpeedTrail(RoomCamera camera, RoomCamera.SpriteLeaser sLeaser, Color color, Color bonusColor, int life) : base(camera, sLeaser, color, life)
        {
            prevScaleX = new float[sLeaser.sprites.Length];
            prevScaleY = new float[sLeaser.sprites.Length];
            centerPos = sLeaser.sprites[1].GetPosition();
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if (i == 0 || i == 1)
                {
                    playerSprites[i].element = Futile.atlasManager.GetElementWithName(i == 0? "BodyA" : "HipsA");
                    playerSprites[i].SetPosition(sLeaser.sprites[i].GetPosition());
                    playerSprites[i].scaleX = sLeaser.sprites[i].scaleX;
                    playerSprites[i].scaleY = sLeaser.sprites[i].scaleY;
                    playerSprites[i].rotation = sLeaser.sprites[i].rotation;
                    playerSprites[i].shader = camera.game.rainWorld.Shaders["Basic"];
                    playerSprites[i].isVisible = prevVisible[i] = sLeaser.sprites[i].isVisible;
                    prevAlpha[i] = sLeaser.sprites[i].alpha;
                    prevPos[i] = sLeaser.sprites[i].GetPosition();
                }
                prevScaleX[i] = sLeaser.sprites[i].scaleX;
                prevScaleY[i] = sLeaser.sprites[i].scaleY;
            }
            this.bonusColor = bonusColor;
            maxLife = life;
        }

        public override void Draw(RoomCamera camera, float timeStacker, Vector2 camPos)
        {
            base.Draw(camera, timeStacker, camPos);
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
                    playerSprites[j].color = Color.Lerp(bonusColor, color, skipOne);
                    playerSprites[j].scaleX = prevScaleX[j] * lerped;
                    playerSprites[j].scaleY = prevScaleY[j] * skipOne;
                    playerSprites[j].SetPosition(
                        new Vector2(Mathf.Lerp(centerPos.x, prevPos[j].x, jumpOne), Mathf.Lerp(centerPos.y, prevPos[j].y, jumpOne)) - offset
                    );
                }
            }
        }
    }

    public class BodyDouble : SlugcatTrail
    {
        private readonly Player.BodyModeIndex playerBodyMode;
        private readonly Player.AnimationIndex playerAnimation;
        private readonly Vector2[] playerPos, playerVel;

        public BodyDouble(RoomCamera camera, RoomCamera.SpriteLeaser sLeaser, Color color, Player player, int life = 80) : base(camera, sLeaser, color, life)
        {
            playerBodyMode = player.bodyMode;
            playerAnimation = player.animation;
            playerPos = new Vector2[2];
            playerVel = new Vector2[2];
            playerPos[0] = player.bodyChunks[0].pos;
            playerPos[1] = player.bodyChunks[1].pos;
            playerVel[0] = player.bodyChunks[0].vel;
            playerVel[1] = player.bodyChunks[1].vel;
        }

        public void Set_Recall(Player player)
        {
            player.bodyMode = playerBodyMode;
            player.animation = playerAnimation;
            player.bodyChunks[0].pos = playerPos[0];
            player.bodyChunks[1].pos = playerPos[1];
            player.bodyChunks[0].vel = playerVel[0];
            player.bodyChunks[1].vel = playerVel[1];
        }
    }

}