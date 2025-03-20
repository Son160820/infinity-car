using System.Collections.Generic;
using UnityEngine;

public class InfiniteMapGenerator : MonoBehaviour
{
    public GameObject roadPrefab; // Prefab của đoạn đường
    public int poolSize = 20; // Số lượng đoạn đường trong pool
    public float roadLength = 20f; // Chiều dài của mỗi đoạn đường
    public Transform player; // Transform của người chơi

    private Queue<GameObject> roadPool = new Queue<GameObject>();
    private float spawnZ = 10f; // Vị trí Z để spawn đoạn đường mới

    void Start()
    {
        // Khởi tạo pool với các đoạn đường
        for (int i = 0; i < poolSize; i++)
        {
            GameObject road = Instantiate(roadPrefab, transform);
            road.SetActive(false);
            roadPool.Enqueue(road);
        }

        // Spawn các đoạn đường ban đầu
        for (int i = 0; i < poolSize; i++)
        {
            SpawnRoad();
        }
    }

    void Update()
    {
        // Kiểm tra nếu người chơi đã đi qua một đoạn đường
        if (player.position.z > (spawnZ - poolSize * roadLength))
        {
            SpawnRoad();
            RecycleRoad();
        }
    }

    void SpawnRoad()
    {
        // Lấy một đoạn đường từ pool và đặt nó vào vị trí mới
        GameObject road = roadPool.Dequeue();
        road.transform.position = Vector3.forward * spawnZ;
        road.SetActive(true);
        spawnZ += roadLength*1.2f;
        roadPool.Enqueue(road);
    }

    void RecycleRoad()
    {
        // Tái sử dụng đoạn đường đầu tiên trong pool
        GameObject road = roadPool.Peek();
        if (road.transform.position.z + roadLength < player.position.z)
        {
            road.SetActive(false);
            roadPool.Dequeue();
            roadPool.Enqueue(road);
        }
    }
}
