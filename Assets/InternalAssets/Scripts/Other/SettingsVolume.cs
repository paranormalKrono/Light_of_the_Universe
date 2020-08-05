using UnityEngine;
using UnityEngine.UI;

public class SettingsVolume : MonoBehaviour
{
    [SerializeField] private AudioMixerExposedParameter parameter;
    [SerializeField] private Slider sliderVolume;
    [SerializeField] private Text textVolume;
    [SerializeField] private Text textVolumeNow;

    public void SetVolume()
    {
        textVolume.text = ((int)sliderVolume.value).ToString();
    }
    public void Confirm()
    {
        Settings.volumes[(int)parameter] = (int)sliderVolume.value;
        textVolumeNow.text = ((int)sliderVolume.value).ToString();
    }
    public void ResetOptions()
    {
        sliderVolume.value = Settings.volumes[(int)parameter];
        textVolumeNow.text = sliderVolume.value.ToString();
        textVolume.text = textVolumeNow.text;
    }
    public AudioMixerExposedParameter GetParameter()
    {
        return parameter;
    }
}
