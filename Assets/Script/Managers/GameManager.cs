using MonkeyMonk.Enemies;
using MonkeyMonk.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player Player { get => monkey; }

    public event Action OnLoseEvent;

    [SerializeField] private Player monkey;

    private Vector3 _respawnPoint;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _respawnPoint = monkey.transform.position;
        Player.OnDeathEvent += OnPlayerDeath;
    }

    public void OnPlayerDeath()
    {
        GameMaster.Instance.RemoveLife();

        if (!GameMaster.Instance.HasLifeLeft())
        {
            OnLoseEvent?.Invoke();
        }
        else
        {
            StartCoroutine(PlayerLifeLoss());
        }
    }

    private IEnumerator PlayerLifeLoss()
    {
        yield return LoadingScreen.Instance.FadeIn(0.3f, LoadingScreen.TransitionType.Round);

        // Show Life
        yield return new WaitForSeconds(0.3f);

        foreach (var enemySp in FindObjectsOfType<EnemySpawner>()) // TODO : Opti
        {
            enemySp.Reset();
        }

        RespawnMonkey();
        yield return LoadingScreen.Instance.FadeOut(0.3f, LoadingScreen.TransitionType.Round);
    }

    public void RespawnMonkey()
    {
        monkey.gameObject.transform.position = _respawnPoint;
        monkey.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        monkey.Reset();
        monkey.gameObject.SetActive(true);
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        _respawnPoint = newRespawnPoint;
    }
}
