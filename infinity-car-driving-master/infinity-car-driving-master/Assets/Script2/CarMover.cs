using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float baseSpeed = 15f;
    private float speed;
    private CarSpawner spawner;
    private GameController gameController;
    private bool isOpposite;
    private Transform playerTransform;

    public GameObject Prefab { get; private set; }
    private float destroySqrDistance;

    private float destroySqrDistanceForward = 300f * 300f;
    private float destroyDistanceBackward = 20f;

    public void Initialize(CarSpawner spawner, GameController gameController, GameObject prefab)
    {
        this.spawner = spawner;
        this.gameController = gameController;
        this.Prefab = prefab;
        playerTransform = spawner.playerCar;
        destroySqrDistance = spawner.spawnDistanceForward * spawner.spawnDistanceForward;
    }

    public void SetMovementParameters(bool isOppositeDirection)
    {
        isOpposite = isOppositeDirection;
        speed = isOppositeDirection ? baseSpeed : baseSpeed * 1.2f;
        if (!isOppositeDirection) speed *= 1.2f;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        Vector3 playerPos = playerTransform.position;
        Vector3 position = transform.position;
        float sqrDistance = (position - playerPos).sqrMagnitude;

        float zDistance = position.z - playerPos.z; // Kiểm tra khoảng cách theo trục Z

        if (sqrDistance > destroySqrDistance)
        {
            spawner.ReturnCarToPool(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameController.PlayerLose();
        }
    }
}