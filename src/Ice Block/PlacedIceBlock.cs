using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace SlugTemplate.Ice_Block;

public class PlacedIceBlock : SnowSource
{
    private PlacedObject _po;

    public PlacedIceBlock(PlacedObject owner, Room room) : base(owner.pos)
    {
        //IceBlockData iceBlockData = owner.data as IceBlockData;
        _po = owner;
        //new SnowSource()
    }

    private IceBlockData _Data
    {
        get
        {
            PlacedObject po = _po;
            return po?.data as IceBlockData;
        }
    }

    public override void Update(bool eu)
    {
        pos = _po.pos;
        rad = _Data.radHandle.magnitude;
        base.Update(eu);
        
        
        Rect areaRect = new Rect(_po.pos.x, _po.pos.y, _Data.xHandle.magnitude, _Data.yHandle.magnitude);
        Vector2 center = areaRect.center;
        float angle = Custom.VecToDeg(_Data.yHandle);
        foreach (UpdatableAndDeletable obj in room.updateList)
        {
            if (obj is PhysicalObject p)
            {
                foreach (BodyChunk chunk in p.bodyChunks)
                {
                    Vector2 rotatedPosition = Custom.RotateAroundVector(chunk.pos, _po.pos, -angle);
                    Vector2 centerBias = (center - rotatedPosition).normalized * 0.01f;
                    Vector2 collisionCandidate =
                        Custom.RotateAroundVector(areaRect.GetClosestInteriorPoint(rotatedPosition), _po.pos, angle);
                    centerBias = Custom.RotateAroundOrigo(centerBias, angle);
                    p.PushOutOf(collisionCandidate + centerBias, 0f, -1);
                }
            }
        }
    }
}