using UnityEngine;

namespace SlugTemplate.Ice_Block;

using Pom;
using RWCustom;

public class IceBlockPhysData : Pom.ManagedData
{
    // Empty Constructor
    public IceBlockPhysData(PlacedObject po) : base(po, newFields) {}

    private static readonly Pom.ManagedField[] newFields = new Pom.ManagedField[]
    {
        new Pom.Vector2Field("ev2", new Vector2(-100f, -40f), Pom.Vector2Field.VectorReprType.none, null),
        new Pom.DrivenVector2Field("ev3", "ev2", new Vector2(-100f, -40f),
            Pom.DrivenVector2Field.DrivenControlType.rectangle, null),
        new Pom.Vector2Field("SnowPivot", new Vector2(-100f, 0f), Pom.Vector2Field.VectorReprType.none),
        new Pom.FloatField("Intensity", 0f, 100f, 100f),
        new Pom.ExtEnumField<PlacedObject.SnowSourceData.Shape>("Shape", PlacedObject.SnowSourceData.Shape.Radial),
        new Pom.DrivenVector2Field("Radius", "SnowPivot", new Vector2(0, 100), Pom.DrivenVector2Field.DrivenControlType.perpendicularOval)
    };
    
    [BackedByField("ev2")] public Vector2 xHandle;
    [BackedByField("ev3")] public Vector2 yHandle;
    [BackedByField("SnowPivot")] public Vector2 snowPivot;
    [BackedByField("Radius")] public Vector2 radHandle;
    [BackedByField("Intensity")] public float intensity;
    [BackedByField("Shape")] public PlacedObject.SnowSourceData.Shape shape;
}