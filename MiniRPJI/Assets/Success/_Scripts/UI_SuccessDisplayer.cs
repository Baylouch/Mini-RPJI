using UnityEngine;

public class UI_SuccessDisplayer : MonoBehaviour
{
    public static UI_SuccessDisplayer instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public const string activePopUpKey = "POPUPACTIVATE"; // 0 = Off   1 = ON

    [SerializeField] GameObject successDisplayer; // Must have SuccessDisplayerElements on it

    bool displayPopUp = true;
    public void TogglePopUp(bool value)
    {
        displayPopUp = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Destroy all child
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        // Set playerprefs
        if (PlayerPrefs.HasKey(activePopUpKey))
        {
            if (PlayerPrefs.GetInt(activePopUpKey) == 1)
            {
                displayPopUp = true;
            }
            else
            {
                displayPopUp = false;
                return;
            }
        }
        else
        {
            PlayerPrefs.SetInt(activePopUpKey, 1);
            displayPopUp = true;
        }
    }

    public void DisplaySuccessPopUp(int _successID)
    {
        if (!displayPopUp)
            return;

        if (Player_Success.instance)
        {
            if (Player_Success.instance.successDatabase.GetSuccessByID(_successID) == null)
            {
                Debug.Log("Wrong success ID : " + _successID.ToString());
                return;
            }

            Success_Config curConfig = Player_Success.instance.successDatabase.GetSuccessByID(_successID);

            GameObject newPopUp = Instantiate(successDisplayer, transform);

            SuccessDisplayerElements elements = newPopUp.GetComponent<SuccessDisplayerElements>();

            elements.successTitle.text = curConfig.successTitle;
            if (curConfig.isDone)
            {
                elements.successObjective.text = "Accompli !";
                elements.successImage.sprite = curConfig.successSprite;
            }
            else
            {
                elements.successObjective.text = Player_Success.instance.successObjectives[_successID].ToString() + " / " + curConfig.successObjective.ToString();
                elements.successImage.sprite = curConfig.successSprite;
                elements.successImage.color = Color.black;
            }

            elements.quitButton.onClick.AddListener(()=> Destroy(newPopUp));

        }
    }
}
