using BepInEx;
using RWCustom;
using UnityEngine;
using static Player;
using Fisobs;
using Fisobs.Core;
using System.IO;
using System;

namespace SlugTemplate.Hooks
{
    internal static class FireHooks
    {
        public static int fireBalls = 0;
        public static bool metIterator = true;
        //private static bool filoaded = false;

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
            Debug.Log("Fireball is enabled");

            Content.Register(new FireballFisob());
            On.Player.SwallowObject += SnowSwallowObject;
            On.Room.AddObject += RoomAddFire;
        }

        private static void WeaponSetRandomSpin(On.Weapon.orig_SetRandomSpin orig, Weapon self)
        {
            // Room for some reason never saves its value in fireball so this will throw an error
            // I've seen no other errors as a result of this yet, but it may need to be fixed eventually
            if (self is not Fireball)
            {
                orig(self);
            }
        }


        //Adds a fireball to the room
        private static void RoomAddFire(On.Room.orig_AddObject orig, Room self, UpdatableAndDeletable obj)
        {
            if (obj is Fireball fire && fireBalls > 0)
            {
                fireBalls--;
                firingState firing = new firingState();
                firing.direction = new IntVector2(0, 1);
                var abstr = new FireballAbstract(self.world, self.PlayersInRoom[0].abstractCreature.pos, self.game.GetNewID());
                PlacedObject fakePObj = new PlacedObject(PlacedObject.Type.LightFixture, null);
                self.AddObject(new HolyFire(self, fakePObj, fakePObj.data as PlacedObject.LightFixtureData));
                obj = new Fireball(abstr, fire.tailPos, firing.Velocity, fakePObj);
                obj.room = self;
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
            if (self.SlugCatClass == new SlugcatStats.Name("Snowcat") && metIterator)
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
        /*
        public static void LoadSprites()
        {
            if(!filoaded)
            {
                string path = Path.DirectorySeparatorChar +
               "RainWorld_Data" + Path.DirectorySeparatorChar + "StreamingAssets" +
               Path.DirectorySeparatorChar + "mods" + Path.DirectorySeparatorChar + "GrimmChildren" +
               Path.DirectorySeparatorChar;
                try
                {
                    Debug.Log(Directory.GetCurrentDirectory());
                    Debug.Log(Directory.GetFiles(Directory.GetCurrentDirectory()));
                    Futile.atlasManager.LoadAtlas(Directory.GetCurrentDirectory() + path + "icon_Fireball.json");
                    Futile.atlasManager.LoadImage(Directory.GetCurrentDirectory() + path + "icon_Fireball.png");
                    filoaded = true;
                }
                catch (Exception ex)
                {
                    Debug.LogError("LoadSprites exception: " + ex.ToString());
                }
                Debug.Log("LoadSprites called in fireball");
            }
        }*/
    }
}
