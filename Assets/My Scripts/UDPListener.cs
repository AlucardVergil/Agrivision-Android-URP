using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Globalization;

public class UDPListener : MonoBehaviour
{
    private UdpClient udpClient;
    private const int port = 11000;
    private CancellationTokenSource cts;
    private float lastReceivedTime;
    private float timeoutDuration = 20f; // 20 seconds timeout duration
    private string lastReceivedMessage = "";

    [HideInInspector] public float latitude;
    [HideInInspector] public float longitude;

    public TMP_Text debugText; // Reference to the TextMeshPro UI component

    void Start()
    {
        try
        {
            udpClient = new UdpClient(port);
            cts = new CancellationTokenSource();
            lastReceivedMessage = $"UDP Listener started on port {port}";
            lastReceivedTime = Time.time;
            ReceiveData(cts.Token);
        }
        catch (Exception e)
        {
            lastReceivedMessage = $"Error initializing UDP listener: {e.Message}";
        }
    }

    void Update()
    {
        // Check for timeout
        if (Time.time - lastReceivedTime > timeoutDuration)
        {
            debugText.text = lastReceivedMessage + $"\nNo data received for {timeoutDuration} seconds.";
        }
        else
        {
            debugText.text = lastReceivedMessage;
        }
    }

    async void ReceiveData(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                lastReceivedMessage += "\nWaiting for data...";
                UdpReceiveResult result = await udpClient.ReceiveAsync();                
                if (token.IsCancellationRequested) break;

                string receivedData = Encoding.UTF8.GetString(result.Buffer);
                lastReceivedTime = Time.time; // Update the last received time
                lastReceivedMessage = $"Received GPS Data: {receivedData}";

                // Process the GPS data
                ProcessGPSData(receivedData);
            }
            catch (Exception e)
            {
                lastReceivedMessage = $"Error receiving data: {e.Message}";
            }
        }
    }

    void ProcessGPSData(string data)
    {
        // Parse and use the GPS data
        string[] parts = data.Split(',');
        if (parts.Length == 2)
        {
            //NumberStyles.Float, CultureInfo.InvariantCulture are used to keep the decimal format
            if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out latitude) 
                && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
            {
                lastReceivedMessage += $"\nLatitude: {latitude}, Longitude: {longitude}";
            }
            else
            {
                lastReceivedMessage += "\nError parsing GPS data";
            }
        }
        else
        {
            lastReceivedMessage += "\nReceived invalid GPS data format";
        }
    }

    void OnApplicationQuit()
    {
        cts.Cancel();
        udpClient.Close();
        udpClient.Dispose();
    }

    void OnDestroy()
    {
        cts.Cancel();
        udpClient.Close();
        udpClient.Dispose();
    }
}
