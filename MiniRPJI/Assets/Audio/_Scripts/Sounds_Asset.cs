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
    public AudioClip teleportSound;
    public AudioClip teleportailSound;
    public AudioClip achievement;
    public AudioClip questInteraction;
    public AudioClip playerBonus;
    public AudioClip fireWorks;

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
    public AudioClip bankStoreAndGet;

    [Header("Player")]
    public AudioClip punchNoHit;
    public AudioClip punchHit;
    public AudioClip bowAttackNormal;
    public AudioClip bowAttackNormalImpact;
    public AudioClip bowAttackLaser;
    public AudioClip bowAttackLaserImpact;
    public AudioClip levelUp;
    public AudioClip playerDash;

    [Header("Pets")]
    public AudioClip catInvoke;
    public AudioClip dogInvoke;
    public AudioClip alienInvoke;

    [Header("NPC Karen")]
    public AudioClip[] karenGreetings;
    public AudioClip[] karenFarewell;
    public AudioClip[] karenCompletion;

    [Header("NPC Meghan")]
    public AudioClip[] meghanGreetings;
    public AudioClip[] meghanFarewell;
    public AudioClip[] meghanCompletion;

    [Header("NPC Ian")]
    public AudioClip[] ianGreetings;
    public AudioClip[] ianFarewell;
    public AudioClip[] ianCompletion;

    [Header("NPC Alex")]
    public AudioClip[] alexGreetings;
    public AudioClip[] alexFarewell;
    public AudioClip[] alexCompletion;

    [Header("NPC Sean")]
    public AudioClip[] seanGreetings;
    public AudioClip[] seanFarewell;
    public AudioClip[] seanCompletion;
}
