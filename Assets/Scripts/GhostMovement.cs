using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [SerializeField] private float _speed;

    private GameObject _target;

    private void Start()
    {
        _target = FindFirstObjectByType<GameManager>().gameObject;

        transform.LookAt(_target.transform);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 moveVec = transform.position;

        moveVec += transform.forward * _speed * Time.deltaTime;

        transform.position = moveVec;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _target) Destroy(gameObject);
    }
}
