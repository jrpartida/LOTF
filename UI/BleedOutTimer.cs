// ======================================================================
//    Land Forgotten : Bleed Out Timer UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BleedOutTimer : MonoBehaviour 
{

	private Image _FillImage = null;
	private Revive _Player;

	public Revive Player
	{
		get { return _Player; }
		set { _Player = value; }
	}

	private void OnEnable() 
	{
		_FillImage = GetComponentInChildren<Image>();
	}

	private void Update() 
	{
		if(Player == null) 
		{
			_FillImage.fillAmount = 1f;
			return;
		}
		
		_FillImage.fillAmount = ( (Player.TimeLeftBeforeDie * 100 )/ Player.TimeBeforeDie) * 0.01f;
	}
}
