using DevInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreSlugcats;
using UnityEngine;

namespace SlugTemplate.Hooks
{
    internal class BossHooks
    {
        enum Behavior
        {
            Frozen,
            Speaking,
            Normal
        }

        private static Behavior bossBehavior;
        public static void Init()
        {
            On.Lizard.Update += BossUpdate;
            bossBehavior = Behavior.Frozen;
        }

        private static void BossUpdate(On.Lizard.orig_Update orig, Lizard self, bool eu)
        {
            if (self.ToString().Contains("Pink Lizard") || self.ToString().Contains("Boss"))
            {
                Debug.Log("Found Boss");
                switch (bossBehavior)
                {
                    case Behavior.Frozen:

                        break;
                    case Behavior.Speaking:
                        break;
                    case Behavior.Normal:
                        orig(self, eu);
                        break;
                }
            }
            else
            {
                orig(self, eu);
            }
        }

        private static void BossMoveAnimation()
        {

        }
    }
}
