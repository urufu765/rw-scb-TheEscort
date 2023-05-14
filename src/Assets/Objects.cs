using UnityEngine;

namespace TheEscort
{

    public class GrappleBackpack : TubeWorm
    {
        public static readonly CreatureTemplate.Type GrapplingPack = new("BackpackWorm", register: true);

        public static readonly int BackpackID = 0;

        //private Player Attachment { get; set; }

        public GrappleBackpack(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
        {
            for (int i = 0; i < this.bodyChunks.Length; i++)
            {
                this.bodyChunks[i].mass *= 0.01f;
            }
            this.canBeHitByWeapons = false;
            this.CollideWithObjects = false;
            this.CollideWithSlopes = false;
            this.CollideWithTerrain = false;
            this.sleeping = false;
            //this.Attachment = null;
            // for (int i = 0; i < this.tongues.Length; i++){
            //     this.tongues[i] = new GrappleTongue(this, i);
            // }
            this.abstractCreature.saveCreature = false;
            Debug.Log("Backpack Initiated!");
        }
/*
        public GrappleBackpack(AbstractCreature abstractCreature, World world, Player owner) : base(abstractCreature, world)
        {
            for (int i = 0; i < this.bodyChunks.Length; i++)
            {
                this.bodyChunks[i].mass *= 0.01f;
            }
            this.canBeHitByWeapons = false;
            this.CollideWithObjects = false;
            this.CollideWithSlopes = false;
            this.CollideWithTerrain = false;
            this.sleeping = false;
            //this.Attachment = owner;
            // for (int i = 0; i < this.tongues.Length; i++){
            //     this.tongues[i] = new GrappleTongue(this, i);
            // }
            this.abstractCreature.saveCreature = false;
            Debug.Log("Backpack Initiated!");
        }
*/

        public override void InitiateGraphicsModule()
        {
            Debug.Log("Backpack Graphics initiated");
            this.graphicsModule ??= new CommonBackpackGraphics(this);
        }

        public override void Update(bool eu)
        {

            this.sleeping = false;
            // Destroy worm upon resting in shelter
            if (this.grabbedBy.Count == 0)
            {
                Debug.Log("Sending backpack to Lost & Find");
                this.Destroy();
                return;
            }

            if (this.useBool)
            {
                if (this.tongues[0].Attached)
                {
                    for (int j = 0; j < this.grabbedBy.Count; j++)
                    {
                        if (grabbedBy[j].grabber is Player p)
                        {
                            if (p.animation == Player.AnimationIndex.Flip)
                            {
                                p.mainBodyChunk.vel.x *= 2f;
                            }
                            if (p.bodyMode != Player.BodyModeIndex.ZeroG)
                            {
                                p.bodyChunks[0].vel.y += 15f;
                            }
                            break;
                        }
                    }
                }
            }
            base.Update(eu);
        }

        public override bool CanBeGrabbed(Creature grabber)
        {
            if (this.grabbedBy.Count > 0)
            {
                return false;
            }
            return base.CanBeGrabbed(grabber);
        }

        public override void Violence(BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, Appendage.Pos onAppendagePos, DamageType type, float damage, float stunBonus)
        {
            base.Violence(source, directionAndMomentum, hitChunk, onAppendagePos, type, 0f, stunBonus);
        }

        public new bool JumpButton(Player plr)
        {
            if (plr.canJump < 1 && plr.bodyMode == Player.BodyModeIndex.ZeroG)
            {
                this.useBool = true;
                return false;
            }
            return base.JumpButton(plr);
        }
        public static void BackpackGrabbedByPlayer(On.TubeWorm.orig_GrabbedByPlayer orig, TubeWorm self)
        {
            if (self is not GrappleBackpack)
            {
                orig(self);
                return;
            }
            if (!self.lastGrabbed)
            {
                self.tongues[0].elastic = 1f;
                self.tongues[1].elastic = 1f;
            }
            self.sleeping = false;
            Player player = null;
            for (int i = 0; i < self.grabbedBy.Count; i++)
            {
                if (self.grabbedBy[i].grabber is Player)
                {
                    player = self.grabbedBy[i].grabber as Player;
                    break;
                }
            }
            self.tongues[0].baseChunk = player.mainBodyChunk;
            if (self.tongues[1].Attached)
            {
                self.tongues[1].Release();
            }
            self.onRopePos = Mathf.Min(1f, self.onRopePos + 0.05f);

            // Get direction of tongue
            Vector2 v1 = new Vector2(player.flipDirection, 0.7f).normalized;
            if (player.input[0].y > 0) v1 = new Vector2(0f, 1f);
            if (player.bodyMode == Player.BodyModeIndex.ZeroG)
            {
                v1 = (player.input[0].x, player.input[0].y) switch
                {
                    (not 0, 0) => new Vector2(player.flipDirection, 0f),
                    (0, > 0) => new Vector2(0f, 1f),
                    (0, < 0) => new Vector2(0f, -1f),
                    (_, _) => new Vector2(player.flipDirection, (player.input[0].y < 0 ? -0.7f : 0.7f)).normalized
                };
            }

            // Aim down when zeroG

            player.tubeWorm = self;
            self.mainBodyChunk.vel += v1 * 3f;
            self.bodyChunks[1].vel -= v1 * 3f;
            if (player.enteringShortCut.HasValue)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (self.tongues[j].Attached)
                    {
                        self.tongues[j].Release();
                    }
                }
            }
            else if (ModManager.MMF && player.animation == Player.AnimationIndex.VineGrab)
            {
                if (!player.input[0].jmp || player.input[1].jmp)
                {
                    return;
                }
                for (int k = 0; k < 2; k++)
                {
                    if (self.tongues[k].Attached)
                    {
                        self.tongues[k].Release();
                        self.useBool = false;
                    }
                }
                if (self.useBool && !self.dead)
                {
                    self.tongues[0].Shoot(v1);
                    self.useBool = false;
                }
            }
            else
            {
                if (self.useBool && !self.dead)
                {
                    self.tongues[0].Shoot(v1);
                }
                self.useBool = false;
            }
        }
    }

    public class LauncherBackpack : TubeWorm
    {
        public static readonly CreatureTemplate.Type LaunchingPack = new("LaunchpackWorm", register: true);
        public static readonly int BackpackID = 1;
        public int detatcherClock;

        //private Player Attachment { get; set; }
        public class LauncherTongue : TubeWorm.Tongue
        {
            public LauncherTongue(TubeWorm worm, int tongueNum) : base(worm, tongueNum)
            {
                this.idealRopeLength = 300f;
                this.ropeExtendSpeed += 1f;
            }

            public new void Shoot(Vector2 dir)
            {
                this.worm.playerCheatAttachPos = null;
                if (Attached)
                {
                    Release();
                }
                else if (!(this.mode != Mode.Retracted))
                {
                    this.mode = Mode.ShootingOut;
                    this.room.PlaySound(SoundID.Tube_Worm_Shoot_Tongue, this.baseChunk);
                    dir = (this.worm.grabbedBy.Count <= 0 || this.worm.grabbedBy[0].grabber is not Player) ? CheapAutoAim(dir) : ProperAutoAim(dir);
                    this.pos = baseChunk.pos + dir * 5f;
                    this.vel = dir * 50f;
                    this.elastic = 0.85f;
                    this.requestedRopeLength = 280f;
                    this.returning = false;
                }
            }
        }
        public LauncherBackpack(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
        {
            for (int i = 0; i < this.bodyChunks.Length; i++)
            {
                this.bodyChunks[i].mass *= 0.01f;
            }
            this.canBeHitByWeapons = false;
            this.CollideWithObjects = false;
            this.CollideWithSlopes = false;
            this.CollideWithTerrain = false;
            this.sleeping = false;
            //this.Attachment = null;
            // for (int i = 0; i < this.tongues.Length; i++){
            //     this.tongues[i] = new GrappleTongue(this, i);
            // }
            this.abstractCreature.saveCreature = false;
            Debug.Log("Backpack Initiated!");
        }

/*
        public LauncherBackpack(AbstractCreature abstractCreature, World world, Player owner) : base(abstractCreature, world)
        {
            for (int i = 0; i < this.bodyChunks.Length; i++)
            {
                this.bodyChunks[i].mass *= 0.1f;
            }
            this.canBeHitByWeapons = false;
            this.CollideWithObjects = false;
            this.CollideWithSlopes = false;
            this.CollideWithTerrain = false;
            this.sleeping = false;
            this.Attachment = owner;
            for (int i = 0; i < this.tongues.Length; i++)
            {
                this.tongues[i] = new LauncherTongue(this, i);
            }
            this.abstractCreature.saveCreature = false;
            Debug.Log("Backpack Initiated!");
        }
*/


        public override void InitiateGraphicsModule()
        {
            Debug.Log("Backpack Graphics initiated");
            this.graphicsModule ??= new CommonBackpackGraphics(this);
        }

        public override void Update(bool eu)
        {

            this.sleeping = false;
            // Destroy worm upon resting in shelter
            if (this.grabbedBy.Count == 0)
            {
                Debug.Log("Sending backpack to Lost & Find");
                this.Destroy();
                return;
            }
            // for (int i = 0; i < this.tongues.Length; i++){
            //     if (this.tongues[i].Attached){
            //         for (int j = 0; j < this.grabbedBy.Count; j++){
            //             if (grabbedBy[j].grabber is Player p){
            //                 p.mainBodyChunk.vel.x *= 1.7f;
            //             }
            //         }
            //     }
            // }
            for (int j = 0; j < this.grabbedBy.Count; j++)
            {
                if (this.tongues[1].Attached)
                {
                    if (grabbedBy[j].grabber is Player p)
                    {
                        p.mainBodyChunk.vel.x = Mathf.Clamp(p.mainBodyChunk.vel.x, -12f, 12f);
                        p.mainBodyChunk.vel.y = Mathf.Clamp(p.mainBodyChunk.vel.y, -24f, 24f);
                        this.mainBodyChunk.vel.x = Mathf.Clamp(this.mainBodyChunk.vel.x, -12f, 12f);
                        this.mainBodyChunk.vel.y = Mathf.Clamp(this.mainBodyChunk.vel.y, -24f, 24f);
                    }
                }
            }

            if (this.useBool)
            {
                for (int i = 0; i < this.tongues.Length; i++)
                {
                    if (this.tongues[i].Attached)
                    {
                        for (int j = 0; j < this.grabbedBy.Count; j++)
                        {
                            if (grabbedBy[j].grabber is Player p)
                            {
                                if (p.bodyMode != Player.BodyModeIndex.ZeroG)
                                {
                                    p.bodyChunks[0].vel.y += 15f;
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            base.Update(eu);
        }

        public override bool CanBeGrabbed(Creature grabber)
        {
            if (this.grabbedBy.Count > 0)
            {
                return false;
            }
            return base.CanBeGrabbed(grabber);
        }

        public override void Violence(BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, Appendage.Pos onAppendagePos, DamageType type, float damage, float stunBonus)
        {
            base.Violence(source, directionAndMomentum, hitChunk, onAppendagePos, type, 0f, stunBonus);
        }

        public new bool JumpButton(Player plr)
        {
            if (plr.canJump < 1 && plr.bodyMode == Player.BodyModeIndex.ZeroG)
            {
                this.useBool = true;
                return false;
            }
            if (plr.canJump > 0 && plr.bodyMode != Player.BodyModeIndex.ZeroG && plr.bodyChunks[1].contactPoint.y < 0 && (this.tongues[0].Attached || this.tongues[1].Attached))
            {
                this.useBool = true;
                return false;
            }
            return base.JumpButton(plr);
        }

        /*
                public new void GrabbedByPlayer(){
                    base.GrabbedByPlayer();
                    if (this.tongues[0].mode == TubeWorm.Tongue.Mode.ShootingOut){
                        Player player = null;
                        for (int i = 0; i < grabbedBy.Count; i++){
                            if (grabbedBy[i].grabber is Player){
                                player = grabbedBy[i].grabber as Player;
                                break;
                            }
                        }
                        Vector2 vector = new Vector2(-player.flipDirection, -0.7f).normalized;
                        if (player.input[0].y > 0){
                            vector = new Vector2(0f, -1f);
                        }
                        this.tongues[1].Shoot(vector);
                        Ebug("Use!");
                    }
                }
        */

        public static void BackpackGrabbedByPlayer(On.TubeWorm.orig_GrabbedByPlayer orig, TubeWorm self)
        {
            if (self is not LauncherBackpack)
            {
                orig(self);
                return;
            }
            if (!self.lastGrabbed)
            {
                self.tongues[0].elastic = 1f;
                self.tongues[1].elastic = 1f;
            }
            self.sleeping = false;
            Player player = null;
            for (int i = 0; i < self.grabbedBy.Count; i++)
            {
                if (self.grabbedBy[i].grabber is Player)
                {
                    player = self.grabbedBy[i].grabber as Player;
                    break;
                }
            }
            self.tongues[0].baseChunk = player.mainBodyChunk;
            self.tongues[1].baseChunk = (self.tongues[1].Attached ? player.mainBodyChunk : player.bodyChunks[1]);
            int changer = 0;
            if (self.tongues[0].Attached && self.tongues[1].Attached)
            {
                if (Mathf.Abs(self.tongues[0].AttachedPos.x - self.tongues[1].AttachedPos.x) < Mathf.Abs(self.tongues[0].AttachedPos.y - self.tongues[1].AttachedPos.y))
                {
                    changer = -player.input[0].y;
                }
                else
                {
                    changer = player.input[0].x;
                }
            }

            self.onRopePos = (self.tongues[0].Attached, self.tongues[1].Attached) switch
            {
                (true, false) => Mathf.Min(1f, self.onRopePos + 0.1f),
                (false, true) => Mathf.Max(0f, self.onRopePos - 0.1f),
                (false, false) => (self.onRopePos < 0.5f ? Mathf.Min(0.49f, self.onRopePos + 0.025f) : Mathf.Max(0.5f, self.onRopePos - 0.025f)),
                (true, true) => Mathf.Clamp(self.onRopePos + 0.035f * changer, 0.01f, 0.99f)
            };
            //Ebug("OnRopePos: " + self.onRopePos);

            // Get direction of tongue
            bool doubleMode = (player.input[0].x == 0 && player.input[0].y == 0 || !(player.input[0].x != 0 && player.input[0].y != 0));
            (Vector2 v1, Vector2 v2) = (player.input[0].x, player.input[0].y) switch
            {
                (0, 0) => (new Vector2(1f, 0f), new Vector2(-1f, 0f)),
                (not 0, 0) => (new Vector2(player.flipDirection, 0.7f).normalized, new Vector2(player.flipDirection, -0.7f).normalized),
                (0, > 0) => (new Vector2(player.flipDirection, 0.7f).normalized, new Vector2(-player.flipDirection, 0.7f).normalized),
                (0, < 0) => (new Vector2(player.flipDirection, -0.7f).normalized, new Vector2(-player.flipDirection, -0.7f).normalized),
                (_, _) => (new Vector2(player.flipDirection, 0.7f).normalized, new Vector2(player.flipDirection, -0.7f).normalized)
            };

            // Aim down when zeroG
            if (!doubleMode && player.bodyMode == Player.BodyModeIndex.ZeroG && player.input[0].y < 0)
            {
                v1 = v2;
            }

            player.tubeWorm = self;
            if (!doubleMode && !self.tongues[0].Attached) self.onRopePos = 1f;

            if (doubleMode)
            {
                self.mainBodyChunk.vel += (v1 + v2).normalized * 3f;
                self.bodyChunks[1].vel -= (v1 + v2).normalized * 3f;
            }
            else
            {
                self.mainBodyChunk.vel += v1 * 3f;
                self.bodyChunks[1].vel -= v1 * 3f;
            }
            if (player.enteringShortCut.HasValue)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (self.tongues[j].Attached)
                    {
                        self.tongues[j].Release();
                    }
                }
            }
            else if (ModManager.MMF && player.animation == Player.AnimationIndex.VineGrab)
            {
                if (!player.input[0].jmp || player.input[1].jmp)
                {
                    return;
                }
                for (int k = 0; k < 2; k++)
                {
                    if (self.tongues[k].Attached)
                    {
                        self.tongues[k].Release();
                        self.useBool = false;
                    }
                }
                if (self.useBool && !self.dead)
                {
                    (self.tongues[0] as LauncherTongue).Shoot(v1);
                    if (doubleMode) (self.tongues[1] as LauncherTongue).Shoot(v2);
                    self.useBool = false;
                }
            }
            else
            {

                for (int j = 0; j < 2; j++)
                {
                    if (self.tongues[j].mode == TubeWorm.Tongue.Mode.ShootingOut)
                    {
                        (self as LauncherBackpack).detatcherClock = 40;
                    }
                    if (Mathf.Max(Mathf.Abs(self.mainBodyChunk.vel.x), Mathf.Abs(self.mainBodyChunk.vel.y)) < 3f)
                    {
                        if (self.tongues[j].Attached)
                        {
                            (self as LauncherBackpack).detatcherClock--;
                            if ((self as LauncherBackpack).detatcherClock < 0)
                            {
                                self.tongues[j].Release();
                            }
                        }
                    }
                }
                if (self.useBool && !self.dead)
                {
                    (self.tongues[1] as LauncherTongue).Shoot(v2);
                    if (doubleMode) (self.tongues[0] as LauncherTongue).Shoot(v1);
                }
                self.useBool = false;
            }
        }
    }

    public class CommonBackpackGraphics : TubeWormGraphics
    {
        public Color tipColor = Color.white;
        public Color tongueColor = Color.gray;
        public CommonBackpackGraphics(PhysicalObject ow) : base(ow)
        {
            this.color = Color.white;
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            base.ApplyPalette(sLeaser, rCam, palette);
            //sLeaser.sprites[2].color = tongueColor;
            for (int i = 3; i < 5; i++)
            {
                sLeaser.sprites[i].color = tipColor;
            }
            for (int j = 0; j < (sLeaser.sprites[2] as TriangleMesh).verticeColors.Length; j++)
            {
                (sLeaser.sprites[2] as TriangleMesh).verticeColors[j] = Color.Lerp(tipColor, tongueColor, Mathf.InverseLerp(0, (sLeaser.sprites[2] as TriangleMesh).verticeColors.Length, j));
            }
        }

        public void SetColor(string name, Color color)
        {
            if (name == "Backpack")
            {
                this.color = color;
            }
            else if (name == "Tongue")
            {
                this.tongueColor = color;
            }
            else if (name == "TongueTip")
            {
                this.tipColor = color;
            }
        }

        public void SetColor(Color bodyColor = new(), Color tongueColor = new(), Color tipColor = new()){
            Color defaulted = new();
            if (bodyColor != defaulted) this.color = bodyColor;
            if (tongueColor != defaulted) this.tongueColor = tongueColor;
            if (tipColor != defaulted) this.tipColor = tipColor;
        }
    }
}
