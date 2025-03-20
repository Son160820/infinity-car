using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed = 15f;
    private CarSpawner spawner;
    private GameController gameController;

    private void Start()
    {
        gameController = FindAnyObjectByType<GameController>();
    }

    // Thêm vào script CarMovement
    public void Init(CarSpawner spawner, bool moveOpposite)
    {
        this.spawner = spawner;
        speed = moveOpposite ? Mathf.Abs(speed) : Mathf.Abs(speed)*2;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Hủy khi xa người chơi
        if (Vector3.Distance(transform.position, spawner.playerCar.position) > 200f)
        {
            spawner.RemoveCar(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameController.PlayerLose();
            print("GameOver!");
        }
    }
}