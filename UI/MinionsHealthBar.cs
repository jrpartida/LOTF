// ======================================================================
//    Land Forgotten : Minons Health Bar UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================
using UnityEngine;
using UnityEngine.UI;

public class MinionsHealthBar : MonoBehaviour {

	[SerializeField] private GameObject _Character;
	private Health _MyHealth;
	private bool _IsShown = false;

	private void Start() 
	{
		_MyHealth = transform.GetComponentInParent<Health>();

		_MyHealth.OnHealthChange += ShowHealthBar;
	}

	private void ShowHealthBar(float health, bool isHealing)
	{
		if(_IsShown == false && isHealing == false)
		{
			//Debug.Log("Health Value: " + health);
			//Debug.Log("IsHealing Value: " + isHealing);
			
			_IsShown = true;
			transform.GetChild(0).gameObject.SetActive(true);
		}
			
	}
}
