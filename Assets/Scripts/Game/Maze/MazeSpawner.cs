using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    [SerializeField] [Range(0, 31)] private int _blockingLayerIndex = 6;
    
    public Cell CellPrefab;
    public Finish FinishPrefab;
    public Vector3 CellSize = new Vector3(1,1,0);

    public Maze maze;

    private void Start()
    {
        MazeGenerator generator = new MazeGenerator();
        maze = generator.GenerateMaze();

        var mazeLenghtX = maze.cells.GetLength(0);
        var mazeLenghtY = maze.cells.GetLength(0);

        for (int x = 0; x < maze.cells.GetLength(0); x++)
        {
            for (int y = 0; y < maze.cells.GetLength(1); y++)
            {
                Cell c = Instantiate(CellPrefab, new Vector3((x * CellSize.x), (y * CellSize.y), y * CellSize.z), Quaternion.identity);
                c.transform.parent = transform;

                c.WallLeft.layer = _blockingLayerIndex;
                c.WallBottom.layer = _blockingLayerIndex;
                c.Floor.layer = _blockingLayerIndex;

                c.WallLeft.SetActive(maze.cells[x, y].WallLeft);
                c.WallBottom.SetActive(maze.cells[x, y].WallBottom);
            }
        }

        Finish f = Instantiate(FinishPrefab, new Vector3((maze.finishPosition.x * CellSize.x), (maze.finishPosition.y * CellSize.y), maze.finishPosition.y * CellSize.z), Quaternion.identity);

        if (maze.finishPosition.x == 0)
        {
            f.WallRight.SetActive(false);
            f.gameObject.transform.position = new Vector3(f.gameObject.transform.position.x - 1, f.gameObject.transform.position.y, f.gameObject.transform.position.z);
        }
        else if (maze.finishPosition.y == 0)
        {
            f.WallUpper.SetActive(false);
            f.gameObject.transform.position = new Vector3(f.gameObject.transform.position.x, f.gameObject.transform.position.y - 1, f.gameObject.transform.position.z);
        }
        else if (maze.finishPosition.x == mazeLenghtX)
        {
            f.WallLeft.SetActive(false);
            f.gameObject.transform.position = new Vector3(f.gameObject.transform.position.x + 1, f.gameObject.transform.position.y, f.gameObject.transform.position.z);
        }
        else
        {
            f.WallBottom.SetActive(false);
            f.gameObject.transform.position = new Vector3(f.gameObject.transform.position.x, f.gameObject.transform.position.y + 1, f.gameObject.transform.position.z);
        }


        Debug.Log(maze.finishPosition.x);
        Debug.Log(maze.finishPosition.y);
    }
}