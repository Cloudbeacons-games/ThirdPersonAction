using UnityEngine;

public abstract class EnemyBaseWeapon : MonoBehaviour
{
    public float weaponDmg;
    public new CapsuleCollider collider;

    public virtual void EnableCollider()
    {
        collider.enabled = true;
    }

    public virtual void DisableCollider()
    {
        collider.enabled = false;
    }
}
