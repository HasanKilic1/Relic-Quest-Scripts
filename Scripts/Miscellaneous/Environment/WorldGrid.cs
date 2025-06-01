using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid 
{
    private Vector3 position;
    public int coordinateX;
    public int coordinateZ;
    public bool isEmpty;
    public WorldGrid(Vector3 position, int coordinateX, int coordinateZ)
    {
        this.position = position;
        this.coordinateX = coordinateX;
        this.coordinateZ = coordinateZ;        
    }
    public Vector3 Coordinate => new Vector3(coordinateX, 0, coordinateZ);
    public Vector3 Position => position;
    public void Fill() => isEmpty = false;
    public void Clear() => isEmpty = true;

}
