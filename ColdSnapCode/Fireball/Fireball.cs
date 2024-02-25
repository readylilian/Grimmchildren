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

    public FireballAbstract Abstr { get; }

    public Fireball(FireballAbstract abstr, Vector2 pos, Vector2 vel) : base(abstr, abstr.world)
    {
        Abstr = abstr;

        bodyChunks = new BodyChunk[1];//new[] { new BodyChunk(this, 0, pos + vel, 4 * (Abstr.scaleX + Abstr.scaleY), 0.35f) { goThroughFloors = true } };
        //bodyChunks[0].lastPos = bodyChunks[0].pos;
        //bodyChunks[0].vel = vel;
        bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 5f, 0.07f);
        /*TODO: change all this so it acts like a fireball instead of a shield*/
        bodyChunkConnections = new BodyChunkConnection[0];
        airFriction = 0.999f;
        gravity = 0.9f;
        bounce = 0.6f;
        surfaceFriction = 0.45f;
        collisionLayer = 1;
        waterFriction = 0.92f;
        buoyancy = 0.75f;
    }

    public override void Update(bool eu)
    {
        //base physics
        if (mode != Mode.Thrown) //layer 0 during throw, so HitAnotherThrownWeapon works
            ChangeCollisionLayer(grabbedBy.Count == 0 ? 2 : 1);
        firstChunk.collideWithTerrain = grabbedBy.Count == 0;
        firstChunk.collideWithSlopes = grabbedBy.Count == 0;

        surfaceFriction = 0.55f;
        if (firstChunk.onSlope != 0)
            surfaceFriction = 0f; //makes ball roll off slopes faster

        //Allows ball to roll off slopes
        base.doNotTumbleAtLowSpeed = mode == Mode.Thrown;

        base.Update(eu);

        var chunk = firstChunk;
        if (base.firstChunk.ContactPoint.y != 0)
            this.rotationSpeed = (this.rotationSpeed * 2f + base.firstChunk.vel.x * 5f) / 3f;
    }

    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunk, direction, speed, firstContact);

        if (speed > 10)
        {
            room.PlaySound(SoundID.Spear_Fragment_Bounce, firstChunk.pos, 0.35f, 2f);
        }
    }

    public override void ChangeMode(Mode newMode)
    { }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("icon_Fireball", true);
        sLeaser.sprites[1] = new FSprite("icon_Fireball", true);
        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
        float num = Mathf.InverseLerp(305f, 380f, timeStacker);
        pos.y -= 20f * Mathf.Pow(num, 3f);
        lastDarkness = darkness;
        darkness = rCam.room.Darkness(pos);
        darkness *= 1f - 0.5f * rCam.room.LightSourceExposure(pos);

        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = pos.x - camPos.x;
            sLeaser.sprites[i].y = pos.y - camPos.y;
            sLeaser.sprites[i].rotation = Custom.AimFromOneVectorToAnother(new Vector2(), Vector3.Slerp(this.lastRotation, this.rotation, timeStacker));
        }

        //blink when able to pick up
        if (blink > 0)
        {
            sLeaser.sprites[0].color = blinkColor;
        }
        else
        {
            sLeaser.sprites[0].color = Color.Lerp(Color.white, blackColor, darkness);
        }

        //remove sprite
        if (slatedForDeletetion || room != rCam.room)
            sLeaser.CleanSpritesAndRemove();
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        blackColor = palette.blackColor;
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
