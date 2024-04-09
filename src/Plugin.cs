using System;
using BepInEx;
using Fisobs.Core;
using RWCustom;
using Fisobs.Core;
using UnityEngine;
using SlugBase.Features;
using SlugTemplate.Hooks;
using SlugTemplate.Ice_Block;
using static SlugBase.Features.FeatureTypes;
using System.IO;
using IteratorKit.CMOracle;
using static IteratorKit.CMOracle.CMOracleBehavior;
using static IteratorKit.IteratorKit;

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
            
            // Add hooks to the game
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        }

       // public static readonly Oracle.OracleID SRS = new Oracle.OracleID("SRS", register: true);


        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            
            // Required or keys mess up
            //On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            Pom.Pom.RegisterManagedObject<PlacedIceBlock, IceBlockData, Pom.Pom.ManagedRepresentation>("IceBlock",
                "ColdSnap", false);
            Pom.Pom.RegisterManagedObject<PlacedIceBlockPhys, IceBlockPhysData, Pom.Pom.ManagedRepresentation>("Melting Block",
                "ColdSnap", false);
            // Honestly, no idea what the extra class does. I never had one in my mod.
            // This hook can probably be deleted if we never load resources
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            //On.OracleBehavior.Update += OracleBehavior_Update;
            CMOracleBehavior.OnEventEnd += snowcatIterator.OnEventEnd;
            CMOracleBehavior.OnEventStart += snowcatIterator.OnEventStart;
            On.OracleBehavior.Update += snowcatIterator.OracleBehavior_Update;
            // Enable our custom hooks
            PlayerHooks.Init();
            RoomHooks.Init();
            FireHooks.Init();
            
            
            // Enables the options menu
            MachineConnector.SetRegisteredOI("GrimmChildrenMod", options);
        }

        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            //Pom.Pom.RegisterManagedObject<PlacedIceBlock, IceBlockData, Pom.Pom.ManagedRepresentation>("IceBlock",
            //"ColdSnap", false);
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