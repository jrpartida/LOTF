// ======================================================================
//    Land Forgotten : Hero Gameplay UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroesUI : MonoBehaviour
{
    [Header("Player Status UI")]
    [SerializeField] private PlayerStatusUI[] _PlayersStatus;
    [SerializeField] private PlayerDiedAnnouncement _DeadAnnouncement;
    [Header("Timer UI")]
    [SerializeField] private TMPro.TMP_Text _TimerText;
    [Header("Time Events")]
    [SerializeField] private GameObject _RollCooldownIcon;
    [SerializeField] private GameObject _ReviveCooldown;
    [SerializeField] private GameObject _BleedOutTimer;
    [SerializeField] private TMPro.TMP_Text _ScoreTxt;
    [SerializeField] private Animator _ScoreAnim;

    private float _Score;
    private bool _IsSetupReady = false;
    private bool _StaminaRunning = false;
    private PlayerMovement _MyPlayerMovement;

    private GameObject _MyPlayer;
    public GameObject MyPlayer
    {
        get { return _MyPlayer; }
        private set { _MyPlayer = value; }
    }

    public void ResetVariables()
    {
        // Debug.Log("HEROES UI Reset variableeeeeeeees!");
        _IsSetupReady = false;
        _StaminaRunning = false;
        _BleedOutTimer.SetActive(false);
        _DeadAnnouncement.Reset();
    }

    private void OnEnable()
    {
        
        _ScoreTxt.text = "Score: 0" ;
        _Score = 0;
        AkSoundEngine.PostEvent("Play_Llorona_Intro", gameObject);
    }

    private void Update()
    {
        // Debug.Log("HEROES UI PlayerStats count: "+PlayerNetworkManager.PlayerStats.Count + " PhotonNetwork player list:"+ PhotonNetwork.playerList.Length + " isSetupReady: "+_IsSetupReady);
        if (PlayerNetworkManager.PlayerStats.Count == PhotonNetwork.playerList.Length - 1 && _IsSetupReady == false && PlayerNetworkManager.PlayerStats.Count>0)
        {
            InitialSetup();
            foreach (var player in PlayerNetworkManager.PlayerStats)
            {
                if (player.GetComponent<PlayerMovement>()._PhotonView.isMine)
                {
                    _MyPlayer = player.gameObject;
                }
            }
        }

        if (_IsSetupReady == true && MyPlayer != null)
        {
            ShowDashCooldown();
        }

    }
    
    #region Setup
        private void InitialSetup()
        {
            // Debug.Log("INITIAL SETUP!!!");
            // Subscribre to health change event
            for (int i = 0; i < PlayerNetworkManager.PlayerStats.Count; i++)
            {
                //  Debug.Log("INITIAL SETUP: for: "+i);
                PlayerNetworkManager.PlayerStats[i].OnDeadCounterChange += ShowPlayerDead;

                if (PlayerNetworkManager.PlayerStats[i].GetComponent<PlayerMovement>().OwnerID == PhotonNetwork.player.ID)
                {
                    // Debug.Log("INITIAL SETUP: subscribe health ");
                    // Firt position in UI should be my local player
                    PlayerNetworkManager.PlayerStats[i].OnHealthChange += _PlayersStatus[0].UpdateHealthMeter;
                    //ph.OnDeadCounterChange += _PlayersStatus[0].UpdateDeadCounter;
                    _PlayersStatus[0].ResetHealthBars();
                    _PlayersStatus[0].SetCharacterIcons(PlayerNetworkManager.PlayerStats[i].GetComponent<PlayerMovement>()._PlayerCharacter);
                    _PlayersStatus[0].MyCurrentCharacter = PlayerNetworkManager.PlayerStats[i].gameObject;
                    _PlayersStatus[0].PlayerInitialHealth = PlayerNetworkManager.PlayerStats[i].InitialHealth;
                    
                }
                // others players order doesn't matter
                // feel like i should add i + 1 or something
                PlayerNetworkManager.PlayerStats[i].OnHealthChange += _PlayersStatus[i].UpdateHealthMeter;
                //ph.OnDeadCounterChange += _PlayersStatus[i].UpdateDeadCounter;
                _PlayersStatus[i].ResetHealthBars();
                _PlayersStatus[i].SetCharacterIcons(PlayerNetworkManager.PlayerStats[i].GetComponent<PlayerMovement>()._PlayerCharacter);
                _PlayersStatus[i].MyCurrentCharacter = PlayerNetworkManager.PlayerStats[i].gameObject;
                _PlayersStatus[i].PlayerInitialHealth = PlayerNetworkManager.PlayerStats[i].InitialHealth;
            
                _IsSetupReady = true;
                // Debug.Log("Setup Ready");
            }

        }
        
    #endregion


    public void ShowCurrentTime(float passedTime, float gameDuration)
    {
        float totalTime = gameDuration * 60;
        float currentTime = totalTime - passedTime;

        // Timer format 00:00
        _TimerText.text = Mathf.Floor(currentTime / 60).ToString("00") +
                        ":" + (currentTime % 60).ToString("00");
    }

    public void ShowReviveCooldow(bool show)
    {
        _ReviveCooldown.SetActive(show);
    }

    public void ShowBleedOutTimer(bool show, Revive player)
    {
        if(player.gameObject != MyPlayer) return;
        
        _BleedOutTimer.SetActive(show);
        _BleedOutTimer.GetComponent<BleedOutTimer>().Player = player;
    }

    private void ShowPlayerDead(int deadCount, CharacterSelectionType character, bool isMine)
    {
        if(isMine == false)
        {
            _DeadAnnouncement.StartCoroutine("PlayerDeadAnnouncement", character);
        }
    }

    private void ShowDashCooldown()
    {
        float percentage = 0f;
        var pm = MyPlayer.GetComponent<PlayerMovement>();
        percentage = ((pm.GetRollCooldown() * 100) / pm.GetRollCooldownTime()) * 0.01f;
        
        float fillAmount = Mathf.Lerp(1f, 0f, percentage);
        var image = _RollCooldownIcon.GetComponentInChildren<Image>();
        var colorTransition = Color.Lerp(Color.red, Color.white, fillAmount);
        

        //Debug.Log("Dash Cooldown: " + fillAmount); // This goes from 1 to zero
        image.fillAmount = fillAmount;
        image.color = colorTransition;

    }

    public void UpdateScore(float value)
    {
        _Score += value;
        _ScoreTxt.text = "Score: " + _Score.ToString();
        _ScoreAnim.SetTrigger("UpdateScore");
    }

}
