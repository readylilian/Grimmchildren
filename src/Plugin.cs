using System;
using BepInEx;
using RWCustom;
using Fisobs.Core;
using UnityEngine;
using SlugBase.Features;
using SlugTemplate.Hooks;
using static SlugBase.Features.FeatureTypes;
using System.IO;

namespace SlugTemplate
{
    // Additional dependency for Fisobs to prevent errors. I think. I don't know what this does.
    [BepInDependency("github.notfood.BepInExPartialityWrapper", BepInDependency.DependencyFlags.SoftDependency)]
    
    // Connects us to the api
    [BepInPlugin("GrimmChildrenMod", "GrimmChildren", "0.1.0")]
    public class Plugin : BaseUnityPlugin
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

            On.RainWorld.OnModsInit += RainWorld_OnModsInit;

        }

       // public static readonly Oracle.OracleID SRS = new Oracle.OracleID("SRS", register: true);


        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            
            // Honestly, no idea what the extra class does. I never had one in my mod.
            // This hook can probably be deleted if we never load resources
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            //On.OracleBehavior.Update += OracleBehavior_Update;

            // Enable our custom hooks
            PlayerHooks.Init();
            FireHooks.Init();
            
            // Enables the options menu
            MachineConnector.SetRegisteredOI("GrimmChildrenMod", options);
        }

        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            //Futile.atlasManager.LoadImage("atlases/icon_Fireball");
            //Futile.atlasManager.LoadImage("icon_Fireball");
        }

/*        private void OracleBehavior_Update(On.OracleBehavior.orig_Update orig, OracleBehavior self, bool eu)
        {
            orig(self, eu);
            if (self.oracle.ID == SRS)
            {
                // your custom code here
            }
        }*/
    }
}