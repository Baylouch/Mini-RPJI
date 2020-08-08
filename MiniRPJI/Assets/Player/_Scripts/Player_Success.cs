using UnityEngine;

public class Player_Success : MonoBehaviour
{
    public static Player_Success instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public const int totalSuccess = 18;

    // For this script i'll create an int array representing all the objective avancement.
    // each index represent a success config ID so its must have the same length as the total of success.
    public int[] successObjectives;

    // To retain where player already went. Remember to save / load this.
    public bool[] forestPartExplored; // [0] = Part 1 (buildindex 6),       [1] = Part 2 (buildindex 7)       
                                      // [2] = Part 3 (buildindex 8),       [3] = Part 4 (buildindex 9)      
                                      // [4] = Part 5 (buildindex 10),      [5] = Sublevel1_1 (buildindex 11)
                                      // [6] = Sublevel1_2 (buildindex 12), [7] = Sublevel2_1 (buildindex 13)
                                      // [8] = Sublevel2_2 (buildindex 14), [9] = Sublevel2_3 (buildindex 15).  Length = 10

    public bool[] beachPartExplored; // [0] = Part 1 (buildindex 16), [1] = Part 2 (buildindex 17)
                                     // [2] = Part 3 (buildindex 18), [3] = Part 4 (buildindex 19)
                                     // [4] = Part 5 (buildindex 20), [5] = Part 6 (buildindex 21).             Length = 6

    public bool[] volcanoPartExplored; // [0] = Part 1 (buildindex 22), [1] = Part 2 (buildindex 23)
                                       // [2] = Part 3 (buildindex 24), [3] = Part 4 (buildindex 25)
                                       // [4] = Part 5 (buildindex 26), [5] = Part 6 (buildindex 27)
                                       // [6] = Part 7 (buildindex 28), [7] = Part 8 (buildindex 29)
                                       // [8] = Part Final (buildindex 30).                                     Length = 9


    public const int forestPartsLength = 10;
    public const int beachPartsLength = 6;
    public const int volcanoPartsLength = 9;

    public SuccessDatabase successDatabase;

    private void Start()
    {
        successObjectives = new int[totalSuccess];

        forestPartExplored = new bool[forestPartsLength];
        beachPartExplored = new bool[beachPartsLength];
        volcanoPartExplored = new bool[volcanoPartsLength];
    }

    public void ValidSuccess(int _successID)
    {
        if (successDatabase.GetSuccessByID(_successID) == null)
        {
            Debug.Log("No success linked to this ID : " + _successID);
            return;
        }

        successDatabase.GetSuccessByID(_successID).isDone = true;

        // TODO Setup a button to player receive XP instead of giving him instantly when success is done.
        if (Player_Stats.instance)
        {
            Player_Stats.instance.AddExperience(successDatabase.GetSuccessByID(_successID).xpReward);
        }
    }

    public void SetSuccessObjective(int _successID, int _value)
    {
        if (successDatabase.GetSuccessByID(_successID) == null)
        {
            Debug.Log("No success linked to this ID : " + _successID);
            return;
        }

        if (!successDatabase.GetSuccessByID(_successID).isDone)
        {
            if (successObjectives.Length < _successID)
            {
                successObjectives[_successID] = _value;
            }
        }
    }

    public void ResetObjectives()
    {
        for (int i = 0; i < successDatabase.success.Length; i++)
        {
            successDatabase.GetSuccessByID(i).isDone = false;
            successObjectives[i] = 0;
        }
    }

    // Use this on each element when we want to increment the objective. Ex -> AI_Health, Player_Quest....
    public void IncrementSuccessObjectiveByID(int _successID, bool displaySuccessPopUp = true)
    {
        Success_Config currentConfig = successDatabase.GetSuccessByID(_successID);

        if (currentConfig == null)
        {
            Debug.Log("No success ID linked to this id : " + _successID.ToString());
        }

        successObjectives[_successID]++;

        if (successObjectives[_successID] >= currentConfig.successObjective)
        {
            // Objective of the success is done.
            ValidSuccess(_successID);
        }

        // Display relative pop up to this success
        if (UI_SuccessDisplayer.instance && displaySuccessPopUp)
        {
            UI_SuccessDisplayer.instance.DisplaySuccessPopUp(_successID);
        }
    }

    // To know which zone parts player already discovered
    public void SetExploredZone(int buildIndex)
    {
        if (buildIndex > 5 && buildIndex < 16) // Forest parts succes ID = 11
        {
            // TODO Make a switch condition here to check every index linked to the buildindex
            switch(buildIndex)
            {
                case 6:
                    if (!forestPartExplored[0])
                    {
                        forestPartExplored[0] = true;
                        IncrementSuccessObjectiveByID(11);
                    }
                    break;
                case 7:
                    if (!forestPartExplored[1])
                    {
                        forestPartExplored[1] = true;
                        IncrementSuccessObjectiveByID(11);
                    }
                    break;
                case 8:
                    if (!forestPartExplored[2])
                    {
                        forestPartExplored[2] = true;
                        IncrementSuccessObjectiveByID(11);
                    }
                    break;
                case 9:
                    if (!forestPartExplored[3])
                    {
                        forestPartExplored[3] = true;
                        IncrementSuccessObjectiveByID(11);
                    }
                    break;
                case 10:
                    if (!forestPartExplored[4])
                    {
                        forestPartExplored[4] = true;
                        IncrementSuccessObjectiveByID(11);
                    }
                    break;
                case 11:
                    if (!forestPartExplored[5])
                    {
                        forestPartExplored[5] = true;
                        IncrementSuccessObjectiveByID(11);
                    }
                    break;
                case 12:
                    if (!forestPartExplored[6])
                    {
                        forestPartExplored[6] = true;
                        IncrementSuccessObjectiveByID(11);
                    }
                    break;
                case 13:
                    if (!forestPartExplored[7])
                    {
                        forestPartExplored[7] = true;
                        IncrementSuccessObjectiveByID(11);
                    }
                    break;
                case 14:
                    if (!forestPartExplored[8])
                    {
                        forestPartExplored[8] = true;
                        IncrementSuccessObjectiveByID(11);
                    }
                    break;
                case 15:
                    if (!forestPartExplored[9])
                    {
                        forestPartExplored[9] = true;
                        IncrementSuccessObjectiveByID(11);
                    }
                    break;
            }
        }
        else if (buildIndex > 15 && buildIndex < 22) // Beach parts succes ID = 12
        {
            switch (buildIndex)
            {
                case 16:
                    if (!beachPartExplored[0])
                    {
                        beachPartExplored[0] = true;
                        IncrementSuccessObjectiveByID(12);
                    }
                    break;
                case 17:
                    if (!beachPartExplored[1])
                    {
                        beachPartExplored[1] = true;
                        IncrementSuccessObjectiveByID(12);
                    }
                    break;
                case 18:
                    if (!beachPartExplored[2])
                    {
                        beachPartExplored[2] = true;
                        IncrementSuccessObjectiveByID(12);
                    }
                    break;
                case 19:
                    if (!beachPartExplored[3])
                    {
                        beachPartExplored[3] = true;
                        IncrementSuccessObjectiveByID(12);
                    }
                    break;
                case 20:
                    if (!beachPartExplored[4])
                    {
                        beachPartExplored[4] = true;
                        IncrementSuccessObjectiveByID(12);
                    }
                    break;
                case 21:
                    if (!beachPartExplored[5])
                    {
                        beachPartExplored[5] = true;
                        IncrementSuccessObjectiveByID(12);
                    }
                    break;
            }

        }
        else if (buildIndex > 21 && buildIndex < 31) // Volcano parts succes ID = 13
        {
            switch (buildIndex)
            {
                case 22:
                    if (!volcanoPartExplored[0])
                    {
                        volcanoPartExplored[0] = true;
                        IncrementSuccessObjectiveByID(13);
                    }
                    break;
                case 23:
                    if (!volcanoPartExplored[1])
                    {
                        volcanoPartExplored[1] = true;
                        IncrementSuccessObjectiveByID(13);
                    }
                    break;
                case 24:
                    if (!volcanoPartExplored[2])
                    {
                        volcanoPartExplored[2] = true;
                        IncrementSuccessObjectiveByID(13);
                    }
                    break;
                case 25:
                    if (!volcanoPartExplored[3])
                    {
                        volcanoPartExplored[3] = true;
                        IncrementSuccessObjectiveByID(13);
                    }
                    break;
                case 26:
                    if (!volcanoPartExplored[4])
                    {
                        volcanoPartExplored[4] = true;
                        IncrementSuccessObjectiveByID(13);
                    }
                    break;
                case 27:
                    if (!volcanoPartExplored[5])
                    {
                        volcanoPartExplored[5] = true;
                        IncrementSuccessObjectiveByID(13);
                    }
                    break;
                case 28:
                    if (!volcanoPartExplored[6])
                    {
                        volcanoPartExplored[6] = true;
                        IncrementSuccessObjectiveByID(13);
                    }
                    break;
                case 29:
                    if (!volcanoPartExplored[7])
                    {
                        volcanoPartExplored[7] = true;
                        IncrementSuccessObjectiveByID(13);
                    }
                    break;
                case 30:
                    if (!volcanoPartExplored[8])
                    {
                        volcanoPartExplored[8] = true;
                        IncrementSuccessObjectiveByID(13);
                    }
                    break;
            }

        }
    }
}
