namespace SlugTemplate.Ice_Block;
using UnityEngine;
using RWCustom;

public class IceBlock : PhysicalObject
{
    private static float Rand => Random.value;

    public AbstractIceBlock Abstr { get; }
    
    public IceBlock(AbstractIceBlock abstractIceBlock) : base(abstractIceBlock)
    {
        Abstr = abstractIceBlock;
    }
    
    // Not currently used but hopefully can be reformatted for melting
    private void Shatter()
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
    }
    
}