using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCar : MonoBehaviour
{
    [Header("Information")]
    public float N_hp = 100f;       //Mã lực
    public float M = 1000f;         //trọng lượng
    public float Brake = 0.5f;        //Phanh
    private float N;                //Năng lượng đơn vị J
    private float v = 0;            //gia tốc xe

    [Header("Enviroment")]
    private float g = 9.81f;        //gia tốc trọng trường
    private float mu = 0.1f;        //hệ số ma sát

    [Header("Wheel")]
    public Transform frontLeftWheel;  // Bánh xe trước bên trái
    public Transform frontRightWheel; // Bánh xe trước bên phải
    public float rotationSpeed = 10f; // Tốc độ xoay của xe
    public float wheelTurnAngle = 30f; // Góc xoay tối đa của bánh xe

    [Header("UI")]
    public float V_in_km_h = 0;  // Tốc độ tính theo km/h
    public float TotalPath = 0f; // Tổng quãng đường đã đi
    public Text textFPS;        // Hiển thị FPS
    public Text textSpeed;      // Hiển thị tốc độ
    public Text textPath;       // Hiển thị quãng đường đã đi
    public Text textTotalPath;  // Hiển thị tổng quãng đường

    //private float t = 0;  // Thời gian
    //private float v0 = 0; // Tốc độ ban đầu


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        N = N_hp * 735f;            //Chuyển đôi thành J
        // Bắt đầu coroutine để hiển thị tốc độ
        StartCoroutine(dataShow(textFPS.GetComponent<Text>(), textSpeed.GetComponent<Text>()));
    }

    // Update is called once per frame
    void Update()
    {
        calsPower();
        HandleWheelSteering();
    }

    void calsPower()
    {
        bool isAccelerating = Input.GetKey(KeyCode.W);
        bool isDecelerating = Input.GetKey(KeyCode.S);

        float F_friction = mu * M * g;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyUp(KeyCode.S))
        {
            //v0 = v;
            //t = 0;
        }

        if (isAccelerating)
        {
            float F_engine = v > 0 ? (N / v) : (N / 0.1f);          //Công do máy sinh ra
            float F_total = Mathf.Max(F_engine - F_friction, 0);    // Đảm bảo lực không âm
            float a = F_total / M;                                  // Gia tốc

            v += a * Time.deltaTime;                                //Vận tốc so với thời gian thực
        }
        else if (isDecelerating)
        {
            v -= Brake * Time.deltaTime;                            //Phanh xe
        }
        else
        {
            v -= (F_friction / M) * Time.deltaTime;                       // Ma sát làm xe giảm tốc từ từ
            if (v < 0) v = 0;
        }

        // Di chuyển xe theo hướng trước
        transform.Translate(Vector3.forward * v * Time.deltaTime);
        V_in_km_h = v * 3.6f;

        // Cập nhật vị trí
        float dist = v * Time.deltaTime;
        transform.position += transform.forward * dist;
        TotalPath += dist;
    }

    void Controller()
    {
        // Lấy hướng của 2 bánh xe phía trước
        Vector3 leftWheelDirection = frontLeftWheel.forward;
        Vector3 rightWheelDirection = frontRightWheel.forward;

        // Tính hướng di chuyển trung bình của 2 bánh xe
        Vector3 averageDirection = (leftWheelDirection + rightWheelDirection ).normalized;

        // Di chuyển xe theo hướng đã tính toán
        transform.position += averageDirection * v * Time.deltaTime;

        // Tính góc giữa 2 hướng của bánh xe
        float angleDifference = Vector3.SignedAngle(leftWheelDirection, rightWheelDirection, Vector3.up);

        // Xoay xe dựa trên góc chênh lệch
        transform.Rotate(Vector3.up, angleDifference * rotationSpeed * Time.deltaTime);
    }

    void HandleWheelSteering()
    {
        float flagInput = 0f;

        frontLeftWheel.localScale = new Vector3(-1f, 1f, 1f);

        // Nhận input từ phím A và D
        if (Input.GetKey(KeyCode.A))
        {
            flagInput = -1f; // Xoay sang trái
        }
        else if (Input.GetKey(KeyCode.D))
        {
            flagInput = 1f; // Xoay sang phải
        }

        // Tính góc xoay dựa trên input
        float targetAngle = flagInput * wheelTurnAngle;

        // Áp dụng góc xoay cho cả 2 bánh xe
        frontLeftWheel.localRotation = Quaternion.Euler(0f, targetAngle, 0f);
        frontRightWheel.localRotation = Quaternion.Euler(0f, targetAngle, 0f);


        // Tính toán góc xoay của xe dựa trên góc của bánh trước
        float carRotationSpeed = rotationSpeed * Time.deltaTime;
        float newRotationY = Mathf.LerpAngle(transform.eulerAngles.y, transform.eulerAngles.y + targetAngle, carRotationSpeed);

        // Xoay xe theo trục Y để rẽ mượt mà hơn
        transform.rotation = Quaternion.Euler(0f, newRotationY, 0f);

        Controller();
    }

    IEnumerator dataShow(Text fpsText, Text speedText)
    {
        while (true)
        {
            fpsText.text = "FPS = " + ((int)(1 / Time.deltaTime)).ToString();
            speedText.text = ((int)V_in_km_h).ToString() + " km/h";
            textPath.text = ((int)TotalPath).ToString() + " m";
            textTotalPath.text = "Your path is " + ((int)TotalPath).ToString() + " m";
            yield return new WaitForSeconds(0.1f);
        }
    }
}
