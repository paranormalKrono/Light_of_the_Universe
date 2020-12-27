using System.Collections;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class GameAudio : MonoBehaviour
{
    [SerializeField] private float TimeOut = 10; //Время ожидания между музыкой
    [SerializeField] private float TimeToEnd = 2; //Время окончания музыки

    private AudioClip ChoosedClip; //Выбранная музыка
    private AudioSource AudioSource;
    private int clipChoose; //Номер Выбранной музыки


    public delegate void EventA(AudioClip audioClip, bool isRepeat);
    public static EventA StartAudioEvent;
    public delegate void EventAs(AudioClip[] audioClip, bool isRandomize);
    public static EventAs StartAudiosEvent;
    public delegate void Event();
    public static Event StopAudioEvent;

    private void Awake()
    {
        StartAudioEvent = StartAudio;
        StartAudiosEvent = StartAudios;
        StopAudioEvent = StopAudio;

        AudioSource = GetComponent<AudioSource>();
    }

    private void StartAudio(AudioClip audioClip, bool isRepeat)
    {
        StartCoroutine(IStartAudio(audioClip, isRepeat));
    }
    private IEnumerator IStartAudio(AudioClip audioClip, bool isRepeat)
    {
        yield return StartCoroutine(IStopAudio());
        AudioSource.volume = 1;
        AudioSource.clip = audioClip;
        AudioSource.loop = isRepeat;
        AudioSource.Play();
    }
    private void StartAudios(AudioClip[] audioClips, bool isRandomize)
    {
        StartCoroutine(IStartAudios(audioClips, isRandomize));
    }
    private IEnumerator IStartAudios(AudioClip[] audioClips, bool isRandomize)
    {
        //Выбор музыки
        if (!isRandomize)
        {
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
        AudioSource.volume = 1;
        AudioSource.clip = ChoosedClip;
        AudioSource.loop = false;
        AudioSource.Play(); //Воспроизводим музыку

        yield return new WaitForSeconds(ChoosedClip.length + TimeOut); //Ждём
        StartCoroutine(IStartAudios(audioClips,isRandomize));
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
}