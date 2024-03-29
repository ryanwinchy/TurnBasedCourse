using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Main difference is struct is a value type (class is ref type). Struct has the actual values. Vectors, ints , floats all structs. Use for data centric things, this is basically a vector so better use struct.
public struct GridPosition
{

    public int x;
    public int z;

    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public override string ToString()       //All objects have ToString. This by default returns the struct name, 'GridPosition'. We want it to return the vals, so can override.
    {
        return "x: " + x + "; z: " + z;
    }

}
