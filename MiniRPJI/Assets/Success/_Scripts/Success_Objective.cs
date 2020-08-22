using UnityEngine;

public class Success_Objective : MonoBehaviour
{
    [SerializeField] int successID;

    Success_Config config;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Player_Success.instance)
        {
            if (Player_Success.instance.successDatabase.GetSuccessByID(successID) != null)
            {
                config = Player_Success.instance.successDatabase.GetSuccessByID(successID);
            }
            else
            {
                Debug.Log("Problem with Succes Objective. Wrong ID : " + successID);
                Destroy(this);
            }
        }
        else
        {
            Debug.Log("No Player_Success.Instance found.");
            Destroy(this);
        }

        if (config && config.isDone)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (config && config.isDone)
        {
            Destroy(this);
        }
    }

    public void IncrementSuccessObjective()
    {
        if (Player_Success.instance)
        {
            Player_Success.instance.IncrementSuccessObjectiveByID(successID);
        }
    }
}
