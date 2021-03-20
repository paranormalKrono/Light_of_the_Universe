using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent (typeof (AudioSource))]
public class GameAudio : MonoBehaviour
{
    [SerializeField] private float TimeOut = 10; //Время ожидания между музыкой
    [SerializeField] private float TimeToEnd = 2; //Время окончания музыки
    [SerializeField] private AudioMixer AudioMixer;

    private AudioClip ChoosedClip; //Выбранная музыка
    private AudioSource AudioSource;
    private int clipChoose; //Номер Выбранной музыки

    private float masterVolume;
    private bool isPaused;


    public delegate void EventA(AudioClip audioClip, float volume, bool isRepeat);
    public static EventA StartAudioEvent;
    public delegate void EventAs(AudioClip[] audioClip, float volume, bool isRandomize);
    public static EventAs StartAudiosEvent;
    public delegate void Event();
    public static Event StopAudioEvent;
    public static Event PauseAudioEvent;
    public static Event UnPauseAudioEvent;
    public delegate float EventGetMixerVolume(AudioMixerExposedParameter parameter);
    public static EventGetMixerVolume GetMixerVolumeEvent;
    public delegate void EventSetMixerVolume(AudioMixerExposedParameter parameter, float volume);
    public static EventSetMixerVolume SetMixerVolumeEvent;
    public delegate AudioMixer EventGetAudioMixer();
    public static EventGetAudioMixer GetAudioMixerEvent;

    public void Initialise()
    {
        StartAudioEvent = StartAudio;
        StartAudiosEvent = StartAudios;
        StopAudioEvent = StopAudio;
        PauseAudioEvent = PauseAudio;
        UnPauseAudioEvent = UnPauseAudio;
        GetMixerVolumeEvent = GetMixerVolume;
        SetMixerVolumeEvent = SetMixerVolume;
        GetAudioMixerEvent = GetAudioMixer;

        AudioSource = GetComponent<AudioSource>();
    }

    private void PauseAudio()
    {
        isPaused = true;
        AudioMixer.GetFloat(AudioMixerExposedParameter.Master.ToString(), out masterVolume);
        AudioMixer.SetFloat(AudioMixerExposedParameter.Master.ToString(), -80);
        AudioSource.Pause();
    }
    private void UnPauseAudio()
    {
        isPaused = false;
        AudioSource.UnPause();
        AudioMixer.SetFloat(AudioMixerExposedParameter.Master.ToString(), masterVolume);
    }

    private void StartAudio(AudioClip audioClip, float volume, bool isRepeat)
    {
        StartCoroutine(IStartAudio(audioClip, volume, isRepeat));
    }
    private IEnumerator IStartAudio(AudioClip audioClip, float volume, bool isRepeat)
    {
        yield return StartCoroutine(IStopAudio());
        AudioSource.volume = volume;
        AudioSource.clip = audioClip;
        AudioSource.loop = isRepeat;
        AudioSource.Play();
    }
    private void StartAudios(AudioClip[] audioClips, float volume, bool isRandomize)
    {
        StartCoroutine(IStartAudios(audioClips, volume, isRandomize));
    }
    private IEnumerator IStartAudios(AudioClip[] audioClips, float volume, bool isRandomize)
    {
        //Выбор музыки
        if (!isRandomize)
        {
            if (clipChoose >= audioClips.Length)
            {
                clipChoose = 0;
            }
            ChoosedClip = audioClips[clipChoose]; //Выбранный клип равен воспроизводящейся музыке
            clipChoose += 1;
        }
        else
        {
            clipChoose = Random.Range(0, audioClips.Length);
            //Проверка на повторение музыки
            if (audioClips[clipChoose] != ChoosedClip || audioClips.Length == 1)
            {
                ChoosedClip = audioClips[clipChoose]; //Выбранный клип равен воспроизводящейся музыке
            }
        }

        yield return StartCoroutine(IStopAudio());
        AudioSource.Stop();
        AudioSource.volume = volume;
        AudioSource.clip = ChoosedClip;
        AudioSource.loop = false;
        AudioSource.Play(); //Воспроизводим музыку

        yield return new WaitForSeconds(ChoosedClip.length + TimeOut); //Ждём
        StartCoroutine(IStartAudios(audioClips, volume, isRandomize));
    }
    private void StopAudio()
    {
        StopAllCoroutines();
        StartCoroutine(IStopAudio());
    }
    private IEnumerator IStopAudio()
    {
        while (AudioSource.volume - Time.deltaTime > 0)
        {
            AudioSource.volume = AudioSource.volume - Time.deltaTime / TimeToEnd;
            yield return null;
        }
        AudioSource.volume = 0;
    }

    public AudioMixer GetAudioMixer() => AudioMixer;
    public float GetMixerVolume(AudioMixerExposedParameter parameter)
    {
        float mixerVolume;
        AudioMixer.GetFloat(parameter.ToString(), out mixerVolume);
        return mixerVolume;
    }
    public void SetMixerVolume(AudioMixerExposedParameter parameter, float volume)
    {
        if (parameter != AudioMixerExposedParameter.Master || !isPaused)
        {
            AudioMixer.SetFloat(parameter.ToString(), volume);
        }
        if (parameter == AudioMixerExposedParameter.Master)
        {
            masterVolume = volume;
        }
    }
}