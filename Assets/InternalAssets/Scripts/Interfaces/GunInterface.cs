
using UnityEngine;

public interface IGun
{

    void Initialise(Collider[] ChildrenColliders, RigidbodyConstraints rigidbodyConstraints, out Guns.ShootDelegate shoot);

}

