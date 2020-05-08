using UnityEngine;

public enum ProjectileType { Normal, Frost, Fire, Poison, Electric };

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player_Projectile : MonoBehaviour
{
    [HideInInspector]
    public int projectileDamage = 0; // Is set in Player_Combat

    public ProjectileType projectileType; // To set projectile effect (frost, fire, nothing..)

    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float timerBeforeDestroy = 3f;
    [SerializeField] float overPowerRange = 2f;
    [SerializeField] GameObject impactEffect;
    [SerializeField] GameObject overPowerEffect;

    bool used = false;

    // TODO Think about move the malus timer here.
    // Create a unique malus timer in AI_Health set with this projectile. Then for each different projectile you can set
    // a different time. BUT, i think its more logical biologically pov, to have each enemies act different themself.
    // Exemple : A mutant can more resist poison than a squirrel.

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, overPowerRange);
    }

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

                // Now get projectile type to set the right malus on the enemy
                switch (projectileType)
                {
                    // Normal projectile
                    case ProjectileType.Normal:
                        enemyHealth.TakeDamage(projectileDamage, true);


                        break;
                    // Frost projectile
                    case ProjectileType.Frost:
                        if (enemyHealth.GetCurrentMalusType() == MalusType.Slowed) // If enemy is already slowed, deal overPower
                        {
                            // Do overpower things
                            ProjectileOverPower(enemyHealth, MalusType.Slowed);
                        }
                        else // Else just set malus on the enemy
                        {
                            ProjectileNormalAttack(enemyHealth, MalusType.Slowed);
                        }
                        break;
                    // Fire projectile
                    case ProjectileType.Fire:
                        if (enemyHealth.GetCurrentMalusType() == MalusType.InFire)
                        {
                            // Do overpower
                            ProjectileOverPower(enemyHealth, MalusType.InFire);
                        }
                        else // else set malus on the enemy
                        {
                            ProjectileNormalAttack(enemyHealth, MalusType.InFire);
                        }
                        break;
                    // Poison projectile
                    case ProjectileType.Poison:
                        if (enemyHealth.GetCurrentMalusType() == MalusType.Poisoned)
                        {
                            // Do overpower
                            ProjectileOverPower(enemyHealth, MalusType.Poisoned);
                        }
                        else
                        {
                            ProjectileNormalAttack(enemyHealth, MalusType.Poisoned);
                        }
                        break;
                    // Electric projectile
                    case ProjectileType.Electric:
                        if (enemyHealth.GetCurrentMalusType() == MalusType.Electrified)
                        {
                            // Do overpower
                            ProjectileOverPower(enemyHealth, MalusType.Electrified);
                        }
                        else
                        {
                            ProjectileNormalAttack(enemyHealth, MalusType.Electrified);
                        }
                        break;
                }

                // For now just play a normal sound
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.bowAttackNormalImpact);

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

            used = true;
            Destroy(gameObject);
        }
    }

    // Method to deal not overpower arrow attack, inflict damage and set malus if projectile got a type linked.
    void ProjectileNormalAttack(AI_Health enemy, MalusType type)
    {
        enemy.SetMalus(type, projectileDamage);

        if (impactEffect)
        {
            GameObject _impact = Instantiate(impactEffect, transform.position, impactEffect.transform.rotation);
            Destroy(_impact, .5f);
        }
    }

    // Method to deal overpower explosion
    void ProjectileOverPower(AI_Health enemy, MalusType type)
    {
        // Target receive extra damage
        float extraDamage = projectileDamage + projectileDamage * 0.2f; // For the extra damage, we increase projectile damage by 20%.
        enemy.TakeDamage(Mathf.RoundToInt(extraDamage), true);

        // Create explosion
        if (overPowerEffect)
        {
            GameObject _impact = Instantiate(overPowerEffect, transform.position, overPowerEffect.transform.rotation);
            Destroy(_impact, 1f);
        }

        // Nearby ennemies take no damage but only the malus on them
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, overPowerRange);
        foreach(Collider2D collider in nearbyColliders)
        {
            if (collider.GetComponent<AI_Health>())
            {
                collider.GetComponent<AI_Health>().SetMalus(type, projectileDamage, false);
            }
        }

        // Target's malus removed
        enemy.RemoveMalus();
    }
}
