using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerRotation : MonoBehaviour
{
    private Rigidbody _rb;
    private Timer _timer;
    private Queue<int> _rotValues;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _rotValues = new Queue<int>();

        _timer = new Timer(0.16f);

        // _timer.OnTimerDone += Test;
    }

    private void Test()
    {
        Debug.Log("AAAA");

        (byte[] bytes, int size) = UDPComunication.Instance.SendMessage(new byte[] { 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA });

        int rot = bytes[0];

        if (bytes[1] == 0) rot = -rot;

        Debug.LogWarning($"ANGLE : {rot}");

        string list = "ANGLES : {";

        foreach (int r in _rotValues) list += $" {r} ";

        list += "}";

        Debug.LogWarning(list);

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

    private void Update()
    {
        _timer.CountTimer();
    }


}
