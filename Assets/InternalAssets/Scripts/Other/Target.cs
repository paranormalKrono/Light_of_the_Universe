using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Target : MonoBehaviour
{
    [SerializeField] private Color newEmissionColor;
    [SerializeField] private int materialID = 0;
    private bool isActived;
    internal delegate void TargetDelegate();
    internal TargetDelegate TargetEvent;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isActived && collision.gameObject.CompareTag("Bullet"))
        {
            isActived = true;

            GetComponent<MeshRenderer>().materials[materialID].SetColor("_EmissionColor", newEmissionColor);
            TargetEvent();
        }
    }
}
