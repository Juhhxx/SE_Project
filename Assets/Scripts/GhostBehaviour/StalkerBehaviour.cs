using UnityEngine;

public class StalkerBehaviour : MonoBehaviour
{
    [SerializeField] bool isDetectable;
    [SerializeField] bool isAudible;
    private GameObject _target;
    private int channel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _target = FindFirstObjectByType<GameManager>().gameObject;

        transform.LookAt(_target.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
