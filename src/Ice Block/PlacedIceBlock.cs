using System;
using System.IO;
using System.Runtime.CompilerServices;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace SlugTemplate.Ice_Block;

public class PlacedIceBlock : SnowSource, IDrawable
{
    private PlacedObject _po;
    private Vector2 center;

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

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("Items");

        foreach (FSprite fsprite in sLeaser.sprites)
        {
            fsprite.RemoveFromContainer();
            newContatiner.AddChild(fsprite);
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        //fuck the police coming straight from the underground
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {      
            /*foreach (FireParticles fire in fires)
            {
                fire.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }*/

                for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
            float rotation = Custom.VecToDeg(_Data.yHandle);
            sLeaser.sprites[i].scaleX = _Data.xHandle.magnitude / 80;
            sLeaser.sprites[i].scaleY = _Data.yHandle.magnitude / 50;   
            sLeaser.sprites[i].x = _po.pos.x - camPos.x + _Data.xHandle.x/2 + _Data.yHandle.x / 2;
                sLeaser.sprites[i].y = _po.pos.y - camPos.y + _Data.xHandle.y / 2 + _Data.yHandle.y/2;
                //UnityEngine.Debug.Log("Sin: " + Math.Sin(rotation) + " _Data.xHandle.x:" + _Data.xHandle.x + " Rotation:" + sLeaser.sprites[i].rotation);
                sLeaser.sprites[i].rotation = rotation;
        }
        //sLeaser.sprites[i].height / 2;
        //remove sprite
        if (slatedForDeletetion || room != rCam.room)
            {
                
                sLeaser.CleanSpritesAndRemove();
            }
        
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        Debug.Log(Directory.GetCurrentDirectory());
        FAtlas atlas = AssetLoader.GetAtlas("ice");
        if (atlas == null)
        {
            //Debug.Log("Failed to draw sprite");
            return;
        }
        //Debug.Log("Atlast exists");

        //Debug.Log("Tried to draw sprite");
        sLeaser.sprites = new FSprite[1];

        //Debug.Log(atlas.name);
        sLeaser.sprites[0] = new FSprite("snowcat_ice", true);
        sLeaser.sprites[0].color = new Color(1.0f, 1.00f, 1.00f, 1);


        AddToContainer(sLeaser, rCam, null);
    }

    public override void Update(bool eu)
    {
        // Pass over values as updated in dev mode
        pos = _Data.snowPivot + _po.pos;
        rad = _Data.radHandle.magnitude;
        shape = _Data.shape;
        intensity = _Data.intensity / 100f;
        
        base.Update(eu);
        
        
        Rect areaRect = new Rect(_po.pos.x, _po.pos.y, _Data.xHandle.magnitude, _Data.yHandle.magnitude);
        center = areaRect.center;
        float angle = Custom.VecToDeg(_Data.yHandle);
        Console.WriteLine("Center = " + center + ", Angle = " + angle + ", Width = " + _Data.xHandle.magnitude +
                          ", Height = " + _Data.yHandle.magnitude);

        
        foreach (UpdatableAndDeletable obj in room.updateList)
        {
            // TODO: Remove Player line
            if ((obj is PhysicalObject p))
            {
                foreach (BodyChunk chunk in p.bodyChunks)
                {
                    Vector2 rotatedPosition = Custom.RotateAroundVector(chunk.pos,_po.pos, -angle);
                    Console.WriteLine("RotatedPosition = " + rotatedPosition.ToString());
                    Vector2 centerBias = (center - rotatedPosition).normalized * 0.01f;
                    Vector2 collisionCandidate =
                        Custom.RotateAroundVector(areaRect.GetClosestInteriorPoint(rotatedPosition), _po.pos, angle);
                    Console.WriteLine("CollisionCandidate = " + collisionCandidate.ToString());
                    centerBias = Custom.RotateAroundOrigo(centerBias, angle);
                    Console.WriteLine("centerBias = " + centerBias);
                    /*if(obj is Player cat && cat is Player)
                    {
                        cat.customPlayerGravity = 5.0f;
                        p.PushOutOf(collisionCandidate + centerBias, 1f, -1);
                        
                    }
                    else
                    {
                        p.gravity = 0.5f;
                        p.PushOutOf(collisionCandidate + centerBias, 1f, -1);
                        p.gravity = 1.0f;
                    }*/
                    p.PushOutOf(collisionCandidate + centerBias, 1f, -1);
                    //p.SetLocalAirFriction(5.0f);



                    //kp.gravity = 2.0f;
                    //p.SetLocalAirFriction(1.0f);
                }
            }

            
        }
    }
}