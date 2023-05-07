using System.Runtime.CompilerServices;
using UnityEngine;

namespace TheEscort
{
    public static class SandboxClass2
    {
        public class Testing2
        {
            public Vector2[] safeBodyChunksPos;
            public int updateTimer;

            public Testing2()
            {
                this.safeBodyChunksPos = new Vector2[2];
                this.safeBodyChunksPos[0] = new Vector2();
                this.safeBodyChunksPos[1] = new Vector2();
            }

            public void SetSafe(BodyChunk[] bodyChunkz)
            {
                for (int x = 0; x < bodyChunkz.Length; x++)
                {
                    this.safeBodyChunksPos[x] = bodyChunkz[x].pos;
                }
            }
        }

        private static readonly ConditionalWeakTable<Player, Testing2> CWT = new();
        public static Testing2 GetCat(this Player player) => CWT.GetValue(player, _ => new());
    }
}