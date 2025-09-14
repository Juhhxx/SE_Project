using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _ghostPrefab;

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

            Instantiate(_ghostPrefab, pos, Quaternion.identity);

            Debug.Log($"SPAWNED GHOST AT {pos}");
        }
    }

    private Vector3 ChooseLocation()
    {
        Vector3 dir = Random.insideUnitCircle;
        dir.z = dir.y;
        dir.y = 0;

        float dist = Random.Range(_spawningRangeRadius.x, _spawningRangeRadius.y);

        return dir * dist;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, _spawningRangeRadius.x-1);
        Gizmos.DrawWireSphere(transform.position, _spawningRangeRadius.y-1);
    }
}
