using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] carPrefabs;
    public Transform playerCar;
    public float spawnDistanceForward = 150f;   // Spawn phía trước người chơi
    public float spawnDistanceBackward = 30f;  // Spawn phía sau người chơi
    public int maxCars = 10;
    public float spawnInterval = 0.5f;

    private float minXSpawn = -3.5f;
    private float maxXSpawn = 3.5f;

    private List<GameObject> activeCars = new List<GameObject>(); // Bỏ static

    private void Start()
    {
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

    void SpawnCar()
    {
        bool spawnInFront = Random.value > 0.5f; // 50% spawn phía trước, 50% spawn phía sau
        float spawnZ = playerCar.position.z + (spawnInFront ? spawnDistanceForward : -spawnDistanceBackward);
        float spawnX = Random.Range(minXSpawn, maxXSpawn);

        Vector3 spawnPosition = new Vector3(spawnX, 0, spawnZ);

        // Xoay hướng xe theo chiều di chuyển
        Quaternion spawnRotation = spawnInFront ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

        GameObject car = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)], spawnPosition, spawnRotation);
        activeCars.Add(car);

        car.GetComponent<CarMovement>().Init(this, spawnInFront);
    }

    public void RemoveCar(GameObject car)
    {
        if (activeCars.Contains(car))
        {
            activeCars.Remove(car);
            Destroy(car);
        }
    }
}