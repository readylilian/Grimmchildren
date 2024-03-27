using Fisobs.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SlugTemplate
{
    internal class FireballAbstract :AbstractPhysicalObject
    {
        public FireballAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, FireballFisob.Fireball, null, pos, ID)
        {
            scaleX = 1;
            scaleY = 1;
            saturation = 0.5f;
            hue = 1f;
        }
        public override void Realize()
        {
            base.Realize();
            if (realizedObject == null)
            {
                PlacedObject fakePObj = new PlacedObject(PlacedObject.Type.LightFixture, null);
                Room.realizedRoom.AddObject(new HolyFire(Room.realizedRoom, fakePObj, fakePObj.data as PlacedObject.LightFixtureData));
                realizedObject = new Fireball(this, Room.realizedRoom.MiddleOfTile(pos.Tile), Vector2.zero, fakePObj);
            }
                
        }

        public float hue;
        public float saturation;
        public float scaleX;
        public float scaleY;

        public override string ToString()
        {
            return this.SaveToString($"{hue};{saturation};{scaleX};{scaleY}");
        }
    }
}
