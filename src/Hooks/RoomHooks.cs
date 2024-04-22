using SlugTemplate.Ice_Block;

namespace SlugTemplate.Hooks;

public static class RoomHooks
{
    public static void Init()
    {
        // example of a hook
        //On.Room.Loaded += Room_Loaded;
        //On.World.GetAbstractRoom_string += World_GetAbstractRoom_string;
    }

    private static AbstractRoom World_GetAbstractRoom_string(On.World.orig_GetAbstractRoom_string orig, World self, string room)
    {
        //AbstractRoom value = orig(self, room);

       // if (value == null)
        {
            for (int i = 0; i < self.abstractRooms.Length; i++)
            {
                if (self.abstractRooms[i].name == room)
                {
                    return self.abstractRooms[i];
                }
            }
            if (room == "OFFSCREEN")
            {
                return self.abstractRooms[self.abstractRooms.Length - 1];
            }
            return null;
        }

        //return value;
    }

    private static void Room_Loaded(On.Room.orig_Loaded orig, Room self)
    {
        orig(self);

        for (int i = 0; i < self.roomSettings.placedObjects.Count; i++)
        {
           // if (self.roomSettings.placedObjects[i].type == EnumExt_IceBlock.IceBlock)
           {
               //self.AddObject(
                 //  new IceBlock((self.roomSettings.placedObjects[i].data as PlacedObject.GridRectObjectData).Rect, self));
           }
        }
        
    }
}
