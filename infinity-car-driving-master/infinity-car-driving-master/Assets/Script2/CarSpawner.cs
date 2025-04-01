using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] carPrefabs;
    public Transform playerCar;
    public float spawnDistanceForward = 300f;
    public float spawnDistanceBackward = 30f;
    public int maxCars = 10;
    public float spawnInterval = 0.5f;
    public int prewarmCount = 3;

    private float minXSpawn = 1.15f;
    private float maxXSpawn = 5f;

    private HashSet<GameObject> activeCars = new HashSet<GameObject>();
    private Dictionary<GameObject, Queue<GameObject>> carPool = new Dictionary<GameObject, Queue<GameObject>>();
    private GameController gameController;

    private void Start()
    {
        gameController = FindAnyObjectByType<GameController>();
        InitializePool();
        StartCoroutine(SpawnCarCoroutine());
    }

    IEnumerator SpawnCarCoroutine()
    {
        while (true)
        {
            if (activeCars.Count < maxCars)
            {
                SpawnCar();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void InitializePool()
    {
        foreach (GameObject prefab in carPrefabs)
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            for (int i = 0; i < prewarmCount; i++)
            {
                GameObject car = CreateNewPooledCar(prefab);
                queue.Enqueue(car);
            }
            carPool[prefab] = queue;
        }
    }

    private GameObject CreateNewPooledCar(GameObject prefab)
    {
        GameObject car = Instantiate(prefab);
        car.SetActive(false);
        CarMovement cm = car.GetComponent<CarMovement>();
        cm.Initialize(this, gameController, prefab);
        return car;
    }

    void SpawnCar()
    {
        bool spawnInFront = Random.value > 0.5f;
        bool isSameDirection = Random.value > 0.5f;

        float spawnZ = playerCar.position.z + (spawnInFront ? spawnDistanceForward : -spawnDistanceBackward);
        float spawnX = isSameDirection ? Random.Range(minXSpawn, maxXSpawn) : Random.Range(-maxXSpawn, -minXSpawn);

        Vector3 spawnPosition = new Vector3(spawnX, 0, spawnZ);
        Quaternion spawnRotation = isSameDirection ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

        GameObject selectedPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
        GameObject car = GetCarFromPool(selectedPrefab);

        car.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        car.SetActive(true);

        CarMovement cm = car.GetComponent<CarMovement>();
        cm.SetMovementParameters(!isSameDirection);

        activeCars.Add(car);
    }

    private GameObject GetCarFromPool(GameObject prefab)
    {
        if (carPool.TryGetValue(prefab, out Queue<GameObject> queue) && queue.Count > 0)
        {
            return queue.Dequeue();
        }
        return CreateNewPooledCar(prefab);
    }

    public void ReturnCarToPool(GameObject car)
    {
        if (activeCars.Contains(car))
        {
            activeCars.Remove(car);
            CarMovement cm = car.GetComponent<CarMovement>();
            car.SetActive(false);

            if (cm != null && cm.Prefab != null)
            {
                if (!carPool.ContainsKey(cm.Prefab))
                {
                    carPool[cm.Prefab] = new Queue<GameObject>();
                }
                carPool[cm.Prefab].Enqueue(car);
            }
        }
    }
}