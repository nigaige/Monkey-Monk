using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonkeyMonk.Map
{
    public class LevelNode : Node
    {
        [Header("Level")]
        [SerializeField] private LevelSO level;
        public LevelSO Level { get => level; }

        public override void OnClick()
        {
            base.OnClick();

            SceneMaster.Instance.LoadLevel(level);
        }

    }
}