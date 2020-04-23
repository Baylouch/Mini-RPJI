/* Combat_Control.cs
Utilisé pour gérer les combats du joueur
*/


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ProjectileType { Normal, Frost, Fire };

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Player_Stats))]
public class Player_Combat : MonoBehaviour
{
    public bool isInCombat = false; // Set in AI_Movement_Control
    public bool endingCombat = false; // Same as isInCombat
    [SerializeField] float timerBeforeEndCombat = 5f;
    float currentTimerBeforeEndCombat;

    [SerializeField] Transform firePoint;
    [SerializeField] float energyNeededForShoot;

    Animator animator;
    AI_Health currentEnemy;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is in combat
        if (isInCombat)
        {
            if (endingCombat) // If combat is ending start ending timer
            {
                if (currentTimerBeforeEndCombat > 0f)
                {
                    currentTimerBeforeEndCombat -= Time.deltaTime;
                }
                else
                {
                    isInCombat = false;
                    currentTimerBeforeEndCombat = timerBeforeEndCombat;
                    endingCombat = false;
                }
            }
        }

        if (IsMouseOverUI())
        {
            return;
        }

        if (!animator.GetBool("isAttacking"))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // TODO add a "powered shoot". Multiply arrow damage with a value between 1.0 - 1.2. ???
                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0)) // To centralise in Player_Input.cs later
            {
                UseNormalAttack();
                if (currentEnemy)
                {
                    currentEnemy.GetDamage(GetAttackDamage());

                    if (Sound_Manager.instance)
                    {
                        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.punchHit);
                    }

                    // If we kill the enemy
                    if (currentEnemy.IsDead())
                    {
                        // Check if npc got AI_Stats on him
                        AI_Stats enemyStats = currentEnemy.gameObject.GetComponent<AI_Stats>();
                        if (enemyStats)
                        {
                            // If yes add experience to player (to securise later Stats_Control)
                            Player_Stats.instance.AddExperience(enemyStats.GetExperienceGain());
                            currentEnemy = null;
                        }
                    }
                }
                else
                {
                    if (Sound_Manager.instance)
                    {
                        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.punchNoHit);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse1)) // To centralise in Player_Input.cs later
            {
                if (Player_Inventory.instance.GetCurrentBow())
                {
                    if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= energyNeededForShoot)
                    {
                        UseBowAttack();
                    }
                }
                else
                    Debug.Log("No bow equiped");
            }
        }
    }

    bool IsMouseOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return true;
            }
        }
        return false;
    }
    
    void UseNormalAttack()
    {
        animator.SetBool("isAttacking", true);
        animator.SetBool("normalAttack", true);
    }

    void UseBowAttack()
    {
        animator.SetBool("isAttacking", true);
        animator.SetBool("bowAttack", true);
    }

    private void OnTriggerStay2D(Collider2D collision) // Find other way later
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (!currentEnemy)
            {
                currentEnemy = collision.gameObject.GetComponent<AI_Health>();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (currentEnemy)
            {
                if (currentEnemy == collision.gameObject.GetComponent<AI_Health>())
                {
                    // Security because of adding xp when enemy dies
                    if (!currentEnemy.IsDead())
                        currentEnemy = null;
                }
            }
        }        
    }

    int GetAttackDamage()
    {
        float tempCritCondition = Random.Range(0, 100);
        if (tempCritCondition <= Player_Stats.instance.GetCriticalRate()) // Do critical strike
        {
            int criticalAttack = Mathf.RoundToInt((Random.Range(Player_Stats.instance.GetCurrentMinDamage(), Player_Stats.instance.GetCurrentMaxDamage()) * 1.5f));
            return criticalAttack;
        }
        else
        {
            int currAttack = (Random.Range(Player_Stats.instance.GetCurrentMinDamage(), Player_Stats.instance.GetCurrentMaxDamage()));
            return currAttack;
        }

    }

    int GetRangedAttackDamage()
    {
        float tempCritCondition = Random.Range(0, 100);
        if (tempCritCondition <= Player_Stats.instance.GetRangedCriticalRate())
        {
            int criticalRangedAttack = Mathf.RoundToInt((Random.Range(Player_Stats.instance.GetCurrentRangedMinDamage(), Player_Stats.instance.GetCurrentRangedMaxDamage()) * 1.5f));
            return criticalRangedAttack;
        }
        else
        {
            int currRangedattack = (Random.Range(Player_Stats.instance.GetCurrentRangedMinDamage(), Player_Stats.instance.GetCurrentRangedMaxDamage()));
            return currRangedattack;
        }
    }

    // To use in each event in animations attack
    public void TurnOffAnimatorParamAttack()
    {
        animator.SetBool("isAttacking", false);

        if (animator.GetBool("normalAttack"))
        {
            animator.SetBool("normalAttack", false);
        }
        if (animator.GetBool("bowAttack"))
        {
            animator.SetBool("bowAttack", false);
        }
    }

    // used for each bow attack to launch an arrow
    public void Shoot()
    {
        if (Player_Inventory.instance.GetCurrentBow()) // IF player got equiped bow
        {
            GameObject _projectile = Instantiate(Player_Inventory.instance.GetCurrentBow().projectile, firePoint.position, firePoint.rotation);
            Projectile currentProjectileComponent = _projectile.GetComponent<Projectile>();

            if (currentProjectileComponent == null)
                return; // There is a bug here or a miss by game master. Every projectile must have Projectile.cs attach

            currentProjectileComponent.projectileDamage = GetRangedAttackDamage();

            // Use energy
            Player_Stats.instance.playerEnergy.SetCurrentEnergyPoints(Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() - energyNeededForShoot);

            // Play sound
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.bowAttackNormal);
        }
    }

    // Use for set firepoint rotation depending of player's movement (as animation's event)
    public void SetFirePointRotation(float newRotation)
    {
        firePoint.rotation = Quaternion.Euler(new Vector3(0f, 0f, newRotation));
    }

    public void ResetEndingCombatTimer()
    {
        currentTimerBeforeEndCombat = timerBeforeEndCombat;
    }
}
