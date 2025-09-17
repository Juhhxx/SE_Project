using UnityEngine;
using System.Collections.Generic;

public class GhostIndicator : MonoBehaviourSingleton<GhostIndicator>
{
    [SerializeField] private Transform _player;
    [SerializeField] private List<Transform> _ghosts;
    [SerializeField] private int _channel;

    public void AddGhost(Transform ghost) => _ghosts.Add(ghost);
    public void RemoveGhost(Transform ghost) => _ghosts.Remove(ghost);
    public void SetChannel(int channel) => _channel = channel;

    private byte[] _arrowUp = new byte[8]
    {
        0b00000000,
        0b00011000,
        0b00111100,
        0b01111110,
        0b00011000,
        0b00011000,
        0b00011000,
        0b00000000
    };

    private byte[] _arrowDown = new byte[8]
    {
        0b00000000,
        0b00011000,
        0b00011000,
        0b00011000,
        0b01111110,
        0b00111100,
        0b00011000,
        0b00000000
    };

    private byte[] _arrowRight = new byte[8]
    {
        0b00000000,
        0b00001000,
        0b00001100,
        0b01111110,
        0b01111110,
        0b00101100,
        0b00001000,
        0b00000000
    };

    private byte[] _arrowLeft = new byte[8]
    {
        0b00000000,
        0b00010000,
        0b00110000,
        0b01111110,
        0b01111110,
        0b00110000,
        0b00010000,
        0b00000000
    };

    Timer _timer;

    private void Awake()
    {
        base.SingletonCheck(this);
    }
    private void Start()
    {
        // UDPComunication.Instance.SendMessage(_arrowUp);

        _timer = new Timer(1f);

        _timer.OnTimerDone += () => UDPComunication.Instance.SendMessage(BuildMessage());
    }

    private void Update()
    {
        // byte[] message = BuildMessage();
        // UDPComunication.Instance.SendMessage(message);

        _timer.CountTimer();
    }

    private byte[] BuildMessage()
    {
        // 1. Find closest ghost
        Transform closest = null;

        float minDist = float.MaxValue;
        foreach (var g in _ghosts)
        {
            if (g.GetComponent<GhostBehaviour>().Class != _channel) continue;

            float d = Vector2.Distance(_player.position, g.position);
            if (d < minDist)
            {
                minDist = d;
                closest = g;
            }
        }

        if (closest == null) return _arrowLeft;

        Debug.DrawLine(_player.position, closest.position);

        // 2. Compute direction vector
        Vector3 worldDir = (closest.position - _player.position).normalized;
        Vector3 dir = _player.InverseTransformDirection(worldDir);

        Debug.Log($"DIRECTION : {dir}");

        // 3. Map direction to LED coordinates
        int px = 3; // _player at approx. center (3,3)
        int py = 3;
        int dx = Mathf.RoundToInt(dir.x * 3.5f);
        int dy = Mathf.RoundToInt(-dir.z * 3.5f);
        int gx = Mathf.Clamp(px + dx, 0, 7);
        int gy = Mathf.Clamp(py + dy, 0, 7);

        // 4. Fill matrix with a line from _player → ghost direction
        bool[,] matrix = new bool[8, 8];
        DrawLine(px, py, gx, gy, matrix);

        // 5. Convert to bytes
        return MatrixToBytes(matrix);
    }

    private void DrawLine(int x0, int y0, int x1, int y1, bool[,] matrix)
    {
        int dx = Mathf.Abs(x1 - x0);
        int sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0);
        int sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        while (true)
        {
            matrix[y0, x0] = true; // light LED at (x0,y0)

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy)
            {
                err += dy;
                x0 += sx;
            }
            if (e2 <= dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    private byte[] MatrixToBytes(bool[,] matrix)
    {
        byte[] rows = new byte[8];
        for (int y = 0; y < 8; y++)
        {
            byte row = 0;
            for (int x = 0; x < 8; x++)
            {
                if (matrix[y, x])
                {
                    row |= (byte)(1 << x);
                }
            }
            rows[y] = row;
        }
        return rows;
    }
}
