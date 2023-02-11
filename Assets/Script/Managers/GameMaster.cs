using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster Instance { get; private set; }

    public int LifeLeft { get => Save.lifes; }

    [SerializeField] private GameSave Save;

    private void Awake()
    {
        Instance = this;
    }

    public void RemoveLife()
    {
        Save.lifes--;
    }

    public bool HasLifeLeft()
    {
        return Save.lifes > 0;
    }

    public void GiveLives()
    {
        Save.lifes = 5;
    }


}

[System.Serializable]
public class GameSave // temporary
{
    public int lifes = 5;
}