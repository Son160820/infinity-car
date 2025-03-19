using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficCar : MonoBehaviour
{
    public Transform player;                            // Tham chiếu đến người chơi
    public GameController gameController;

    public float minDistanceToDeleteObjects = 15f;      // Khoảng cách phía sau người chơi để hủy xe
    public float maxDistanceToDeleteObjects = 120f;     // Khoảng cách phía trước người chơi để hủy xe

    private float velocity = 0f;                        // Tốc độ của xe
    private float timeOfSpawn;                          // Thời gian xe được sinh ra

    // Hàm Start được gọi một lần khi đối tượng được khởi tạo
    void Start()
    {
        timeOfSpawn = Time.time;  // Lưu lại thời gian xe được sinh ra

        // Nếu xe thuộc làn đường ngược chiều
        if (gameObject.tag == "Oncoming Line Traffic")
        {
            transform.Rotate(0f, 180f, 0f);  // Xoay xe 180 độ để hướng về phía người chơi
            velocity = Random.Range(10f, 11f); // Thiết lập tốc độ ngẫu nhiên trong khoảng 10 - 11
        }
    }

    // Hàm Update được gọi mỗi frame
    void Update()
    {
        // Di chuyển xe về phía trước theo hướng hiện tại với tốc độ velocity
        transform.Translate(Vector3.forward * velocity * Time.deltaTime);

        // Nếu xe quá xa người chơi (phía trước hoặc phía sau), hủy xe để tối ưu bộ nhớ
        if (player.position.z - transform.position.z > minDistanceToDeleteObjects ||
            transform.position.z - player.position.z > maxDistanceToDeleteObjects)
        {
            Destroy(gameObject);
        }
    }

    // Thiết lập tốc độ của xe dựa trên tốc độ của người chơi
    public void SetVelocityByPlayer(float playerVelocity)
    {
        if (playerVelocity < 10f)  // Nếu tốc độ người chơi nhỏ hơn 10
            velocity = Random.Range(4f, 5f);  // Xe chạy chậm hơn (4 - 5)
        else
            velocity = playerVelocity / 2f + Random.Range(3f, 4f);  // Tốc độ bằng một nửa tốc độ người chơi + giá trị ngẫu nhiên
    }

    // Thiết lập tốc độ của xe
    public void SetVelocity(float newVelo)
    {
        velocity = newVelo;
    }

    // Lấy tốc độ hiện tại của xe
    public float GetVelocity()
    {
        return velocity;
    }

    // Xử lý khi xe va chạm với vật thể khác
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player") // Nếu xe va chạm với người chơi
        {
            gameController.PlayerLose(); // Gọi hàm xử lý thua cuộc trong GameController
            print("GameOver!");  // Hiển thị thông báo game over
        }
        else
        {
            // Nếu xe tồn tại chưa đến 0.1 giây và đã va chạm với vật thể khác
            if (timeOfSpawn > Time.time - 0.1f)
                Destroy(gameObject); // Hủy xe
        }
    }
}