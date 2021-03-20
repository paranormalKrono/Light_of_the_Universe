using UnityEngine;

public class EngineParticles : MonoBehaviour
{
    [SerializeField] private Starship_Engine Engine;
    [SerializeField] private ParticleSystem ParticleSystem;
    private ParticleSystem.MainModule ParticleSystemModule;

    private float startSpeed;
    private float startSize;
    private float startForce;
    private float curForce;

    private void Awake()
    {
        startSpeed = ParticleSystem.main.startSpeed.constant;
        startSize = ParticleSystem.main.startSize.constant;
        ParticleSystemModule = ParticleSystem.main;
        Engine.OnMove += Engine_OnMove;
        Engine.OnSlowDown += Engine_OnSlowDown;
        Engine.OnParametersChanged += Engine_OnParametersChanged;
        ParticleSystemModule.startSpeed = 0;
        ParticleSystemModule.startSize = 0;
    }
    private void Start()
    {
        startForce = Engine.force;
        curForce = startForce;
    }

    private void Engine_OnMove(Vector3 direction)
    {
        ParticleSystemModule.startSpeed = startSpeed * ((curForce / startForce - 1) / 2 + 1) * direction.magnitude;
        ParticleSystemModule.startSize = startSize * ((curForce / startForce - 1) / 2 + 1) * direction.magnitude;
    }
    private void Engine_OnParametersChanged(float force, float speedMax, float friction)
    {
        curForce = force;
    }
    private void Engine_OnSlowDown()
    {
        ParticleSystemModule.startSpeed = 0;
        ParticleSystemModule.startSize = 0;
    }
}
