using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    public Transform player;                                           // Tham chiếu đến người chơi
    private Driving playerStats;                                       // Trạng thái của người chơi
    public GameController gameController;                              // Điều khiển trò chơi
    public GameObject[] trafficCarPrefabs;                             // Mảng chứa các prefab xe giao thông

    public int maxCarsOnLine = 4;                                      // Số lượng xe tối đa có thể xuất hiện trên mỗi làn
    public float minDistanceToDeleteObjects = 15f;                     // Khoảng cách phía sau người chơi mà xe sẽ bị xóa
    public float maxDistanceToDeleteObjects = 120f;                    // Khoảng cách phía trước người chơi mà xe sẽ bị xóa
    public float distanceToReload = 500f;                              // Khoảng cách để thiết lập lại giao thông

    public float minSpawnTime = 0.4f;                                  // Thời gian spawn tối thiểu của xe
    public float maxSpawnTime = 2f;                                    // Thời gian spawn tối đa của xe

    public int minSpawnDistanceX = 2;                                  // Khoảng cách tối thiểu theo trục X từ trung tâm
    public int maxSpawnDistanceX = 5;                                  // Khoảng cách tối đa theo trục X từ trung tâm

    public Transform[] motherOfTraffic = new Transform[2];             // Mảng chứa các đối tượng xe giao thông: 
                                                                       // 0 - Xe ngược chiều, 1 - Xe cùng chiều

    private GameObject[] lastTraffic = new GameObject[2];              // Xe cuối cùng được spawn trên mỗi làn đường

    private float nextSpawnTime;                                       // Thời gian tiếp theo để spawn xe

    void Start()
    {
        nextSpawnTime = maxSpawnTime;
        playerStats = player.gameObject.GetComponent<Driving>();
        StartCoroutine(SpawnOncomingObject());    // Bắt đầu coroutine sinh xe ngược chiều
        StartCoroutine(SpawnSameLineObject());    // Bắt đầu coroutine sinh xe cùng chiều
    }

    void Update()
    {
        float koefSpawn = playerStats.GetVelocity() / playerStats.MaxSpeed;   // Hệ số tần suất spawn xe, từ 0 đến 1
        nextSpawnTime = maxSpawnTime * (1 - koefSpawn) + minSpawnTime;

        // Khi người chơi di chuyển vượt qua khoảng cách distanceToReload, thiết lập lại thế giới
        if (player.transform.position.z > distanceToReload)
            MoveWorld();
    }

    // Coroutine để spawn xe ngược chiều
    IEnumerator SpawnOncomingObject()
    {
        while (true)
        {
            if (motherOfTraffic[0].childCount < maxCarsOnLine)
            {
                Vector3 coord;

                if (lastTraffic[0] == null)
                    lastTraffic[0] = player.gameObject;

                coord = new Vector3(Random.Range(-maxSpawnDistanceX, -minSpawnDistanceX),
                                    0f,
                                    player.position.z + 100f);

                if (Mathf.Abs(lastTraffic[0].transform.position.z - coord.z) < 5f)
                {
                    coord.z += 15f;
                }
                lastTraffic[0] = Instantiate(trafficCarPrefabs[Random.Range(0, trafficCarPrefabs.Length)], coord, transform.rotation);
                lastTraffic[0].transform.SetParent(motherOfTraffic[0]);
                lastTraffic[0].tag = "Oncoming Line Traffic";

                TrafficCar tc = lastTraffic[0].GetComponent<TrafficCar>();
                tc.minDistanceToDeleteObjects = maxDistanceToDeleteObjects;
                tc.minDistanceToDeleteObjects = minDistanceToDeleteObjects;
                tc.player = player;
                tc.gameController = gameController;
            }
            yield return new WaitForSeconds(nextSpawnTime);
        }
    }

    // Coroutine để spawn xe cùng chiều
    IEnumerator SpawnSameLineObject()
    {
        while (true)
        {
            if (motherOfTraffic[1].childCount < maxCarsOnLine)
            {
                Vector3 coord;

                if (lastTraffic[1] == null)
                    lastTraffic[1] = player.gameObject;

                coord = new Vector3(Random.Range(minSpawnDistanceX, maxSpawnDistanceX),
                                    0f,
                                    player.position.z + 100f);

                bool canSpawn = true;

                if (playerStats.GetVelocity() < 4f)
                {
                    coord.z = player.position.z - 10f;
                    if (Mathf.Abs(coord.x - player.position.x) < 1.5f)
                    {
                        canSpawn = false;
                        lastTraffic[1] = null;
                    }
                    if (Mathf.Abs(lastTraffic[0].transform.position.z - coord.z) < 20f)
                        canSpawn = false;
                }
                else
                {
                    if (Mathf.Abs(lastTraffic[0].transform.position.z - coord.z) < 10f)
                        coord.z += 15f;
                }

                if (canSpawn)
                {
                    lastTraffic[1] = Instantiate(trafficCarPrefabs[Random.Range(0, trafficCarPrefabs.Length)], coord, transform.rotation);
                    lastTraffic[1].transform.SetParent(motherOfTraffic[1]);
                    lastTraffic[1].tag = "Same Line Traffic";

                    TrafficCar tc = lastTraffic[1].GetComponent<TrafficCar>();
                    tc.minDistanceToDeleteObjects = maxDistanceToDeleteObjects;
                    tc.minDistanceToDeleteObjects = minDistanceToDeleteObjects;
                    tc.player = player;
                    tc.gameController = gameController;
                    tc.SetVelocityByPlayer(playerStats.GetVelocity());
                }
            }
            yield return new WaitForSeconds(nextSpawnTime);
        }
    }

    // Di chuyển toàn bộ thế giới giao thông khi người chơi vượt quá khoảng cách distanceToReload
    void MoveWorld()
    {
        print("lapTraffic");

        for (int j = 0; j < 2; j++)
            for (int i = 0; i < motherOfTraffic[j].childCount; i++)
            {
                try
                {
                    float zz = player.position.z - motherOfTraffic[j].GetChild(i).position.z;
                    motherOfTraffic[j].GetChild(i).transform.position = new Vector3(motherOfTraffic[j].GetChild(i).transform.position.x,
                                                                                    motherOfTraffic[j].GetChild(i).transform.position.y,
                                                                                    0 - zz);
                }
                catch
                {
                    print("Có lỗi xảy ra trong MoveWorld");
                }
            }
    }
}
