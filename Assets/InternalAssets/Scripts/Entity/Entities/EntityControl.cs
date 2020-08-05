using System.Collections;
using UnityEngine;

public class EntityControl : MonoBehaviour, IEntity
{
    [SerializeField] private string description;
    [SerializeField] private string endUseDescription;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 8;

    [SerializeField] private Camera Camera;
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private Player_Character_Controller PlayerCharacterController;
    [SerializeField] private Character_Camera_Controller PlayerCameraController;
    [SerializeField] private Player_Hand PlayerHand;
    [SerializeField] private Transform TargetTr;

    public string Description { get => description; }

    private bool isDoingSomething;

    private Transform CameraTr;
    private Transform PlayerCameraTr;

    private void Awake()
    {
        CameraTr = Camera.transform;
        PlayerCameraTr = PlayerCamera.transform;
    
    }

    public void Interact() => StartCoroutine(IUse());

    private IEnumerator IUse()
    {
        if (!isDoingSomething)
        {
            isDoingSomething = true;
            PlayerHand.SetEntityText("");
            PlayerCharacterController.SetLockMove(true);
            PlayerCameraController.SetLockLook(true);
            PlayerHand.StartUse(EndUse);

            CameraTr.position = PlayerCameraTr.position;
            CameraTr.rotation = PlayerCameraTr.rotation;

            PlayerCamera.enabled = false;
            Camera.enabled = true;

            yield return StartCoroutine(IMoveCamera(TargetTr.position, TargetTr.rotation));
            GameMenu.SetGameCursorLock(false);
            PlayerHand.SetEntityText(endUseDescription);
            isDoingSomething = false;
        }
    }


    private void EndUse() => StartCoroutine(IEndUse());

    private IEnumerator IEndUse()
    {
        if (!isDoingSomething)
        {
            isDoingSomething = true;
            PlayerHand.SetEntityText("");
            GameMenu.SetGameCursorLock(true);
            yield return StartCoroutine(IMoveCamera(PlayerCameraTr.position, PlayerCameraTr.rotation));

            Camera.enabled = false;
            PlayerCamera.enabled = true;

            PlayerCharacterController.SetLockMove(false);
            PlayerCameraController.SetLockLook(false);

            isDoingSomething = false;
            PlayerHand.EndUse();
        }
    }

    private IEnumerator IMoveCamera(Vector3 TargetPos, Quaternion TargetRot)
    {

        while (CameraTr.position != TargetPos || Quaternion.Angle(CameraTr.rotation, TargetRot) > 0.1f)
        {
            CameraTr.position = Vector3.Lerp(CameraTr.position, TargetPos, Time.deltaTime * moveSpeed);
            CameraTr.position = Vector3.MoveTowards(CameraTr.position, TargetPos, Time.deltaTime * moveSpeed);
            CameraTr.rotation = Quaternion.Slerp(CameraTr.rotation, TargetRot, Time.deltaTime * rotationSpeed);
            CameraTr.rotation = Quaternion.RotateTowards(CameraTr.rotation, TargetRot, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        CameraTr.rotation = TargetRot;

    }
}
