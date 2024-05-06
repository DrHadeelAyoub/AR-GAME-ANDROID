using UnityEngine;

[System.Serializable]
public struct Vector3d
{
    public double x;
    public double y;
    public double z;

    public Vector3d(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Vector3d operator +(Vector3d a, Vector3d b)
    {
        return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vector3d operator -(Vector3d a, Vector3d b)
    {
        return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vector3d operator *(Vector3d a, double d)
    {
        return new Vector3d(a.x * d, a.y * d, a.z * d);
    }

    public static Vector3d operator /(Vector3d a, double d)
    {
        return new Vector3d(a.x / d, a.y / d, a.z / d);
    }

    public static implicit operator Vector3(Vector3d v)
    {
        return new Vector3((float)v.x, (float)v.y, (float)v.z);
    }

    public static implicit operator Vector3d(Vector3 v)
    {
        return new Vector3d(v.x, v.y, v.z);
    }

    public static Vector3d Lerp(Vector3d a, Vector3d b, double t)
    {
        return a + (b - a) * t;
    }

    public double magnitude
    {
        get { return System.Math.Sqrt(x * x + y * y + z * z); }
    }

    public Vector3d normalized
    {
        get { return this / magnitude; }
    }

    public double sqrMagnitude
    {
        get { return x * x + y * y + z * z; }
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ", " + z + ")";
    }

    public string ToString(string format)
    {
        return "(" +
            x.ToString(format) + ", " +
            y.ToString(format) + ", " +
            z.ToString(format) +
            ")";
    }
}