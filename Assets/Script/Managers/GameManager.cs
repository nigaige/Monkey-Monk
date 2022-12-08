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
        _respawnPoint = new Vector3(-4.83f, -3.2f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        IsFallingToDeath();
    }

    private void IsFallingToDeath()
    {
        if (monkey.gameObject.transform.position.y < -4)
        {
            RespawnMonkey();
        }
    }

    private void RespawnMonkey()
    {
        monkey.gameObject.transform.position = _respawnPoint;
        monkey.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        _respawnPoint = newRespawnPoint;
    }
}
