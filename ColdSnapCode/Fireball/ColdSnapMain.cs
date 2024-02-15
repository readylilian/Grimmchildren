using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreSlugcats;
using CoralBrain;
using Expedition;
using JollyCoop;
using RWCustom;
using UnityEngine;


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
        public bool metIterator = false;

        public class firingState
        {
            public Vector2 direction;
            public bool firing;
            private int charging;
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
                    return direction.normalized * charging;
                }
            }
        }

        private void OnEnable()
        {
            On.Player.SwallowObject += SnowSwallowObject;
            On.Player.Regurgitate += NoRegurge;
            On.Player.ThrowObject += Fire;
        }

        private void Fire(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu)
        {
            if (self.SlugCatClass == new SlugcatStats.Name("SnowCat"))
            {
            }
            else
            {
                orig(self, grasp, eu);
            }
        }

        void SnowSwallowObject(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            //Registering didn't work, even moving this stuff over to slugbase
            //so create the name when you check it
            if (self.SlugCatClass == new SlugcatStats.Name("SnowCat"))
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
                self.mainBodyChunk.vel.y += 2f;
                self.room.PlaySound(SoundID.Slugcat_Swallow_Item, self.mainBodyChunk);
                //We can make this fancy a little later, so the # of fireballs you get
                //Can change based on what we eat or something, for now just add two
                fireBalls += 2;
                Debug.Log(fireBalls);
                return;
            }
            else
            {
                orig(self, grasp);
            }

        }
        void NoRegurge(On.Player.orig_Regurgitate orig, Player self)
        {
            //If our cat tries to throw up don't do anything
            if (self.SlugCatClass == new SlugcatStats.Name("SnowCat"))
            {
                return;
            }
            else
            {
                orig(self);
            }
        }
    }
}
