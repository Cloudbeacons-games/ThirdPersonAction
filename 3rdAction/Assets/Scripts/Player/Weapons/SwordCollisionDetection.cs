using UnityEngine;

public class SwordCollisionDetection : MonoBehaviour
{ 
    public SwordScript swordScript;
    public PlayerController playerController;
    private void OnTriggerEnter(Collider other)
    {
        GameObject enemy = other.gameObject;

        if (enemy.GetComponent<EnemyBrain>()!=null)
        {
            enemy.GetComponent<EnemyBrain>().TakeHit(swordScript.swordDamage);
            if(playerController.stateMachine.CurrentState is GroundCombo1Attack)
            {
                enemy.GetComponent<EnemyBrain>().KnockUp();
            }
        }
    }
}
