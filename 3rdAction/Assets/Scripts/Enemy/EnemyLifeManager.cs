using UnityEngine;

public class EnemyLifeManager : MonoBehaviour
{
    private float maxHp;
    public float Hp;
    void Start()
    {
        maxHp = Hp;
    }

    public void TakeDamage(float dmg)
    {
        Hp -= dmg;
        if(Hp <= 0)
        {
            EnemyManager.DeregisterEnemy(this.gameObject);
            Destroy(gameObject);
        }
    }
}
