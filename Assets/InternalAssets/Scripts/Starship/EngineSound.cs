using UnityEngine;

public class EngineSound : MonoBehaviour
{
    [SerializeField] private Starship_Engine Engine;
    [SerializeField] private AudioSource audioSource;

    private float startVolume;

    private void Awake()
    {
        startVolume = audioSource.volume;
        Engine.OnMove += Engine_OnMove;
        Engine.OnSlowDown += Engine_OnSlowDown;
        audioSource.volume = 0;
    }
    private void Start()
    {
        audioSource.Play();
    }

    private void Engine_OnMove(Vector3 direction)
    {
        audioSource.volume = startVolume * direction.magnitude;
    }
    private void Engine_OnSlowDown()
    {
        audioSource.volume = 0;
    }
}
