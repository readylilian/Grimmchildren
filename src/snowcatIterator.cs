using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IteratorKit;
using IteratorKit.CMOracle;
using static IteratorKit.CMOracle.CMOracleBehavior;
using static IteratorKit.CMOracle.OracleJSON.OracleEventsJson;
using static IteratorKit.IteratorKit;
using IteratorKit.Debug;
using SlugTemplate.Hooks;

namespace SlugTemplate
{
    


    internal class snowcatIterator
    {
        static bool doFireball = false;
        static int playerConversationCount = 0;

        public static readonly Oracle.OracleID iteratorObject = new Oracle.OracleID("snowcat_iterator", register: true);
        
        public static void OracleBehavior_Update(On.OracleBehavior.orig_Update orig, OracleBehavior self, bool eu)
        {
            orig(self, eu);
            if (self.oracle.ID == iteratorObject)
            {
                
                if (doFireball)
                {
                    doFireball = false;
                    playerConversationCount++;
                    if (playerConversationCount > 1)
                    {
                        CMOracleBehavior cmBehavior = self as CMOracleBehavior;
                        cmBehavior.cmConversation = new CMConversation(cmBehavior, CMConversation.CMDialogType.Generic, "fireballGift");
                    }
                    
                }
                

            }
        }

        public static void OnEventEnd(CMOracleBehavior cMOracleBehavior, string eventName)
        {
            UnityEngine.Debug.Log("event ended " + eventName.Trim());
            switch (eventName.Trim())
            {
                case "fireballGift":

               
                    cMOracleBehavior.roomGravity = 0.5f;
                    FireHooks.Init();
                    break;
                case "playerConversation":

                    UnityEngine.Debug.Log("tried fireball");
                    doFireball = true;

                    break;

            } 
        }
        public static void OnEventStart(CMOracleBehavior cmBehavior, string eventName, OracleEventObjectJson eventData)
        {
           // UnityEngine.Debug.Log("Event started " + eventData.text);
            //UnityEngine.Debug.Log("Event action " + eventData.action);
            if (cmBehavior.oracle.ID == iteratorObject)
            {
               // Logger.LogInfo("event triggered " + eventName);
                if (eventName == "myCustomEvent")
                {
                    // run code your own event code
                }
            }

        }

    }
}
