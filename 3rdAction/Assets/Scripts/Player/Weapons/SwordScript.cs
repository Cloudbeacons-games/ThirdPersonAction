using UnityEngine;

public class SwordScript : MonoBehaviour
{
    public CapsuleCollider swordCollider;
    public float swordDamage = 100f;

    public void EnableCollider()
    {
        swordCollider.enabled = true;
    }

    public void DisableCollider()
    {
        swordCollider.enabled = false;
    }
}
