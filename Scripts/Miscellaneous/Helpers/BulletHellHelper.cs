using System.Collections.Generic;
using UnityEngine;

public static class BulletHellHelper
{
    public static Vector3 GetRotatedVector(Vector3 vect , float angle , Vector3 axis)
    {
        Vector3 rotated = vect;
        rotated = Quaternion.AngleAxis(angle, axis) * rotated;
        return rotated;
    }


}
