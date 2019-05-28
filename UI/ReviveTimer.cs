// ======================================================================
//    Land Forgotten : Revive Timer UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================
using UnityEngine;
using UnityEngine.UI;

public class ReviveTimer : MonoBehaviour 
{
	[SerializeField] private Image _Meter;
	[SerializeField] private TMPro.TMP_Text _Counter;

	private Revive _Player;

	private void OnEnable() 
	{
		_Player = gameObject.GetComponentInParent<HeroesUI>().MyPlayer.GetComponent<Revive>();
	}

	private void Update() 
	{
		_Meter.fillAmount = ( (_Player.GetCooldown() * 100 )/ _Player.GetReviveTime()) * 0.01f;
		_Counter.text = _Player.GetCooldown() <0.0f? "0.0" : _Player.GetCooldown().ToString("F2");
	}


}
