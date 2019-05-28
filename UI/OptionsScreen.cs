// ======================================================================
//    Land Forgotten : Options Screen UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.1
//    Program        : Unity 2018.2.18f1
// ======================================================================

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class OptionsScreen : UIScreen 
{
	[Header("Tabs")]
	[SerializeField]
	private Button _VideoButton;
	[SerializeField]
	private Button _AudioButton;
	[SerializeField]
	private GameObject _Video;
	[SerializeField]
	private GameObject _Audio;

	[Header("Video Tab")]
	[SerializeField]
	private Toggle _FullScreen;
	[SerializeField]
	private TMPro.TMP_Dropdown _Resolution;
	[SerializeField]
	private Slider _Brightness;

	[Header("Audio Tab")]
	[SerializeField]
	private Slider _Master;
	[SerializeField]
	private Slider _SFX;
	[SerializeField]
	private Slider _Music;
	[SerializeField]
	private Slider _Voice;
	[SerializeField]
	private PostProcessProfile _PPP;

	private Resolution[] _AvailableResolutions;
	private ColorGrading _ColorGrading = null;

	private void OnEnable() 
	{
		// PostProcessVolume volume = Camera.main.GetComponent<PostProcessVolume>();
		_PPP.TryGetSettings(out _ColorGrading);

		_Video.SetActive(true);
		_Audio.SetActive(false);

		_FullScreen.isOn = Screen.fullScreen;

		_FullScreen.onValueChanged.AddListener(delegate { OnFullScreenToggle(); });
		_Resolution.onValueChanged.AddListener(delegate { OnResolutionChange(); });
		_Brightness.onValueChanged.AddListener(delegate { OnBrightnessChange(); });
		_Master.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
		_SFX.onValueChanged.AddListener(delegate { OnSFXVolumeChange(); });

		_AvailableResolutions = Screen.resolutions;
		
		LoadResolutions();
		SetInitialVolumeValues();
	}

    private void OnBrightnessChange()
    {
		_ColorGrading.brightness.value = _Brightness.value;
    }

    private void OnResolutionChange()
    {
		Screen.SetResolution(_AvailableResolutions[_Resolution.value].width, _AvailableResolutions[_Resolution.value].height, Screen.fullScreen);
    }

    private void OnFullScreenToggle()
    {
        Screen.fullScreen = _FullScreen.isOn;
    }

	private void LoadResolutions()
	{
		foreach (Resolution resolution in _AvailableResolutions)
		{
			_Resolution.options.Add(new TMPro.TMP_Dropdown.OptionData(resolution.ToString()));
		}
	}

	private void SetInitialVolumeValues()
	{
		// TODO: Check how to affect the wise thing
	}
	
	private void OnMasterVolumeChange()
	{
	  AkSoundEngine.SetRTPCValue("Master_Slider", _Master.value * 100  );	
	}

	private void OnSFXVolumeChange()
	{
	  AkSoundEngine.SetRTPCValue("SFX_Slider", _SFX.value * 100 );	
	}

	public void OnVideoPressed()
	{
		_Video.SetActive(true);
		_Audio.SetActive(false);
	}

	public void OnAudioPressed()
	{
		_Video.SetActive(false);
		_Audio.SetActive(true);
	}

	public void UpdateBrightness(float val)
	{
		_ColorGrading.brightness.value = val;
	}

}
