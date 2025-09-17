using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [Space(5f)]
    [SerializeField] private float _updateRate;

    [Space(10f)]
    [Header("Ghost Spawning")]
    [Space(5f)]
    [SerializeField] private List<GameObject> _ghostPrefabs = new List<GameObject>();
    [SerializeField][MinMaxSlider(5, 100)] private Vector2 _spawningRangeRadius;
    [SerializeField][MinMaxSlider(1, 10)] private Vector2 _spawningRangeTime;

    private PlayerRotation _playerRot;
    private PlayerCamera _playerCam;

    private Timer _timer;

    private void Start()
    {
        _timer = new Timer(_updateRate);

        _playerRot = GetComponent<PlayerRotation>();
        _playerCam = GetComponent<PlayerCamera>();

        StartCoroutine(SpawnGhosts());
    }
    private void OnEnable()
    {
        _timer.OnTimerDone += UpdateGame;
    }
    private void OnDisable()
    {
        _timer.OnTimerDone -= UpdateGame;
    }

    private void UpdateGame()
    {
        /***********************************************************************
        **  The message sent will always be a 8 byte array in which:          **
        **                                                                    **
        **  The first 8 bytes represent what should be displayed on the       **
        **  LED 8x8 matrix   (Each byte represents a line of the matrix       **
        **  so that 0 is off and 1 is on).                                    **
        **                                                                    **
        ***********************************************************************/

        byte[] radar = GhostIndicator.Instance.DrawRadar();

        List<byte> message = new List<byte>();

        message.AddRange(radar);

        /***********************************************************************
        **  The message recieved will always be a 4 byte array in which:      **
        **                                                                    **
        **           message = [ 0-180 ,  0-1 ,  0-1 ,  0-3 ]                 **
        **                                                                    **
        **  The first 2 bytes represent the player's rotation, 0-180 being    **
        **  the amount of the rotation and 0-1 being it's direction.          **
        **                                                                    **
        **  The 3rd byte represents if the player has pressed the Flash.      **
        **                                                                    **
        **  The 4rth byte represents the channel to player is tuned into.     **
        **                                                                    **
        ***********************************************************************/

        (byte[] response, _) = UDPComunication.Instance.SendMessage(message.ToArray());

        _playerRot.SendRotation(response[0], response[1]);

        if (response?[3] == 1) _playerCam.Flash();

        GhostIndicator.Instance.SetChannel(response[4]);
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
