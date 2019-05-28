// ======================================================================
//    Land Forgotten : Minimap UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopDownMinimap : MonoBehaviour 
{
	[SerializeField] private GameObject _MasterLocator;
	[SerializeField] private GameObject _HeroIconPrefabs;
	
	[Header("Sprites")]
	[SerializeField] private Sprite[] _CharacterIcons;
	[SerializeField] private Sprite _ClayIcon;
	[SerializeField] private Sprite[] _CPIcon;
	[SerializeField] private Sprite _PingIcon;
	[SerializeField] private Sprite _CrossIcon;
	[SerializeField] private Sprite _OutlineIcon;
	[SerializeField] private Color _OutlineColor;


	private GameObject[] _HeroIcons;
	private GameObject[] _HeroGO;
	
	// private GameObject[] _CPGO;
	private GameObject[] _CPIcons;
	private GameObject[] _PingIcons;

	[Header("Minimap Icons Offset")]
	[Range(0, 1)]
	[SerializeField] private float _XOffset = 1.0f;
	[Range(0, 1)]
	[SerializeField] private float _YOffset = 1.0f;

	[Header("Master Map Position Offset")]
	[Range(-1, 1)]
	[SerializeField] private float _MMXOffset = 1.0f;
	[Range(-1, 1)]
	[SerializeField] private float _MMYOffset = 1.0f;

	[Header("Master World Position Offset")]
	[Range(-500, 500)]
	[SerializeField] private float _MXOffset = 0.0f;
	[Range(-500, 500)]
	[SerializeField] private float _MZOffset = 0.0f;

	// Minimap and world map measures
	private bool _IsMapReady = false;
	private const float _WWidth = 360f;
	private const float _WHeight = 318f;
	private float _MWidth;
	private float _MHeight;

	private void Start() 
	{
		// Get minimap size
		_MWidth = GetComponent<RectTransform>().rect.width;
		_MHeight = GetComponent<RectTransform>().rect.height;
	}

	public void ResetVariables()
	{
		CleanUpMap();
		_IsMapReady = false;
		_HeroIcons = null;
		_HeroGO = null;
		_CPIcons = null;
		_PingIcons = null;
	}

	private void Update() 
	{
		// Create player icons until all players are all ready in game
		if(PlayerNetworkManager.PlayerStats.Count == PhotonNetwork.playerList.Length - 1 && _IsMapReady == false && PlayerNetworkManager.PlayerStats.Count>0)
			PopulateMap();

			// Update players position until their icons are crated
		if(_IsMapReady == true)
		{
			// Update the players position in the map
			for (int i = 0; i < _HeroGO.Length; i++)
			{
				// TODO: If null change sprite to red cross
				if(_HeroGO[i] != null)
				{
					Vector3 newPos = WorldToMapCoordinates(_HeroGO[i].transform.position);
					UpdateIconPosition(newPos, _HeroIcons[i]);
					UpdateIconRotation(_HeroGO[i], _HeroIcons[i]);
				}
			}

			// Update the CP condition
			for (int i = 0; i < ControllPointPing.ControlPointList.Count; i++)
			{
				if(ControllPointPing.ControlPointList[i] == null)
				{
					_CPIcons[i].GetComponent<Image>().sprite = _CPIcon[1];
				}
			}

			if(PhotonNetwork.isMasterClient == true)
				ShowMasterLocationOnMinimap();
		}
	}

	private Vector3 WorldToMapCoordinates(Vector3 playerPos)
	{
		float posX = (playerPos.x * _MWidth)/_WWidth;
		float posY = (playerPos.z * _MHeight)/_WHeight;
		// Debug.LogFormat("X {0}, Y {1}", posX, posY);

		return new Vector3(posX, posY, 0f);
	}

	private Vector3 MapToWolrdCoordinates(Vector2 clickPos)
	{
		float posX = (clickPos.x * _WWidth)/_MWidth;
		float posY = Camera.main.transform.position.y;
		float posZ = (clickPos.y * _WHeight)/_MHeight;

		return new Vector3(posX + _MXOffset, posY, posZ + _MZOffset);
	}

	private void ShowMasterLocationOnMinimap()
	{
		float xOffset = _MWidth * _MMXOffset;
		float yOffset = _MHeight * _MMYOffset;
		Vector3 masterNewPos = WorldToMapCoordinates(Camera.main.transform.position);
		_MasterLocator.transform.localPosition = new Vector3(masterNewPos.x + xOffset, masterNewPos.y + yOffset, 0f);
	}

	private void PopulateMap()
	{
		// Find players and control points
		// FIXME: Use static lists for this
		_HeroGO = GameObject.FindGameObjectsWithTag("Player");
		// _CPGO = GameObject.FindGameObjectsWithTag("ControlPoint");
		
		// Equal the lenght of the icons to the totall amount of heroes in the game
		_HeroIcons = new GameObject[_HeroGO.Length];
		_CPIcons = new GameObject[ControllPointPing.ControlPointList.Count];
		_PingIcons = new GameObject[_CPIcons.Length];

		// Add one icon per player
		for(int i = 0; i < _HeroIcons.Length; i ++)
		{
			GameObject currentIcon = _HeroIconPrefabs;

			var pm = _HeroGO[i].GetComponent<PlayerMovement>();
			var currentImage = currentIcon.GetComponent<Image>();
			int charSelect = (int)pm._PlayerCharacter;

			
			if(pm._PhotonView.isMine == true)
			{
				currentImage.sprite = _OutlineIcon;
				currentImage.color = _OutlineColor;
				
				_HeroIcons[i] = Instantiate(currentIcon, Vector3.zero, Quaternion.identity);
				_HeroIcons[i].transform.SetParent(transform, false);
				_HeroIcons[i].transform.GetChild(1).GetComponent<Image>().sprite = _CharacterIcons[charSelect];
			}
			else
			{
				currentImage.color = Color.white;
				currentImage.sprite = _CharacterIcons[charSelect];

				_HeroIcons[i] = Instantiate(currentIcon, Vector3.zero, Quaternion.identity);
				_HeroIcons[i].transform.SetParent(transform, false);
				Destroy(_HeroIcons[i].transform.GetChild(1).gameObject);
			}
		}
		
		// Add one icon per control point
		for (int i = 0; i < _CPIcons.Length; i++)
		{
			// Debug.Log("Number of Control points: " + _CPIcons.Length);
			GameObject currentIcon = _HeroIconPrefabs;
			currentIcon.name = "ControlPoint_Icon_" + (i + 1);
			currentIcon.GetComponent<Image>().sprite = _CPIcon[0];
			currentIcon.GetComponent<Image>().color = Color.white;

			Vector3 pos = WorldToMapCoordinates(currentIcon.transform.position);
			
			_CPIcons[i] = Instantiate(currentIcon, pos, Quaternion.identity);
			_CPIcons[i].transform.SetParent(transform, false);

			foreach (Transform child in _CPIcons[i].transform) 
			{
				GameObject.Destroy(child.gameObject);
			}

			// Set the correct position
			Vector3 worldToMap = WorldToMapCoordinates(ControllPointPing.ControlPointList[i].transform.position);
			UpdateIconPosition(worldToMap, _CPIcons[i]);

			GameObject combatIcon = _HeroIconPrefabs;
			combatIcon.name = "Combat_Icon_" + (i + 1);
			combatIcon.GetComponent<Image>().sprite = _PingIcon;

			_PingIcons[i] = Instantiate(combatIcon, pos, Quaternion.identity);
			_PingIcons[i].transform.SetParent(transform, false);
			foreach (Transform child in _PingIcons[i].transform) 
			{
				GameObject.Destroy(child.gameObject);
			}

			_PingIcons[i].SetActive(false);

			UpdateIconPosition(worldToMap, _PingIcons[i]);
		}

		_IsMapReady = true;
	}

	private void CleanUpMap()
	{
		foreach (Transform child in this.transform) 
		{
			if(child.name != "Minimap")
				GameObject.Destroy(child.gameObject);
		}
	}

	private void UpdateIconPosition(Vector3 newPos, GameObject icon)
	{
		float xOffset = _MWidth * _XOffset;
		float yOffset = _MHeight * _YOffset;
		// Add offset to player icons
		Vector3 finalPos = new Vector3(newPos.x + xOffset, newPos.y + yOffset, 0f);
		icon.transform.localPosition =  finalPos;
		
	}

	private void UpdateIconRotation(GameObject player, GameObject icon)
	{
        // Debug.Log("Rotate icon on minimap?");
		icon.transform.GetChild(0).GetComponent<RectTransform>().transform.rotation = Quaternion.Euler(0f, 0f, - player.transform.rotation.eulerAngles.y);
	}

	public void ActivatePing(GameObject cp, bool isBlinking)
	{
		for (int i = 0; i < ControllPointPing.ControlPointList.Count; i++)
		{
			if(cp == null)
			{
				_PingIcons[i].SetActive(false);
				return;
			}
			if(ControllPointPing.ControlPointList[i] == cp)
			{
				_PingIcons[i].SetActive(isBlinking);
				return;
			}
		}

	}

	public void ControlPointDestroyed(GameObject cp)
	{
		for (int i = 0; i < ControllPointPing.ControlPointList.Count; i++)
		{
			if(ControllPointPing.ControlPointList[i] == cp)
			{
				_PingIcons[i].GetComponent<Image>().sprite = _CrossIcon;
				return;
			}
		}
	}

	public void ChangeMasterPosisiton(Vector2 cliclPos)
	{
		// var master = Camera.main.gameObject.transform;
		var master = CameraFinder.Get();
		var masterObj = master.GetComponentInParent<MasterController>();
		Vector3 Pos =	MapToWolrdCoordinates(cliclPos);	
		masterObj.SetPos(Pos); 
	}
}

// public static class CameraTransform
// {
// 	private static Transform CamTransform;
// 	public static Transform Get()
// 	{
// 		if(CamTransform == null)
// 		{
// 			CamTransform = Camera.main.transform;
// 		}
// 		return CamTransform;
// 	}
// }
