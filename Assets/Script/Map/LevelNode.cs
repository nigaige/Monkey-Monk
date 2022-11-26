using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonkeyMonk.Map
{
    public class LevelNode : Node
    {
        [Header("Level")]
        [SerializeField] private LevelSO level;
        public LevelSO Level { get => level; }

        public override void OnClick(PlayerMapMovement origin)
        {
            base.OnClick(origin);

            SceneMaster.Instance.LoadLevel(level);
            origin.Lock();
        }

    }
}