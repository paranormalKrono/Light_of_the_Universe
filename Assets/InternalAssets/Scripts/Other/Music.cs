using System.Collections;
using UnityEngine;

public class Music : MonoBehaviour
{
    //Ссылка на воспроизводитель музыки
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private bool isRandom;
    [SerializeField] private bool isLoop;

    //Выбранная музыка
    private AudioClip ChoosedClip;
    //Номер Выбранной музыки
    private int clipChoose;

    //Время ожидания между музыкой
    public float TimeOut;

    //Музыка
    public AudioClip[] AClips = new AudioClip[2];

	void Start () {

        //Отключаем повтор
        audioSource.loop = false;

        //Запускаем воспроизведение
        StartCoroutine(StartMusic ());

        clipChoose = 0;
	}

    public IEnumerator StartMusic()
    {
        //Выбор музыки
        if (isRandom)
        {
            clipChoose = Random.Range(0, AClips.Length);
        }

        //Проверка на повторение музыки
        if (AClips[clipChoose] != ChoosedClip || AClips.Length == 1)
        {
            ChoosedClip = AClips[clipChoose]; //Выбранный клип равен воспроизводящейся музыке
            audioSource.clip = ChoosedClip;
            audioSource.Play(); //Воспроизводим музыку
            yield return new WaitForSeconds(ChoosedClip.length + TimeOut); //Ждём
        }

        if (clipChoose == AClips.Length - 1)
        {
            if (isLoop)
            {
                clipChoose = 0;
            }
            else
            {
                yield break;
            }
        }
        else
        {
            clipChoose += 1;
        }

        StartCoroutine(StartMusic());

    }
}
