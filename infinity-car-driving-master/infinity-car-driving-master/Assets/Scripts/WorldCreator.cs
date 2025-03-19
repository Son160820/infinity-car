using System.Collections.Generic;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{
    // --- Tạo và quản lý đường trong game ---
    public Transform player;                                           // Vị trí của người chơi
    public GameObject[] tilePrefabs;                                   // Mảng các prefab đường (các đoạn đường)
    public float tileLength = 24f;                                     // Chiều dài của một đoạn đường
    public float distanceToReload = 500f;                              // Khoảng cách để reset thế giới về vị trí ban đầu
    public int numberOfTiles = 5;                                      // Số lượng đoạn đường tồn tại cùng lúc trong game
    private float zSpawn = 0f;                                         // Vị trí Z tiếp theo để spawn đoạn đường
    private List<GameObject> activeTiles = new List<GameObject>();     // Danh sách các đoạn đường đang hiển thị
    public Transform motherOfTiles;                                    // Đối tượng cha chứa tất cả đoạn đường


    void Start()
    {
        SpawnTile(-1);      // Tạo các đoạn đường ban đầu
    }

    void Update()
    {
        // Kiểm tra nếu người chơi di chuyển xa khỏi đoạn đường gần nhất, tạo thêm đoạn đường mới
        if (player.position.z - (tileLength + 5) > zSpawn - (numberOfTiles * tileLength))
        {
            SpawnTile(Random.Range(0, tilePrefabs.Length)); // Tạo một đoạn đường ngẫu nhiên
            DeleteTile(); // Xóa đoạn đường cũ nhất
        }

        // Nếu người chơi đạt đến khoảng cách nhất định, reset thế giới về vị trí ban đầu
        if (player.transform.position.z > distanceToReload)
            MoveWorld();
    }


    // Hàm tạo đoạn đường, có thể tạo toàn bộ đường ban đầu (-1) hoặc một đoạn riêng lẻ
    public void SpawnTile(int tileIndex)
    {
        if (tileIndex == -1)        // Nếu tileIndex = -1, tạo bản đồ ban đầu
        {
            for (int i = 0; i < numberOfTiles; i++)
                SpawnTile(Random.Range(0, tilePrefabs.Length));

        }
        else                        // Nếu không, tạo một đoạn đường mới khi người chơi tiến lên
        {
            GameObject go = Instantiate(tilePrefabs[tileIndex], transform.forward * zSpawn, transform.rotation);
            go.transform.SetParent(motherOfTiles); // Gán đoạn đường vào đối tượng cha
            activeTiles.Add(go); // Thêm vào danh sách đoạn đường đang hiển thị
            zSpawn += tileLength; // Cập nhật vị trí Z cho đoạn đường tiếp theo
        }
    }

    // Hàm xóa đoạn đường cũ, có thể xóa một đoạn hoặc toàn bộ (-1)
    private void DeleteTile(int num = 1)
    {
        if (num == -1) // Nếu num = -1, xóa toàn bộ đoạn đường
        {
            for (int i = 0; i < numberOfTiles; i++)
            {
                Destroy(activeTiles[0]);     // Xóa khỏi scene
                activeTiles.RemoveAt(0);     // Xóa khỏi danh sách
            }
            zSpawn = 0; // Reset vị trí spawn về 0
        }
        else // Xóa đoạn đường đầu tiên trong danh sách
        {
            Destroy(activeTiles[0]);     // Xóa khỏi scene
            activeTiles.RemoveAt(0);     // Xóa khỏi danh sách
        }
    }

    // Hàm reset thế giới khi người chơi đi quá xa
    void MoveWorld()
    {
        print("lapWorld"); // In thông báo khi reset thế giới

        DeleteTile(-1); // Xóa toàn bộ đường cũ
        SpawnTile(-1);  // Tạo lại đường mới từ đầu

        player.position = new Vector3(player.position.x, player.position.y, 0f); //
    }
}
