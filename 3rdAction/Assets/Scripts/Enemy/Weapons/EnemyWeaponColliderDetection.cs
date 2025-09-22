using UnityEngine;

public class EnemyWeaponColliderDetection : MonoBehaviour
{
    public EnemyBaseWeapon weaponScript;
    private void OnTriggerEnter(Collider other)
    {
        GameObject player = other.gameObject;

        if (player.tag=="Player")
        {
            player.GetComponent<PlayerController>().GotHit(weaponScript.weaponDmg);
        }
    }
}
