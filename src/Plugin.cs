using System;
using BepInEx;
using Fisobs.Core;
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
            Content.Register(new IceBlockFisobs());
            
            // Add hooks to the game
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            
            // Temp method for testing
            On.Player.ctor += Player_ctor;
            //On.Player.MovementUpdate += PlayerOnMovementUpdate;
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