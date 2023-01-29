using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Vector3 _respawnPoint;

    [SerializeField] GameObject monkey;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _respawnPoint = monkey.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
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
