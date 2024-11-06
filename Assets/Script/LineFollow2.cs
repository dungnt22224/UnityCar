using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFollow2 : MonoBehaviour
{

    public Transform leftSensor;
    public Transform middleSensor;
    public Transform rightSensor;

    public float defaultSpeed = 5f;  // Tốc độ mặc định khi không có lệnh
    public float defaultTurnSpeed = 300f;  // Tốc độ quay mặc định khi không có lệnh

    // PID constants
    public float Kp = 0.1f;
    public float Ki = 0.0f;
    public float Kd = 0.1f;

    private float integral = 0f;
    private float lastError = 0f;

    private Queue<float[]> movementQueue = new Queue<float[]>();  // Hàng đợi lệnh điều khiển từ front-end
    private float commandDuration = 0f;

    // Hàm nhận lệnh từ front-end
    public void ReceiveCommand(string dataFromFrontEnd)
    {
        string[] parameters = dataFromFrontEnd.Split(',');
        if (parameters.Length == 3)
        {
            float wheelSpeed = float.Parse(parameters[0]);
            float rotationSpeed = float.Parse(parameters[1]);
            float duration = float.Parse(parameters[2]);
            movementQueue.Enqueue(new float[] { wheelSpeed, rotationSpeed, duration });
        }
    }

    void Update()
    {
        if (movementQueue.Count > 0)
        {
            // Thực thi lệnh từ front-end
            ExecuteCommand();
        }
        else
        {
            // Thực hiện điều khiển PID khi không có lệnh từ front-end
            PIDControl();
        }
    }

    private void PIDControl()
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
            error = 0;
        }
        else if (leftDetected && hitLeft.collider.CompareTag("Line"))
        {
            error = 1.5f;
        }
        else if (rightDetected && hitRight.collider.CompareTag("Line"))
        {
            error = -1.5f;
        }
        else
        {
            error = lastError;
        }

        // Tính toán điều chỉnh PID
        integral += error * Time.deltaTime;
        float derivative = (error - lastError) / Time.deltaTime;
        float adjustment = Kp * error + Ki * integral + Kd * derivative;

        // Điều chỉnh hướng xe dựa trên lỗi PID
        transform.Translate(Vector3.forward * defaultSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, adjustment * defaultTurnSpeed * Time.deltaTime);

        // Cập nhật lỗi lần trước
        lastError = error;
    }

    private void ExecuteCommand()
    {
        if (commandDuration <= 0 && movementQueue.Count > 0)
        {
            // Lấy lệnh tiếp theo từ hàng đợi
            var command = movementQueue.Dequeue();
            float wheelSpeed = command[0];
            float rotationSpeed = command[1];
            commandDuration = command[2];
        }

        if (commandDuration > 0)
        {
            // Thực hiện lệnh di chuyển từ front-end
            transform.Translate(Vector3.forward * movementQueue.Peek()[0] * Time.deltaTime);
            transform.Rotate(Vector3.up, movementQueue.Peek()[1] * Time.deltaTime);
            commandDuration -= Time.deltaTime;
        }
    }
}
