using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NaughtyAttributes;
using UnityEngine;

public class UDPComunication : MonoBehaviourSingleton<UDPComunication>
{
    [SerializeField] private string _ipAdress;
    [SerializeField] private int _port;
    [SerializeField] private float _refreshRate;

    IPAddress _adress;
    Socket _sender;
    IPEndPoint _endpoint;

    Timer _timer;

    private void Start()
    {
        base.SingletonCheck(this);

        _timer = new Timer(_refreshRate);
    }

    private void OnEnable()
    {
        CreateSocket();

        // _timer.OnTimerDone += RecieveMessage;
    }
    private void OnDisable()
    {
        DisconnectSocket();

        // _timer.OnTimerDone -= RecieveMessage;
    }

    private void CreateSocket()
    {
        _adress = IPAddress.Parse(_ipAdress);

        _sender = new Socket(_adress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

        _endpoint = new IPEndPoint(_adress, _port);

        try
        {
            _sender.Connect(_endpoint);
        }
        catch (SocketException e)
        {
            // In case of error, just write it
            Debug.Log($"SocketException : {e}");
        }
    }
    private void DisconnectSocket()
    {
        // Shutdown the socket (sends a termination signal, so the
        // other side knows we're terminating the connection)
        _sender.Shutdown(SocketShutdown.Both);
        // Closes the socket
        _sender.Close();
    }

    public void RecieveMessage()
    {
        if (!_sender.Connected) return;

        byte[] bytes = new byte[8 * 1024 * 1024];
        int bytesRec = _sender.Receive(bytes);

        Debug.Log($"Recived Message");
    }
    public void SendMessage(byte[] message)
    {
        if (!_sender.Connected) return;

        try
        {
            int bytesSent = _sender.Send(message);

            Debug.Log($"Sent Message");

            RecieveMessage();

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

        if (Input.GetKeyDown(KeyCode.R)) RecieveMessage();
    }
}
