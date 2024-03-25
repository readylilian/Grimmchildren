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
	    sLeaser.sprites = new FSprite[3];
	    sLeaser.sprites[0] = new FSprite("JetFishEyeA", true);
	    sLeaser.sprites[1] = new FSprite("tinyStar", true);
	    sLeaser.sprites[2] = new FSprite("Futile_White", true);
	    sLeaser.sprites[2].shader = rCam.game.rainWorld.Shaders["FlatLightBehindTerrain"];
	    this.AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
	    /*Vector2 vector = Vector2.Lerp(firstChunk.lastPos, base.firstChunk.pos, timeStacker);
	    float num = Mathf.Lerp(this.lastGlimmer, this.glimmer, timeStacker);
	    if (this.AbstractPearl.hidden)
	    {
		    vector.y -= 5f;
		    num *= 1.2f;
	    }
	    sLeaser.sprites[1].x = vector.x - camPos.x - 0.5f;
	    sLeaser.sprites[1].y = vector.y - camPos.y + 1.5f;
	    sLeaser.sprites[0].x = vector.x - camPos.x;
	    sLeaser.sprites[0].y = vector.y - camPos.y;
	    sLeaser.sprites[2].x = vector.x - camPos.x;
	    sLeaser.sprites[2].y = vector.y - camPos.y;*/
	    /*sLeaser.sprites[0].color = Color.Lerp(Custom.RGB2RGBA(this.color * Mathf.Lerp(1f, 0.2f, this.darkness), 1f), new Color(1f, 1f, 1f), num);
	    if (this.highlightColor != null)
	    {
		    Color color = Color.Lerp(this.highlightColor.Value, new Color(1f, 1f, 1f), num);
		    sLeaser.sprites[2].color = color;
		    sLeaser.sprites[1].color = Color.Lerp(Custom.RGB2RGBA(this.highlightColor.Value * Mathf.Lerp(1f, 0.5f, this.darkness), 1f), color, num);
	    }
	    else
	    {
		    sLeaser.sprites[1].color = Color.Lerp(Custom.RGB2RGBA(this.color * Mathf.Lerp(1.3f, 0.5f, this.darkness), 1f), new Color(1f, 1f, 1f), Mathf.Lerp(0.5f + 0.5f * num, 0.2f + 0.8f * num, this.darkness));
	    }
	    if (num > 0.9f && base.firstChunk.submersion == 1f)
	    {
		    sLeaser.sprites[0].color = new Color(0f, 0.003921569f, 0f);
		    sLeaser.sprites[1].color = new Color(0f, 0.003921569f, 0f);
	    }
	    sLeaser.sprites[2].alpha = num * 0.5f;
	    sLeaser.sprites[2].scale = 20f * num * ((this.AbstractPearl.dataPearlType != DataPearl.AbstractDataPearl.DataPearlType.Misc && this.AbstractPearl.dataPearlType != DataPearl.AbstractDataPearl.DataPearlType.Misc2) ? 1.35f : 1f) / 16f;
	    sLeaser.sprites[1].isVisible = true;*/
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