using BepInEx;
using RWCustom;
using UnityEngine;
using static Player;
using Fisobs;
using Fisobs.Core;
using System;


namespace Fireball
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    //Put mod dependencies here, like slugbase
    public class ColdSnapMain : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "GrimmChildren.ColdSnap"; // This should be the same as the id in modinfo.json
        public const string PLUGIN_NAME = "Cold Snap"; 
        public const string PLUGIN_VERSION = "1.0.0";

        public static int fireBalls = 0;
        public bool metIterator = true;

        //private AbstractPhysicalObject fireEntity = new Fireball();
        public class firingState
        {
            public IntVector2 direction;
            public bool firing;
            private int charging = 1;
            //The most it could be charged is 10
            public int Charging
            {
                get
                {
                    return charging;
                }
                set
                {
                    if (charging + value > 10)
                    {
                        charging += value;
                    }
                    else if (charging + value <=0)
                    {
                        charging = 1;
                    }
                    else
                    {
                        charging = 10;
                    }
                }
            }

            //Basic velocity, will need to change once it's applied
            public Vector2 Velocity
            {
                get 
                {
                    return direction.ToVector2().normalized * charging;
                }
            }
        }

        private void OnEnable()
        {
            Content.Register(new FireballFisob());
            On.RainWorld.OnModsInit += RainWorldOnModsInitHook;

            On.Player.SwallowObject += SnowSwallowObject;
            On.Room.AddObject += RoomAddFire;

        }

        private void RainWorldOnModsInitHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            Fireball.LoadSprites();
        }

        private void RoomAddFire(On.Room.orig_AddObject orig, Room self, UpdatableAndDeletable obj)
        {
            if(obj is Fireball fire && fireBalls > 0)
            {
                fireBalls--;
                firingState firing = new firingState();
                firing.direction = new IntVector2(0,1);
                var abstr = new FireballAbstract(self.world, self.PlayersInRoom[0].abstractCreature.pos, self.game.GetNewID());
                obj = new Fireball(abstr, fire.tailPos, firing.Velocity);
                self.abstractRoom.AddEntity(abstr);
            }
            orig(self, obj);
        }

        private void SnowSwallowObject(On.Player.orig_SwallowObject orig, Player self, int grasp)
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
