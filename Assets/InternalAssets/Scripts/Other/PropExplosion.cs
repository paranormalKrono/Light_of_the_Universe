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

    private bool isBlowedUp;

    private void Awake()
    {
        GetComponent<Prop>().OnForce += Force;
    }

    internal void Force(float Force, bool isHit)
    {
        if (Force > minForceToExplosion)
        {
            BlowUp();
        }
        else
        {
            minForceToExplosion -= Force / 2;
        }
    }
    public void BlowUp()
    {
        if (!isBlowedUp)
        {
            isBlowedUp = true;
            GameObject bg = Instantiate(BrokenObject, transform.position, transform.rotation);
            bg.transform.localScale = transform.lossyScale;
            Rigidbody[] rigidbodies = bg.GetComponent<Prop>().Rigidbodies;

            int i, j, t;
            for (i = 0; i < rigidbodies.Length; ++i)
            {
                rigidbodies[i].velocity = propRigidbody.velocity;
            }

            Collider[] allColliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
            List<Prop> BlownUpProps = new List<Prop>();
            Prop prop;
            Ray ray = new Ray();
            Prop[] ps;
            for (i = 0; i < allColliders.Length; ++i)
            {
                ps = allColliders[i].GetComponentsInParent<Prop>();
                for (j = 0; j < ps.Length; ++j)
                {
                    prop = ps[j];
                    if (prop != null && !BlownUpProps.Contains(prop))
                    {
                        BlownUpProps.Add(prop);
                        ray.origin = transform.position;
                        ray.direction = (prop.transform.position - transform.position).normalized;
                        if (!Physics.Raycast(ray, Vector3.Distance(prop.transform.position, transform.position), obstacleLayerMask))
                        {
                            prop.TakeDamage(ExplosionForce * (ExplosionRadius / Vector3.Distance(prop.transform.position, transform.position) - 1) * DamageKoef);
                            prop.Force(ExplosionForce * (ExplosionRadius / Vector3.Distance(prop.transform.position, transform.position) - 1) * ForceKoef, false);
                            for (t = 0; t < prop.Rigidbodies.Length; ++t)
                            {
                                prop.Rigidbodies[t].AddExplosionForce(ExplosionForce + Random.Range(-ExplosionForceMod, ExplosionForceMod), transform.position, ExplosionRadius);
                            }
                        }
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}