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
        new Pom.Vector2Field("HitboxPivot", new Vector2(-150f, 0f), Pom.Vector2Field.VectorReprType.none),
        new Pom.DrivenVector2Field("ev2", "HitboxPivot", new Vector2(-100, -40), 
            Pom.DrivenVector2Field.DrivenControlType.relativeLine, "Height"),
        //new Pom.Vector2Field("ev2", new Vector2(-100f, -40f), Pom.Vector2Field.VectorReprType.none, null),
        new Pom.DrivenVector2Field("ev3", "ev2", new Vector2(-100f, 0f),
            Pom.DrivenVector2Field.DrivenControlType.relativeLine, "Rotation and Width"),
        new Pom.FloatField("Intensity", 0f, 100f, 100f),
        new Pom.ExtEnumField<PlacedObject.SnowSourceData.Shape>("Shape", PlacedObject.SnowSourceData.Shape.Radial),
        new Pom.Vector2Field("Radius", 0, 100, Pom.Vector2Field.VectorReprType.circle)
    };
    
    [BackedByField("ev2")] public Vector2 xHandle;
    [BackedByField("ev3")] public Vector2 yHandle;
    [BackedByField("HitboxPivot")] public Vector2 HitboxPivot;
    [BackedByField("Radius")] public Vector2 radHandle;
    [BackedByField("Intensity")] public float intensity;
    [BackedByField("Shape")] public PlacedObject.SnowSourceData.Shape shape;
}