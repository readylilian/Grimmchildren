using Fireball;
using Fisobs.Core;
using System;
using UnityEngine;
using RWCustom;

namespace SlugTemplate.Hooks
{

    public class PlayerHooks
    {
        public static int fireBalls = 0;
        public static bool metIterator = true;

        //This doesn't do much rn, would love to expand it out later.
        public class firingState
        {
            public IntVector2 direction;

            //Basic velocity, will need to change once it's applied
            public Vector2 Velocity
            {
                get
                {
                    return direction.ToVector2().normalized * 1;
                }
            }
        }

        public static void Init()
        {
            // example of a hook
            On.Player.Jump += Player_Jump;

            Debug.Log("Fireball is enabled");
            Content.Register(new FireballFisob());
            On.RainWorld.OnModsInit += RainWorldOnModsInitHook;

            On.Player.SwallowObject += SnowSwallowObject;
            On.Room.AddObject += RoomAddFire;
        }

        private static void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            // Call the original method we hook from first
            orig(self);

            // Adding power to jump
            self.jumpBoost *= 1f + 0.5f;


            // Currently, we don't have an if statement to specify which slugcat this happens to
            // This means it happens to all of them, in every campaign for each jump
        }
        private static void RainWorldOnModsInitHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            Debug.Log("Fireball is loading sprites");
            orig(self);
            Fireball.Fireball.LoadSprites();
        }

        //Adds a fireball to the room
        private static void RoomAddFire(On.Room.orig_AddObject orig, Room self, UpdatableAndDeletable obj)
        {
            if (obj is Fireball.Fireball fire && fireBalls > 0)
            {
                fireBalls--;
                firingState firing = new firingState();
                firing.direction = new IntVector2(0, 1);
                var abstr = new FireballAbstract(self.world, self.PlayersInRoom[0].abstractCreature.pos, self.game.GetNewID());
                obj = new Fireball.Fireball(abstr, fire.tailPos, firing.Velocity);
                self.abstractRoom.AddEntity(abstr);
            }
            orig(self, obj);
        }

        //When we swallow an object, if it's our cat, make it a fireball
        private static void SnowSwallowObject(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            Debug.Log("Tried to swallow");
            //Registering didn't work, even moving this stuff over to slugbase
            //so create the name when you check it
            if (self.SlugCatClass == new SlugcatStats.Name("SnowCat") && metIterator)
            {
                //This is mostly the normal code just updated for our dude and our level of access
                //Removed any reference to objectInStomach to prevent storage
                if (grasp < 0 || self.grasps[grasp] == null)
                {
                    return;
                }
                //Remove the object from the room
                AbstractPhysicalObject abstractPhysicalObject = self.grasps[grasp].grabbed.abstractPhysicalObject;
                if (ModManager.MMF && self.room.game.session is StoryGameSession)
                {
                    (self.room.game.session as StoryGameSession).RemovePersistentTracker(abstractPhysicalObject);
                }
                self.ReleaseGrasp(grasp);
                abstractPhysicalObject.realizedObject.RemoveFromRoom();
                abstractPhysicalObject.Abstractize(self.abstractCreature.pos);
                abstractPhysicalObject.Room.RemoveEntity(abstractPhysicalObject);

                //We can make this fancy a little later, so the # of fireballs you get
                //Can change based on what we eat or something, for now just add two
                fireBalls += 1;
                //Add fireball to room
                var abstr = new FireballAbstract(self.room.world, self.abstractCreature.pos, self.room.game.GetNewID());
                //Use our velocity
                self.objectInStomach = abstr;
                self.objectInStomach.Abstractize(self.abstractCreature.pos);
                self.mainBodyChunk.vel.y += 2f;
                self.room.PlaySound(SoundID.Slugcat_Swallow_Item, self.mainBodyChunk);
                return;
            }
            else
            {
                orig(self, grasp);
            }

        }
    }
}