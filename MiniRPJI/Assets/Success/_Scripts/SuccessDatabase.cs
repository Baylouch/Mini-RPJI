using UnityEngine;

[CreateAssetMenu(fileName = "SucessDatabase", menuName = "ScriptableObjects/Success/SuccessDatabase", order = 1)]
public class SuccessDatabase : ScriptableObject
{
    public Success_Config[] success;

    // Method to get objects by its unique ID
    public Success_Config GetSuccessByID(int _successID)
    {
        foreach (var success_ in success)
        {
            if (success_ != null && success_.successID == _successID)
                return success_;
        }
        return null;
    }
}
