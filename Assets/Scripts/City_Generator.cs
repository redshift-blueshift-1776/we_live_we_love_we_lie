using UnityEngine;
// using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
// using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System.Linq;
using NUnit.Framework;

public class City_Generator : MonoBehaviour
{
    [SerializeField] private bool menuOrLevelSelect;

    [Header("Dimensions")]
    [SerializeField] public int x_dimension; // Should be an odd number
    [SerializeField] public int z_dimension; // Should be an odd number
    [SerializeField] public float x_length;
    [SerializeField] public float z_length;
    [SerializeField] public float sidewalk_width;
    [SerializeField] public float road_width;
    [SerializeField] public float lightpole_edge_distance;

    [Header("Prefabs")]
    [SerializeField] public GameObject sidewalk;
    [SerializeField] public GameObject building;
    [SerializeField] public GameObject lightpole;
    [SerializeField] public GameObject roof;
    [SerializeField] public GameObject specialRoof;

    [SerializeField] private GameObject emptyRoom;
    [SerializeField] private GameObject doorRoom;

    [Header("Hardcoded")]
    [SerializeField] public string[] hardcoded;

    [Header("Probabilities")]
    [SerializeField] public float p_flat_roof;
    [SerializeField] public float p_sloped_roof;
    [SerializeField] public float p_special_roof;
    [SerializeField] private float probabilityOfEmptyRoomSpawning;


    [SerializeField] private int numberOfDoorRooms;

    private HashSet<float[]> lightpoleGlobalPositions;

    public HashSet<int> buildingLocations;

    private Stack<int[]> doorRoomLocations;

    [SerializeField] private bool useHardcodedLocations = true;
    [SerializeField] private float percentAutomaticLocations = 1f;

    [SerializeField] private bool shuffleDoorColors = false;

    [SerializeField] private bool placeRandomRooms = true;


    private float[][] doorColors =
    {
        new float[] {1, 0, 0},
        new float[] {1, 0.25f, 0},
        new float[] {1, 1, 0},
        new float[] {0, 1, 0},
        new float[] {0, 0, 1},
        new float[] {0.5f, 0, 1}
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (shuffleDoorColors)
        {
            doorColors = doorColors.OrderBy(x => UnityEngine.Random.value).ToArray();
        }

        buildingLocations = new HashSet<int>();
        lightpoleGlobalPositions = new HashSet<float[]>();
        if (useHardcodedLocations)
        {
            ParseHardcoded();
        }
        else
        {
            generateAutomaticLocations();
        }
        
        GenerateCity();

        //empty and special rooms
        if (placeRandomRooms) {
            generateRooms();
        }
        
        
    }

    public void ParseHardcoded() {
        foreach (string s in hardcoded) {
            string[] parts = s.Split(',');
            int x_val = int.Parse(parts[0]);
            int z_val = int.Parse(parts[1]);
            buildingLocations.Add(z_dimension * x_val + z_val);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void generateAutomaticLocations()
    {
        int totalLocations = x_dimension * z_dimension;
        int emptyLocations = (int)Mathf.Ceil(percentAutomaticLocations * totalLocations);

        HashSet<int[]> locationCoordinates = new HashSet<int[]>();
        for (int i = 0; i < x_dimension; i++)
        {
            for (int j = 0; j < z_dimension; j++)
            {
                locationCoordinates.Add(new int[] { i, j });
            }
        }

        int[][] shuffledLocations = locationCoordinates.OrderBy(x => UnityEngine.Random.value).ToArray();
        
        for (int k = 0; k < Mathf.Min(shuffledLocations.Length, emptyLocations); k++)
        {
            int i = shuffledLocations[k][0];
            int j = shuffledLocations[k][1];
            buildingLocations.Add(z_dimension * i + j);
        }
    }

    public void GenerateBuilding(int i, int j) {
        GameObject newBuilding = Instantiate(building);

        newBuilding.transform.position = getGlobalCoordinates(i, 0, j);

        int height = UnityEngine.Random.Range(1, 6) * 15;
        newBuilding.transform.localScale = new Vector3(x_length - road_width - 2 * sidewalk_width, (float) height, z_length - road_width - 2 * sidewalk_width);
    }

    public void GenerateRoofedBuilding(int i, int j)
    {
        GameObject newBuilding = Instantiate(building);

        newBuilding.transform.position = getGlobalCoordinates(i, 0, j);

        int height = UnityEngine.Random.Range(1, 6) * 15;
        newBuilding.transform.localScale = new Vector3(x_length - road_width - 2 * sidewalk_width, (float)height, z_length - road_width - 2 * sidewalk_width);

        GameObject newRoof = Instantiate(roof);
        newRoof.transform.position = getGlobalCoordinates(i, height, j);
        newRoof.transform.localScale = 100f / 12f * new Vector3(x_length - road_width - 2 * sidewalk_width, Mathf.Min(x_length - road_width - 2 * sidewalk_width, (z_length - road_width - 2 * sidewalk_width) / 2f), (z_length - road_width - 2 * sidewalk_width) / 2f);
    }

    public void GenerateSpecialRoofedBuilding(int i, int j)
    {
        GameObject newBuilding = Instantiate(building);
        float globalX = (i - (x_dimension - 1) / 2) * x_length;
        float globalZ = (j - (z_dimension - 1) / 2) * z_length;

        newBuilding.transform.position = new Vector3(globalX, 0, globalZ);

        int height = UnityEngine.Random.Range(1, 6) * 15;
        float building_x_length = x_length - road_width - 2 * sidewalk_width;
        float building_z_length = z_length - road_width - 2 * sidewalk_width;
        newBuilding.transform.localScale = new Vector3(building_x_length, (float)height, building_z_length);

        GameObject newSpecialRoof = Instantiate(specialRoof);
        float true_width = (Mathf.Min(building_x_length, building_z_length) + sidewalk_width) / 2;
        float offset = Mathf.Max(Mathf.Abs(building_x_length - building_z_length) / 2, 0);
        Vector3 roofPos1 = new Vector3(
            globalX - (building_x_length >= building_z_length ? 1 : 0) * offset,
            height,
            globalZ
            );
        Vector3 roofPos2 = new Vector3(
            globalX + (building_x_length >= building_z_length ? 1 : 0) * offset,
            height,
            globalZ
            );
        newSpecialRoof.transform.position = roofPos1;
        newSpecialRoof.transform.localScale = 100f / 12f * new Vector3(true_width, true_width, (building_z_length + sidewalk_width) / 2);

        newSpecialRoof = Instantiate(specialRoof);
        newSpecialRoof.transform.position = roofPos2;
        newSpecialRoof.transform.localRotation = Quaternion.Euler(0, 180, 0);
        newSpecialRoof.transform.localScale = 100f / 12f * new Vector3(true_width, true_width, (building_z_length + sidewalk_width) / 2);

        if (2 * (building_x_length / 2f - sidewalk_width) > 0f)
        {
            GameObject newRoof = Instantiate(roof);
            newRoof.transform.position = new Vector3(globalX, height, globalZ);
            newRoof.transform.localScale = 100f / 12f * new Vector3(
                Mathf.Max(building_x_length - building_z_length, 0),
                true_width,
                (building_z_length + sidewalk_width) / 2);
        }
    }
    public void GenerateEmptyRoom(int i, int j)
    {

        GameObject newEmptyRoom = Instantiate(emptyRoom);

        newEmptyRoom.transform.position = getGlobalCoordinates(i, 0, j);

        newEmptyRoom.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0);
    }

    public GameObject GenerateDoorRoom(int i, int j, Color c)
    {
        GameObject newDoorRoom = Instantiate(doorRoom);
        float globalX = (i - (x_dimension - 1) / 2) * x_length;
        float globalZ = (j - (z_dimension - 1) / 2) * z_length;

        newDoorRoom.transform.position = new Vector3(globalX, 0, globalZ);
        newDoorRoom.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0);

        return newDoorRoom;
    }

    public void GenerateLightPoles(int i, int j)
    {
        float globalX;
        float globalY;
        float globalZ;
        const float lightSourceLocalYOffset = 10.5f;

        int[,] corner =
        {
            {1, 1}, {-1, 1}, {1, -1}, {-1, -1}
        };

        for (int k = 0; k < 4; k++)
        {
            int dx = corner[k, 0];
            int dz = corner[k, 1];
            GameObject newLightpole = Instantiate(lightpole);
            globalX = (i - (x_dimension - 1) / 2) * x_length + dx * (x_length / 2f - road_width / 2f - lightpole_edge_distance);
            globalY = 0;
            globalZ = (j - (z_dimension - 1) / 2) * z_length + dz * (z_length / 2f - road_width / 2f - lightpole_edge_distance);
            newLightpole.transform.position = new Vector3(globalX, globalY, globalZ);
            Streetlight sl = newLightpole.GetComponent<Streetlight>();
            sl.displacement = (i + j) % 4;

            lightpoleGlobalPositions.Add(new float[] { globalX, globalY + lightSourceLocalYOffset, globalZ });
        }
    }

    private void generateRooms()
    {
        int[] randomizedHardcodedLocations = buildingLocations.OrderBy(x => UnityEngine.Random.value).ToArray();
        doorRoomLocations = new Stack<int[]>();

        int[] prevKeyLocation = null;
        int doorRoomsPlaced = 0;
        for (int k = 0; k < randomizedHardcodedLocations.Length; k++)
        {
            int location = randomizedHardcodedLocations[k];
            int j = location % z_dimension;
            int i = (location - j) / x_dimension;
            
            if (k == 0)
            {
                prevKeyLocation = new int[] { i, j };
                GenerateEmptyRoom(i, j);
                continue;
            }
            if (doorRoomsPlaced < numberOfDoorRooms)
            {
                doorRoomLocations.Push(new int[] { i, j });
                float[] vectorColor = doorColors[doorRoomsPlaced];
                Color c = new Color(vectorColor[0], vectorColor[1], vectorColor[2]);
                GameObject doorRoom = GenerateDoorRoom(i, j, c);

                GameObject key = doorRoom.transform.Find("Key").gameObject;

                key.transform.position = getGlobalCoordinates(prevKeyLocation[0], 3.5f, prevKeyLocation[1]);

                prevKeyLocation = new int[] { i, j };
                doorRoomsPlaced++;
            } else
            {
                if (doorRoomsPlaced == numberOfDoorRooms)
                {
                    doorRoomsPlaced++;
                }
                if (UnityEngine.Random.Range(0f, 1f) <= probabilityOfEmptyRoomSpawning)
                {
                    GenerateEmptyRoom(i, j);
                }
            }
        }
    }

    public void GenerateCity()
    {
        for (int i = 0; i < x_dimension; i++)
        {
            for (int j = 0; j < z_dimension; j++)
            {
                if (!buildingLocations.Contains(z_dimension * i + j))
                {
                    float rv = UnityEngine.Random.Range(0f, 1f);
                    if (rv < p_flat_roof)
                    {
                        GenerateBuilding(i, j);
                    }
                    else if (rv < p_flat_roof + p_sloped_roof)
                    {
                        GenerateRoofedBuilding(i, j);
                    } else if (rv < p_flat_roof + p_sloped_roof + p_special_roof) {
                        GenerateSpecialRoofedBuilding(i, j);
                    }
                }

                GameObject newSidewalk = Instantiate(sidewalk);

                newSidewalk.transform.position = getGlobalCoordinates(i, 0, j);
                newSidewalk.transform.localScale = new Vector3(x_length - road_width, 1, z_length - road_width);

                GenerateLightPoles(i, j);
            }
        }
    }

    public HashSet<float[]> getLightpoleGlobalPositions()
    {
        return lightpoleGlobalPositions;
    }

    private Vector3 getGlobalCoordinates(int i, float offsetY, int j)
    {
        float globalX = (i - (x_dimension - 1) / 2) * x_length;
        float globalZ = (j - (z_dimension - 1) / 2) * z_length;
        return new Vector3(globalX, offsetY, globalZ);
    }
}
