namespace SlugTemplate.Fireball;

using RWCustom;
using Fisobs;
using UnityEngine;
using System.IO;
using System;
using System.Threading;
using MoreSlugcats;

    class Fireball : Weapon, IProvideWarmth
    {
        public float lastDarkness = -1f;
        public float darkness;

        public override float ThrowPowerFactor => 0.5f;
        private Color blackColor;

        public FireballAbstract Abstr { get; }

        public Room loadedRoom => room;
        public float warmth => RainWorldGame.DefaultHeatSourceWarmth;
        public float range => 350f;

        public float damage;

        private float lifespan;
        private float timer;
        private bool beenThrown = false;
        //Mostly pearl, rock and spear stats
        public Fireball(FireballAbstract abstr, Vector2 pos, Vector2 vel) : base(abstr, abstr.world)
        {
            Abstr = abstr;
            bodyChunks = new BodyChunk[1];
            bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 5f, 0.07f);
            bodyChunkConnections = new BodyChunkConnection[0];
            airFriction = 0.999f;
            gravity = 0.9f;
            bounce = 0.4f;
            surfaceFriction = 0.45f;
            collisionLayer = 2;
            waterFriction = 0.92f;
            buoyancy = 0.4f;
            tailPos = base.firstChunk.pos;
            damage = 2f;
            lifespan = 3f;
        }

        //Update the state of the fireball
        public override void Update(bool eu)
        {
            //Added the fire disappearing due to a weird bug that popped up
            if (beenThrown)
            {
                timer += Time.deltaTime;
                if (timer >= lifespan)
                {
                    Destroy();
                }
            }
            firstChunk.collideWithTerrain = grabbedBy.Count == 0;
            firstChunk.collideWithSlopes = grabbedBy.Count == 0;

            if (firstChunk.onSlope != 0)
                surfaceFriction = 0f; //makes ball roll off slopes faster

            //Allows ball to roll off slopes
            base.doNotTumbleAtLowSpeed = mode == Mode.Thrown;


            //This is stolen almost directly from weapon update to get it to hit things
            Vector2 pos = base.firstChunk.pos + base.firstChunk.vel;
            SharedPhysics.CollisionResult result = SharedPhysics.TraceProjectileAgainstBodyChunks(this, room, firstFrameTraceFromPos.HasValue ? firstFrameTraceFromPos.Value : base.firstChunk.pos, ref pos, base.firstChunk.rad + ((thrownBy != null && thrownBy is Player) ? 5f : 0f), 1, thrownBy, hitAppendages: true);
            bool flag2 = HitSomething(result, eu);
            float num6 = 0f;
            for (int num7 = 0; num7 < room.physicalObjects[0].Count; num7++)
            {
                for (int num9 = 0; num9 < room.physicalObjects[0][num7].bodyChunks.Length; num9++)
                {
                    BodyChunk bodyChunk = room.physicalObjects[0][num7].bodyChunks[num9];
                    float num10 = Custom.CirclesCollisionTime(base.firstChunk.lastPos.x, base.firstChunk.lastPos.y, bodyChunk.pos.x, bodyChunk.pos.y, base.firstChunk.pos.x - base.firstChunk.lastPos.x, base.firstChunk.pos.y - base.firstChunk.lastPos.y, base.firstChunk.rad + ((thrownBy != null && thrownBy is Player) ? 5f : 0f), bodyChunk.rad);
                    if (!(num10 > 0f) || !(num10 < 1f))
                    {
                        continue;
                    }
                    if (room.physicalObjects[0][num7] is Weapon && mode == Mode.Thrown && (room.physicalObjects[0][num7] as Weapon).mode == Mode.Thrown && (room.physicalObjects[0][num7] as Weapon).HeavyWeapon)
                    {
                        if (HeavyWeapon)
                        {
                            HitAnotherThrownWeapon(room.physicalObjects[0][num7] as Weapon);
                            num6 = float.MaxValue;
                            break;
                        }
                        continue;
                    }
                    bodyChunk.vel += base.firstChunk.vel;
                    HitSomethingWithoutStopping(room.physicalObjects[0][num7], bodyChunk, null);
                    num6 += bodyChunk.mass;
                    if (num6 > 0.6f)
                    {
                        base.firstChunk.pos = bodyChunk.pos;
                        ChangeMode(Mode.Free);
                        base.firstChunk.vel *= 0.5f;
                        room.PlaySound(SoundID.Spear_Hit_Small_Creature, bodyChunk);
                        break;
                    }
                    if (num6 <= 0.6f && room.physicalObjects[0][num7].appendages != null)
                    {
                        for (int num11 = 0; num11 < room.physicalObjects[0][num7].appendages.Count; num11++)
                        {
                            if (room.physicalObjects[0][num7].appendages[num11].canBeHit && room.physicalObjects[0][num7].appendages[num11].LineCross(base.firstChunk.lastPos, base.firstChunk.pos))
                            {
                                (room.physicalObjects[0][num7].appendages[num11].owner as IHaveAppendages).ApplyForceOnAppendage(new Appendage.Pos(room.physicalObjects[0][num7].appendages[num11], 0, 0.5f), base.firstChunk.vel * base.firstChunk.mass);
                                HitSomethingWithoutStopping(room.physicalObjects[0][num7], null, room.physicalObjects[0][num7].appendages[num11]);
                            }
                        }
                    }
                    if (num6 > 0.6f)
                    {
                        break;
                    }
                }
            }
            //Then do normal update stuff
            base.Update(eu);

            var chunk = firstChunk;
            if (base.firstChunk.ContactPoint.y != 0)
                this.rotationSpeed = (this.rotationSpeed * 2f + base.firstChunk.vel.x * 5f) / 3f;
        }
        //This hits without changing direction
        public override void HitSomethingWithoutStopping(PhysicalObject obj, BodyChunk chunk, Appendage appendage)
        {
            //Don't hit oracles
            if (obj is Oracle)
                return;
            base.HitSomethingWithoutStopping(obj, chunk, appendage);

            //If you hit a creature, do explosion and electric damage
            if (obj is Creature && obj != thrownBy)
            {
                Debug.Log(obj);
                Debug.Log(thrownBy);
                Creature creature = (Creature)obj;
                if (creature != null && thrownBy != null)
                {
                    creature.SetKillTag(thrownBy.abstractCreature);
                    creature.Violence(thrownBy.mainBodyChunk, null, chunk, null, Creature.DamageType.Explosion, damage / 1, 0f);
                    creature.Violence(thrownBy.mainBodyChunk, null, chunk, null, Creature.DamageType.Electric, damage / 1, 0f);
                }
            }
        }

        //Hit and change direction
        public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
        {
            Mode prevMode = this.mode;
            bool hit = base.HitSomething(result, eu);

            //failsafe allow pick up (if TerrainImpact wasn't triggered), & throwdir up works
            if (firstChunk.vel.magnitude < 5f && base.firstChunk.ContactPoint.y != 0)
            {
                ChangeMode(Mode.Free);
            }

            if (!hit || this.room == null)
                return hit;

            //Don't hit oracles
            if (result.obj is Oracle)
                return hit;
            //another creature was 
            if (result.obj is Creature && result.obj != thrownBy)
            {
                if(!result.obj.ToString().Contains("Slugcat"))
                {
                    room.PlaySound(SoundID.Rock_Hit_Creature, firstChunk);
                }
                Debug.Log(result.obj);
                Debug.Log("Fireball hit creature");
                BodyChunk bodyChunk = null;
                Creature creature = (Creature)result.obj;
                if (result.chunk != null)
                {
                    bodyChunk = result.chunk;
                }
                if (creature != null && thrownBy != null)
                {
                    creature.SetKillTag(thrownBy.abstractCreature);
                    creature.Violence(thrownBy.mainBodyChunk, null, bodyChunk, null, Creature.DamageType.Explosion, damage / 1, 0f);
                    creature.Violence(thrownBy.mainBodyChunk, null, bodyChunk, null, Creature.DamageType.Electric, damage / 1, 0f);
                }
            }
            return hit;
        }
        //If we hit the ground start the timer to poof
        //Also reset who threw it to prevent the bug with it being grabbed
        public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            base.TerrainImpact(chunk, direction, speed, firstContact);
            //Reset thrownby once it hits the ground, so you can't use a thrown ball to do damage while it's held
            thrownBy = null;
            beenThrown = true;
        }
        //I know this doesn't do anything but the whole thing breaks if we get rid of it
        public override void ChangeMode(Mode newMode)
        { }


        // room is null which fires error, but if no error occurs the fireball doesn't spawn?
        /*public override void PlaceInRoom(Room placeRoom)
        {
            room = placeRoom;
            placeRoom.AddObject(this);
            for (int i = 0; i < base.bodyChunks.Length; i++)
            {
                base.bodyChunks[i].HardSetPosition(placeRoom.MiddleOfTile(this.abstractPhysicalObject.pos));
            }
            this.NewRoom(placeRoom);
            this.SetRandomSpin();
        }*/

        //-----------------------------------Sprite Stuff Do Not Touch-----------------------------------------------------------------
        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            Debug.Log("Tried to draw sprite");
            //Futile.atlasManager.LogAllElementNames();
            sLeaser.sprites = new FSprite[2];
            sLeaser.sprites[0] = new FSprite(Futile.atlasManager.GetElementWithName("icon_Fireball"));
            sLeaser.sprites[1] = new FSprite(Futile.atlasManager.GetElementWithName("icon_Fireball"));
            AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
            float num = Mathf.InverseLerp(305f, 380f, timeStacker);
            pos.y -= 20f * Mathf.Pow(num, 3f);
            lastDarkness = darkness;
            darkness = rCam.room.Darkness(pos);
            darkness *= 1f - 0.5f * rCam.room.LightSourceExposure(pos);

            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].x = pos.x - camPos.x;
                sLeaser.sprites[i].y = pos.y - camPos.y;
                sLeaser.sprites[i].rotation = Custom.AimFromOneVectorToAnother(new Vector2(), Vector3.Slerp(this.lastRotation, this.rotation, timeStacker));
            }

            //blink when able to pick up
            if (blink > 0)
            {
                sLeaser.sprites[0].color = blinkColor;
            }
            else
            {
                sLeaser.sprites[0].color = Color.Lerp(Color.white, blackColor, darkness);
            }

            //remove sprite
            if (slatedForDeletetion || room != rCam.room)
                sLeaser.CleanSpritesAndRemove();
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            blackColor = palette.blackColor;
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContainer)
        {
            newContainer ??= rCam.ReturnFContainer("Items");

            foreach (FSprite fsprite in sLeaser.sprites)
            {
                fsprite.RemoveFromContainer();
                newContainer.AddChild(fsprite);
            }
        }

        public Vector2 Position()
        {
            return tailPos;
        }
    }
