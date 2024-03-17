namespace SlugTemplate.Ice_Block;

using System;
using System.Runtime.CompilerServices;
using Pom;
using RWCustom;

public class IceBlockData : Pom.ManagedData
{
    // Get the intVector
    public IntVector2 hitbox
    {
        get
        {
            return this.GetValue<IntVector2>("hitbox");
        }
    }
    
    public IceBlockData(PlacedObject po) : base(po, new Pom.ManagedField[]
    {
       new Pom.IntVector2Field("hitbox", new IntVector2(3, 3), Pom.IntVector2Field.IntVectorReprType.rect) 
    }){}
}