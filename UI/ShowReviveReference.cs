// ======================================================================
//    Land Forgotten : Show Revive Reference Diegetic UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowReviveReference : MonoBehaviour {

	[SerializeField] private Revive _ReviveComponent;
	private float _ShowingDistance = 0f;
	private GameObject _ImageReference = null;

	private void Awake() 
	{
		_ShowingDistance =  _ReviveComponent.ViewRadius;
		_ImageReference	= transform.GetChild(0).gameObject;
	}
	
	void Update ()
	{
		if(PlayerNetworkManager.PlayerStats.Count <= 0)
			return;

		foreach (var player in PlayerNetworkManager.PlayerStats)
		{
			if(player != transform.GetComponentInParent<Health>())
			{
				float distance = Vector3.Distance(this.transform.position, player.transform.position);

				if(distance <= _ShowingDistance)
				{
					if(player.CurrentHealth <= 0f)
						_ImageReference.SetActive(true);
					else
						_ImageReference.SetActive(false);

				}
				else
					_ImageReference.SetActive(false);

			}
			else
				_ImageReference.SetActive(false);
		}

	}
}
