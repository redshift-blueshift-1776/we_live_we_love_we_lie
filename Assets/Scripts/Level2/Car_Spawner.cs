using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Car_Spawner : MonoBehaviour
{
    [SerializeField] public GameObject gameManager;
    public ToFindWhatIveBecome gm;

    [SerializeField] public GameObject car;

    [SerializeField] public Vector3 startPosition;
    [SerializeField] public Vector3 targetPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = gameManager.GetComponent<ToFindWhatIveBecome>();
        StartCoroutine(spawnCars());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator spawnCars() {
        while (true) {
            if (gm.gameActive) {
                Debug.Log("Spawning Car");
                var newCar = Instantiate(car, transform);
                newCar.SetActive(true);
                var newCarScript = newCar.GetComponent<Car>();
                newCarScript.speed = gm.carSpeed;
                newCarScript.targetPosition = targetPosition;
                newCarScript.startPosition = startPosition;  
            }
            float waitTime = Random.Range(2.5f, 5f);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
