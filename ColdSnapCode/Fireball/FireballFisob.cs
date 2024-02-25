using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace Fireball;

sealed class FireballFisob : Fisob
{
    public static readonly AbstractPhysicalObject.AbstractObjectType Fireball = new("Fireball", true);
    public FireballFisob() : base(Fireball)
    {
        this.Icon = new FireballIcon();

        SandboxPerformanceCost = new(linear: 0.35f, exponential: 0f);
    }
    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // Centi shield data is just floats separated by ; characters.
        string[] p = saveData.CustomData.Split(';');

        if (p.Length < 5)
        {
            p = new string[5];
        }

        var result = new FireballAbstract(world, saveData.Pos, saveData.ID)
        {
            hue = float.TryParse(p[0], out var h) ? h : 0,
            saturation = float.TryParse(p[1], out var s) ? s : 1,
            scaleX = float.TryParse(p[2], out var x) ? x : 1,
            scaleY = float.TryParse(p[3], out var y) ? y : 1
        };

        // If this is coming from a sandbox unlock, the hue and size should depend on the data value (see CentiShieldIcon below).
        if (unlock is SandboxUnlock u)
        {
            result.hue = u.Data / 1000f;

            if (u.Data == 0)
            {
                result.scaleX += 0.2f;
                result.scaleY += 0.2f;
            }
        }

        return result;
    }

    private static readonly FireballProperties properties = new();

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        // If you need to use the forObject parameter, pass it to your ItemProperties class's constructor.
        // The Mosquitoes example demonstrates this.
        return properties;
    }
}

