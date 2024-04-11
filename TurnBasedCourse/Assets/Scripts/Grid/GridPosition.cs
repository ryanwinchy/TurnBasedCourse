using System;



//Main difference is struct is a value type (class is ref type). Struct has the actual values. Vectors, ints , floats all structs. Use for data centric things, this is basically a vector so better use struct.
//IEQUTABLE is a standrd thing we added so can evaluate equality between 2 grid pos.
public struct GridPosition : IEquatable<GridPosition>
{

    public int x;
    public int z;
    public int floor;

    public GridPosition(int x, int z, int floor)
    {
        this.x = x;
        this.z = z;
        this.floor = floor;
    }



    public override string ToString()       //All objects have ToString. This by default returns the struct name, 'GridPosition'. We want it to return the vals, so can override.
    {
        return "x: " + x + "; z: " + z + " floor " + floor;
    }

    public static bool operator ==(GridPosition a, GridPosition b)        //As our own struct, we have to define == and != so we can use comparisons.
    {
        return a.x == b.x && a.z == b.z && a.floor == b.floor;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.z + b.z, a.floor + b.floor);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.z - b.z, a.floor - b.floor);
    }



    //STANDARD GENERATED CODE TO MAKE OPERATORS WORK.
    public override bool Equals(object obj)       //This is standard code when override operators. Can generate using alt enter on func.
    {
        return obj is GridPosition position &&
               x == position.x &&
               z == position.z &&
               floor == position.floor;
    }

    public override int GetHashCode()         //This is standard code when override operators. Can generate using alt enter on func.
    {
        return HashCode.Combine(x, z, floor);
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }
}
