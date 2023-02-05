using MonkeyMonk.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player Player { get => monkey; }

    [SerializeField] private Player monkey;

    private Vector3 _respawnPoint;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _respawnPoint = monkey.transform.position;
    }

    public void RespawnMonkey()
    {
        monkey.gameObject.transform.position = _respawnPoint;
        monkey.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        _respawnPoint = newRespawnPoint;
    }
}
