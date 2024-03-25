using UnityEngine;

namespace SlugTemplate.Ice_Block;

using Pom;
using RWCustom;

public class IceBlockData : Pom.ManagedData
{
    // Empty Constructor
    public IceBlockData(PlacedObject po) : base(po, newFields) {}

    private static readonly Pom.ManagedField[] newFields = new Pom.ManagedField[]
    {
        new Pom.Vector2Field("ev2", new Vector2(-100f, -40f), Pom.Vector2Field.VectorReprType.none, null),
        new Pom.DrivenVector2Field("ev3", "ev2", new Vector2(-100f, -40f),
            Pom.DrivenVector2Field.DrivenControlType.rectangle, null),
        new Pom.FloatField("Intensity", 0f, 1f, 1f),
        new Pom.FloatField("Irregularity", 0f, 100f, 100f)
    };

    [BackedByField("ev2")] public Vector2 xHandle;
    [BackedByField("ev3")] public Vector2 yHandle;
}