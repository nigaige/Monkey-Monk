using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuMaster : MonoBehaviour
{
    [SerializeField] private List<Menu> menus;

    private Queue<Menu> _menusQueue = new(); 

    private Menu _currentMenu;

    protected virtual void Awake()
    {
        if (menus.Count == 0) return;
        _currentMenu = menus[0];
    }

    protected virtual void ResetMenu()
    {
        if (menus.Count == 0) return;
        
        _currentMenu = menus[0];
        _currentMenu.OpenMenu();
        for (int i = 1; i < menus.Count; i++)
        {
            menus[i].CloseMenu();
        }
        _menusQueue.Clear();
    }

    public void SwitchMenu(Menu menu)
    {
        _menusQueue.Enqueue(_currentMenu);

        _currentMenu.CloseMenu();
        _currentMenu = menu;
        _currentMenu.OpenMenu();
    }

    public void BackMenu()
    {
        Menu last = _menusQueue.Dequeue();

        _currentMenu.CloseMenu();
        _currentMenu = last;
        _currentMenu.OpenMenu();
    }

}
