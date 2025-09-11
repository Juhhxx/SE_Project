using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPComunication : MonoBehaviour
{
    [SerializeField] private string _ipAdress;
    [SerializeField] private int _port;

    IPAddress _adress;
    Socket _sender;
    IPEndPoint _endpoint;

    private void OnEnable()
    {
        CreateSocket();
    }
    private void OnDisable()
    {
        DisconnectSocket();
    }

    private void CreateSocket()
    {
        _adress = Dns.GetHostAddresses(_ipAdress)[0];

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
        byte[] bytes = new byte[8 * 1024 * 1024];
        int bytesRec = _sender.Receive(bytes);

        Debug.Log($"Recived Message : {Encoding.ASCII.GetString(bytes, 0, bytesRec)}");
    }
    public void SendMessage(byte[] message)
    {
        try
        {
            int bytesSent = _sender.Send(message);

            RecieveMessage();

        }
        catch (SocketException e)
        {
            // In case of error, just write it
            Debug.Log($"SocketException : {e}");
        }
    }

    private void FixedUpdate()
    {
    
    }
    private void Update()
    {
        SendMessage(new byte[] { 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA });
        Thread.Sleep(1000);
        SendMessage(new byte[] { 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55 });
        Thread.Sleep(1000);
    }
}
