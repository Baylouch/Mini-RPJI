using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    // Damage
    public int projectileDamage = 0; // Is set in Player_Combat_Control for player
    [SerializeField] ProjectileType type; // To set projectile effect (frost, fire, nothing..)

    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float timerBeforeDestroy = 3f;

    private void Awake()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * projectileSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timerBeforeDestroy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.GetComponent<AI_Health>())
            {
                AI_Health enemyHealth = collision.gameObject.GetComponent<AI_Health>();
                enemyHealth.GetDamage(projectileDamage);
                // Now get projectile type for frost or make enemy in fire 
                switch (type)
                {
                    case ProjectileType.Normal:
                        // Do nothing
                        break;
                    case ProjectileType.Frost:
                        enemyHealth.Slowed();
                        break;
                    case ProjectileType.Fire:
                        // Do something;
                        break;
                }

                // If we kill the enemy
                if (enemyHealth.IsDead())
                {
                    // Check if npc got AI_Stats on him
                    AI_Stats enemyStats = collision.gameObject.GetComponent<AI_Stats>();
                    if (enemyStats)
                    {
                        if (Player_Stats.stats_instance)
                        {
                            Player_Stats.stats_instance.AddExperience(enemyStats.GetExperienceGain());
                        }
                    }
                }

            }
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Player_Health>())
            {
                Player_Health playerHealth = collision.gameObject.GetComponent<Player_Health>();
                playerHealth.GetDamage(projectileDamage);
            }
            Destroy(gameObject);
        }
    }
}
