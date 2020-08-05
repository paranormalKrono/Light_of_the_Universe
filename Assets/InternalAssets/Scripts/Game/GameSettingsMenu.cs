using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class GameSettingsMenu : MonoBehaviour
{

    [SerializeField] private Dropdown ddResolution;
    [SerializeField] private Dropdown ddTextureQuality;
    [SerializeField] private Dropdown ddVSyncCount;
    [SerializeField] private Dropdown ddFullScreenMode;
    [SerializeField] private Dropdown ddAntiAliasing;
    [SerializeField] private Dropdown ddShadowQuality;
    [SerializeField] private Dropdown ddShadowResolution;
	[SerializeField] private Slider sliderSensitivity;
	[SerializeField] private Text textSensitivity;
	[SerializeField] private Text textSensitivityNow;
	[SerializeField] private SettingsVolume[] SettingsVolumes;

	[SerializeField] private Button buttonConfirm;
	[SerializeField] private Button buttonSave;

	[SerializeField] private GameObject SettingsPanel;
	[SerializeField] private AudioMixer AudioMixer;

	public delegate void EventHandlerG(GameObject G);
	public static EventHandlerG OpenMenuEvent;
	public delegate void EventHandler();
	public static EventHandler CloseMenuEvent;

	private GameObject menuToOpen;

	private bool isNormallyLoaded;

	private void Awake()
	{
		if (Settings.TryLoad())
		{
			isNormallyLoaded = true;
			UpdateGameSettings();
		}
		else
		{
			ResetSettings();
		}

		OpenMenuEvent = OpenMenu;
		CloseMenuEvent = CloseMenu;

		if (!Settings.isModified)
		{
			buttonConfirm.interactable = false;
			buttonSave.interactable = false;
		}

		Resolution[] resolutionsList = Screen.resolutions;

		ddResolution.options = new List<Dropdown.OptionData>();
		foreach (Resolution R in resolutionsList)
		{
			ddResolution.options.Add(new Dropdown.OptionData(R.width + "x" + R.height + " " + R.refreshRate + "Hz"));
		}
		ResetOptions();

	}
	private void Start()
	{
		if (isNormallyLoaded)
		{
			UpdateAudioMixer();
		}
		else
		{
			ResetAudioMixer();
		}
	}


	public void SetSomething()
	{
		buttonConfirm.interactable = true;
		buttonSave.interactable = true;
	}

	public void SetSensitivity()
	{
		textSensitivity.text = string.Format("{0:f2}", sliderSensitivity.value);
	}

	public void Save()
	{
		Confirm();
		Settings.Save();
		buttonConfirm.interactable = false;
		buttonSave.interactable = false;
	}

	public void OpenMenu(GameObject G)
	{
		menuToOpen = G;
		SettingsPanel.SetActive(true);
	}

	public void CloseMenu()
	{
		SettingsPanel.SetActive(false);
		if (menuToOpen)
		{
			menuToOpen.SetActive(true);
		}
	}

	public void Confirm()
	{
		buttonConfirm.interactable = false;
		Settings.isModified = true;
		Settings.resolution = ddResolution.value;
		Settings.textureQuality = ddTextureQuality.value;
		Settings.vSyncCount = ddVSyncCount.value;
		Settings.fullScreenMode = ddFullScreenMode.value;
		Settings.antiAliasing = ddAntiAliasing.value;
		Settings.shadowQuality = ddShadowQuality.value;
		Settings.shadowResolution = ddShadowResolution.value;

		for (int i = 0; i < SettingsVolumes.Length; ++i)
		{
			SettingsVolumes[i].Confirm();
		}

		Settings.Sensitivity = (int)(sliderSensitivity.value * 100);
		textSensitivityNow.text = string.Format("{0:f2}", sliderSensitivity.value);

		UpdateGameSettings();
	}

	public void ResetOptions()
	{
		ddResolution.value = Settings.resolution;
		ddTextureQuality.value = Settings.textureQuality;
		ddVSyncCount.value = Settings.vSyncCount;
		ddFullScreenMode.value = Settings.fullScreenMode;
		ddAntiAliasing.value = Settings.antiAliasing;
		ddShadowQuality.value = Settings.shadowQuality;
		ddShadowResolution.value = Settings.shadowResolution;
		sliderSensitivity.value = (float)Settings.Sensitivity / 100;
		textSensitivityNow.text = string.Format("{0:f2}", (float)Settings.Sensitivity / 100);
		textSensitivity.text = string.Format("{0:f2}", (float)Settings.Sensitivity / 100); 
		
		for (int i = 0; i < SettingsVolumes.Length; ++i)
		{
			SettingsVolumes[i].ResetOptions();
		}
	}

	private void ResetSettings()
	{
		Debug.Log("ResetSettings");
		for (int i = 0; i < Screen.resolutions.Length; ++i)
		{
			if (Screen.currentResolution.Equals(Screen.resolutions[i]))
			{
				Settings.resolution = i;
				break;
			}
		}
		Settings.textureQuality = QualitySettings.masterTextureLimit;
		Settings.vSyncCount = QualitySettings.vSyncCount;
		Settings.fullScreenMode = (int)Screen.fullScreenMode;
		Settings.antiAliasing = QualitySettings.antiAliasing;
		Settings.shadowQuality = (int)QualitySettings.shadows;
		Settings.shadowResolution = (int)QualitySettings.shadowResolution;
		Settings.Sensitivity = (int)(Settings.SensetivityDefault * 100);
	}

	public void UpdateGameSettings()
	{
		Debug.Log("UpdateSettings");
		Screen.SetResolution(Screen.resolutions[Settings.resolution].width, Screen.resolutions[Settings.resolution].height, (FullScreenMode)Settings.fullScreenMode, Screen.resolutions[Settings.resolution].refreshRate);
		QualitySettings.masterTextureLimit = Settings.textureQuality;
		QualitySettings.vSyncCount = Settings.vSyncCount;
		QualitySettings.antiAliasing = Settings.antiAliasing;
		QualitySettings.shadows = (ShadowQuality)Settings.shadowQuality;
		QualitySettings.shadowResolution = (ShadowResolution)Settings.shadowResolution;

		UpdateAudioMixer();
	}

	private void ResetAudioMixer()
	{
		float f;
		AudioMixerExposedParameter parameter;
		for (int i = 0; i < SettingsVolumes.Length; ++i)
		{
			parameter = SettingsVolumes[i].GetParameter();
			AudioMixer.GetFloat(parameter.ToString(), out f);
			f += 80;
			if (f < 20)
			{
				f /= 3f;
			}
			else if (f < 80)
			{
				f = (f - 60) * 3 + 20;
			}
			Settings.volumes[(int)parameter] = (int)f;
		}
	}

	private void UpdateAudioMixer()
	{
		AudioMixerExposedParameter parameter;
		float f;
		for (int i = 0; i < SettingsVolumes.Length; ++i)
		{
			parameter = SettingsVolumes[i].GetParameter();
			f = Settings.volumes[(int)parameter];
			if (f < 20)
			{
				f *= 3f;
			}
			else if (f < 80)
			{
				f = 60 + (f - 20) / 3;
			}
			f -= 80;
			AudioMixer.SetFloat(parameter.ToString(), f);
		}
	}
}