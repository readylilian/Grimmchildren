namespace SlugTemplate.Ice_Block;

public class AbstractIceBlock : AbstractPhysicalObject
{
    public float scaleX;
    public float scaleY;
    public float hue;
    public float saturation;
    
    public AbstractIceBlock(World world, AbstractObjectType type, PhysicalObject realizedObject, WorldCoordinate pos,
        EntityID ID) : base(world, type, null, pos, ID)
    {
        scaleX = 1;
        scaleY = 1;
        saturation = 0.5f;
        hue = 1f;
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