using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Vector3 _respawnPoint;

    [SerializeField] GameObject monkey;

    // Start is called before the first frame update
    void Start()
    {
        _respawnPoint = new Vector3(-1.5f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        IsFallingToDeath();
    }

    private void IsFallingToDeath()
    {
        if (monkey.gameObject.transform.position.y < -6)
        {
            RespawnMonkey();
        }
    }

    private void RespawnMonkey()
    {
        monkey.gameObject.transform.position = _respawnPoint;
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        _respawnPoint = newRespawnPoint;
    }
}
