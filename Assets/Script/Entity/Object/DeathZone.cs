using MonkeyMonk.Enemies;
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
            player.DamageWithoutInvulnerability(999);
            
            if (!player.IsDead)
            {
                GameManager.Instance.RespawnMonkey();
            }
        }
        else if (other.TryGetComponent(out Enemy entity))
        {
            entity.DamageWithoutInvulnerability(999);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (TryGetComponent(out BoxCollider collider))
        {
            Gizmos.color = new Color(255, 0, 0, 0.5f);
            Gizmos.DrawCube(transform.position + collider.center, new Vector3(collider.size.x * transform.lossyScale.x, collider.size.y * transform.lossyScale.y, collider.size.z * transform.lossyScale.z));
        }
    }
#endif
}