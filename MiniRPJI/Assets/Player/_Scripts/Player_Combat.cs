/* Player_Combat.cs
 * 
* Pièce centrale pour les combats du joueur.
* 
* Contient les Inputs des attaques primaire et secondaire.
* 
* Gère les animations d'attaques, les dégats des attaques, détérmine si le joueur est en mode combat.
* 
* 
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Player_Stats))]
[RequireComponent(typeof(Player_Abilities))]
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
    Player_Abilities playerAbilities;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerAbilities = GetComponent<Player_Abilities>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfPlayerIsInCombat();

        if (IsMouseOverUI())
        {
            return;
        }

        // If we're not attacking yet. (Bool isAttacking is reset in each combat animation's end.
        if (!animator.GetBool("isAttacking"))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // TODO add a "powered shoot". Multiply arrow damage with a value between 1.0 - 1.2. ???
                return;
            }

            // Left clic input
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                // Use Primary Ability
                if (playerAbilities.GetPrimaryAbility() != null) // Check if there is a primary ability set (must be)
                {
                    if (playerAbilities.GetPrimaryAbility().abilityType == AbilityType.Bow) // Check if its a Bow type (in this case we need a bow)
                    {
                        if (Player_Inventory.instance.GetCurrentBow() != null) // Be sure player got a bow equiped
                        {
                            // Now we can use ability
                            // First check if player got enough energy
                            if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetPrimaryAbility().energyCost)
                            {
                                StartCoroutine(UseBowAttack(playerAbilities.GetPrimaryAbility(), .3f));
                            }
                        }
                        else
                        {
                            Debug.Log("No bow equiped. Impossible to use ability. (Ability ID : " + playerAbilities.GetPrimaryAbility().abilityID + ")");
                        }
                    }
                    else // else its a punch ability type
                    {
                        if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetPrimaryAbility().energyCost)
                        {
                            StartCoroutine(UsePunchAttack(playerAbilities.GetPrimaryAbility(), 0f));
                        }
                    }
                }
            }
            // Right clic input
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                // Use Secondary Ability
                if (playerAbilities.GetSecondaryAbility() != null) // Check if there is a primary ability set (must be)
                {
                    if (playerAbilities.GetSecondaryAbility().abilityType == AbilityType.Bow) // Check if its a Bow type (in this case we need a bow)
                    {
                        if (Player_Inventory.instance.GetCurrentBow() != null) // Be sure player got a bow equiped
                        {
                            // Now we can use ability
                            // First check if player got enough energy
                            if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetSecondaryAbility().energyCost)
                            {
                                StartCoroutine(UseBowAttack(playerAbilities.GetSecondaryAbility(), .3f));
                            }
                        }
                        else
                        {
                            // Debug.Log("No bow equiped. Impossible to use ability. (Ability ID : " + playerAbilities.GetSecondaryAbility().abilityID + ")");
                        }
                    }
                    else // else its a punch ability type
                    {
                        if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetSecondaryAbility().energyCost)
                        {
                            StartCoroutine(UsePunchAttack(playerAbilities.GetSecondaryAbility(), 0f));
                        }
                    }
                }              
            }
        }
    }

    // used for each bow attack to launch an arrow
    IEnumerator UseBowAttack(Ability_Config _ability, float _delay)
    {
        PlayBowAnimation();

        yield return new WaitForSeconds(_delay);

        if (Player_Inventory.instance.GetCurrentBow()) // IF player got equiped bow (already check before using Shoot() in Update(), but we never sure enough)
        {
            // spawn ability's prefab, who represent a projectile
            GameObject _projectile = Instantiate(_ability.abilityPrefab, firePoint.position, firePoint.rotation);

            // To keep clean hierarchy, because projectiles maybe got a parent gameobject who's been instantiate. Destroy _projectile after 20sec if still there.
            // 20sec should be a great amount.
            Destroy(_projectile, 20f);

            // If this ability prefab got children (for multiple arrow for instance)
            if (_projectile.transform.childCount > 0)
            {
                for (int i = 0; i < _projectile.transform.childCount; i++)
                {
                    Player_Projectile currentProjectileComponent = _projectile.transform.GetChild(i).GetComponent<Player_Projectile>();

                    currentProjectileComponent.projectileDamage = GetRangedAttackDamage();
                }
            }
            else
            {
                Player_Projectile currentProjectileComponent = _projectile.GetComponent<Player_Projectile>();

                currentProjectileComponent.projectileDamage = GetRangedAttackDamage();
            }
          
            // Use energy
            Player_Stats.instance.playerEnergy.SetCurrentEnergyPoints(Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() - _ability.energyCost);

            // Play sound
            if (Sound_Manager.instance)
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.bowAttackNormal);
        }
    }

    IEnumerator UsePunchAttack(Ability_Config _ability, float _delay)
    {
        PlayPunchAnimation();

        yield return new WaitForSeconds(_delay);

        if (currentEnemy != null) // If we got a currentEnemy, then we're close to one because its set with triggerEnter2D, and unset in triggerExit2D
        {
            // Damage enemy
            currentEnemy.TakeDamage(GetAttackDamage(), true); // So we can direclty set damage to the enemy

            // Use energy
            Player_Stats.instance.playerEnergy.SetCurrentEnergyPoints(Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() - _ability.energyCost);

            // Play sound
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
        else // else just punch in the wind
        {
            if (Sound_Manager.instance)
            {
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.punchNoHit);
            }
        }
    }

    // TODO use coroutine
    void CheckIfPlayerIsInCombat()
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
    }

    // Method to know when mouse is over UI then dont attack
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
    
    void PlayPunchAnimation()
    {
        animator.SetBool("isAttacking", true);
        animator.SetBool("punchAttack", true);
    }

    void PlayBowAnimation()
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

    // Punch damage
    int GetAttackDamage()
    {
        bool tempCritCondition = Player_Stats.instance.GetCriticalRate() > Random.Range(0, 101);

        if (tempCritCondition) // Do critical strike (add 20% damage to the attack)
        {
            float criticalAttack = Mathf.RoundToInt((Random.Range(Player_Stats.instance.GetCurrentMinDamage(), Player_Stats.instance.GetCurrentMaxDamage())));
            criticalAttack = criticalAttack + criticalAttack * 0.2f;
            return Mathf.RoundToInt(criticalAttack);
        }
        else
        {
            int currAttack = (Random.Range(Player_Stats.instance.GetCurrentMinDamage(), Player_Stats.instance.GetCurrentMaxDamage()));
            return currAttack;
        }

    }

    // Bow damages
    int GetRangedAttackDamage()
    {
        bool tempCritCondition = Player_Stats.instance.GetRangedCriticalRate() > Random.Range(0, 101);
        if (tempCritCondition)
        {
            float criticalRangedAttack = Mathf.RoundToInt((Random.Range(Player_Stats.instance.GetCurrentRangedMinDamage(), Player_Stats.instance.GetCurrentRangedMaxDamage())));
            criticalRangedAttack = criticalRangedAttack + criticalRangedAttack * 0.2f;
            return Mathf.RoundToInt(criticalRangedAttack);
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

        if (animator.GetBool("punchAttack"))
        {
            animator.SetBool("punchAttack", false);
        }
        if (animator.GetBool("bowAttack"))
        {
            animator.SetBool("bowAttack", false);
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
