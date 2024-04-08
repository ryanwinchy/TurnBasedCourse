using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{

    [SerializeField] Transform ragdollRootBone;

    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);

        Vector3 randomDir = new Vector3(Random.Range(-1f,1f), 0, Random.Range(-1f,1f));
        ApplyExplosionToRagdoll(ragdollRootBone, 300f, transform.position , 10f);
    }

    void MatchAllChildTransforms(Transform root, Transform clone)          //Copies all bone positions and rotations from unit into ragdoll so looks fluid.
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);

            if (cloneChild != null)     //If found two bones of same name.
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;

                MatchAllChildTransforms(child, cloneChild);    //Recursion. Runs again next layer down, as bones have a lot of layers.
            }
        }
    }

    void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))        //Adds explosion to all direct children in route, so all rigidbodies on ragdoll.
            {  
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);     //Built in physics function.
            }

            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);     //Recursion so applies force to children of children, bones have many layers.

        }
    }


}
