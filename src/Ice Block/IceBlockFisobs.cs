using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Sandbox;
using JetBrains.Annotations;
using MoreSlugcats;

namespace SlugTemplate.Ice_Block;

public class IceBlockFisobs : Fisob
{
    // This method registers the Fisob in the game
    public IceBlockFisobs() : base(EnumExt_IceBlock.IceBlock)
    {
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        // Centi shield data is just floats separated by ; characters.
        string[] p = entitySaveData.CustomData.Split(';');

        if (p.Length < 4) {
            p = new string[4];
        }

        AbstractIceBlock result = new AbstractIceBlock(world, AbstractPhysicalObject.AbstractObjectType.CollisionField , 
            null, entitySaveData.Pos, entitySaveData.ID)
        {
            hue = float.TryParse(p[0], out var h) ? h : 0,
            saturation = float.TryParse(p[1], out var s) ? s : 1,
            scaleX = float.TryParse(p[2], out var x) ? x : 1,
            scaleY = float.TryParse(p[3], out var y) ? y : 1,
        };
        

        return result;
    }
}