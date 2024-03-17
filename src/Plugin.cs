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