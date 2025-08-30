using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Maze_Generator : MonoBehaviour
{
    [SerializeField] public int size = 5;
    [SerializeField] public int startingSquare = 0;

    private Dictionary<int, List<(int neighbor, float weight)>> graph;
    public List<(int from, int to)> mstEdges;

    [SerializeField] public GameObject cam1;
    [SerializeField] public GameObject cam2;

    private List<GameObject> walls;

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
            if (cmp == 0)
            {
                // Ensure uniqueness in SortedSet by using a tie-breaker
                cmp = (From, To).CompareTo((other.From, other.To));
            }
            return cmp;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        cam1.SetActive(true);
        cam2.SetActive(false);
        // Creates a maze out of a square grid measuring size by size.
        // The grid spaces are considered with 0 as the top left, then 1 being the space to the right,
        // Then 2 being to the right of that, etc.
        // All the walls that could be in the maze are stored as children.
        // The walls are labelled Wall_a_b, where a and b are integers.
        // Add all the walls into a list.
        walls = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Wall_"))
            {
                walls.Add(child.gameObject);
            }
        }

        // Create a weighted undirected graph representing the grid. Add each grid space as a vertex.
        GenerateGraph();
        // Generate the maze
        GenerateMaze();
        
    }

    public void GenerateGraph() {
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Wall_"))
            {
                child.gameObject.SetActive(true);
                walls.Add(child.gameObject);
            }
        }
        // Create a weighted undirected graph representing the grid. Add each grid space as a vertex.
        graph = new Dictionary<int, List<(int, float)>>();

        for (int i = 0; i < size * size; i++)
        {
            graph[i] = new List<(int, float)>();
        }
        // Add the edges in the graph. They will be the ones across (one apart, excluding the left and
        // right edges) and the ones down (the numbers differ by exactly size)
        // For each edge, make the weight a random float between 0 and 1
        for (int i = 0; i < size * size; i++)
        {
            // Horizontal edges (exclude right edge)
            if ((i + 1) % size != 0)
            {
                float weight = UnityEngine.Random.Range(0f, 1f);
                graph[i].Add((i + 1, weight));
                graph[i + 1].Add((i, weight));
            }
            
            // Vertical edges (exclude bottom row)
            if (i + size < size * size)
            {
                float weight = UnityEngine.Random.Range(0f, 1f);
                graph[i].Add((i + size, weight));
                graph[i + size].Add((i, weight));
            }
        }

    }

    // public void GenerateMaze() {
    //     mstEdges = new List<(int, int)>();
    //     // Run Prim's algorithm to get a minimum spanning tree.
    //     HashSet<int> visited = new HashSet<int>();
    //     List<(int from, int to, float weight)> edges = new List<(int, int, float)>();

    //     int startVertex = (size * size - 1) / 2;
    //     visited.Add(startVertex);

    //     foreach (var edge in graph[startVertex])
    //     {
    //         edges.Add((startVertex, edge.neighbor, edge.weight));
    //     }

    //     // Loop until we visit all vertices
    //     while (mstEdges.Count < size * size - 1)
    //     {
    //         // Sort edges by weight
    //         edges.Sort((x, y) => x.weight.CompareTo(y.weight));

    //         // Find the smallest edge leading to an unvisited vertex
    //         (int from, int to, float weight) minEdge = (0, 0, float.MaxValue);
    //         foreach (var edge in edges)
    //         {
    //             if (!visited.Contains(edge.to))
    //             {
    //                 minEdge = edge;
    //                 break;
    //             }
    //         }

    //         if (minEdge.weight == float.MaxValue)
    //         {
    //             Debug.LogError("MST construction failed!");
    //             break;
    //         }

    //         // Add the edge to the MST
    //         mstEdges.Add((minEdge.from, minEdge.to));
    //         visited.Add(minEdge.to);

    //         // Remove the edge from the list
    //         edges.Remove(minEdge);

    //         // Add new edges from the newly visited vertex
    //         foreach (var edge in graph[minEdge.to])
    //         {
    //             if (!visited.Contains(edge.neighbor))
    //             {
    //                 // For each edge added to the MST, add it to a list.
    //                 edges.Add((minEdge.to, edge.neighbor, edge.weight));
    //             }
    //         }
    //     }

    //     Debug.Log($"Maze Generated! Expected edges: {size * size - 1}, Found edges: {mstEdges.Count}");
    // }

    public void GenerateMaze() {
        mstEdges = new List<(int, int)>();
        HashSet<int> visited = new HashSet<int>();
        SortedSet<Edge> pq = new SortedSet<Edge>(); // Priority queue

        // Start from the middle of the graph
        int startVertex = startingSquare;
        visited.Add(startVertex);

        // Add all edges from the start vertex to the priority queue
        foreach (var edge in graph[startVertex])
        {
            pq.Add(new Edge(startVertex, edge.neighbor, edge.weight));
        }

        // Loop until we add the right number of edges to make the maze
        while (mstEdges.Count < size * size - 1)
        {
            // If nothing in priority queue, there's a problem
            if (pq.Count == 0)
            {
                Debug.LogError("MST construction failed! Graph may be disconnected.");
                break;
            }

            // Get the minimum edge and remove it
            Edge minEdge = pq.Min;
            pq.Remove(minEdge);

            // Skip if it goes back into the visited vertices
            if (visited.Contains(minEdge.To)) continue;

            // Otherwise, add it to the mst
            visited.Add(minEdge.To);
            mstEdges.Add((minEdge.From, minEdge.To));

            // Add the other edges to the priority queue
            foreach (var edge in graph[minEdge.To])
            {
                if (!visited.Contains(edge.neighbor))
                {
                    pq.Add(new Edge(minEdge.To, edge.neighbor, edge.weight));
                }
            }
        }

        Debug.Log($"Maze Generated! Expected edges: {size * size - 1}, Found edges: {mstEdges.Count}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VisualizeMapGeneration() {
        // Once the MST is determined, delete the walls in the MST one by one
        // at a rate of 10 walls per second.
        Debug.Log("Visualizing");
        cam2.SetActive(true);
        cam1.SetActive(false);
        StartCoroutine(DeleteWallsOneByOne());
    }

    public IEnumerator DeleteWallsOneByOne()
    {
        foreach (var edge in mstEdges)
        {
            string wallName = GetWallName(edge.from, edge.to);
            GameObject wallToDelete = FindWall(wallName);

            if (wallToDelete)
            {
                walls.Remove(wallToDelete);
                wallToDelete.SetActive(false);
            }
            yield return new WaitForSeconds(0.1f); // 10 walls per second
        }
    }

    public void DeleteAllWallsAtOnce()
    {
        cam1.SetActive(true);
        cam2.SetActive(false);
        foreach (var edge in mstEdges)
        {
            string wallName = GetWallName(edge.from, edge.to);
            GameObject wallToDelete = FindWall(wallName);

            if (wallToDelete)
            {
                walls.Remove(wallToDelete);
                wallToDelete.SetActive(false);
            }
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private string GetWallName(int from, int to)
    {
        int a = Mathf.Min(from, to);
        int b = Mathf.Max(from, to);
        return $"Wall_{a}_{b}";
    }

    private GameObject FindWall(string wallName)
    {
        foreach (GameObject wall in walls)
        {
            if (wall != null && wall.name == wallName)
            {
                return wall;
            }
        }
        return null;
    }

    public List<(int from, int to)> getMST() {
        return mstEdges;
    }
}
