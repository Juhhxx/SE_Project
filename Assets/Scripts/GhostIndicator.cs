using UnityEngine;
using System.Collections.Generic;

public class GhostIndicator : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private List<Transform> ghosts;

    private void Update()
    {
        byte[] message = BuildMessage();
        UDPComunication.Instance.SendMessage(message);
    }

    private byte[] BuildMessage()
    {
        // 1. Find closest ghost
        Transform closest = null;
        float minDist = float.MaxValue;
        foreach (var g in ghosts)
        {
            float d = Vector2.Distance(player.position, g.position);
            if (d < minDist)
            {
                minDist = d;
                closest = g;
            }
        }

        if (closest == null) return new byte[8];

        // 2. Compute direction vector
        Vector2 dir = (closest.position - player.position).normalized;

        // 3. Map direction to LED coordinates
        int px = 3; // player at approx. center (3,3)
        int py = 3;
        int dx = Mathf.RoundToInt(dir.x * 3.5f);
        int dy = Mathf.RoundToInt(dir.y * 3.5f);
        int gx = Mathf.Clamp(px + dx, 0, 7);
        int gy = Mathf.Clamp(py + dy, 0, 7);

        // 4. Fill matrix with a line from player → ghost direction
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
