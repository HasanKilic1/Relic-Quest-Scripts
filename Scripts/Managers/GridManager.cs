using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{   
    public static GridManager Instance { get; private set; }
    [SerializeField] private GridSO testGridSO;
    GridSO currentGridSO;
    [SerializeField] List<GridSO> gridSOList;
    [field: SerializeField] public float gridLength { get; private set; }
    [SerializeField] Vector3 start;
    private Vector3 center;
    private List<WorldGrid> gridList;
    private List<IGridObject> gridObjects;

    private void OnEnable()
    {
        GameStateManager.OnLevelFinished += ClearLegacyObjects;
        GameStateManager.OnLevelStart += SpawnNewObjects;
    }
    private void OnDisable()
    {
        GameStateManager.OnLevelFinished -= ClearLegacyObjects;
        GameStateManager.OnLevelStart -= SpawnNewObjects;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else Instance = this;

        gridList = new List<WorldGrid>();
        gridObjects = new List<IGridObject>();     
    }

    private void Start()
    {        
        AssignNewGridSO();
        OperateGridInitialization();       
    }

   
    private void AssignNewGridSO()
    {
        currentGridSO = gridSOList.Find(so => so.Level == GameStateManager.Instance.CurrentLevel);
        OperateGridInitialization();
    }

    private void OperateGridInitialization()
    {
        if(currentGridSO == null) { return; }
        for (int x = 0; x < currentGridSO.XMax; x++)
        {
            for (int z = 0; z < currentGridSO.ZMax; z++)
            {
                Vector3 gridPos = start + new Vector3(x * gridLength, start.y, z * gridLength);
                WorldGrid grid = new(gridPos - (Vector3.right * gridLength), x, z)
                                {
                                    isEmpty = true
                                };

                gridList.Add(grid);
            }            
        }        
    }
    
    public void HandleGridTransition()
    {
        AssignNewGridSO();
    }

    public void ClearLegacyObjects(int i)
    {
        if(gridObjects.Count == 0) return;
        var gridObjectsCopy = new List<IGridObject>(gridObjects);

        foreach (var gridObject in gridObjectsCopy)
        {
            gridObject?.Disable();
        }
        gridObjects.Clear();
    }

    public void RemoveObjectFromList(IGridObject gridObject)
    {
        if (gridObjects.Contains(gridObject))
        {
            gridObjects.Remove(gridObject);
        }
    }

    public void SpawnNewObjects(int level)
    {
        AssignNewGridSO();

        if(!currentGridSO || currentGridSO.ObjectAndCoordinates == null) return;

        foreach (var objAndCoord in currentGridSO.ObjectAndCoordinates)
        {           
            WorldGrid grid = GetGridOnCoordinate(objAndCoord.Coordinate.x , objAndCoord.Coordinate.y);
            Vector3 objectSpawnPosition = grid.Position;
            GameObject gridObject = Instantiate(objAndCoord.GridObject.Prefab , objectSpawnPosition , objAndCoord.GridObject.Prefab.transform.rotation);
            gridObject.transform.SetParent(transform);
            IGridObject IgridObject = gridObject.GetComponent<IGridObject>(); 

            if(gridObject != null)
            {
                IgridObject.SetPosition(objectSpawnPosition);
                IgridObject.SetGrid(grid);
                gridObjects.Add(IgridObject);
                grid.Fill();
            }
        }
    }
    private WorldGrid GetGridOnCoordinate(int x, int z)
    {
        foreach (var grid in gridList)
        {
            if (grid.Coordinate.x == x && grid.Coordinate.z == z) { return grid; }
        }
        return null;
    }
    public List<Vector3> GetEmptyPositions()
    {
        List<WorldGrid> validGrids = gridList.Where(grid => grid.isEmpty).ToList();
        List<Vector3> validPositions = new List<Vector3>();

        foreach (WorldGrid grid in gridList) { validPositions.Add(grid.Position); }

        return validPositions;
    }

    public WorldGrid GetGridOfCoordinate(int x , int z)
    {
        foreach(var grid in gridList)
        {
            if(grid.Coordinate.x == x && grid.Coordinate.z == z)
            {
                return grid;
            }
        }
        return null;
    }

    public WorldGrid GetClosestGridOnLocation(Vector3 location)
    {
        float distance = 10000f;
        WorldGrid closest = null;
        foreach (var grid in gridList)
        {
            if(Vector3.Distance(location, grid.Position) < distance)
            {
                closest = grid;
                distance = Vector3.Distance(location, grid.Position);
            }
        }
        return closest;
    }

    public bool HasValidGridOnCoordinate(int x , int z)
    {
        if(x < currentGridSO.XMax && z < currentGridSO.ZMax && x > 0 && z > 0)
        {
            if(GetGridOnCoordinate(x, z).isEmpty)
            {
                return true;
            } 
        }
        return false;
    }

    public Vector3 GetRandomValidGridPosition(Transform requester,int xSensivity = 1 , int zSensivity = 1)
    {
        List<WorldGrid> filteredGrids = gridList.Where(grid => grid.coordinateZ < currentGridSO.ZMax - xSensivity && 
                                                   grid.Coordinate.x < currentGridSO.ZMax - zSensivity).ToList();

        int random = Random.Range(0 , filteredGrids.Count);

        if (filteredGrids == null)
        {
            return requester.transform.position;
        } 
        else return filteredGrids[random].Position;
    }

    public Vector3 GetCenterPosition()
    {
        return GetGridOfCoordinate(currentGridSO.XMax / 2, currentGridSO.ZMax / 2).Position;
    }

    private void OnDrawGizmosSelected()
    {
        var firstPos = start - (Vector3.forward + Vector3.right) * gridLength / 2f;
        if(testGridSO != null)
        {
            for (int x = 0; x < testGridSO.XMax; x++)
            {
                for (int z = 0; z < testGridSO.ZMax; z++)
                {
                    Vector3 pos = firstPos + new Vector3(x * gridLength, start.y, z * gridLength);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(pos, 1f);
                }
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, 1f);
        }       
    }  
}

