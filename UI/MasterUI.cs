// ======================================================================
//    Land Forgotten : Master Gameplay UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterUI : MonoBehaviour
{
	[SerializeField] private TopDownMinimap _Minimap;
	[SerializeField] private TMPro.TMP_Text _TimerText;
	[SerializeField] private PlayerDiedAnnouncement _DeadAnnouncement;
	
	private bool _IsSetup = false;

	private void Update() 
	{
		if(PlayerNetworkManager.PlayerStats.Count == PhotonNetwork.playerList.Length - 1 && _IsSetup == false)	
		{
			foreach (var player in PlayerNetworkManager.PlayerStats)
			{
				player.OnDeadCounterChange += ShowPlayerDead;
				_IsSetup = true;
			}
		}
	}

	private void ShowPlayerDead(int deadCount, CharacterSelectionType character, bool isMine)
    {
		_DeadAnnouncement.StartCoroutine("PlayerDeadAnnouncement", character);
    }

	// Timer goes here
	public void ShowCurrentTime(float passedTime, float gameDuration)
	{
		float totalTime = gameDuration * 60;
		float currentTime = totalTime - passedTime;
		
		// Timer format 00:00
		_TimerText.text = Mathf.Floor(currentTime / 60).ToString("00") + 
						":" + (currentTime % 60).ToString("00");
	}

	public void Reset()
	{
		_IsSetup = false;
		_DeadAnnouncement.Reset();
	}

}
