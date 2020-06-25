/* Sounds_Asset.cs (ScriptableObject)
 * Permet d'obtenir d'un moyen rapide la plupart des sons utilisés dans le jeu pour les jouer dans Sound_Manager.cs
 * (exclus: sons des ennemies)
 * 
 * */
using UnityEngine;

[CreateAssetMenu(fileName = "Sounds_Asset", menuName = "ScriptableObjects/SoundsAsset", order = 1)]
public class Sounds_Asset : ScriptableObject
{
    [Header("General")]
    public AudioClip[] gameOver;

    [Header("UI")]
    public AudioClip toggleUI;

    [Header("Items")]
    public AudioClip itemEquip;
    public AudioClip itemUnequip;
    public AudioClip itemTrash;
    public AudioClip itemPickup;
    public AudioClip sell;
    public AudioClip buy;
    public AudioClip potUse;
    public AudioClip questAchievement;
    public AudioClip bankStoreAndGet;

    [Header("Player")]
    public AudioClip punchNoHit;
    public AudioClip punchHit;
    public AudioClip bowAttackNormal;
    public AudioClip bowAttackNormalImpact;
    public AudioClip bowAttackLaser;
    public AudioClip bowAttackLaserImpact;
    public AudioClip levelUp;
    public AudioClip unlockAbility;

    [Header("Pets")]
    public AudioClip catInvoke;
    public AudioClip dogInvoke;
    public AudioClip alienInvoke;
}
