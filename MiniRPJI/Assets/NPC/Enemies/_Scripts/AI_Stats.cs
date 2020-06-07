/* AI_Stats.cs
 * Contient toutes les statistiques de l'ennemi (vitesse inclus)
 * Il est important de le pré-configurer sur le prefab pour un lvl 1 pour permettre le bon fonctionnement
 * de l'automatisation des statistiques par le level.
 * un ennemi lvl 2 sera 1,2* plus fort qu'un ennemi lvl 1
 * */

using UnityEngine;

public class AI_Stats : MonoBehaviour
{
    [Tooltip("will be set automaticaly in EnemySpawner")]
    [SerializeField] int level = 1;

    [Header("Set stats for a level 1")]
    [SerializeField] int totalHealthPoints = 50;
    [SerializeField] int experienceGain = 50;

    [SerializeField] float speed = 2f;

    // attack
    [SerializeField] private int damageMin = 5;
    [SerializeField] private int damageMax = 10;

    // Projectile damage
    [SerializeField] private int projectileDamageMin = 6;
    [SerializeField] private int projectileDamageMax= 8;

    // Critical rates
    [SerializeField] private float criticalRate = 3f;
    [SerializeField] private float rangedCriticalRate = 5f;

    int currentHealthPoints;
    bool enemySet; // If game master want to place enemy with AI_Stats in game, we must check if its already set because of EnemySpawner who's spawn and set ennemies.

    // Start is called before the first frame update
    void Start()
    {
        if (!enemySet)
        {
            InitializeEnemy(level);
        }
    }

    // Method to initialize an enemy via his level. Default value is level 1
    // To call for every enemy
    public void InitializeEnemy(int _level = 1)
    {
        if (_level <= 1)
        {
            SetCurrentHealthPoints(totalHealthPoints);
            return;
        }

        level = _level;
        float statsMultiplier = 1.2f * (level - 1); // level - 1 to start multiplication with 1.2f instead of 2.4f

        totalHealthPoints = Mathf.RoundToInt(totalHealthPoints * statsMultiplier);
        experienceGain = Mathf.RoundToInt(experienceGain * statsMultiplier);

        // We need to know the range between damageMin and damageMax to keep the difference whatever level enemy is.
        int damageRange = damageMax - damageMin;
        damageMin = Mathf.RoundToInt(damageMin * statsMultiplier);
        // /!\ Set damageMax with damageMin for not loosing start range between min and max.
        // If enemy is level 10 and we multiply 10 by 10 (100) and 5 by 10 (50) the range between them is too important
        damageMax = damageMin + damageRange;

        // Same as damage to keep projectile damage range.
        int projectileDamageRange = projectileDamageMax - projectileDamageMin;
        projectileDamageMin = Mathf.RoundToInt(projectileDamageMin * statsMultiplier);
        projectileDamageMax = projectileDamageMin + projectileDamageRange; // Same calculation for same reason as damageMax

        SetCurrentHealthPoints(totalHealthPoints);

        enemySet = true;
    }

    // For now just return experienceGain as experience gain. Later think about some maths (reduce/increase amount with player level)
    public int GetExperienceGain()
    {
        return experienceGain;
    }

    public int GetTotalHealthPoints()
    {
        return totalHealthPoints;
    }

    public int GetCurrentHealthPoints()
    {
        return currentHealthPoints;
    }
    public void SetCurrentHealthPoints(int newHealthPoints)
    {
        currentHealthPoints = newHealthPoints;
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetDamageMin()
    {
        return damageMin;
    }

    public int GetDamageMax()
    {
        return damageMax;
    }

    public int GetProjectileDamageMin()
    {
        return projectileDamageMin;
    }

    public int GetProjectileDamageMax()
    {
        return projectileDamageMax;
    }

    public void SetSpeed(float value)
    {
        speed = value;
    }
    public float GetSpeed()
    {
        return speed;
    }

    public float GetCriticalRate()
    {
        return criticalRate;
    }

    public float GetRangedCriticalRate()
    {
        return rangedCriticalRate;
    }
}
