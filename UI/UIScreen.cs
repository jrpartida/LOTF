using System;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public static Dictionary<Type, UIScreen> Screens = new Dictionary<Type, UIScreen>();
    private static UIScreen _CurrentScreen;

    private void Awake()
    {
        AddScreen(this);
        gameObject.SetActive(false);
    }

    private void OnDestroy() 
    {
        Screens.Clear();
        _CurrentScreen = null;
    }

    public void OnPressedReturn()
    {
        ShowScreen<SplashScreen>();
    }

    private static void AddScreen(UIScreen screen)
    {
        if (Screens.ContainsKey(screen.GetType()) == false)
        {
            Screens.Add(screen.GetType(), screen);
        }
    }

    public static void ShowScreen<T>() where T : UIScreen
    {
        if (_CurrentScreen != null)
        {
            _CurrentScreen.gameObject.SetActive(false);
        }

        var tOff = typeof(T);
        if (Screens.ContainsKey(tOff) == false)
        {
            return;
        }
        _CurrentScreen = Screens[tOff];
        _CurrentScreen.gameObject.SetActive(true);
    }

    public static void ShowScreen(Type tOf)
    {
        if (_CurrentScreen != null)
        {
            _CurrentScreen.gameObject.SetActive(false);
        }

        if (Screens.ContainsKey(tOf) == false)
        {
            return;
        }
        _CurrentScreen = Screens[tOf];
        _CurrentScreen.gameObject.SetActive(true);
    }

}
