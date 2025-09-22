using UnityEngine;

public class PlayerLifeManager : MonoBehaviour
{
    private float maxHp;
    public float Hp;
    private PlayerController player;
    void Start()
    {
        player = GetComponent<PlayerController>();
        maxHp = Hp;
    }

    public void TakeDamage(float dmg)
    {
        Hp -= dmg;
        if (Hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
