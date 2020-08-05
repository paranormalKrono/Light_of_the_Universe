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

    private void OnCollisionEnter(Collision collision)
    {
        Force((collision.impulse / Time.fixedDeltaTime).magnitude, true);
    }
    internal void Force(float Force, bool isHit)
    {
        if (Force > minForceToExplosion)
        {
            BrokenObject.transform.parent = null;
            BrokenObject.SetActive(true);
            foreach (Rigidbody brokenCellRigidbody in BrokenObject.GetComponent<Prop>().Rigidbodies)
            {
                brokenCellRigidbody.velocity = propRigidbody.velocity;
            }
            Collider[] allColliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
            List<Prop> BlownUpProps = new List<Prop>();
            Prop curProp;
            PropExplosion propExplosion;
            foreach (Collider curCollider in allColliders)
            {
                if (isHit)
                {
                    propExplosion = curCollider.GetComponentInParent<PropExplosion>();
                    if (propExplosion != null)
                    {
                        propExplosion.Force(ExplosionForce * (ExplosionRadius / Vector3.Distance(propExplosion.transform.position, transform.position) - 1) * ForceKoef, false);
                    }
                }
                curProp = curCollider.GetComponentInParent<Prop>();
                if (curProp != null && !BlownUpProps.Contains(curProp))
                {
                    BlownUpProps.Add(curProp);
                    curProp.TakeDamage(ExplosionForce * (ExplosionRadius / Vector3.Distance(curProp.transform.position, transform.position) - 1) * DamageKoef);
                    foreach (Rigidbody curPropRigidbody in curProp.Rigidbodies)
                    {
                        curPropRigidbody.AddExplosionForce(ExplosionForce + Random.Range(-ExplosionForceMod, ExplosionForceMod), transform.position, ExplosionRadius);
                    }
                }
            }
            Destroy(gameObject);
        }
        else
        {
            minForceToExplosion -= Force / 2;
        }
    }
}
