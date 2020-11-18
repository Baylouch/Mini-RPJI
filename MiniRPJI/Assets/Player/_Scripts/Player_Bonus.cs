using System.Collections;
using UnityEngine;

public enum Bonus_Type { Energy, Speed, AlienSpecial, SportCar, None };

public class Player_Bonus : MonoBehaviour
{
    [SerializeField] AnimatorOverrideController baseAnimator;
    [SerializeField] AnimatorOverrideController sportCarAnimator;
    [SerializeField] AnimatorOverrideController specialAlienAnimator;

    Bonus_Type currentPlayerBonus = Bonus_Type.None;

    private void OnDisable()
    {
        ResetAllBonus();
    }

    private void OnDestroy()
    {
        ResetAllBonus();
    }

    void ResetAllBonus()
    {
        StopAllCoroutines();

        UnSetEnergyBonus();
        UnSetSpeedBonus();
        UnSetSportCarBonus();
        UnSetSpecialAlienBonus();
    }

    public Bonus_Type GetCurrentPlayerBonus()
    {
        return currentPlayerBonus;
    }

    #region Energy Bonus
    void SetEnergyBonus()
    {
        if (Player_Stats.instance && Player_Stats.instance.playerEnergy)
        {
            Player_Stats.instance.playerEnergy.SetCurrentEnergyPoints(Player_Stats.instance.playerEnergy.GetTotalEnergyPoints());
            Player_Stats.instance.playerEnergy.freeEnergy = true;
            currentPlayerBonus = Bonus_Type.Energy;
        }
        else
        {
            Debug.Log("No Player_Stats instance or playerEnergy set to it in the scene.");
        }
    }

    void UnSetEnergyBonus()
    {
        if (Player_Stats.instance && Player_Stats.instance.playerEnergy)
        {
            if (Player_Stats.instance.playerEnergy.freeEnergy == true)
            {
                Player_Stats.instance.playerEnergy.freeEnergy = false;
                currentPlayerBonus = Bonus_Type.None;
            }
        }
        else
        {
            Debug.Log("No Player_Stats instance or playerEnergy set to it in the scene.");
        }
    }

    IEnumerator EnergyBonus()
    {
        SetEnergyBonus();

        yield return new WaitForSeconds(60f);

        UnSetEnergyBonus();
    }

    #endregion

    #region Speed Bonus

    void SetSpeedBonus()
    {
        if (GetComponent<Player_Movement>())
        {
            GetComponent<Player_Movement>().moveFaster = true;
            currentPlayerBonus = Bonus_Type.Speed;
        }
        else
        {
            Debug.Log("No player movement attach to Player_Bonus.");
        }
    }

    void UnSetSpeedBonus()
    {
        if (GetComponent<Player_Movement>())
        {
            if (GetComponent<Player_Movement>().moveFaster == true)
            {
                GetComponent<Player_Movement>().moveFaster = false;
                currentPlayerBonus = Bonus_Type.None;
            }
        }
        else
        {
            Debug.Log("No player movement attach to Player_Bonus.");
        }
    }

    IEnumerator SpeedBonus()
    {
        SetSpeedBonus();

        yield return new WaitForSeconds(60f);

        UnSetSpeedBonus();
    }

    #endregion

    #region Alien Special Bonus

    void SetSpecialAlienBonus()
    {
        if (GetComponent<Player_Combat>() && GetComponent<Animator>())
        {
            GetComponent<Player_Combat>().specialAlienAttack = true;
            GetComponent<Animator>().runtimeAnimatorController = specialAlienAnimator;
            currentPlayerBonus = Bonus_Type.AlienSpecial;

        }
        else
        {
            Debug.Log("No Player_Combat or Animator attach to the Player_Bonus component.");
        }
    }

    void UnSetSpecialAlienBonus()
    {
        if (GetComponent<Animator>() && GetComponent<Player_Combat>())
        {
            if (GetComponent<Player_Combat>().specialAlienAttack == true)
            {
                GetComponent<Player_Combat>().specialAlienAttack = false;
                GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
                currentPlayerBonus = Bonus_Type.None;
            }
        }
        else
        {
            Debug.Log("No Animator or Player_Combat found on player.");
        }
    }

    IEnumerator SpecialAlienBonus()
    {
        SetSpecialAlienBonus();

        yield return new WaitForSeconds(120f);

        UnSetSpecialAlienBonus();
    }

    #endregion

    #region SportCar Bonus

    void SetSportCarBonus()
    {
        if (GetComponent<Animator>() && GetComponent<Player_Movement>())
        {
            GetComponent<Player_Movement>().moveMoreFaster = true;
            GetComponent<Animator>().runtimeAnimatorController = sportCarAnimator;
            currentPlayerBonus = Bonus_Type.SportCar;
        }
        else
        {
            Debug.Log("No Animator or Player_Movement found on player.");
        }
    }

    void UnSetSportCarBonus()
    {
        if (GetComponent<Animator>() && GetComponent<Player_Movement>())
        {
            if (GetComponent<Player_Movement>().moveMoreFaster == true)
            {
                GetComponent<Player_Movement>().moveMoreFaster = false;
                GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
                currentPlayerBonus = Bonus_Type.None;
            }
        }
        else
        {
            Debug.Log("No Animator or Player_Movement found on player.");
        }
    }

    IEnumerator SportCarBonus()
    {
        SetSportCarBonus();

        yield return new WaitForSeconds(120f);

        UnSetSportCarBonus();
    }

    public void StopSportCarBonusNow()
    {
        StopAllCoroutines();
        UnSetSportCarBonus();
    }

    #endregion

    public void SetPlayerBonus(Bonus_Type _bonusType)
    {
        if (currentPlayerBonus != Bonus_Type.None)
        {
            // Clear old bonus
            ResetAllBonus();
        }

        switch (_bonusType)
        {
            case Bonus_Type.Energy:
                StartCoroutine(EnergyBonus());
                break;
            case Bonus_Type.Speed:
                StartCoroutine(SpeedBonus());
                break;
            case Bonus_Type.AlienSpecial:
                StartCoroutine(SpecialAlienBonus());
                break;
            case Bonus_Type.SportCar:
                StartCoroutine(SportCarBonus());
                break;
            case Bonus_Type.None:

                break;
            default:break;
        }
    }
}
