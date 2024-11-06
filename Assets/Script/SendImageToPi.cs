using UnityEngine;
using System.Net.Sockets;
using System.IO;


public class SendImageToPi : MonoBehaviour
{
    private string raspberryPiIP = "192.168.1.XX"; // Địa chỉ IP của Raspberry Pi
    private int port = 5000;

    void CaptureAndSendImage()
    {
        // Chụp màn hình hoặc camera
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = renderTexture;
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Nén ảnh
        byte[] imgData = screenShot.EncodeToJPG(50); // Giảm chất lượng để giảm kích thước ảnh

        // Gửi qua socket
        using (TcpClient client = new TcpClient(raspberryPiIP, port))
        using (NetworkStream stream = client.GetStream())
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(imgData.Length);
            writer.Write(imgData);
            writer.Flush();
        }
    }
}
