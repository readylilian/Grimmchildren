using SlugTemplate.Ice_Block;

namespace SlugTemplate.Hooks;

public static class RoomHooks
{
    public static void Init()
    {
        // example of a hook
        On.Room.Loaded += Room_Loaded;
    }

    private static void Room_Loaded(On.Room.orig_Loaded orig, Room self)
    {
        orig(self);

        for (int i = 0; i < self.roomSettings.placedObjects.Count; i++)
        {
            if (self.roomSettings.placedObjects[i].type == EnumExt_IceBlock.IceBlock)
            {
                self.AddObject(
                    new IceBlock((self.roomSettings.placedObjects[i].data as PlacedObject.GridRectObjectData).Rect, self));
            }
        }
    }
}