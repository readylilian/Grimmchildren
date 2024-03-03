using System;
using System.IO;
using UnityEngine;
using RWCustom;
using SlugBase.Assets;
using BepInEx;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using IL.Menu;

namespace SlugTemplate.Hooks;

public static class PlayerHooks
{
    public static void Init()
    {
        // example of a hook
        On.Player.Jump += Player_Jump;

        AssetLoader.LoadAssets();
        ApplyPlayerGraphicsHooks();
        //MenuDepthIllustrationHook();
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

/*    public static void MenuDepthIllustrationHook()
    {
        On.Menu.MenuScene.AddIllustration += MenuScene_AddIllustration;
    }
    public static void MenuScene_AddIllustration(On.Menu.MenuScene.orig_AddIllustration orig, Menu.MenuScene self, Menu.MenuIllustration newIllu)
    {
        orig.Invoke(self, newIllu);
        if (newIllu is MenuDepthIllustration && newIllu.fileName == "snow.png")
        {
            newIllu.sprite
        }
    }*/



    //Most of this code was ripped in pieces from a mod called pearlcat, they did it really well and I didn't understand the graphics code without it
    public static void ApplyPlayerGraphicsHooks()
    {

        On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
        On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
        On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;


    }
    public static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig.Invoke(self, sLeaser, rCam);
        /* sLeaser.sprites = new FSprite[2];
     sLeaser.sprites[0] = new FSprite("icon_Fireball", true);
     sLeaser.sprites[1] = new FSprite("icon_Fireball", true);*/

        //no new sprites are being drawn. shouldn't be used
    }
    public static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, UnityEngine.Vector2 camPos)
    {
        orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
        if (self.player.SlugCatClass == new SlugcatStats.Name("Snowcat"))
        {
            UpdateReplacementPlayerSprite(sLeaser, 0, "Body", "body", "");/*sLeaser.sprites[0].alpha = 0f;*/
            UpdateReplacementPlayerSprite(sLeaser, 1, "Hips", "hips", "");
            UpdateReplacementPlayerSprite(sLeaser, 3, "Head", "head", "");
            UpdateReplacementPlayerSprite(sLeaser, 4, "Legs", "legs", "");
            UpdateReplacementPlayerSprite(sLeaser, 5, "PlayerArm", "arm", "");
            UpdateReplacementPlayerSprite(sLeaser, 6, "PlayerArm", "arm", "");

            OrderAndColorSprites(self, sLeaser, rCam);
        }


    }

    private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        orig.Invoke(self, sLeaser, rCam, newContatiner);
        if (newContatiner == null)
        {
            newContatiner = rCam.ReturnFContainer("Midground");
        }
    }

    public static void UpdateReplacementPlayerSprite(RoomCamera.SpriteLeaser sLeaser, int spriteIndex, string toReplace, string atlasName, string nameSuffix = "")
    {
        FAtlas atlas = AssetLoader.GetAtlas(atlasName);
        if (atlas == null)
        {
            return;
        }
        FSprite fsprite = sLeaser.sprites[spriteIndex];
        string text;
        if (fsprite == null)
        {
            text = null;
        }
        else
        {
            FAtlasElement element2 = fsprite.element;
            text = ((element2 != null) ? element2.name : null);
        }
        string name = text;
        if (name == null)
        {
            return;
        }
        if (!name.StartsWith(toReplace))
        {
            return;
        }
        FAtlasElement element;
        if (!atlas._elementsByName.TryGetValue("snowcat" + name + nameSuffix, out element))
        {
            return;
        }
        sLeaser.sprites[spriteIndex].element = element;


    }

    public static void OrderAndColorSprites(PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer = null)
    {
        FSprite bodySprite = sLeaser.sprites[0];
        FSprite armLSprite = sLeaser.sprites[5];
        FSprite armRSprite = sLeaser.sprites[6];
        FSprite hipsSprite = sLeaser.sprites[1];
        FSprite tailSprite = sLeaser.sprites[2];
        FSprite headSprite = sLeaser.sprites[3];
        FSprite handLSprite = sLeaser.sprites[7];
        FSprite handRSprite = sLeaser.sprites[8];
        FSprite legsSprite = sLeaser.sprites[4];
        FSprite markSprite = sLeaser.sprites[11];
        Color bodyColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        Color accentColor = new Color(0.8f, 0.8f, 0.8f, 1.0f); ;
        Color cloakColor = new Color(0.8f, 0.8f, 0.8f, 1.0f); ;

        bodySprite.color = bodyColor;
        hipsSprite.color = bodyColor;
        headSprite.color = bodyColor;
        legsSprite.color = bodyColor;

        armLSprite.color = accentColor;
        armRSprite.color = accentColor;
        handLSprite.color = accentColor;
        handRSprite.color = accentColor;
        markSprite.color = new Color(0.8f, 0.8f, 0.8f, 1.0f); ;
        tailSprite.color = new Color(0.8f, 0.8f, 0.8f, 1.0f); ;


    }


}
