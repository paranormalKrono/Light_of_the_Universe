using UnityEngine;

public class EmissionColorChanger : MonoBehaviour
{
    [SerializeField] private int materialID = 0;
    public void ChangeEmissionColor(Color newColor) => GetComponent<MeshRenderer>().materials[materialID].SetColor("_EmissionColor", newColor);
}
