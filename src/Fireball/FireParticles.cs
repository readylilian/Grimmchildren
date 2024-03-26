using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SlugTemplate
{
    internal class FireParticles : CosmeticSprite
    {
        public FireParticles()
        {

        }

        public void UpdatePos(Vector2 pos)
        {
            
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[2];
            for (int i = 0; i < 2; i++)
            {
                sLeaser.sprites[i] = new FSprite("Futile_White", true);
                sLeaser.sprites[i].shader = rCam.room.game.rainWorld.Shaders["FireSmoke"];
                sLeaser.sprites[i].color = Color.red;
            }
            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                this.pos.y = room.Height / 2;
                this.pos.y = room.Width / 2;
                sLeaser.sprites[i].x = this.pos.x - camPos.x;
                sLeaser.sprites[i].y = this.pos.y - camPos.y;
                //sLeaser.sprites[i].rotation = Custom.AimFromOneVectorToAnother(new Vector2(), Vector3.Slerp(this.lastRotation, this.rotation, timeStacker));
            }
            //remove sprite
            if (slatedForDeletetion || room != rCam.room)
                sLeaser.CleanSpritesAndRemove();
            //base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }
    }
}
