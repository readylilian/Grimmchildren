using System;
using BepInEx;
using Fisobs.Core;
using RWCustom;
using UnityEngine;
using SlugBase.Features;
using SlugTemplate.Hooks;
using SlugTemplate.Ice_Block;
using static SlugBase.Features.FeatureTypes;

namespace SlugTemplate
{
    // Additional dependency for Fisobs to prevent errors. I think. I don't know what this does.
    [BepInDependency("github.notfood.BepInExPartialityWrapper", BepInDependency.DependencyFlags.SoftDependency)]
    
    // Connects us to the api
    [BepInPlugin("GrimmChildrenMod", "GrimmChildren", "0.1.0")]
    public sealed class Plugin : BaseUnityPlugin
    {
        public static Options options;
        
        public Plugin()
        {
            try
            {
                options = new Options(this, Logger);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }
        
        // Enable all mod hooks. This is the entry point for the entire mod
        public void OnEnable()
        {
            // Add custom objects to the game
            //Content.Register(new IceBlockFisobs());
            
            // Add hooks to the game
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            
            // Temp method for testing
            On.Player.ctor += Player_ctor;
            
        }

        private PhysicalObject PlayerOnPickupCandidate(On.Player.orig_PickupCandidate orig, Player self, float favorSpears)
        {
            PhysicalObject result = null;
            float num = float.MaxValue;
            for (int i = 0; i < self.room.physicalObjects.Length; i++)
            {
                for (int j = 0; j < self.room.physicalObjects[i].Count; j++)
                {
                    // ReSharper disable once ReplaceWithSingleAssignment.False
                    bool check1 = false;

                    if (!(self.room.physicalObjects[i][j] is PlayerCarryableItem))
                    {
                        check1 = true;
                    }

                    if (!check1 && (self.room.physicalObjects[i][j] as PlayerCarryableItem).forbiddenToPlayer < 1)
                    {
                        if (Custom.DistLess(self.bodyChunks[0].pos, self.room.physicalObjects[i][j].bodyChunks[0].pos,
                                self.room.physicalObjects[i][j].bodyChunks[0].rad + 40f))
                        {
                            if (Custom.DistLess(self.bodyChunks[0].pos, self.room.physicalObjects[i][j].bodyChunks[0].pos,
                                    self.room.physicalObjects[i][j].bodyChunks[0].rad + 20f))
                            {
                                check1 = true;
                            }
                        }
                    }

                    if (!check1 && self.room.VisualContact(self.bodyChunks[0].pos,
                            self.room.physicalObjects[i][j].bodyChunks[0].pos))
                    {
                        if (self.CanIPickThisUp(self.room.physicalObjects[i][j]))
                        {
                            check1 = true;
                        }
                    }
                    
                    if (check1)
                    {
                        float num2 = Vector2.Distance(self.bodyChunks[0].pos, self.room.physicalObjects[i][j].bodyChunks[0].pos);
                        if (self.room.physicalObjects[i][j] is Spear)
                        {
                            num2 -= favorSpears;
                        }
                        if (self.room.physicalObjects[i][j].bodyChunks[0].pos.x < self.bodyChunks[0].pos.x == self.flipDirection < 0)
                        {
                            num2 -= 10f;
                        }
                        if (num2 < num)
                        {
                            result = self.room.physicalObjects[i][j];
                            num = num2;
                        }
                    }
                }
            }
            return result;
        }

        private void PlayerOnMovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            orig(self, eu);
        }

        private void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            AbstractIceBlock iceBlock = new AbstractIceBlock(world, AbstractPhysicalObject.AbstractObjectType.CollisionField, null,
                self.room.GetWorldCoordinate(self.mainBodyChunk.pos), abstractCreature.Room.world.game.GetNewID());
            
            // Add to room 
            abstractCreature.Room.AddEntity(iceBlock);
            
            // Realize in room
            iceBlock.RealizeInRoom();
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            
            // Honestly, no idea what the extra class does. I never had one in my mod.
            // This hook can probably be deleted if we never load resources
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            
            // Enable our custom hooks
            PlayerHooks.Init();
            RoomHooks.Init();
            
            
            // Enables the options menu
            MachineConnector.SetRegisteredOI("GrimmChildrenMod", options);
        }

        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            // Unused, sad
        }
    }
}