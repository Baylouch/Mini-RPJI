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
    [HideInInspector] public bool specialAlienAttack = false; // To switch abilities when player has alien bonus

    public bool playerCanCombat = true; // To make player not able to combat.

    public bool isInCombat = false; // Set in AI_Movement_Control
    public bool endingCombat = false; // Same as isInCombat
    [SerializeField] float timerBeforeEndCombat = 5f;
    float currentTimerBeforeEndCombat;

    [SerializeField] Transform firePoint;

    Animator animator;
    AI_Health currentEnemy;
    Player_Abilities playerAbilities;

    [SerializeField] Ability_Config[] specialAlienAbilities; // All alien's abilities are bow (ranged) one. So in attacks methods, it'll only affect abilityType.Bow.

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

        if (playerCanCombat == false)
            return;

        if (Player_Shortcuts.GetShortCuts() == 0)
        {
            UseAttackWithMouse();
        }
        else
        {
            UseAttackWithKeyboard();
        }
    }

    void UseAttackWithMouse()
    {
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
                // Check for player bonus to unset before attack
                if (GetComponent<Player_Bonus>())
                {
                    if (GetComponent<Player_Bonus>().GetCurrentPlayerBonus() == Bonus_Type.SportCar)
                    {
                        // Unset bonus
                        GetComponent<Player_Bonus>().StopSportCarBonusNow();
                    }
                }

                // Use Primary Ability
                if (playerAbilities.GetPrimaryAbility() != null) // Check if there is a primary ability set (must be)
                {
                    if (playerAbilities.GetPrimaryAbility().abilityType == AbilityType.Bow) // Check if its a Bow type (in this case we need a bow)
                    {
                        if (specialAlienAttack == true)
                        {
                            Ability_Config abilityToUse = specialAlienAbilities[Random.Range(0, specialAlienAbilities.Length)];

                            StartCoroutine(UseBowAbility(abilityToUse, .3f));
                        }
                        else
                        {
                            if (Player_Inventory.instance.GetCurrentBow() != null) // Be sure player got a bow equiped
                            {
                                // Now we can use ability
                                // First check if player got enough energy
                                if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetPrimaryAbility().energyCost)
                                {
                                    StartCoroutine(UseBowAbility(playerAbilities.GetPrimaryAbility(), .3f));
                                }
                            }
                            else
                            {
                                //Debug.Log("No bow equiped. Impossible to use ability. (Ability ID : " + playerAbilities.GetPrimaryAbility().abilityID + ")");
                                if (UI_Player_Informations.instance)
                                {
                                    UI_Player_Informations.instance.DisplayInformation("Il te faut un arc !");
                                }
                            }
                        }
                    }
                    else if (playerAbilities.GetPrimaryAbility().abilityType == AbilityType.Punch) // else its a punch ability type
                    {
                        if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetPrimaryAbility().energyCost)
                        {
                            StartCoroutine(UsePunchAbility(playerAbilities.GetPrimaryAbility(), 0f));
                        }
                    }
                    else // else its an other ability type
                    {
                        if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetPrimaryAbility().energyCost)
                        {
                            StartCoroutine(UseOtherAbility(playerAbilities.GetPrimaryAbility(), 0f));
                        }
                    }
                }
            }
            // Right clic input
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                // Check for player bonus to unset before attack
                if (GetComponent<Player_Bonus>())
                {
                    if (GetComponent<Player_Bonus>().GetCurrentPlayerBonus() == Bonus_Type.SportCar)
                    {
                        // Unset bonus
                        GetComponent<Player_Bonus>().StopSportCarBonusNow();
                    }
                }

                // Use Secondary Ability
                if (playerAbilities.GetSecondaryAbility() != null) // Check if there is a primary ability set (must be)
                {
                    if (playerAbilities.GetSecondaryAbility().abilityType == AbilityType.Bow) // Check if its a Bow type (in this case we need a bow)
                    {
                        if (specialAlienAttack == true)
                        {
                            Ability_Config abilityToUse = specialAlienAbilities[Random.Range(0, specialAlienAbilities.Length)];

                            StartCoroutine(UseBowAbility(abilityToUse, .3f));
                        }
                        else
                        {
                            if (Player_Inventory.instance.GetCurrentBow() != null) // Be sure player got a bow equiped
                            {
                                // Now we can use ability
                                // First check if player got enough energy
                                if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetSecondaryAbility().energyCost)
                                {
                                    StartCoroutine(UseBowAbility(playerAbilities.GetSecondaryAbility(), .3f));
                                }
                            }
                            else
                            {
                                // Debug.Log("No bow equiped. Impossible to use ability. (Ability ID : " + playerAbilities.GetSecondaryAbility().abilityID + ")");
                                if (UI_Player_Informations.instance)
                                {
                                    UI_Player_Informations.instance.DisplayInformation("Il te faut un arc !");
                                }
                            }
                        }
                    }
                    else if (playerAbilities.GetSecondaryAbility().abilityType == AbilityType.Punch) // else its a punch ability type
                    {
                        if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetSecondaryAbility().energyCost)
                        {
                            StartCoroutine(UsePunchAbility(playerAbilities.GetSecondaryAbility(), 0f));
                        }
                    }
                    else // else its an other ability type
                    {
                        if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetSecondaryAbility().energyCost)
                        {
                            StartCoroutine(UseOtherAbility(playerAbilities.GetSecondaryAbility(), 0f));
                        }
                    }
                }
            }
        }
    }

    void UseAttackWithKeyboard()
    {
        // If we're not attacking yet. (Bool isAttacking is reset in each combat animation's end.
        if (!animator.GetBool("isAttacking"))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // TODO add a "powered shoot". Multiply arrow damage with a value between 1.0 - 1.2. ???
                return;
            }

            // Left clic input
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // Check for player bonus to unset before attack
                if (GetComponent<Player_Bonus>())
                {
                    if (GetComponent<Player_Bonus>().GetCurrentPlayerBonus() == Bonus_Type.SportCar)
                    {
                        // Unset bonus
                        GetComponent<Player_Bonus>().StopSportCarBonusNow();
                    }
                }

                // Use Primary Ability
                if (playerAbilities.GetPrimaryAbility() != null) // Check if there is a primary ability set (must be)
                {
                    if (playerAbilities.GetPrimaryAbility().abilityType == AbilityType.Bow) // Check if its a Bow type (in this case we need a bow)
                    {
                        if (specialAlienAttack == true)
                        {
                            Ability_Config abilityToUse = specialAlienAbilities[Random.Range(0, specialAlienAbilities.Length)];

                            StartCoroutine(UseBowAbility(abilityToUse, .3f));
                        }
                        else
                        {
                            if (Player_Inventory.instance.GetCurrentBow() != null) // Be sure player got a bow equiped
                            {
                                // Now we can use ability
                                // First check if player got enough energy
                                if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetPrimaryAbility().energyCost)
                                {
                                    StartCoroutine(UseBowAbility(playerAbilities.GetPrimaryAbility(), .3f));
                                }
                            }
                            else
                            {
                                //Debug.Log("No bow equiped. Impossible to use ability. (Ability ID : " + playerAbilities.GetPrimaryAbility().abilityID + ")");
                                if (UI_Player_Informations.instance)
                                {
                                    UI_Player_Informations.instance.DisplayInformation("Il te faut un arc !");
                                }
                            }
                        }
                    }
                    else if (playerAbilities.GetPrimaryAbility().abilityType == AbilityType.Punch) // else its a punch ability type
                    {
                        if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetPrimaryAbility().energyCost)
                        {
                            StartCoroutine(UsePunchAbility(playerAbilities.GetPrimaryAbility(), 0f));
                        }
                    }
                    else // else its an other ability type
                    {
                        if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetPrimaryAbility().energyCost)
                        {
                            StartCoroutine(UseOtherAbility(playerAbilities.GetPrimaryAbility(), 0f));
                        }
                    }
                }
            }
            // Right clic input
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // Check for player bonus to unset before attack
                if (GetComponent<Player_Bonus>())
                {
                    if (GetComponent<Player_Bonus>().GetCurrentPlayerBonus() == Bonus_Type.SportCar)
                    {
                        // Unset bonus
                        GetComponent<Player_Bonus>().StopSportCarBonusNow();
                    }
                }

                // Use Secondary Ability
                if (playerAbilities.GetSecondaryAbility() != null) // Check if there is a primary ability set (must be)
                {
                    if (playerAbilities.GetSecondaryAbility().abilityType == AbilityType.Bow) // Check if its a Bow type (in this case we need a bow)
                    {
                        if (specialAlienAttack == true)
                        {
                            Ability_Config abilityToUse = specialAlienAbilities[Random.Range(0, specialAlienAbilities.Length)];

                            StartCoroutine(UseBowAbility(abilityToUse, .3f));
                        }
                        else
                        {
                            if (Player_Inventory.instance.GetCurrentBow() != null) // Be sure player got a bow equiped
                            {
                                // Now we can use ability
                                // First check if player got enough energy
                                if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetSecondaryAbility().energyCost)
                                {
                                    StartCoroutine(UseBowAbility(playerAbilities.GetSecondaryAbility(), .3f));
                                }
                            }
                            else
                            {
                                // Debug.Log("No bow equiped. Impossible to use ability. (Ability ID : " + playerAbilities.GetSecondaryAbility().abilityID + ")");
                                if (UI_Player_Informations.instance)
                                {
                                    UI_Player_Informations.instance.DisplayInformation("Il te faut un arc !");
                                }
                            }
                        }
                    }
                    else if (playerAbilities.GetSecondaryAbility().abilityType == AbilityType.Punch) // else its a punch ability type
                    {
                        if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetSecondaryAbility().energyCost)
                        {
                            StartCoroutine(UsePunchAbility(playerAbilities.GetSecondaryAbility(), 0f));
                        }
                    }
                    else // else its an other ability type
                    {
                        if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= playerAbilities.GetSecondaryAbility().energyCost)
                        {
                            StartCoroutine(UseOtherAbility(playerAbilities.GetSecondaryAbility(), 0f));
                        }
                    }
                }
            }
        }
    }

    // used for each bow attack to launch an arrow
    IEnumerator UseBowAbility(Ability_Config _ability, float _delay)
    {
        PlayBowAnimation();

        yield return new WaitForSeconds(_delay);

        if (Player_Inventory.instance.GetCurrentBow() || specialAlienAttack) // IF player got equiped bow (already check before using Shoot() in Update(), but we never sure enough). We bypass when player is a special alien (should not happen but if player got no bow but is in alien, we want him to be able to attack anyway).
        {
            // spawn ability's prefab, who represent a projectile
            GameObject _projectile = Instantiate(_ability.abilityPrefab, firePoint.position, firePoint.rotation);

            // To keep clean hierarchy, because projectiles maybe got a parent gameobject who's been instantiate. Destroy _projectile after 20sec if still there.
            // 20sec should be a great amount.
            Destroy(_projectile, 20f);

            // If this ability prefab got children (for multiple arrow for instance)
            if (_projectile.GetComponent<Player_Projectile>())
            {
                Player_Projectile currentProjectileComponent = _projectile.GetComponent<Player_Projectile>();

                currentProjectileComponent.projectileDamage = GetRangedAttackDamage() + _ability.abilityBonus;
                currentProjectileComponent.projectilePower = _ability.abilityPower;
                currentProjectileComponent.malusTimer = _ability.abilityTimer;
            }
            else if (_projectile.transform.childCount > 0)
            {
                for (int i = 0; i < _projectile.transform.childCount; i++)
                {
                    if (_projectile.transform.GetChild(i).GetComponent<Player_Projectile>())
                    {
                        Player_Projectile currentProjectileComponent = _projectile.transform.GetChild(i).GetComponent<Player_Projectile>();

                        currentProjectileComponent.projectileDamage = GetRangedAttackDamage() + _ability.abilityBonus;
                        currentProjectileComponent.projectilePower = _ability.abilityPower;
                        currentProjectileComponent.malusTimer = _ability.abilityTimer;
                    }
                }
            }
            else
            {
                Debug.Log("Can't find Player_Projectile component when instantiating arrow.");
            }
          
            // Use energy
            Player_Stats.instance.playerEnergy.SetCurrentEnergyPoints(Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() - _ability.energyCost);
        }
    }

    IEnumerator UsePunchAbility(Ability_Config _ability, float _delay)
    {
        PlayPunchAnimation();

        yield return new WaitForSeconds(_delay);

        if (currentEnemy != null) // If we got a currentEnemy, then we're close to one because its set with triggerEnter2D, and unset in triggerExit2D
        {
            if (_ability.abilityPrefab)
            {
                GameObject abilityGO = Instantiate(_ability.abilityPrefab, currentEnemy.transform.position, _ability.abilityPrefab.transform.rotation);

                Destroy(abilityGO, 2f);
            }

            // Propulse enemy to the back
            if (currentEnemy.gameObject.GetComponent<AI_Enemy_Movement>())
            {
                currentEnemy.GetHit(.1f, _ability.abilityPower, currentEnemy.gameObject.GetComponent<AI_Enemy_Movement>(), this);
            }

            // Damage enemy
            currentEnemy.TakeDamage(GetAttackDamage() + _ability.abilityBonus, true); // So we can direclty set damage to the enemy

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

        // Use energy
        Player_Stats.instance.playerEnergy.SetCurrentEnergyPoints(Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() - _ability.energyCost);
    }

    IEnumerator UseOtherAbility(Ability_Config _ability, float _delay)
    {
        // No animation for that sort of ability

        yield return new WaitForSeconds(_delay);

        if (_ability.abilityPrefab)
        {
            GameObject abilityPrefab = Instantiate(_ability.abilityPrefab, transform.position, Quaternion.identity);

            // Check if its a decoy
            if (_ability.abilitySubID == 7)
            {
                // Set the healthpoints decoy_health component
                int decoyHealthPoints = Player_Stats.instance.playerHealth.GetTotalHealthPoints() + _ability.abilityBonus;
                abilityPrefab.GetComponent<Decoy_Health>().SetTotalHealthPoints(decoyHealthPoints);
                abilityPrefab.GetComponent<Decoy_Health>().SetCurrentHealthPoints(decoyHealthPoints);
            }

            Destroy(abilityPrefab, _ability.abilityTimer);
        }

        // Use energy
        Player_Stats.instance.playerEnergy.SetCurrentEnergyPoints(Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() - _ability.energyCost);
    }

    // TODO use coroutine ?
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
    // TODO Put this code into a static class to acces from everywhere (because of rebondancy in PlayerCameraZoom.cs)
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
            if (specialAlienAttack && Player_Inventory.instance.GetCurrentBow() == null)
            {
                float criticalAttack = Mathf.RoundToInt(Random.Range(30, 40));
                criticalAttack = criticalAttack + criticalAttack * 0.2f;
                return Mathf.RoundToInt(criticalAttack);
            }
            else
            {
                float criticalAttack = Mathf.RoundToInt((Random.Range(Player_Stats.instance.GetCurrentMinDamage(), Player_Stats.instance.GetCurrentMaxDamage())));
                criticalAttack = criticalAttack + criticalAttack * 0.2f;
                return Mathf.RoundToInt(criticalAttack);
            }
        }
        else
        {
            if (specialAlienAttack && Player_Inventory.instance.GetCurrentBow() == null)
            {
                int currAttack = Random.Range(30, 40);
                return currAttack;
            }
            else
            {
                int currAttack = (Random.Range(Player_Stats.instance.GetCurrentMinDamage(), Player_Stats.instance.GetCurrentMaxDamage()));
                return currAttack;
            }
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

    // Used in Research_Projectile.cs to get the arrow rotation when created.
    public float GetFirePointRotationZ()
    {
        return firePoint.rotation.eulerAngles.z;
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
