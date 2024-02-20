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
using UnityEngine.UIElements;
using static Player;


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

        private AbstractPhysicalObject fireEntity;
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
            if (self.SlugCatClass == new SlugcatStats.Name("SnowCat") && metIterator)
            {
            }
            else
            {
                orig(self, grasp, eu);
            }
        }

        private void SnowSwallowObject(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
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
        private void NoRegurge(On.Player.orig_Regurgitate orig, Player self)
        {
            //If our cat tries to throw up don't do anything
            if (self.SlugCatClass == new SlugcatStats.Name("SnowCat") && metIterator)
            {
                if(fireBalls > 0)
                {
                    //Basically the same that the base has, but adjusted for only spitting out fireballs.
                    //Add fireball to room
                    self.room.abstractRoom.AddEntity(fireEntity);
                    fireEntity.pos = self.abstractCreature.pos;
                    fireEntity.RealizeInRoom();
                    Vector2 pos = self.bodyChunks[0].pos;
                    Vector2 vector = Custom.DirVec(self.bodyChunks[1].pos, self.bodyChunks[0].pos);
                    bool flag = false;
                    if (Mathf.Abs(self.bodyChunks[0].pos.y - self.bodyChunks[1].pos.y) > Mathf.Abs(self.bodyChunks[0].pos.x - self.bodyChunks[1].pos.x) && self.bodyChunks[0].pos.y > self.bodyChunks[1].pos.y)
                    {
                        pos += Custom.DirVec(self.bodyChunks[1].pos, self.bodyChunks[0].pos) * 5f;
                        vector *= -1f;
                        vector.x += 0.4f * (float)self.flipDirection;
                        vector.Normalize();
                        flag = true;
                    }
                    fireEntity.realizedObject.firstChunk.HardSetPosition(pos);
                    fireEntity.realizedObject.firstChunk.vel = Vector2.ClampMagnitude((vector * 2f + Custom.RNV() * UnityEngine.Random.value) / fireEntity.realizedObject.firstChunk.mass, 6f);
                    self.bodyChunks[0].pos -= vector * 2f;
                    self.bodyChunks[0].vel -= vector * 2f;
                    if (self.graphicsModule != null)
                    {
                        (self.graphicsModule as PlayerGraphics).head.vel += Custom.RNV() * UnityEngine.Random.value * 3f;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        self.room.AddObject(new WaterDrip(pos + Custom.RNV() * UnityEngine.Random.value * 1.5f, Custom.RNV() * 3f * UnityEngine.Random.value + vector * Mathf.Lerp(2f, 6f, UnityEngine.Random.value), waterColor: false));
                    }
                    self.room.PlaySound(SoundID.Slugcat_Regurgitate_Item, self.mainBodyChunk);
                    if (fireEntity.realizedObject is Hazer && self.graphicsModule != null)
                    {
                        (fireEntity.realizedObject as Hazer).SpitOutByPlayer(PlayerGraphics.SlugcatColor(self.playerState.slugcatCharacter));
                    }
                    if (flag && self.FreeHand() > -1)
                    {
                        if (ModManager.MMF && ((self.grasps[0] != null) ^ (self.grasps[1] != null)) && self.Grabability(fireEntity.realizedObject) == ObjectGrabability.BigOneHand)
                        {
                            int num = 0;
                            if (self.FreeHand() == 0)
                            {
                                num = 1;
                            }
                            if (self.Grabability(self.grasps[num].grabbed) != ObjectGrabability.BigOneHand)
                            {
                                self.SlugcatGrab(fireEntity.realizedObject, self.FreeHand());
                            }
                        }
                        else
                        {
                            self.SlugcatGrab(fireEntity.realizedObject, self.FreeHand());
                        }
                    }
                    fireBalls--;
                }    
                return;
            }
            else
            {
                orig(self);
            }
        }
    }
}
