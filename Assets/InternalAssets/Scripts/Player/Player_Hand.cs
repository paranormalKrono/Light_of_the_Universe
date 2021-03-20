using UnityEngine;
using UnityEngine.UI;

public class Player_Hand : MonoBehaviour
{
    [SerializeField] private float rayDistance = 3;
    [SerializeField] private Image ImageHand;
    [SerializeField] private Text entityText;
    [SerializeField] private LayerMask layerMask;

    private bool isUsingEntity;

    private IEntity entity;
    private RaycastHit raycastHit;
    private Ray ray;

    internal delegate void EventHandler();
    internal EventHandler EndUseHandler;

    private void Update()
    {

        if (!isUsingEntity)
        {
            ray.origin = transform.position;
            ray.direction = transform.forward;
            if (Physics.Raycast(ray, out raycastHit, rayDistance, layerMask) && raycastHit.collider.TryGetComponent(out entity))
            {
                entityText.text = entity.Description;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    entity.Interact();
                }
            }
            else
            {
                entityText.text = "";
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (EndUseHandler != null)
            {
                EndUseHandler();
            }
            else
            {
                EndUse();
            }
        }

    }

    internal void StartUse(EventHandler EndUseHandler)
    {

        this.EndUseHandler = EndUseHandler;
        ImageHand.enabled = false;
        isUsingEntity = true;

    }
    internal void EndUse()
    {

        isUsingEntity = false;
        ImageHand.enabled = true;

    }

    internal void SetEntityText(string text)
    {
        entityText.text = text;
    }
}