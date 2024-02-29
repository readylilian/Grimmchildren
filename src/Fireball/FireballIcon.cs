using Fisobs.Core;
using UnityEngine;

namespace SlugTemplate.Fireball;

public class FireballIcon: Icon
{
    // Vanilla only gives you one int field to store all your custom data.
    // Here, that int field is used to store the shield's hue, scaled by 1000.
    // So, 0 is red and 70 is orange.
    public override int Data(AbstractPhysicalObject apo)
    {
        return apo is FireballAbstract ball ? (int)(ball.hue * 1000f) : 0;
    }

    public override Color SpriteColor(int data)
    {
        return RWCustom.Custom.HSL2RGB(1f, 1f, 1f);
    }

    public override string SpriteName(int data)
    {
        Debug.Log("Tried to grab fireball name: " + data);
        // Fisobs autoloads the file in the mod folder named "icon_{Type}.png"
        // To use that, just remove the png suffix: "icon_CentiShield"
        return "";
    }
}