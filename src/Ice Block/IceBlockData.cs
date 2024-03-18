using UnityEngine;

namespace SlugTemplate.Ice_Block;

using Pom;
using RWCustom;

public class IceBlockData : Pom.ManagedData
{
    // Get the intVector
    public IntVector2 hitbox
    {
        get
        {
            return GetValue<IntVector2>("hitbox");
        }
    }

    // Empty Constructor
    public IceBlockData(PlacedObject po) : base(po, newFields) {}

    private static readonly Pom.ManagedField[] newFields = new Pom.ManagedField[]
    {
        new Pom.IntVector2Field("hitbox", new IntVector2(3, 3), Pom.IntVector2Field.IntVectorReprType.rect),
        new Pom.Vector2Field("ev2", new Vector2(-100f, -40f), Pom.Vector2Field.VectorReprType.none, null),
        new Pom.DrivenVector2Field("ev3", "ev2", new Vector2(-100f, -40f),
            Pom.DrivenVector2Field.DrivenControlType.rectangle, null)
    };

    [BackedByField("ev2")] public Vector2 xHandle;
    [BackedByField("ev3")] public Vector2 yHandle;
}