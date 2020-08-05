using System.Collections;
using UnityEngine;

public class Music : MonoBehaviour {

    //Ссылка на воспроизводитель музыки
	private AudioSource ASource;

    //Выбранная музыка
    private AudioClip ChoosedClip;
    //Номер Выбранной музыки
    private int clipChoose;

    //Время ожидания между музыкой
    public float TimeOut;

    //Музыка
    public AudioClip[] AClips = new AudioClip[2];

	void Start () {

        //Получение ссылки на воспроизводитель музыки
        ASource = gameObject.GetComponent <AudioSource> ();
        //Отключаем повтор
        ASource.loop = false;

        //Запускаем воспроизведение
        StartCoroutine(StartMusic ());
	}

    public IEnumerator StartMusic()
    {
        //Выбор музыки
        clipChoose = Random.Range(0, AClips.Length);

        //Проверка на повторение музыки
        if (AClips[clipChoose] != ChoosedClip || AClips.Length == 1)
        {
            ChoosedClip = AClips[clipChoose]; //Выбранный клип равен воспроизводящейся музыке
            ASource.clip = ChoosedClip; 
            ASource.Play(); //Воспроизводим музыку
            yield return new WaitForSeconds(ChoosedClip.length + TimeOut); //Ждём
        }
        StartCoroutine(StartMusic());
    }
}
