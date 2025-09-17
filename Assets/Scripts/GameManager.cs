using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Antigo serialized field de qnd tinhamos apenas um tipo de fantasma
    //[SerializeField] private GameObject _ghostPrefab; 
    [SerializeField] private List<GameObject> _ghostPrefabs = new List<GameObject>();

    [SerializeField][MinMaxSlider(5, 100)] private Vector2 _spawningRangeRadius;
    [SerializeField][MinMaxSlider(1, 10)] private Vector2 _spawningRangeTime;

    private void Start()
    {
        StartCoroutine(SpawnGhosts());
    }

    private IEnumerator SpawnGhosts()
    {
        while (true)
        {
            float waitTime = Random.Range(_spawningRangeTime.x, _spawningRangeTime.y);

            Debug.Log($"WAIT {waitTime}");

            yield return new WaitForSeconds(waitTime);

            Vector3 pos = ChooseLocation();

            int gType = Random.Range(0, _ghostPrefabs.Count);

            GameObject g = Instantiate(_ghostPrefabs[gType], pos, Quaternion.identity);

            GhostIndicator.Instance.AddGhost(g.transform);

            Debug.Log($"SPAWNED GHOST AT {pos}");
        }
    }

    private Vector3 ChooseLocation()
    {
        Vector3 dir = Random.insideUnitCircle;
        dir.z = dir.y;
        dir.y = 0;

        float dist = _spawningRangeRadius.x + Random.Range(0, _spawningRangeRadius.y - _spawningRangeRadius.x);

        return dir * dist;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, _spawningRangeRadius.x-1);
        Gizmos.DrawWireSphere(transform.position, _spawningRangeRadius.y-1);
    }
}
