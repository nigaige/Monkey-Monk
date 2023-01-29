using MonkeyMonk.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.DamageWithoutInvulnerability(1);
            
            if (!player.IsDead)
            {
                GameManager.Instance.RespawnMonkey();
            }
        }
    }
}