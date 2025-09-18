using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPComunication : MonoBehaviourSingleton<UDPComunication>
{
    [SerializeField] private string _ipAdress;
    [SerializeField] private int _port;

    IPAddress _adress;
    Socket _sender;
    UdpClient _udp;
    IPEndPoint _endpoint;

    private void Awake()
    {
        base.SingletonCheck(this);
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
        // _adress = Dns.GetHostAddresses(_ipAdress)[0];
        _adress = IPAddress.Parse(_ipAdress);

        _sender = new Socket(_adress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        _sender.Blocking = false;

        _endpoint = new IPEndPoint(_adress, _port);

        try
        {
            await _sender.ConnectAsync(_endpoint);

            Debug.LogWarning("CONNECTED");
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
        byte[] bytes = new byte[4];
        int bytesRec = _sender.Receive(bytes);

        string msg = "MESSAGE RECIEVED : {";

        foreach (byte b in bytes) msg += $" {b} ";

        msg += "}";

        Debug.LogWarning(msg);

        return (bytes, bytesRec);
    }
    public (byte[], int) SendMessage(byte[] message)
    {
        try
        {
            int bytesSent = _sender.Send(message);

            string msg = "MESSAGE SENT : {";

            foreach (byte b in message) msg += $" {b} ";

            msg += "}";

            Debug.LogWarning(msg);

            return RecieveMessage();
        }
        catch (SocketException e)
        {
            // In case of error, just write it
            Debug.Log($"SocketException : {e}");

            return (new byte[4] {0,0,0,0}, default);
        }
    }
}