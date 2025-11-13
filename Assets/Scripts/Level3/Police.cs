using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum EnemyState
{
    Follow,
    Shoot
};

public class Police : MonoBehaviour
{
    public EnemyState currState;
    public float range;

    Rigidbody myRigidbody;

    [SerializeField] public GameObject player;

    GameObject newBullet;

    public int phase;

    [SerializeField] public GameObject Game;

    public WalkAlongThePathUnknown gameScript;

    public int xBound;
    bool direction;

    [SerializeField] GameObject bullet;
    [SerializeField] float speed;
    [SerializeField] GameObject bulletSpawn;

    [Header("Maze Configuration")]
    [SerializeField] public GameObject mazeGeneratorObject;
    [SerializeField] public Maze_Generator mazeGenerator;  // Reference to the maze
    [SerializeField] public int mazeWidth = 5;  // X-axis size
    [SerializeField] public int mazeHeight = 5; // Y-axis size

    [Header("Grid Settings")]
    [SerializeField] public float cellSize = 50f;  // Distance between grid points in Unity world units
    [SerializeField] public float topLeftX = -100f;  // Distance between grid points in Unity world units
    [SerializeField] public float topLeftZ = 100f;  // Distance between grid points in Unity world units

    private Queue<Vector3> pathQueue = new Queue<Vector3>();

    private Vector2Int startCell;
    private Vector2Int goalCell;
    private Dictionary<int, List<int>> adjacencyList;

    private Vector3 lastPosition;

    [SerializeField] private AudioSource gunSound;

    private Coroutine currentCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currState = EnemyState.Follow;
        currentCoroutine = null;
        myRigidbody = GetComponent<Rigidbody>();
        gameScript = Game.GetComponent<WalkAlongThePathUnknown>();
        if (!mazeGenerator)
        {
            mazeGenerator = mazeGeneratorObject.GetComponent<Maze_Generator>();
        }

        if (mazeGenerator == null)
        {
            Debug.LogError("Maze_Generator not found! Assign it in the inspector.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameScript.gameActive && gameScript.hitRealWall) {
            // Find path to player
            adjacencyList = BuildAdjacencyList(mazeGenerator);
            startCell = WorldToGrid(transform.position);
            if (goalCell != WorldToGrid(player.transform.position)) {
                goalCell = WorldToGrid(player.transform.position);
                pathQueue = new Queue<Vector3>();
            }

            if (startCell != goalCell) {
                currState = EnemyState.Follow;
                List<Vector3> path = FindPathBFS(startCell, goalCell);
                if (path.Count > 0)
                {
                    foreach (var pos in path)
                    {
                        pathQueue.Enqueue(pos);
                        //Debug.Log(pos);
                    }
                }
                if (pathQueue.Count > 0)
                {
                    Vector3 nextPos = pathQueue.Peek();
                    //Debug.Log(nextPos);
                    transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
                    transform.LookAt(nextPos);
                    transform.RotateAround(transform.position, transform.up, 180f);
                    if (Vector3.Distance(transform.position, nextPos) < 15f)
                    {
                        pathQueue.Dequeue();
                    }
                }
            } else {
                currState = EnemyState.Shoot;
            }

            if (currState == EnemyState.Shoot) {
                Vector3 pointThisWay = new Vector3(player.transform.position.x, 0, player.transform.position.z);
                transform.LookAt(pointThisWay);
                transform.RotateAround(transform.position, transform.up, 180f);
                if (currentCoroutine == null) {
                    currentCoroutine = StartCoroutine(ShootBullet());
                }
                
            } else {
                currentCoroutine = null;
            }
        }
    }

    public IEnumerator ShootBullet() {
        while (currState == EnemyState.Shoot && gameScript.gameActive) {
            gunSound.Play();
            var projectile = Instantiate(bullet, transform);
            projectile.SetActive(true);
            var projectileScript = projectile.GetComponent<Bullet>();
            projectileScript.targetPosition = player.transform.position + new Vector3(0f, 1.5f, 0f);
            projectileScript.startPosition = bulletSpawn.transform.position;
            yield return new WaitForSeconds(1f);
        }
    }

    private Dictionary<int, List<int>> BuildAdjacencyList(Maze_Generator maze)
    {
        Dictionary<int, List<int>> adjList = new Dictionary<int, List<int>>();

        if (maze.mstEdges == null)
        {
            Debug.LogError("Maze MST edges not generated yet!");
            return adjList;
        }

        foreach (var edge in maze.mstEdges)
        {
            if (!adjList.ContainsKey(edge.from))
                adjList[edge.from] = new List<int>();

            if (!adjList.ContainsKey(edge.to))
                adjList[edge.to] = new List<int>();

            adjList[edge.from].Add(edge.to);
            adjList[edge.to].Add(edge.from);
        }

        return adjList;
    }

    private List<Vector3> FindPathBFS(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int?> cameFrom = new Dictionary<Vector2Int, Vector2Int?>();
        queue.Enqueue(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == goal)
                break;
            //Debug.Log(current);
            int currentIndex = GridToIndex(current);
            if (!adjacencyList.ContainsKey(currentIndex))
            {
                Debug.LogWarning($"Skipping out-of-bounds index {currentIndex} for cell {current}");
                continue;
            }

            foreach (int neighborIndex in adjacencyList[currentIndex])
            {
                Vector2Int neighbor = IndexToGrid(neighborIndex);

                if (!cameFrom.ContainsKey(neighbor) && IsValidCell(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        List<Vector3> path = new List<Vector3>();
        Vector2Int? step = goal;

        while (step != null && cameFrom.ContainsKey(step.Value))
        {
            path.Add(GridToWorld(step.Value));
            step = cameFrom[step.Value];
        }

        path.Reverse();
        return path;
    }

    private int GridToIndex(Vector2Int cell)
    {
        if (!IsValidCell(cell))
        {
            Debug.LogError($"Invalid cell coordinates: {cell}");
            return -1; // Return invalid index
        }
        return cell.y * mazeWidth + cell.x;
    }

    private Vector2Int IndexToGrid(int index)
    {
        if (index < 0 || index >= mazeWidth * mazeHeight)
        {
            Debug.LogError($"Invalid index: {index}");
            return new Vector2Int(-1, -1); // Return invalid coordinates
        }
        return new Vector2Int(index % mazeWidth, index / mazeWidth);
    }

    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int gridX = Mathf.RoundToInt((worldPos.x - topLeftX) / cellSize);
        int gridY = Mathf.RoundToInt((topLeftZ - worldPos.z) / cellSize);
        return new Vector2Int(gridX, gridY);
    }

    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        float worldX = gridPos.x * cellSize + topLeftX;
        float worldZ = topLeftZ - gridPos.y * cellSize;
        return new Vector3(worldX, 0, worldZ);
    }

    private bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < mazeWidth && cell.y >= 0 && cell.y < mazeHeight;
    }
}
