using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMixerVolume : MonoBehaviour
{
    [SerializeField] private AudioMixerExposedParameter mixerType;
    [SerializeField] private Slider sliderVolume;
    [SerializeField] private Text textVolume;
    [SerializeField] private Text textVolumeNow;

    public void SetVolume()
    {
        textVolume.text = ((int)sliderVolume.value).ToString();
    }
    public void Confirm()
    {
        Settings.volumes[(int)mixerType] = (int)sliderVolume.value;
        textVolumeNow.text = ((int)sliderVolume.value).ToString();
    }
    public void ResetMenuValues()
    {
        sliderVolume.value = Settings.volumes[(int)mixerType];
        textVolumeNow.text = sliderVolume.value.ToString();
        textVolume.text = textVolumeNow.text;
    }
    public void ResetValues(AudioMixer audioMixer)
    {
        float parameterVolume;
        audioMixer.GetFloat(mixerType.ToString(), out parameterVolume);
        sliderVolume.value = parameterVolume;
        textVolumeNow.text = parameterVolume.ToString();
        textVolume.text = parameterVolume.ToString();
    }
    public AudioMixerExposedParameter GetParameter()
    {
        return mixerType;
    }
}
