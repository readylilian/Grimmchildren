using System;

namespace SlugTemplate.Ice_Block;
using UnityEngine;
using RWCustom;

public class IceBlock : UpdatableAndDeletable, IDrawable
{
    private static float Rand => Random.value;
    
    public IntRect hitbox;

    public FloatRect GetFloatHitbox
    {
	    get
	    {
		    return new FloatRect(hitbox.left * 20f - 2f, hitbox.bottom * 20f - 2f, hitbox.right * 20f + 22f,
			    hitbox.top * 20f + 22f);
	    }
    }
    
    /*public IceBlock(AbstractIceBlock abstractIceBlock, Vector2 pos) : base(abstractIceBlock)
    {
        Abstr = abstractIceBlock;
        
        bodyChunks = new[] { new BodyChunk(this, 0, pos, 4 * (Abstr.scaleX + Abstr.scaleY), 0.35f) { goThroughFloors = true } };
        bodyChunks[0].lastPos = bodyChunks[0].pos;
        bodyChunks[0].vel = Vector2.zero;
    }*/

    public IceBlock(IntRect rect, Room room)
    {
        hitbox = rect;

        // Might instantiate other variables here
    }

    // Not currently used but hopefully can be reformatted for melting
    /*private void Shatter()
    {
        var num = Random.Range(6, 10);
        for (int k = 0; k < num; k++) {
            Vector2 pos = firstChunk.pos + Custom.RNV() * 5f * Rand;
            Vector2 vel = Custom.RNV() * 4f * (1 + Rand);
            room.AddObject(new Spark(pos, vel, new Color(1f, 1f, 1f), null, 10, 170));
        }

        float count = 2 + 4 * (Abstr.scaleX + Abstr.scaleY);

        for (int j = 0; j < count; j++) {
            Vector2 extraVel = Custom.RNV() * Random.value * (j == 0 ? 3f : 6f);

            // TODO: remove line when I figure it out ; I shouldn't need a centipede shell
            room.AddObject(new CentipedeShell(firstChunk.pos, Custom.RNV() * Rand * 15 + extraVel, Abstr.hue, Abstr.saturation, 0.25f, 0.25f));
        }

        room.PlaySound(SoundID.Weapon_Skid, firstChunk.pos, 0.75f, 1.25f);

        AllGraspsLetGoOfThisObject(true);
        abstractPhysicalObject.LoseAllStuckObjects();
        Destroy();
    }*/

   /* public override void Update(bool eu)
    {
	    base.Update(eu);
	    //Rect areaRect = new Rect(owner.pos.x, owner.pos.y, owner.data.xHandle)
    }*/

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
	    TriangleMesh.Triangle[] array = new TriangleMesh.Triangle[6];
	    for (int i = 0; i < 6; i++)
	    {
		    array[i] = new TriangleMesh.Triangle(i, i + 1, i + 2);
	    }
	    TriangleMesh triangleMesh = new TriangleMesh("Futile_White", array, false, false);
	    float num = 0.4f;
	    triangleMesh.UVvertices[0] = new Vector2(0f, 0f);
	    triangleMesh.UVvertices[1] = new Vector2(1f, 0f);
	    triangleMesh.UVvertices[2] = new Vector2(0f, num);
	    triangleMesh.UVvertices[3] = new Vector2(1f, num);
	    triangleMesh.UVvertices[4] = new Vector2(0f, 1f - num);
	    triangleMesh.UVvertices[5] = new Vector2(1f, 1f - num);
	    triangleMesh.UVvertices[6] = new Vector2(0f, 1f);
	    triangleMesh.UVvertices[7] = new Vector2(1f, 1f);
	    sLeaser.sprites = new FSprite[1];
	    sLeaser.sprites[0] = triangleMesh;
	    sLeaser.sprites[0].shader = rCam.room.game.rainWorld.Shaders["FlareBomb"];
	    sLeaser.sprites[0].color = new Color(0f, 0f, 1f);
	    AddToContainer(sLeaser, rCam, null);
	    //sLeaser.sprites[0] = new FSprite("Futile_White");
	    //this.AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        sLeaser.sprites[0].color = Color.cyan;
        
        //float num = Mathf.Lerp(this.lastTurnedOn, this.turnedOn, timeStacker);
        sLeaser.sprites[0].alpha = 1;// = num;
		Vector2 a = new Vector2(hitbox.left * 20f, hitbox.bottom * 20f);
		Vector2 a2 = new Vector2((hitbox.right + 1) * 20f, (hitbox.top + 1) * 20f);
		Vector2 a3 = new Vector2(hitbox.left * 20f, (hitbox.top + 1) * 20f);
		Vector2 a4 = new Vector2((hitbox.right + 1) * 20f, hitbox.bottom * 20f);
		float num2 = 120f; //* num;
		float num3 = 30f;
		float num4 = 1f;
		float num5 = 1f;
		
		a.x -= num2 * num4;
		a3.x -= num2 * num5;
		a2.x += num2 * num5;
		a4.x += num2 * num4;
		a.y -= num3;
		a4.y -= num3;
		a3.y += num3;
		a2.y += num3;
		(sLeaser.sprites[0] as TriangleMesh).MoveVertice(0, a3 - camPos);
		(sLeaser.sprites[0] as TriangleMesh).MoveVertice(1, a2 - camPos);
		(sLeaser.sprites[0] as TriangleMesh).MoveVertice(2, a3 + new Vector2(0f, -num3) - camPos);
		(sLeaser.sprites[0] as TriangleMesh).MoveVertice(3, a2 + new Vector2(0f, -num3) - camPos);
		(sLeaser.sprites[0] as TriangleMesh).MoveVertice(4, a + new Vector2(0f, num3) - camPos);
		(sLeaser.sprites[0] as TriangleMesh).MoveVertice(5, a4 + new Vector2(0f, num3) - camPos);
		(sLeaser.sprites[0] as TriangleMesh).MoveVertice(6, a - camPos);
		(sLeaser.sprites[0] as TriangleMesh).MoveVertice(7, a4 - camPos);
		
            
		/*sLeaser.sprites[0].color = new Color(.25f/*Mathf.InverseLerp(0f, 0.5f, this.zapLit) * num*///, .25f/*Mathf.InverseLerp(0f, 0.5f, this.zapLit) * num*/, 1f);
		if (base.slatedForDeletetion || this.room != rCam.room)
		{
			sLeaser.CleanSpritesAndRemove();
		}
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        // I don't really need this method
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[0]);
    }
}