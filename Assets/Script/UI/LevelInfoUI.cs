using MonkeyMonk.Map;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelInfoUI : MonoBehaviour
{
    [SerializeField] private PlayerMapMovement player;
    [SerializeField] private TMP_Text levelName;

    private void OnEnable()
    {
        if (player != null) player.OnNodeChangeEvent += OnNodeSelected;
    }

    private void OnDisable()
    {
        if (player != null) player.OnNodeChangeEvent -= OnNodeSelected;
    }

    private void OnNodeSelected(Node node)
    {
        LevelNode levelNode = node as LevelNode;

        if (levelNode == null || levelNode.Level == null)
        {
            levelName.text = "";
            return;
        }

        levelName.text = levelNode.Level.DisplayedName;
    }

}
