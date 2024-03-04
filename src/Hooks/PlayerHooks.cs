using Fisobs.Core;
using System;
using Fisobs.Core;
using RWCustom;
using UnityEngine;

namespace SlugTemplate.Hooks;

public static class PlayerHooks
{
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
