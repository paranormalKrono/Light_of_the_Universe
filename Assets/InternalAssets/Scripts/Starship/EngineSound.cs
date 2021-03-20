using UnityEngine;

public class EngineSound : MonoBehaviour
{
    [SerializeField] private Starship_Engine Engine;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float pitchModifier = 0.05f;

    private float startVolume;
    private float startPitch;

    private void Awake()
    {
        startVolume = audioSource.volume;
        startPitch = audioSource.pitch;
        audioSource.pitch = startPitch - pitchModifier;
        audioSource.volume = 0;
        Engine.OnMove += Engine_OnMove;
        Engine.OnSlowDown += Engine_OnSlowDown;
    }
    private void Start()
    {
        audioSource.Play();
    }

    private void Engine_OnMove(Vector3 direction)
    {
        audioSource.volume = startVolume * direction.magnitude;
        audioSource.pitch = startPitch + pitchModifier * direction.magnitude;
    }
    private void Engine_OnSlowDown()
    {
        audioSource.volume = 0;
    }
}
