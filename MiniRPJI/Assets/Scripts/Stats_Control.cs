/* Stats_Control.cs
 Gère les statistiques des entités. Par ex: le taux de vitalité augmentera les points de vies gérés eux dans Health.cs

*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatsType { STRENGTH, AGILITY, VITALITY, INTELLECT }; // Enum pour différencier les différentes statistiques

public class Stats_Control : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] private int level = 1;
    [SerializeField] private float NextLevelExperience = 200f;
    [SerializeField] private float currentExperience = 0f;

    [Header("Statistiques")]
    [SerializeField] private int strength = 10;
    [SerializeField] private int agility = 10;
    [SerializeField] private int vitality = 10;
    [SerializeField] private int intellect = 10;

    [Header("Attack")]
    [SerializeField] private int damageMin = 10;
    [SerializeField] private int damageMax = 15;

    [Header("Ranged Attack")] // To move or delete
    [SerializeField] private int rangedDamageMin = 5;
    [SerializeField] private int rangedDamageMax = 10;

    [Header("General")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float projectileSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrentStatsByType(StatsType type)
    {
        switch (type)
        {
            case StatsType.STRENGTH:
                return strength;
            case StatsType.AGILITY:
                return agility;
            case StatsType.VITALITY:
                return vitality;
            case StatsType.INTELLECT:
                return intellect;
            default:
                Debug.Log("Le type demandé n'est pas reconnu. Stats_Control.cs");
                return -1;
        }
    }

    public int GetCurrentAttackDamage()
    {
        int currAttack = (Random.Range(damageMin, damageMax) + (strength / 2));
        Debug.Log("Dommage infligés : " + currAttack);
        return currAttack;
    }

    public int GetCurrentRangedAttackDamage()
    {
        int currRangedattack = (Random.Range(rangedDamageMin, rangedDamageMax) + (agility / 2));
        Debug.Log("Dommage a distance infligés : " + currRangedattack);
        return currRangedattack;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetProjectileSpeed()
    {
        return projectileSpeed;
    }
}
