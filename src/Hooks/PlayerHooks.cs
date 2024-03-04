using Fisobs.Core;
using System;
using UnityEngine;
using RWCustom;

namespace SlugTemplate.Hooks
{

    public class PlayerHooks
    {
        public static int fireBalls = 0;
        public static bool metIterator = true;

        //This doesn't do much rn, would love to expand it out later.
        public class firingState
        {
            public IntVector2 direction;

            //Basic velocity, will need to change once it's applied
            public Vector2 Velocity
            {
                get
                {
                    return direction.ToVector2().normalized * 1;
                }
            }
        }

        public static void Init()
        {
            // example of a hook
            On.Player.Jump += Player_Jump;

        }

        private static void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            // Call the original method we hook from first
            orig(self);

            // Adding power to jump
            self.jumpBoost *= 1f + 0.5f;


            // Currently, we don't have an if statement to specify which slugcat this happens to
            // This means it happens to all of them, in every campaign for each jump
        }

    }
}