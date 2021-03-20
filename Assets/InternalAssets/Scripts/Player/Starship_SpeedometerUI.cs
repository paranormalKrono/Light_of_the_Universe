using UnityEngine;
using UnityEngine.UI;

public class Starship_SpeedometerUI : MonoBehaviour
{
    [SerializeField] private Starship_Engine Engine;
    [SerializeField] private Text speedText;
    [SerializeField] private Text forceText;
    [SerializeField] private Rigidbody moveRigidbody;

    private float curForce;

    private void Awake()
    {
        Engine.OnMove += Engine_OnMove;
        Engine.OnSlowDown += Engine_OnSlowDown;
        Engine.OnParametersChanged += Engine_OnParametersChanged;
    }
    private void Start()
    {
        curForce = Engine.force;
    }

    private void Engine_OnMove(Vector3 direction)
    {
        forceText.text = Mathf.Round(curForce * direction.magnitude).ToString();
        speedText.text = Mathf.Round(moveRigidbody.velocity.magnitude).ToString();
    }

    private void Engine_OnParametersChanged(float force, float speedMax, float friction)
    {
        curForce = force;
    }

    private void Engine_OnSlowDown()
    {
        forceText.text = "0";
        speedText.text = Mathf.Round(moveRigidbody.velocity.magnitude).ToString();
    }
}
