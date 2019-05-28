// ======================================================================
//    Land Forgotten : Master Resources Meter UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MasterResourcesMeter : MonoBehaviour
{
    [SerializeField] private Image _ClayMeter;
    [SerializeField] private Image _DelayedClayMeter;
    [SerializeField] private TMPro.TMP_Text _MeterAmount;
    [SerializeField] private FloatVar _ClayPool;

    public float ClayPool
    {
        get { return _ClayPool.Value; }
        private set { _ClayPool.Value = value; }
    }

	private Color _OriginalClayBarColor;
    private float _MaxClayPool = 0;
    private float _MeterPercetage = 1f;
    private bool _IsCRRunning = false;

    private void Awake()
    {
        _IsCRRunning = false;
        _MaxClayPool = MasterResourceController.InitialClayPool;
		_OriginalClayBarColor = _ClayMeter.color;
    }

    private void Update()
    {
        _MeterPercetage = ((_ClayPool.Value * 100) / _MaxClayPool) * 0.01f;
        ChangeMeter();
    }

    private void ChangeMeter()
    {
        // Add some pretty transition when decreased or filled
        _MeterAmount.text = Mathf.Floor(_ClayPool.Value).ToString() + "/" + _MaxClayPool.ToString();
        _ClayMeter.fillAmount = _MeterPercetage;
        float valueToSend = _DelayedClayMeter.fillAmount - _MeterPercetage;
        
        if(_IsCRRunning == false)
            StartCoroutine("UpdateDelayedMeter", 0);
        
        if (_MeterPercetage < 0.25f)
        {
			_ClayMeter.color = Color.red;
        }
		else
		{
			_ClayMeter.color = _OriginalClayBarColor;
		}
    }

    private IEnumerator UpdateDelayedMeter(float amount)
    {
        _IsCRRunning = true;
        float delayedPercentage = _DelayedClayMeter.fillAmount;
        float timer = 3f;

        while(timer >= 0f)
        {
            float value = Mathf.Lerp(_MeterPercetage, delayedPercentage, timer);
            _DelayedClayMeter.fillAmount = value;
            timer -= Time.deltaTime;
            yield return null;

        }

        _IsCRRunning = false;
        yield return null;
    }

}
