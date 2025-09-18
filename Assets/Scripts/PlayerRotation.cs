using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerRotation : MonoBehaviour
{
    private Rigidbody _rb;
    private Queue<int> _rotValues;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _rotValues = new Queue<int>();
    }

    public void SendRotation(byte rot, byte dir)
    {
        Debug.Log("AAAA");

        int r = rot;

        if (dir == 0) r = -r;

        AddRotation(rot);
    }

    private void AddRotation(int rot)
    {
        _rotValues.Enqueue(rot);

        if (_rotValues.Count > 10)
        {
            _rotValues.Dequeue();
            ApplyRotationForce(DoMedian());
        }

        Debug.LogWarning($"ANGLE : {rot}");

        string list = "ANGLES : {";

        foreach (int r in _rotValues) list += $" {r} ";

        list += "}";

        Debug.LogWarning(list);

    }

    private int DoMedian()
    {
        int med = 0;

        foreach (int rot in _rotValues) med += rot;

        return med / _rotValues.Count;
    }

    public void ApplyRotationForce(float angleChange)
    {
        Vector3 finalRot = transform.rotation.eulerAngles;

        finalRot.y += angleChange;

        _rb.MoveRotation(Quaternion.Euler(finalRot));
    }

}
