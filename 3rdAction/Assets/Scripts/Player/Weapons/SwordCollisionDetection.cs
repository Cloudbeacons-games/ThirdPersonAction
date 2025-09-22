using UnityEngine;

public class SwordCollisionDetection : MonoBehaviour
{ 
    public SwordScript swordScript;
    private void OnTriggerEnter(Collider other)
    {
        GameObject enemy = other.gameObject;

        if (enemy.GetComponent<EnemyBrain>()!=null)
        {
            enemy.GetComponent<EnemyBrain>().TakeHit(swordScript.swordDamage);
        }
    }
}
