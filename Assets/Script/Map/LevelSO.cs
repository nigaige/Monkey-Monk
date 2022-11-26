using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Map/Level", order = 81)]
public class LevelSO : ScriptableObject
{
    [Header("Scene")]
    [SerializeField] private SceneAsset scene;

    [Header("Info")]
    [SerializeField] private string displayedName;

    public string SceneName { get => scene.name; }
    public string DisplayedName { get => displayedName; }

}
