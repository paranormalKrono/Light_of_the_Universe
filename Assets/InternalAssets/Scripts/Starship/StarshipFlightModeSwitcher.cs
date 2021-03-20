using UnityEngine;

public class StarshipFlightModeSwitcher : MonoBehaviour
{
    [SerializeField] private Starship_Engine engine;
    [SerializeField] private Starship_RotationEngine rotationEngine;

    private FlightMode currentFlightMode = FlightMode.Normal;

    public FlightMode CurrentFlightMode { get => currentFlightMode; }

    private float engineForce;
    private float engineSpeedMax;
    private float engineFriction;
    private float rotationEngineForce;
    private float rotationEngineSpeedMax;
    private float rotationEngineFriction;

    private void Start()
    {
        engineForce = engine.force;
        engineSpeedMax = engine.speedMax;
        engineFriction = engine.friction;
        rotationEngineForce = rotationEngine.force;
        rotationEngineSpeedMax = rotationEngine.speedMax;
        rotationEngineFriction = rotationEngine.friction;
    }

    public void SwitchFlightMode(FlightMode newFlightMode)
    {
        if (currentFlightMode != newFlightMode)
        {
            switch (newFlightMode)
            {
                case FlightMode.Feather:
                    SetEngineCoefficients(0.65f, 1.6f, 0);
                    SetRotationEngineCoefficients(0.6f, 0.7f, 1.4f);
                    break;
                case FlightMode.Normal:
                    SetEngineCoefficients(1, 1, 1);
                    SetRotationEngineCoefficients(1, 1, 1);
                    break;
                case FlightMode.Chain:
                    SetEngineCoefficients(1.5f, 0.75f, 1.3f);
                    SetRotationEngineCoefficients(1.4f, 1.05f, 1.3f);
                    break;
                case FlightMode.Sword:
                    SetEngineCoefficients(0.2f, 0f, 1.8f);
                    SetRotationEngineCoefficients(1.6f, 1.2f, 2f);
                    break;
            }
            currentFlightMode = newFlightMode;
        }
    }

    private void SetEngineCoefficients(float forceC, float speedMaxC, float frictionC) => engine.SetParameters(engineForce * forceC, engineSpeedMax * speedMaxC, engineFriction * frictionC);
    private void SetRotationEngineCoefficients(float forceC, float speedMaxC, float frictionC) => rotationEngine.SetParameters(rotationEngineForce * forceC, rotationEngineSpeedMax * speedMaxC, rotationEngineFriction * frictionC);

}

public enum FlightMode
{
    Feather,
    Normal,
    Chain,
    Sword
}
