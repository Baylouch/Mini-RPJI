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
    [SerializeField] float malusTimer = 2f;
    [SerializeField] float percentageChanceToApplyMalus = 100f;
    [SerializeField] float overPowerRange = 2f;
    [SerializeField] GameObject impactEffect;
    [SerializeField] GameObject overPowerEffect;

    bool used = false;

    // TODO Create a special sprite to apply as arrow image when player got alien's pet to modify the arrow gfx into a laser one.
    [SerializeField] Sprite laserGfx;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, overPowerRange);
    }

    private void Awake()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * projectileSpeed;

        if (Player_Pets.instance && Player_Pets.instance.currentPlayerPet && laserGfx)
        {
            if (Player_Pets.instance.currentPlayerPet.petCategory == PetCategory.Alien)
            {
                if (GetComponent<SpriteRenderer>())
                {
                    GetComponent<SpriteRenderer>().sprite = laserGfx;
                }
                else
                {
                    if (transform.childCount > 0)
                    {
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            if (transform.GetChild(i).GetComponent<SpriteRenderer>())
                            {
                                transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = laserGfx;
                                break;
                            }
                        }
                    }
                }

                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.bowAttackLaser);
                return;
            }
        }

        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.bowAttackNormal);
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

                if (enemyHealth.GetComponent<MalusApplier>())
                {
                    MalusApplier malusApplier = enemyHealth.GetComponent<MalusApplier>();

                    // Now get projectile type to set the right malus on the enemy
                    switch (projectileType)
                    {
                        // Normal projectile
                        case ProjectileType.Normal:
                            enemyHealth.TakeDamage(projectileDamage, true);


                            break;
                        // Frost projectile
                        case ProjectileType.Frost:
                            if (malusApplier.GetCurrentMalusType() == MalusType.Slowed) // If enemy is already slowed, deal overPower
                            {
                                // Do overpower things
                                ProjectileOverPower(enemyHealth, malusApplier, MalusType.Slowed);
                            }
                            else // Else just set malus on the enemy
                            {
                                ProjectileNormalAttack(malusApplier, MalusType.Slowed);
                            }
                            break;
                        // Fire projectile
                        case ProjectileType.Fire:
                            if (malusApplier.GetCurrentMalusType() == MalusType.InFire)
                            {
                                // Do overpower
                                ProjectileOverPower(enemyHealth, malusApplier, MalusType.InFire);
                            }
                            else // else set malus on the enemy
                            {
                                ProjectileNormalAttack(malusApplier, MalusType.InFire);
                            }
                            break;
                        // Poison projectile
                        case ProjectileType.Poison:
                            if (malusApplier.GetCurrentMalusType() == MalusType.Poisoned)
                            {
                                // Do overpower
                                ProjectileOverPower(enemyHealth, malusApplier, MalusType.Poisoned);
                            }
                            else
                            {
                                ProjectileNormalAttack(malusApplier, MalusType.Poisoned);
                            }
                            break;
                        // Electric projectile
                        case ProjectileType.Electric:
                            if (malusApplier.GetCurrentMalusType() == MalusType.Electrified)
                            {
                                // Do overpower
                                ProjectileOverPower(enemyHealth, malusApplier, MalusType.Electrified);
                            }
                            else
                            {
                                ProjectileNormalAttack(malusApplier, MalusType.Electrified);
                            }
                            break;
                    }
                }
                else
                {
                    enemyHealth.TakeDamage(projectileDamage, true);
                }
                
                // TODO Think about each arrow got a specific impact sound
                // For now just play a normal sound
                if (Sound_Manager.instance)
                {
                    if (Player_Pets.instance && Player_Pets.instance.currentPlayerPet && laserGfx)
                    {
                        if (Player_Pets.instance.currentPlayerPet.petCategory == PetCategory.Alien)
                        {
                            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.bowAttackLaserImpact);
                        }
                    }
                    else
                    {
                        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.bowAttackNormalImpact);
                    }
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

            used = true;
            Destroy(gameObject);
        }
    }

    // Method to deal not overpower arrow attack, inflict damage and set malus if projectile got a type linked.
    void ProjectileNormalAttack(MalusApplier _malusApplier, MalusType type)
    {
        _malusApplier.SetMalus(type, projectileDamage, malusTimer, percentageChanceToApplyMalus);

        if (impactEffect)
        {
            GameObject _impact = Instantiate(impactEffect, transform.position, impactEffect.transform.rotation);
            Destroy(_impact, .5f);
        }
    }

    // Method to deal overpower explosion
    void ProjectileOverPower(AI_Health enemy, MalusApplier _malusApplier, MalusType type)
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
            if (collider.gameObject.tag != "Player" && collider.GetComponent<MalusApplier>())
            {
                collider.GetComponent<MalusApplier>().SetMalus(type, projectileDamage, malusTimer, percentageChanceToApplyMalus, false);
            }
        }

        // Target's malus removed
        _malusApplier.RemoveMalus();
    }

    // Used in Research_Projectile.cs
    public float GetProjectileSpeed()
    {
        return projectileSpeed;
    }
}
