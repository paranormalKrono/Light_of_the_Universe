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
	[SerializeField] private SettingsMixerVolume[] SettingsMixerVolumes;

	[SerializeField] private Button buttonConfirm;
	[SerializeField] private Button buttonSave;

	[SerializeField] private AudioMixer AudioMixer;

	private bool isNormallyLoaded;

	private void Awake()
	{
		if (Settings.TryLoad())
		{
			isNormallyLoaded = true;
			ApplySettings();
		}

		if (!Settings.isModified)
		{
			buttonConfirm.interactable = false;
			buttonSave.interactable = false;
		}

		InitialiseResolutionMenu();
	}
	private void Start()
	{
		if (isNormallyLoaded) // Не переносить в Awake, AudioMixer грузится в Awake
		{
			UpdateMixerVolumesBySettingsVolumes();
		}
		else
		{
			ResetSettings();
			UpdateSettingsVolumesByMixerVolumes();
		}
		ResetMenusValues();
	}

	private void InitialiseResolutionMenu()
	{
		Resolution[] resolutionsList = Screen.resolutions;
		ddResolution.options = new List<Dropdown.OptionData>();
		foreach (Resolution R in resolutionsList)
		{
			ddResolution.options.Add(new Dropdown.OptionData(R.width + "x" + R.height + " " + R.refreshRate + "Hz"));
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
		LoadMenusValuesInSettings();
		Settings.Save();
		buttonSave.interactable = false;
	}

	public void LoadMenusValuesInSettings()
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
		Settings.Sensitivity = (int)(sliderSensitivity.value * 100);

		for (int i = 0; i < SettingsMixerVolumes.Length; ++i)
		{
			SettingsMixerVolumes[i].Confirm();
		}

		textSensitivityNow.text = string.Format("{0:f2}", sliderSensitivity.value);

		ApplySettings();
	}

	public void ResetMenusValues()
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
		
		for (int i = 0; i < SettingsMixerVolumes.Length; ++i)
		{
			SettingsMixerVolumes[i].ResetMenuValues();
		}
	}

	private void ResetSettings()
	{
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

		for (int i = 0; i < SettingsMixerVolumes.Length; ++i)
		{
			SettingsMixerVolumes[i].ResetValues(AudioMixer);
		}

	}

	public void ApplySettings()
	{
		Screen.SetResolution(Screen.resolutions[Settings.resolution].width, Screen.resolutions[Settings.resolution].height, (FullScreenMode)Settings.fullScreenMode, Screen.resolutions[Settings.resolution].refreshRate);
		QualitySettings.masterTextureLimit = Settings.textureQuality;
		QualitySettings.vSyncCount = Settings.vSyncCount;
		QualitySettings.antiAliasing = Settings.antiAliasing;
		QualitySettings.shadows = (ShadowQuality)Settings.shadowQuality;
		QualitySettings.shadowResolution = (ShadowResolution)Settings.shadowResolution;

		UpdateMixerVolumesBySettingsVolumes();
	}

	private void UpdateSettingsVolumesByMixerVolumes()
	{
		float mixerVolume;
		AudioMixerExposedParameter parameter;
		for (int i = 0; i < SettingsMixerVolumes.Length; ++i)
		{
			parameter = SettingsMixerVolumes[i].GetParameter();

			AudioMixer.GetFloat(parameter.ToString(), out mixerVolume);
			
			Settings.volumes[(int)parameter] = (int)MixerToPlayerVolume(mixerVolume);
		}
	}
	private void UpdateMixerVolumesBySettingsVolumes()
	{
		float parameterVolume;
		AudioMixerExposedParameter parameter;
		for (int i = 0; i < SettingsMixerVolumes.Length; ++i)
		{
			parameter = SettingsMixerVolumes[i].GetParameter();

			parameterVolume = PlayerToMixerVolume(Settings.volumes[(int)parameter]);

			AudioMixer.SetFloat(parameter.ToString(), parameterVolume);
		}
	}


	private float MixerToPlayerVolume(float mixerVolume)
	{
		mixerVolume += 80;
		if (mixerVolume < 20)
		{
			mixerVolume /= 3f;
		}
		else if (mixerVolume < 80)
		{
			mixerVolume = (mixerVolume - 60) * 3 + 20;
		}
		return mixerVolume;
	}
	private float PlayerToMixerVolume(float playerVolume)
	{
		if (playerVolume < 20)
		{
			playerVolume *= 3f;
		}
		else if (playerVolume < 80)
		{
			playerVolume = 60 + (playerVolume - 20) / 3;
		}
		playerVolume -= 80;
		return playerVolume;
	}


}