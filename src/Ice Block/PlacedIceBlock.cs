using RWCustom;

namespace SlugTemplate.Ice_Block;

public class PlacedIceBlock : IceBlock
{
    private PlacedObject _po;

    public PlacedIceBlock(PlacedObject owner, Room room) : base(
        IntRect.MakeFromIntVector2(((IceBlockData) owner.data).hitbox), room)
    {
        //IceBlockData iceBlockData = owner.data as IceBlockData;
        this._po = owner;
    }

    private IceBlockData _Data
    {
        get
        {
            PlacedObject po = _po;
            return po?.data as IceBlockData;
        }
    }

    // Useless currently. I just want to remember I can do this
    public override void Update(bool eu)
    {
        base.Update(eu);
    }
}