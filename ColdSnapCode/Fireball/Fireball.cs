using RWCustom;
using Fisobs;
using UnityEngine;
using System.IO;
using System;

namespace Fireball;

sealed class Fireball: Weapon
{
    private static float Rand => UnityEngine.Random.value;

    public float lastDarkness = -1f;
    public float darkness;

    private Color blackColor;
    private Color earthColor;

    new public float rotation;
    new public float lastRotation;
    public float rotVel;
    private readonly float rotationOffset;
    public FireballAbstract Abstr { get; }

    public Fireball(FireballAbstract abstr, Vector2 pos, Vector2 vel) : base(abstr, abstr.world)
    {
        Abstr = abstr;

        bodyChunks = new[] { new BodyChunk(this, 0, pos + vel, 4 * (Abstr.scaleX + Abstr.scaleY), 0.35f) { goThroughFloors = true } };
        bodyChunks[0].lastPos = bodyChunks[0].pos;
        bodyChunks[0].vel = vel;
        /*TODO: change all this so it acts like a fireball instead of a shield*/
        bodyChunkConnections = new BodyChunkConnection[0];
        airFriction = 0.999f;
        gravity = 0.9f;
        bounce = 0.6f;
        surfaceFriction = 0.45f;
        collisionLayer = 1;
        waterFriction = 0.92f;
        buoyancy = 0.75f;

        rotation = Rand * 360f;
        lastRotation = rotation;

        rotationOffset = Rand * 30 - 15;

        ResetVel(vel.magnitude);
    }

    public override void Update(bool eu)
    {
        //Debug.Log("Tried to update fireball sprite");
        ChangeCollisionLayer(grabbedBy.Count == 0 ? 2 : 1);
        firstChunk.collideWithTerrain = grabbedBy.Count == 0;
        firstChunk.collideWithSlopes = grabbedBy.Count == 0;

        base.Update(eu);

        var chunk = firstChunk;

        lastRotation = rotation;
        rotation += rotVel * Vector2.Distance(chunk.lastPos, chunk.pos);

        rotation %= 360;

        if (grabbedBy.Count == 0)
        {
            if (firstChunk.lastPos == firstChunk.pos)
            {
                rotVel *= 0.9f;
            }
            else if (Mathf.Abs(rotVel) <= 0.01f)
            {
                ResetVel((firstChunk.lastPos - firstChunk.pos).magnitude);
            }
        }
        else
        {
            var grabberChunk = grabbedBy[0].grabber.mainBodyChunk;
            rotVel *= 0.9f;
            rotation = Mathf.Lerp(rotation, grabberChunk.Rotation.GetAngle() + rotationOffset, 0.25f);
        }

        if (!Custom.DistLess(chunk.lastPos, chunk.pos, 3f) && room.GetTile(chunk.pos).Solid && !room.GetTile(chunk.lastPos).Solid)
        {
            /*TODO: Figure out what this does and if we need it*/
            var firstSolid = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(room, room.GetTilePosition(chunk.lastPos), room.GetTilePosition(chunk.pos));
            if (firstSolid != null)
            {
                FloatRect floatRect = Custom.RectCollision(chunk.pos, chunk.lastPos, room.TileRect(firstSolid.Value).Grow(2f));
                chunk.pos = floatRect.GetCorner(FloatRect.CornerLabel.D);
                bool flag = false;
                if (floatRect.GetCorner(FloatRect.CornerLabel.B).x < 0f)
                {
                    chunk.vel.x = Mathf.Abs(chunk.vel.x) * 0.15f;
                    flag = true;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).x > 0f)
                {
                    chunk.vel.x = -Mathf.Abs(chunk.vel.x) * 0.15f;
                    flag = true;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).y < 0f)
                {
                    chunk.vel.y = Mathf.Abs(chunk.vel.y) * 0.15f;
                    flag = true;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).y > 0f)
                {
                    chunk.vel.y = -Mathf.Abs(chunk.vel.y) * 0.15f;
                    flag = true;
                }
                if (flag)
                {
                    rotVel *= 0.8f;
                }
            }
        }
    }

    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunk, direction, speed, firstContact);

        if (speed > 10)
        {
            room.PlaySound(SoundID.Spear_Fragment_Bounce, firstChunk.pos, 0.35f, 2f);
            ResetVel(speed);
        }
    }

    public override void ChangeMode(Mode newMode)
    { }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        Debug.Log("Tried to initiate fireball sprite");
        sLeaser.sprites[0] = new FSprite("icon_Fireball", true);
        //sLeaser.sprites[0] = new FSprite("CentipedeBackShell", true); 
        //sLeaser.sprites[1] = new FSprite("CentipedeBackShell", true);
        AddToContainer(sLeaser, rCam, null);
        Debug.Log("Finished initiate fireball sprite");
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        //Debug.Log("Tried to draw fire sprite");
        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
        float num = Mathf.InverseLerp(305f, 380f, timeStacker);
        pos.y -= 20f * Mathf.Pow(num, 3f);
        float num2 = Mathf.Pow(1f - num, 0.25f);
        lastDarkness = darkness;
        darkness = rCam.room.Darkness(pos);
        darkness *= 1f - 0.5f * rCam.room.LightSourceExposure(pos);

        for (int i = 0; i < 2; i++)
        {
            sLeaser.sprites[i].x = pos.x - camPos.x;
            sLeaser.sprites[i].y = pos.y - camPos.y;
            sLeaser.sprites[i].rotation = Mathf.Lerp(lastRotation, rotation, timeStacker);
            sLeaser.sprites[i].scaleY = num2 * Abstr.scaleY;
            sLeaser.sprites[i].scaleX = num2 * Abstr.scaleX;
        }

        sLeaser.sprites[0].color = blackColor;
        sLeaser.sprites[0].scaleY *= 1.175f;
        sLeaser.sprites[0].scaleX *= 1.175f;

        sLeaser.sprites[1].color = Color.Lerp(Custom.HSL2RGB(Abstr.hue, Abstr.saturation, 0.55f), blackColor, darkness);

        if (blink > 0 && Rand < 0.5f)
        {
            sLeaser.sprites[0].color = blinkColor;
        }
        else if (num > 0.3f)
        {
            for (int j = 0; j < 2; j++)
            {
                sLeaser.sprites[j].color = Color.Lerp(sLeaser.sprites[j].color, earthColor, Mathf.Pow(Mathf.InverseLerp(0.3f, 1f, num), 1.6f));
            }
        }

        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        blackColor = palette.blackColor;
        earthColor = Color.Lerp(palette.fogColor, palette.blackColor, 0.5f);
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContainer)
    {
        newContainer ??= rCam.ReturnFContainer("Items");

        foreach (FSprite fsprite in sLeaser.sprites)
        {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
    }
    private void ResetVel(float speed)
    {
        rotVel = Mathf.Lerp(-1f, 1f, Rand) * Custom.LerpMap(speed, 0f, 18f, 5f, 26f);
    }

    public static void LoadSprites()
    {
        try
        {
            Futile.atlasManager.LoadAtlas("assets" + Path.DirectorySeparatorChar + "fireball");
        }
        catch (Exception ex)
        {
           Debug.LogError("LoadSprites exception: " + ex.ToString());
        }
        Debug.Log("LoadSprites called");
    }
}
