// ======================================================================
//    Land Forgotten : Game Screen UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.1
//    Program        : Unity 2018.2.18f1
// ======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : UIScreen
{
    [SerializeField] private GameObject _HeroUI;
    [SerializeField] private GameObject _MasterUI;

    private MasterUI _MUI;
    private HeroesUI _HUI;
    private Timer _GameTimer;

    private void OnEnable()
    {

        Time.timeScale = 1.0f;
        LoadingScreen.IsLoadingScreen = false;

        _MUI = _MasterUI.GetComponent<MasterUI>();
        _HUI = _HeroUI.GetComponent<HeroesUI>();

        _MasterUI.SetActive(false);
        _HeroUI.SetActive(true);

        if (PhotonNetwork.isMasterClient)
        {
            _HeroUI.SetActive(false);
            _MasterUI.SetActive(true);

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {

        if (_GameTimer == null)
            _GameTimer = GameObject.FindObjectOfType<Timer>();

        if (_GameTimer != null)
        {
            _MUI.ShowCurrentTime(_GameTimer.GetCurrentTime(), _GameTimer.GetGameDuration());
            _HUI.ShowCurrentTime(_GameTimer.GetCurrentTime(), _GameTimer.GetGameDuration());
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PhotonNetwork.isMasterClient == false)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            UIScreen.ShowScreen<PauseMenuScreen>();
			
        }
    }

}
