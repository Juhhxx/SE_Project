using System.Threading;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField][Range(0, 180)] private int _cameraFOV;
    [SerializeField][Range(0, 180)] private int _cameraDistance;
    [SerializeField] private int _numberOfRaycasts;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Flash();

        DrawWireArc(transform.position, transform.forward, _cameraFOV, _cameraDistance, Color.green);
    }

    public void Flash()
    {
        for (int i = 0; i < _numberOfRaycasts; i++)
        {
            Vector3 dir = CalculateDirection(i);

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, _cameraDistance))
            {
                GhostMovement ghost = hit.collider.gameObject.GetComponent<GhostMovement>();

                if (ghost != null)
                {
                    Destroy(ghost.gameObject);
                    break;
                }
            }

            Debug.DrawLine(transform.position, dir * _cameraDistance, Color.blue);
        }
    }

    private Vector3 CalculateDirection(int iteration)
    {
        float initialAngle = -(_cameraFOV / 2);
        float deviationAngle = _cameraFOV / _numberOfRaycasts;

        float angle = initialAngle + (deviationAngle * iteration);

        Vector3 f = transform.forward;

        Quaternion rotation = Quaternion.Euler(0, angle, 0);

        Vector3 dir = rotation * f;

        return dir;
    }

    private void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, Color color, float maxSteps = 20)
    {
        var srcAngles = GetAnglesFromDir(position, dir);
        var initialPos = position;
        var posA = initialPos;
        var stepAngles = anglesRange / maxSteps;
        var angle = srcAngles - anglesRange / 2;
        for (var i = 0; i <= maxSteps; i++)
        {
            var rad = Mathf.Deg2Rad * angle;
            var posB = initialPos;
            posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

            Debug.DrawLine(posA, posB, color);

            angle += stepAngles;
            posA = posB;
        }
        Debug.DrawLine(posA, initialPos, color);
    }

    private float GetAnglesFromDir(Vector3 position, Vector3 dir)
    {
        var forwardLimitPos = position + dir;
        var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

        return srcAngles;
    }
}
