using System.Linq;

namespace SlugTemplate.Ice_Block;

public class AbstractIceBlock : AbstractPhysicalObject
{
    public float scaleX;
    public float scaleY;
    public float hue;
    public float saturation;
    
    public AbstractIceBlock(World world, AbstractObjectType type, PhysicalObject realizedObject, WorldCoordinate pos,
        EntityID ID) : base(world, EnumExt_IceBlock.IceBlock, null, pos, ID)
    {
        scaleX = 1;
        scaleY = 1;
        saturation = 0.5f;
        hue = 1f;
        this.type = EnumExt_IceBlock.IceBlock;
    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
        {
            realizedObject = new IceBlock(this);
        }
    }
}