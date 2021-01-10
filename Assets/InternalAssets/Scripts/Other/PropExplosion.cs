using System.Collections.Generic;
using UnityEngine;

public class PropExplosion : MonoBehaviour
{
    [SerializeField] private GameObject BrokenObject;
    [SerializeField] private float minForceToExplosion = 1000;
    [SerializeField] private float ExplosionRadius = 40;
    [SerializeField] private float ExplosionForceMod = 400;
    [SerializeField] private float ExplosionForce = 2000;
    [SerializeField] private float ForceKoef = 0.8f;
    [SerializeField] private float DamageKoef = 0.1f;
    [SerializeField] private Rigidbody propRigidbody;
    [SerializeField] private LayerMask obstacleLayerMask;

    private void OnCollisionEnter(Collision collision)
    {
        Force((collision.impulse / Time.fixedDeltaTime).magnitude, true);
    }
    internal void Force(float Force, bool isHit)
    {
        if (Force > minForceToExplosion)
        {
            BlowUp(isHit);
        }
        else
        {
            minForceToExplosion -= Force / 2;
        }
    }
    public void BlowUp(bool isHit)
    {
        GameObject bg = Instantiate(BrokenObject, transform.position, transform.rotation);
        bg.transform.localScale = transform.lossyScale;
        Rigidbody[] rigidbodies =  bg.GetComponent<Prop>().Rigidbodies;

        for (int i = 0; i < rigidbodies.Length; ++i)
        {
            rigidbodies[i].velocity = propRigidbody.velocity;
        }

        Collider[] allColliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
        List<Prop> BlownUpProps = new List<Prop>();
        Prop curProp;
        PropExplosion propExplosion;
        Ray ray = new Ray();
        for (int i = 0; i < allColliders.Length; ++i)
        {
            if (isHit)
            {
                propExplosion = allColliders[i].GetComponentInParent<PropExplosion>();
                if (propExplosion != null)
                {
                    propExplosion.Force(ExplosionForce * (ExplosionRadius / Vector3.Distance(propExplosion.transform.position, transform.position) - 1) * ForceKoef, false);
                }
            }
            curProp = allColliders[i].GetComponentInParent<Prop>();
            if (curProp != null && !BlownUpProps.Contains(curProp))
            {
                BlownUpProps.Add(curProp);
                ray.origin = transform.position;
                ray.direction = (curProp.transform.position - transform.position).normalized;
                if (!Physics.Raycast(ray, Vector3.Distance(curProp.transform.position, transform.position), obstacleLayerMask))
                {
                    curProp.TakeDamage(ExplosionForce * (ExplosionRadius / Vector3.Distance(curProp.transform.position, transform.position) - 1) * DamageKoef);
                    for (int t = 0; t < curProp.Rigidbodies.Length; ++t)
                    {
                        curProp.Rigidbodies[t].AddExplosionForce(ExplosionForce + Random.Range(-ExplosionForceMod, ExplosionForceMod), transform.position, ExplosionRadius);
                    }
                }
            }
        }
        Destroy(gameObject);
    }
}