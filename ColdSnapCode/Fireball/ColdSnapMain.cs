using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fireball
{
    //Put mod dependencies here, like slugbase
    public class ColdSnapMain : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "GrimmChildren.ColdSnap"; // This should be the same as the id in modinfo.json
        public const string PLUGIN_NAME = "Cold Snap"; 
        public const string PLUGIN_VERSION = "1.0.0";
        private void OnEnable()
        {
            ColdSnapMain.SlugcatStatsName.RegisterValues();
            //if(ModManager.MSC && global::SlugCatClass = ColdSnapMain.SlugcatStatsName.PLACEHOLDER)
            //{
            On.Player.SwallowObject += SnowSwallowObject;
            On.Player.Regurgitate += Fire;
            //}
        }
        private void OnDisable()
        {
            ColdSnapMain.SlugcatStatsName.UnregisterValues();
        }
        void SnowSwallowObject(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            
        }
        void Fire(On.Player.orig_Regurgitate orig, Player self)
        {

        }
        /*public static void Main()
        {

        }*/
        //This is where our dudes name goes so we don't ruin all the other campaigns
        public class SlugcatStatsName
        {
            public static SlugcatStats.Name PLACEHOLDER;

            public static void RegisterValues()
            {
                PLACEHOLDER = new SlugcatStats.Name("PLACEHOLDER", true);
            }

            public static void UnregisterValues()
            {
                if (PLACEHOLDER != null) { PLACEHOLDER.Unregister(); PLACEHOLDER = null; }
            }
        }
    }
    
}
