using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MonkeyMonk.Map
{
    public abstract class Node : MonoBehaviour
    {
        [Header("Node paths")]

        [SerializeField] private NodePath northPath;
        [SerializeField] private NodePath southPath;
        [SerializeField] private NodePath westPath;
        [SerializeField] private NodePath eastPath;

        public NodePath GetPath(Vector2 dir)
        {
            float a = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) + 45;

            if (a < 0) return southPath;
            if (a < 90) return eastPath;
            if (a < 180) return northPath;
            if (a < 270) return westPath;

            return null;
        }

        public virtual void OnClick(PlayerMapMovement origin)
        {

        }
    }
}