using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Depthshock : MonoBehaviour
{
    [SerializeField] private float forceToShock = 1000;
    [SerializeField] private float shockRadius = 40;
    [SerializeField] private float shockTime = 6;
    [SerializeField] private float timeOut = 10;
    [SerializeField] private ParticleSystem ParticleSystem;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private MeshRenderer shockMeshRenderer;
    [SerializeField] private AudioSource audioSource;

    private Color shockColor;

    private float currentForceToShock;
    private bool isShocked;

    private void Awake()
    {
        shockColor = shockMeshRenderer.materials[0].GetColor("_EmissionColor");
        currentForceToShock = forceToShock;
        GetComponent<Prop>().OnForce += Force;
    }

    internal void Force(float Force, bool isHit)
    {
        if (!isShocked)
        {
            if (Force > currentForceToShock)
            {
                StartCoroutine(IShock());
            }
            else
            {
                currentForceToShock -= Force / 2;
            }
        }
    }

    private IEnumerator IShock()
    {
        isShocked = true;
        currentForceToShock = forceToShock;
        ParticleSystem.Play(true);
        audioSource.Play();
        shockMeshRenderer.materials[0].SetColor("_EmissionColor", Color.black);
        Collider[] allColliders = Physics.OverlapSphere(transform.position, shockRadius);
        List<Starship> shockedStarships = new List<Starship>();
        Starship starship;
        Ray ray = new Ray();
        for (int i = 0; i < allColliders.Length; ++i)
        {
            starship = allColliders[i].GetComponentInParent<Starship>();
            if (starship != null && !shockedStarships.Contains(starship))
            {
                ray.origin = transform.position;
                ray.direction = (starship.transform.position - transform.position).normalized;
                if (!Physics.Raycast(ray, Vector3.Distance(starship.transform.position, transform.position), obstacleLayerMask))
                {
                    shockedStarships.Add(starship);
                    starship.LostControl(true);
                }
            }
        }
        yield return new WaitForSeconds(shockTime);
        for (int i = 0; i < shockedStarships.Count; ++i)
        {
            if (shockedStarships[i] != null)
            {
                shockedStarships[i].LostControl(false);
            }
        }
        ParticleSystem.Stop(true);
        yield return new WaitForSeconds(timeOut);
        audioSource.Stop();
        shockMeshRenderer.materials[0].SetColor("_EmissionColor", shockColor);
        isShocked = false;
    }
}
