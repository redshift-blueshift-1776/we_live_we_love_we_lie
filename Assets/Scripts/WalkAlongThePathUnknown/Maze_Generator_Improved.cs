using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Maze_Generator_Improved : MonoBehaviour
{
    [Header("Maze Dimensions")]
    [SerializeField] public int width = 5;
    [SerializeField] public int height = 5;
    [SerializeField] public float borderWallHeight = 2f;

    [Header("Cell Dimensions")]
    [SerializeField] public float cellWidth = 2f;    // X direction
    [SerializeField] public float cellHeight = 2f;   // Z direction
    [SerializeField] public float wallThickness = 0.2f;
    [SerializeField] public float wallHeight = 2f;

    [Header("Maze Settings")]
    [SerializeField] public int startingSquare = 0;
    [SerializeField] public bool includePerimeterWalls = true;
    [SerializeField] public Vector3 rotationEuler = Vector3.zero;  // For orientation (e.g., vertical mazes)

    [Header("References")]
    [SerializeField] public GameObject wallPrefab;
    [SerializeField] public GameObject outerWallPrefab;
    [SerializeField] public GameObject stick;
    [SerializeField] public GameObject gameManager;
    [SerializeField] public GameObject cam1;
    [SerializeField] public GameObject cam2;

    private Dictionary<int, List<(int neighbor, float weight)>> graph;
    private Dictionary<(int, int), GameObject> wallLookup;
    public List<(int from, int to)> mstEdges;
    public List<GameObject> walls = new List<GameObject>();
    public List<GameObject> removedWalls = new List<GameObject>();

    private class Edge : IComparable<Edge>
    {
        public int From;
        public int To;
        public float Weight;
        public Edge(int from, int to, float weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }
        public int CompareTo(Edge other)
        {
            int cmp = Weight.CompareTo(other.Weight);
            if (cmp == 0) cmp = (From, To).CompareTo((other.From, other.To));
            return cmp;
        }
    }

    void Start()
    {
        cam1.SetActive(true);
        cam2.SetActive(false);

        // Apply rotation if specified
        transform.rotation = Quaternion.Euler(rotationEuler);

        PlaceWalls();
        GenerateGraph();
        GenerateMaze();
    }

    public void PlaceWalls()
    {
        wallLookup = new Dictionary<(int, int), GameObject>();
        walls.Clear();

        // --- CONFIGURABLE SHRINK OFFSET ---
        // How much to shrink walls inward to make room for the stick lights
        float inset = stick != null ? wallThickness * 0.5f : 0f;  

        // Calculate maze origin so it’s centered and top-left aligned
        Vector3 origin = transform.position - new Vector3(
            (width * cellWidth) / 2f - cellWidth / 2f,
            0,
            (height * cellHeight) / 2f - cellHeight / 2f
        );

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int cellIndex = y * width + x;
                Vector3 cellCenter = origin + new Vector3(x * cellWidth, 0, y * cellHeight);

                // 🔶 HORIZONTAL WALL (right side of this cell)
                if (x < width - 1)
                {
                    Vector3 pos = cellCenter + new Vector3(cellWidth / 2f, wallHeight / 2f, 0);
                    Quaternion rot = Quaternion.identity;
                    GameObject wall = Instantiate(wallPrefab, pos, rot, transform);
                    Wall w = wall.GetComponent<Wall>();
                    w.gameManager = gameManager;
                    wall.transform.localScale = new Vector3(wallThickness, wallHeight, cellHeight - inset * 2f);
                    wall.name = $"Wall_{cellIndex}_{cellIndex + 1}";
                    walls.Add(wall);
                    wallLookup[(cellIndex, cellIndex + 1)] = wall;
                    wallLookup[(cellIndex + 1, cellIndex)] = wall;
                }

                // 🔶 VERTICAL WALL (bottom side of this cell)
                if (y < height - 1)
                {
                    Vector3 pos = cellCenter + new Vector3(0, wallHeight / 2f, cellHeight / 2f);
                    Quaternion rot = Quaternion.Euler(0, 90, 0);
                    GameObject wall = Instantiate(wallPrefab, pos, rot, transform);
                    Wall w = wall.GetComponent<Wall>();
                    w.gameManager = gameManager;
                    wall.transform.localScale = new Vector3(wallThickness, wallHeight, cellWidth - inset * 2f);
                    wall.name = $"Wall_{cellIndex}_{cellIndex + width}";
                    walls.Add(wall);
                    wallLookup[(cellIndex, cellIndex + width)] = wall;
                    wallLookup[(cellIndex + width, cellIndex)] = wall;
                }

                // OUTER PERIMETER WALLS
                if (includePerimeterWalls)
                {
                    // LEFT boundary
                    if (x == 0)
                    {
                        Vector3 pos = cellCenter - new Vector3(cellWidth / 2f, -wallHeight / 2f, 0);
                        Quaternion rot = Quaternion.identity;
                        GameObject wall = Instantiate(outerWallPrefab, pos, rot, transform);
                        wall.transform.localScale = new Vector3(wallThickness, borderWallHeight, cellHeight - inset * 2f);
                        wall.name = $"Boundary_Left_{cellIndex}";
                        walls.Add(wall);
                    }

                    // TOP boundary
                    if (y == 0)
                    {
                        Vector3 pos = cellCenter - new Vector3(0, -wallHeight / 2f, cellHeight / 2f);
                        Quaternion rot = Quaternion.Euler(0, 90, 0);
                        GameObject wall = Instantiate(outerWallPrefab, pos, rot, transform);
                        wall.transform.localScale = new Vector3(wallThickness, borderWallHeight, cellWidth - inset * 2f);
                        wall.name = $"Boundary_Top_{cellIndex}";
                        walls.Add(wall);
                    }

                    // RIGHT boundary
                    if (x == width - 1)
                    {
                        Vector3 pos = cellCenter + new Vector3(cellWidth / 2f, wallHeight / 2f, 0);
                        Quaternion rot = Quaternion.identity;
                        GameObject wall = Instantiate(outerWallPrefab, pos, rot, transform);
                        wall.transform.localScale = new Vector3(wallThickness, borderWallHeight, cellHeight - inset * 2f);
                        wall.name = $"Boundary_Right_{cellIndex}";
                        walls.Add(wall);
                    }

                    // BOTTOM boundary
                    if (y == height - 1)
                    {
                        Vector3 pos = cellCenter + new Vector3(0, wallHeight / 2f, cellHeight / 2f);
                        Quaternion rot = Quaternion.Euler(0, 90, 0);
                        GameObject wall = Instantiate(outerWallPrefab, pos, rot, transform);
                        wall.transform.localScale = new Vector3(wallThickness, borderWallHeight, cellWidth - inset * 2f);
                        wall.name = $"Boundary_Bottom_{cellIndex}";
                        walls.Add(wall);
                    }
                }
            }
        }

        // Sticks
        if (stick != null)
        {
            for (int y = -1; y < height; y++)
            {
                for (int x = -1; x < width; x++)
                {
                    Vector3 pos = origin + new Vector3((x + 0.5f) * cellWidth, wallHeight / 2f, (y + 0.5f) * cellHeight);
                    GameObject s = Instantiate(stick, pos, Quaternion.identity, transform);
                    s.transform.localScale = new Vector3(wallThickness, wallHeight, wallThickness);
                    s.name = $"Stick_{x}_{y}";
                }
            }
        }
    }

    public void GenerateGraph()
    {
        graph = new Dictionary<int, List<(int, float)>>();

        for (int i = 0; i < width * height; i++)
            graph[i] = new List<(int, float)>();

        for (int i = 0; i < width * height; i++)
        {
            int x = i % width;
            int y = i / width;

            // Right neighbor
            if (x < width - 1)
            {
                float w = UnityEngine.Random.Range(0f, 1f);
                graph[i].Add((i + 1, w));
                graph[i + 1].Add((i, w));
            }
            // Down neighbor
            if (y < height - 1)
            {
                float w = UnityEngine.Random.Range(0f, 1f);
                graph[i].Add((i + width, w));
                graph[i + width].Add((i, w));
            }
        }
    }

    public void GenerateMaze()
    {
        mstEdges = new List<(int, int)>();
        HashSet<int> visited = new HashSet<int>();
        SortedSet<Edge> pq = new SortedSet<Edge>();

        int startVertex = startingSquare;
        visited.Add(startVertex);
        foreach (var edge in graph[startVertex])
            pq.Add(new Edge(startVertex, edge.neighbor, edge.weight));

        while (mstEdges.Count < width * height - 1)
        {
            if (pq.Count == 0) break;
            Edge minEdge = pq.Min;
            pq.Remove(minEdge);

            if (visited.Contains(minEdge.To)) continue;

            visited.Add(minEdge.To);
            mstEdges.Add((minEdge.From, minEdge.To));

            foreach (var edge in graph[minEdge.To])
                if (!visited.Contains(edge.neighbor))
                    pq.Add(new Edge(minEdge.To, edge.neighbor, edge.weight));
        }

        Debug.Log($"Maze generated with {mstEdges.Count} edges.");
    }

    public IEnumerator DeleteWallsOneByOne()
    {
        foreach (var edge in mstEdges)
        {
            if (wallLookup.TryGetValue(edge, out GameObject wall))
            {
                wall.SetActive(false);
                removedWalls.Add(wall);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void DeleteAllWallsAtOnce()
    {
        foreach (var edge in mstEdges)
        {
            if (wallLookup.TryGetValue(edge, out GameObject wall))
            {
                wall.SetActive(false);
                removedWalls.Add(wall);
            }
        }
    }

    public void VisualizeMapGeneration() {
        // Once the MST is determined, delete the walls in the MST one by one
        // at a rate of 10 walls per second.
        Debug.Log("Visualizing");
        cam2.SetActive(true);
        cam1.SetActive(false);
        StartCoroutine(DeleteWallsOneByOne());
    }
}