using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFollowerSensor : MonoBehaviour
{
    public Transform leftSensor;
    public Transform middleSensor;
    public Transform rightSensor;

    public float speed = 5f;  // Tốc độ di chuyển
    public float turnSpeed = 300f;  // Tốc độ quay

    // PID constants
    public float Kp = 0.1f;  // Hệ số tỷ lệ
    public float Ki = 0.0f;  // Hệ số tích phân
    public float Kd = 0.1f;  // Hệ số vi phân

    private float integral = 0f;  // Giá trị tích phân
    private float lastError = 0f;  // Giá trị lỗi lần trước

    void Update()
    {
        // Kiểm tra va chạm với đường line
        RaycastHit hitLeft;
        RaycastHit hitMiddle;
        RaycastHit hitRight;

        bool leftDetected = Physics.Raycast(leftSensor.position, Vector3.down, out hitLeft, 1f);
        bool middleDetected = Physics.Raycast(middleSensor.position, Vector3.down, out hitMiddle, 1f);
        bool rightDetected = Physics.Raycast(rightSensor.position, Vector3.down, out hitRight, 1f);

        // Tính toán lỗi dựa trên cảm biến
        float error = 0f;

        if (middleDetected && hitMiddle.collider.CompareTag("Line"))
        {
            error = 0;  // Đang ở giữa đường
        }
        else if (leftDetected && hitLeft.collider.CompareTag("Line"))
        {
            error = 1.5f;  // Đường nằm ở bên trái
        }
        else if (rightDetected && hitRight.collider.CompareTag("Line"))
        {
            error = -1.5f; // Đường nằm ở bên phải
        }
        else if (leftDetected && hitRight.collider.CompareTag("Line"))
        {
            // Khi chỉ có đường ở hai bên, hãy quyết định xem nên quay trái hay phải
            if (leftDetected && !middleDetected)
            {
                error = 1;  // Quay sang phải
            }
            else if (rightDetected && !middleDetected)
            {
                error = -1; // Quay sang trái
            }
        }
        else
        {
            error = lastError; // Giữ nguyên lỗi nếu không có phát hiện
        }

        // Tính toán tích phân và vi phân
        integral += error * Time.deltaTime;
        float derivative = (error - lastError) / Time.deltaTime;

        // Tính toán điều chỉnh
        float adjustment = Kp * error + Ki * integral + Kd * derivative;

        // Điều chỉnh hướng xe
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.Rotate(Vector3.up, adjustment * turnSpeed * Time.deltaTime);

        // Cập nhật lỗi lần trước
        lastError = error;
    }
}
