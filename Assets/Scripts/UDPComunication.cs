using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class UDPComunication : MonoBehaviourSingleton<UDPComunication>
{
    [SerializeField] private string _ipAdress;
    [SerializeField] private int _port;
    [SerializeField] private float _refreshRate;

    IPAddress _adress;
    UdpClient _udpClient;
    IPEndPoint _endpoint;

    Timer _timer;

    private void Awake()
    {
        base.SingletonCheck(this);

        _timer = new Timer(_refreshRate);
    }

    private void OnEnable()
    {
        CreateUDPClient();

        _timer.OnTimerDone += () => _ = RecieveMessage();
    }
    private void OnDisable()
    {
        DisconnectUDPClient();

        _timer.OnTimerDone -= () => _ = RecieveMessage();
    }

    private void CreateUDPClient()
    {
        _adress = IPAddress.Parse(_ipAdress);

        _udpClient = new UdpClient(_port);
        _udpClient.EnableBroadcast = true;

        _endpoint = new IPEndPoint(_adress, _port);

        // try
        // {
        //     _udpClient.Connect(_endpoint);
        // }
        // catch (SocketException e)
        // {
        //     // In case of error, just write it
        //     Debug.Log($"Error Connecting to Adress {_adress}. SocketException : {e}");
        // }
    }
    private void DisconnectUDPClient()
    {
        _udpClient?.Close();
        _udpClient = null;
    }

    public async Task RecieveMessage()
    {
        if (_udpClient == null) return;

        try
        {
            UdpReceiveResult result = await _udpClient.ReceiveAsync();
            byte[] data = result.Buffer;

            string msg = Encoding.ASCII.GetString(data);
            Debug.Log($"Received {data.Length} bytes from {result.RemoteEndPoint}: {msg}");
        }
        catch (Exception e)
        {
            Debug.LogError($"UDP Receive Exception: {e.Message}");
        }
    }
    public void SendMessage(byte[] message)
    {
        if (_udpClient == null) return;

        try
        {
            int bytesSent = _udpClient.Send(message, message.Length, _ipAdress, _port);

            Debug.Log($"Sent Message");
        }
        catch (SocketException e)
        {
            // In case of error, just write it
            Debug.Log($"SocketException : {e}");
        }
    }

    private void Update()
    {
        // SendMessage(new byte[] { 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA });
        // Thread.Sleep(1000);
        // SendMessage(new byte[] { 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55 });
        // Thread.Sleep(1000);

        //SendMessage(new byte[] { 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA });

    }
}
