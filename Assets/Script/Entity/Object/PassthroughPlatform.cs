using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using MonkeyMonk.Player;

public class PassthroughPlatform : MonoBehaviour
{
    private Player _player;

    private Coroutine _ignoreCoroutine;

    private void Awake()
    {
        _player = FindObjectOfType<Player>(); // TODO : Opti this
    }

    private void FixedUpdate()
    {
        if (_ignoreCoroutine != null) return;
        if (_player == null) return;

        if (_player.GetComponent<Collider>().bounds.min.y < GetComponent<Collider>().bounds.max.y - 0.1f)
        {
            Physics.IgnoreCollision(_player.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }
        else
        {
            Physics.IgnoreCollision(_player.GetComponent<Collider>(), GetComponent<Collider>(), false);
        }
    }

    public void IgnorePlayerForSeconds(float seconds)
    {
        _ignoreCoroutine = StartCoroutine(IgnorePlayerForSecondsCoroutine(seconds));
    }

    private IEnumerator IgnorePlayerForSecondsCoroutine(float seconds)
    {
        if (_player != null) Physics.IgnoreCollision(_player.GetComponent<Collider>(), GetComponent<Collider>(), true);
        yield return new WaitForSeconds(seconds);
        if (_player != null) Physics.IgnoreCollision(_player.GetComponent<Collider>(), GetComponent<Collider>(), false);
        _ignoreCoroutine = null;
    }

}
