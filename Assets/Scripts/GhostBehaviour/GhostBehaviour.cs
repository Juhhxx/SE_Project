using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;

public class StalkerBehaviour : MonoBehaviour
{
    [SerializeField] bool isDetectable;
    [SerializeField] bool isAudible;
    [SerializeField] List<AudioClip> audioClips;
    [SerializeField] int audioPlayProb;
    [SerializeField] int audioCooldown;
    [SerializeField] private float _speed;
    [SerializeField] private int _class;
    private GameObject _target;
    private AudioSource audioSource;
    private Timer timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        _target = FindFirstObjectByType<GameManager>().gameObject;

        transform.LookAt(_target.transform);



        if (isAudible)
        {
            StartCoroutine(PlayAudio());
        }
    }

    // Update is called once per frame
    void Update()
    {
        Behaviour(_class);
    }

    private void OnEnable()
    {
        if (_class == 4)
        {
            timer = new Timer(15, Timer.TimerReset.Manual);
        }
    }
    private void OnDisable()
    {
    }
    void Behaviour(int ghostClass)
    {
        Move();
        if (ghostClass == 4)
        {
            timer.OnTimerDone += KillPlayer;
        }
    }

    private IEnumerator PlayAudio()
    {
        while (true)
        {
            int willPlayProb = Random.Range(0, 101);

            if (audioClips.Count == 0) yield break;

            yield return new WaitForSeconds(audioCooldown);
            audioSource.clip = audioClips[Random.Range(0, audioClips.Count)];
            if (willPlayProb <= audioPlayProb)
            {
                audioSource.Play();
            }
        }
    }

    private void Move()
    {
        Vector3 moveVec = transform.position;

        moveVec += transform.forward * _speed * Time.deltaTime;

        transform.position = moveVec;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _target) KillPlayer();
    }

    private void KillPlayer()
    {
        //Mandar sinais para o mootor pa vibrar de acordo
        //Play audio clip do narrador ?
    }
}
