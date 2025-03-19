using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Driving : MonoBehaviour
{
    [Header("Thông tin hiển thị")]
    public Text textFPS;        // Hiển thị FPS
    public Text textSpeed;      // Hiển thị tốc độ
    public Text textPath;       // Hiển thị quãng đường đã đi
    public Text textTotalPath;  // Hiển thị tổng quãng đường

    private GameObject[] wheels;  // Hai bánh trước


    [Header("Dữ liệu tính toán di chuyển")]
    public int N_hp = 100;  // Công suất động cơ (mã lực)
    public int M = 1000;    // Khối lượng xe (kg)
    public float len = 1f;  // Hệ số góc lái
    public float mu = 0.5f; // Hệ số ma sát
    private int N;          // Công suất tính theo Joule
    private float v = 0f;   // Tốc độ của xe (m/s)

    [Range(0f, 90f)]
    public float maxWheelAngle = 40f; // Góc quay tối đa của bánh xe


    [Header("Dữ liệu tính toán lực cản")]
    public bool needToCalculateResist = true; // Bật/tắt tính toán lực cản
    public float K = 0.3f;  // Hệ số cản
    public float S = 2.7f;  // Diện tích mặt trước xe (m²)
    public float Ro = 1.27f; // Mật độ không khí (kg/m³)


    [Header("Tính toán tốc độ")]
    public float V_in_km_h = 0;  // Tốc độ tính theo km/h
    public float MaxSpeed = 0f;  // Tốc độ tối đa của xe (m/s)
    public float TotalPath = 0f; // Tổng quãng đường đã đi
    public float frictionForce = 0.1f; // Lực ma sát gây giảm tốc

    private const float g = 9.80665f; // Gia tốc trọng trường
    private float t = 0;  // Thời gian
    private float v0 = 0; // Tốc độ ban đầu


    void Start()
    {
        N = N_hp * 735;  // Chuyển đổi từ mã lực sang Joules
        MaxSpeed = Mathf.Pow((2 * N / (K * S * Ro)), 0.3333f); // Tính tốc độ tối đa

        // Lấy 2 bánh trước
        wheels = new GameObject[2];
        wheels[0] = transform.GetChild(1).gameObject;
        wheels[1] = transform.GetChild(2).gameObject;

        // Bắt đầu coroutine để hiển thị thông tin FPS và tốc độ
        StartCoroutine(dataShow(textFPS.GetComponent<Text>(), textSpeed.GetComponent<Text>()));
    }

    void Update()
    {
        calcS(); // Tính toán chuyển động tiến
        calcR(); // Tính toán góc quay xe
    }

    // Hàm tính toán di chuyển về phía trước
    void calcS()
    {
        // Nếu nhấn hoặc nhả nút ga/phanh, lưu lại tốc độ ban đầu và reset thời gian
        if (Input.GetButtonDown("RB") || Input.GetButtonUp("RB") || Input.GetButtonDown("LB") || Input.GetButtonUp("LB"))
        {
            v0 = v;
            t = 0;
        }

        int k = 0; // Xác định trạng thái của xe
        if (Input.GetButton("RB")) k = 1;  // Nhấn RB để tăng tốc
        if (Input.GetButton("LB")) k = -1; // Nhấn LB để phanh

        t += Time.deltaTime; // Cập nhật thời gian

        // Tính toán vận tốc theo công thức đơn giản
        if (k == 1) // Tăng tốc
        {
            v += (N / M) * Time.deltaTime;
            if (v > MaxSpeed) v = MaxSpeed; // Giới hạn tốc độ tối đa
            if (v < 0) v = 0; // Đảm bảo xe không có vận tốc âm
        }
        else if (k == -1) // Phanh
        {
            v -= (mu * g) * Time.deltaTime;
        }
        else // Thả trôi (không nhấn gì)
        {
            v -= frictionForce * Time.deltaTime;
            if (v < 0) v = 0; // Đảm bảo tốc độ không bị âm
        }

        // Chuyển đổi tốc độ sang km/h
        V_in_km_h = v * 3.6f;

        // Cập nhật vị trí
        float dist = v * Time.deltaTime;
        transform.position += transform.forward * dist;
        TotalPath += dist;
    }

    // Hàm tính toán góc quay của xe
    void calcR()
    {
        float rotationValue = Input.GetAxis("Horizontal") * maxWheelAngle;

        // Quay hai bánh trước theo hướng điều khiển
        wheels[0].transform.localRotation = Quaternion.Euler(0f, rotationValue, 0f);
        wheels[1].transform.localRotation = Quaternion.Euler(0f, rotationValue, 0f);

        float wheelAngle = wheels[0].transform.localEulerAngles.y;

        // Điều chỉnh góc quay hợp lý
        if (wheelAngle >= 360f - maxWheelAngle)
            wheelAngle -= 360f;

        // Nếu xe đang di chuyển, quay xe theo góc bánh trước
        if (v > 0f)
        {
            transform.Rotate(Vector3.up, wheelAngle * Time.deltaTime);
        }
    }

    // Lấy vận tốc hiện tại của xe
    public float GetVelocity()
    {
        return v;
    }

    // Thiết lập vận tốc mới cho xe
    public void SetVelocity(float newVelo)
    {
        v = newVelo;
    }

    // Hiển thị thông tin FPS, tốc độ và quãng đường
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
