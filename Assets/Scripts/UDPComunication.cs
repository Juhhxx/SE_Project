using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPComunication : MonoBehaviourSingleton<UDPComunication>
{
    [SerializeField] private string _ipAdress;
    [SerializeField] private int _port;
    [SerializeField] private float _updateRate;

    IPAddress _adress;
    Socket _sender;
    IPEndPoint _endpoint;

    private void Awake()
    {
        base.SingletonCheck(this);

        CreateSocket();
    }

    private void OnEnable()
    {
        CreateSocket();
    }
    private void OnDisable()
    {
        DisconnectSocket();
    }

    private async void CreateSocket()
    {
        if (_sender != null) return;

        _adress = Dns.GetHostAddresses(_ipAdress)[0];

        _sender = new Socket(_adress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

        _endpoint = new IPEndPoint(_adress, _port);

        try
        {
            await _sender.ConnectAsync(_endpoint);
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

        _sender = null;
    }

    public (byte[], int) RecieveMessage()
    {
        byte[] bytes = new byte[8 * 1024 * 1024];
        int bytesRec = _sender.Receive(bytes);

        Debug.Log($"Recived Message : {bytes}");

        return (bytes, bytesRec);
    }
    public (byte[], int) SendMessage(byte[] message)
    {
        try
        {
            int bytesSent = _sender.Send(message);

            return RecieveMessage();
        }
        catch (SocketException e)
        {
            // In case of error, just write it
            Debug.Log($"SocketException : {e}");

            return (default, default);
        }
    }
   
    private void Update()
    {
        // SendMessage(new byte[] { 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA });
        // Thread.Sleep(1000);
        // SendMessage(new byte[] { 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55 });
        // Thread.Sleep(1000);
    }
}