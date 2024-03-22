using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onImpact : MonoBehaviour
{
    public BoxCollider mainCollider;
    public GameObject objectRig;
    public Animator objectAnimator;

   
    public AudioSource hitSound;

    Collider[] ragdollColliders;
    Rigidbody[] limbsRigidBodys;

    void Start()
    {
        
        getRagdoll();
        RagdollOff();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "knockDown")
        {
            Vector3 hitDirection = collision.contacts[0].normal; // Use the collision normal as hit direction
            float hitForce = 100.0f; // Adjust the force strength as needed
            RagdollOn(hitDirection, hitForce);
            hitSound.Play(); // Play the hit sound using the Play method
        }
    }

    void getRagdoll()
    {
        ragdollColliders = objectRig.GetComponentsInChildren<Collider>();
        limbsRigidBodys = objectRig.GetComponentsInChildren<Rigidbody>();
    }

    void RagdollOn(Vector3 forceDirection, float forceMagnitude)
    {
        objectAnimator.enabled = false;

        foreach (Collider col in ragdollColliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rigid in limbsRigidBodys)
        {
            rigid.isKinematic = false;
            rigid.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        }

        
        mainCollider.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    void RagdollOff()
    {
        foreach (Collider col in ragdollColliders)
        {
            col.enabled = false;
        }

        foreach (Rigidbody rigid in limbsRigidBodys)
        {
            rigid.isKinematic = true;
        }

        objectAnimator.enabled = true;
        mainCollider.enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }


}
