using System;
using UnityEngine;

public class CeilingDetector : MonoBehaviour
{
    public float ceilingAngleLimit = 10f;

    bool ceilingWasHit;

    private void OnCollisionEnter(Collision collision) => CheckForContact(collision);
    private void OnCollisionStay(Collision collision) => CheckForContact(collision);

    private void CheckForContact(Collision collision)
    {
        if(collision.contacts.Length==0) return;

        float angle = Vector3.Angle(-transform.up, collision.contacts[0].normal);

        if(angle<ceilingAngleLimit)
        {
            ceilingWasHit = true;
        }
    }

    public bool HitCeiling() => ceilingWasHit;
    public void Reset()=> ceilingWasHit = false;
}
