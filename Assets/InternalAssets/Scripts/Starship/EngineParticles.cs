using UnityEngine;

public class EngineParticles : MonoBehaviour
{
    [SerializeField] private Starship_Engine Engine;
    [SerializeField] private ParticleSystem ParticleSystem;
    private ParticleSystem.MainModule ParticleSystemModule;

    private float startSpeed;
    private float startSize;

    private void Awake()
    {
        startSpeed = ParticleSystem.main.startSpeed.constant;
        startSize = ParticleSystem.main.startSize.constant;
        ParticleSystemModule = ParticleSystem.main;
        Engine.OnMove += Engine_OnMove;
        Engine.OnSlowDown += Engine_OnSlowDown;
        ParticleSystemModule.startSpeed = 0;
        ParticleSystemModule.startSize = 0;
    }

    private void Engine_OnMove(Vector3 direction)
    {
        ParticleSystemModule.startSpeed = startSpeed * direction.magnitude;
        ParticleSystemModule.startSize = startSize * direction.magnitude;
    }
    private void Engine_OnSlowDown()
    {
        ParticleSystemModule.startSpeed = 0;
        ParticleSystemModule.startSize = 0;
    }
}
