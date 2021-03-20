using System.Collections.Generic;
using UnityEngine;

public class PropPusher : MonoBehaviour
{
    [SerializeField] private float radius = 30;
    [SerializeField] private float force = 800;
    [SerializeField] private bool isPushesOut;

    private Prop selfProp;
    private Collider[] colliders;
    private List<Prop> props;
    private Prop[] ps;
    private Prop prop;
    private int i, j, t;

    private void Awake()
    {
        selfProp = GetComponent<Prop>();
    }

    private void FixedUpdate()
    {
        colliders = Physics.OverlapSphere(transform.position, radius);
        props = new List<Prop>();
        for (i = 0; i < colliders.Length; ++i)
        {
            ps = colliders[i].GetComponentsInParent<Prop>();
            for (j = 0; j < ps.Length; ++j)
            {
                prop = ps[j];
                if (prop != null && !props.Contains(prop) && prop != selfProp)
                {
                    props.Add(prop);
                    for (t = 0; t < prop.Rigidbodies.Length; ++t)
                    {
                        if (!isPushesOut)
                        {
                            prop.Rigidbodies[t].AddForce(force *
                                 (transform.position - prop.Rigidbodies[t].transform.position).normalized *
                                 (radius / Vector3.Distance(prop.Rigidbodies[t].transform.position, transform.position) - 1) *
                                 Time.fixedDeltaTime, ForceMode.Impulse);
                        }
                        else
                        {
                            prop.Rigidbodies[t].AddForce(force *
                                 (prop.Rigidbodies[t].transform.position - transform.position).normalized *
                                 (radius / Vector3.Distance(prop.Rigidbodies[t].transform.position, transform.position) - 1) * 
                                 Time.fixedDeltaTime, ForceMode.Impulse);
                        }
                    }
                }
            }
        }
    }
}
