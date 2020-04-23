using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    // Damage
    [HideInInspector]
    public int projectileDamage = 0; // Is set in Player_Combat_Control for player. Is set in AI_Combat_Control for AI
    public ProjectileType projectileType; // To set projectile effect (frost, fire, nothing..)

    [SerializeField] float projectileSpeed = 5f;

    [SerializeField] float timerBeforeDestroy = 3f;

    [SerializeField] GameObject impactEffect;

    bool used = false;

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
        if (used)
            return;

        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.GetComponent<AI_Health>())
            {
                AI_Health enemyHealth = collision.gameObject.GetComponent<AI_Health>();
                enemyHealth.GetDamage(projectileDamage);
                // Now get projectile type for frost or make enemy in fire 
                switch (projectileType)
                {
                    case ProjectileType.Normal:
                        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.bowAttackNormalImpact);
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
                        if (Player_Stats.instance)
                        {
                            Player_Stats.instance.AddExperience(enemyStats.GetExperienceGain());
                        }
                    }
                }
            }
            
            if (impactEffect)
            {
                GameObject _impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
                Destroy(_impact, .5f);
            }

            used = true;
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Player_Health>())
            {
                Player_Health playerHealth = collision.gameObject.GetComponent<Player_Health>();
                playerHealth.GetDamage(Random.Range((projectileDamage - 2), (projectileDamage + 2)));
            }

            if (impactEffect)
            {
                GameObject _impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
                Destroy(_impact, .5f);
            }

            Destroy(gameObject);
        }
    }
}
