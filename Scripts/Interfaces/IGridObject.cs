using UnityEngine;

public interface IGridObject 
{
    public void SetPosition(Vector3 position);
    public void SetGrid(WorldGrid grid);
    public void Disable();
}
