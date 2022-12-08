using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Map/Level", order = 81)]
public class LevelSO : ScriptableObject
{
    [Header("Scene")]
    [SerializeField] private string scene;

    [Header("Info")]
    [SerializeField] private string displayedName;

    public string SceneName { get => scene; }
    public string DisplayedName { get => displayedName; }

}
