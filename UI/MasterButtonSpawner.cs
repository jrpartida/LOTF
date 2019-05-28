// ======================================================================
//    Land Forgotten : Master Button Spawner UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================
using UnityEngine;
using UnityEngine.UI;

public class MasterButtonSpawner : MonoBehaviour

{
	[Header("UI Elements")]
	[SerializeField] private TMPro.TMP_Text _ClayCostText;
	[SerializeField] private GameObject _CooldownShadow;
	[Header("Data")]
	[SerializeField] private AbilityData _AbilityData;
	[Header("Tool tip")]
	[SerializeField] private GameObject _TooltipGO;
	[SerializeField] private Vector3 _Offset;

	private AbilityToolTip _AbilityToolTip;
	private MasterObjectSpawner _MasterObjectSpawner;
	private MasterResourcesMeter _MasterResourcesMeter;

	private Image _CDShadow;
	private Vector3 _ToolTipPos;
	private float _CooldownTimer;
	private bool _IsSpawnable;

	private void OnEnable() 
	{
		_ClayCostText.text = _AbilityData.Cost.ToString();
		_IsSpawnable = true;

		if(_CooldownShadow != null)
			_CDShadow = _CooldownShadow.GetComponent<Image>();
		
		if(_AbilityToolTip == null)
		{
			if(_ToolTipPos != null)
				_AbilityToolTip = _TooltipGO.GetComponent<AbilityToolTip>();
		}

	}

	private void Update() 
	{
		if(_MasterObjectSpawner == null)
		{
			_MasterObjectSpawner = FindObjectOfType<MasterObjectSpawner>();
		}

		if(_MasterResourcesMeter == null)
		{
			_MasterResourcesMeter = FindObjectOfType<MasterResourcesMeter>();
		}


		if(_MasterObjectSpawner != null)
		{
			_CooldownTimer = _MasterObjectSpawner.GetCDTimer();
			
			if(_MasterResourcesMeter != null)
			{
				if(_AbilityData.Cost > _MasterResourcesMeter.ClayPool)
					_IsSpawnable = false;
				else
					_IsSpawnable = true;


			}

			if(_IsSpawnable == true)
			{
				GetComponent<Image>().color = Color.white;
				Cooldown();
			}

			if(_IsSpawnable == false)
				GetComponent<Image>().color = Color.red;

			
		}
		

	}

	private void Cooldown()
	{
		if(_CooldownTimer > 0 )
		{
			_CooldownShadow.SetActive(true);
			float percentage = ( (_CooldownTimer * 100 )/ _AbilityData.Cooldown) * 0.01f;
			_CDShadow.fillAmount = percentage;
		}
		else
		{
			_CooldownShadow.SetActive(false);
			_CDShadow.fillAmount = 1;
		}

	}

	public void OnPointerEnterEvent()
	{	
		// Debug.Log("Enter hover");
		// Get the new position Need to find out the correct offset
		_ToolTipPos = new Vector3(0f, 0f, 0f);

		// Activate Tooltip and parent it to myself
		if(_TooltipGO != null)
		{
			_TooltipGO.SetActive(true);
			_TooltipGO.transform.SetParent(this.transform, false);
			_TooltipGO.transform.localPosition = _Offset;//Vector3.zero;
			_TooltipGO.transform.localScale = new Vector3(1f, 1f, 1f);

			// Send new information to display to tooltip
			_AbilityToolTip.ChangeInformation(_AbilityData);
		}
	}

	public void OnPointerExitEvent()
	{
		// Deactivate tooltip and remove parent
		Debug.Log("Exit hover");
		if(_TooltipGO != null)
		{
			_TooltipGO.SetActive(false);
			_TooltipGO.transform.parent = null;
		}
	}

	#region ButtonPresses
		
		public void OnMummyPressed()
		{
			if(_IsSpawnable == true)
				_MasterObjectSpawner.SpawnMummy();
		}

		public void OnGhostPressed()
		{
			if(_IsSpawnable == true)
				_MasterObjectSpawner.SpawnGhost();
		}

		public void OnGolemPressed()
		{
			if(_IsSpawnable == true)
				_MasterObjectSpawner.SpawnGolem();
		}

		public void OnLightingPressed()
		{
			if(_IsSpawnable == true)
				_MasterObjectSpawner.SpawnLighting();
		}

	}
	#endregion
