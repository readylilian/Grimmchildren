using System;
using DevInterface;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace SlugTemplate.Ice_Block;

public class PlacedIceBlock : SnowSource
{
    private PlacedObject _po;

    public PlacedIceBlock(PlacedObject owner, Room room) : base(owner.pos)
    {
        _po = owner;
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
        // Pass over values as updated in dev mode
        pos = _po.pos;
        rad = _Data.radHandle.magnitude;
        shape = _Data.shape;
        intensity = _Data.intensity;
        
        // Make sure that height is always 90 degrees counter clockwise
        var magnitude = _Data.xHandle.magnitude;
        _Data.xHandle = _Data.xHandle.normalized;
        _Data.xHandle = Vector2.Perpendicular(_Data.yHandle);
        //_Data.xHandle = new Vector2(500, 500);
        _Data.fields[2] as DevUINode
        _Data.SetValue("xHandle", new Vector2(500, 500));
        
        base.Update(eu);
        
        
        Rect areaRect = new Rect(_po.pos.x + _Data.HitboxPivot.x, _po.pos.y + _Data.HitboxPivot.y, _Data.xHandle.magnitude, _Data.yHandle.magnitude);
        Vector2 center = areaRect.center;
        float angle = Custom.VecToDeg(_Data.yHandle);
        Console.WriteLine("Center = " + center + ", Angle = " + angle + ", Width = " + _Data.xHandle.magnitude +
                          ", Height = " + _Data.yHandle.magnitude);

        
        foreach (UpdatableAndDeletable obj in room.updateList)
        {
            // TODO: Remove Player line
            if (obj is PhysicalObject p && p is Player)
            {
                foreach (BodyChunk chunk in p.bodyChunks)
                {
                    Vector2 rotatedPosition = Custom.RotateAroundVector(chunk.pos, _Data.HitboxPivot + _po.pos, -angle);
                    Console.WriteLine("RotatedPosition = " + rotatedPosition.ToString());
                    Vector2 centerBias = (center - rotatedPosition).normalized * 0.01f;
                    Vector2 collisionCandidate =
                        Custom.RotateAroundVector(areaRect.GetClosestInteriorPoint(rotatedPosition), _Data.HitboxPivot + _po.pos, angle);
                    Console.WriteLine("CollisionCandidate = " + collisionCandidate.ToString());
                    centerBias = Custom.RotateAroundOrigo(centerBias, angle);
                    Console.WriteLine("centerBias = " + centerBias);
                    
                    p.PushOutOf(collisionCandidate + centerBias, 0f, -1);
                }
            }
        }
    }
}