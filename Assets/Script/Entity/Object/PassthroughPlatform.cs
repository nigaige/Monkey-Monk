using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PassthroughPlatform : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    private void FixedUpdate()
    {
        if (_player.GetComponent<Collider>().bounds.min.y < GetComponent<Collider>().bounds.max.y - 0.1f)
        {
            Physics.IgnoreCollision(_player.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }
        else
        {
            Physics.IgnoreCollision(_player.GetComponent<Collider>(), GetComponent<Collider>(), false);
        }
    }
}
