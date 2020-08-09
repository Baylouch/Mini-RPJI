using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Success : MonoBehaviour
{
    [SerializeField] Success_Button[] successButtons;

    [SerializeField] Text successTitle;
    [SerializeField] Text successDescription;
    [SerializeField] Text successObjective;
    [SerializeField] Text successXpReward;

    [SerializeField] GameObject objectiveLabel;
    [SerializeField] GameObject rewardLabel;
    [SerializeField] Button rewardButton;

    [SerializeField] Button quitButton;

    private void Start()
    {
        ResetSuccessInformation();
        RefreshSuccessButtons();

        if (quitButton)
        {
            quitButton.onClick.AddListener(() => UI_Player.instance.ToggleSuccessUI());
        }
    }

    void RefreshSuccessButtons()
    {
        for (int i = 0; i < successButtons.Length; i++)
        {
            if (successButtons[i].config.isDone)
            {
                if (successButtons[i].successImage.color != Color.white)
                    successButtons[i].successImage.color = Color.white;
            }
            else
            {
                if (successButtons[i].successImage.color != Color.black)
                    successButtons[i].successImage.color = Color.black;
            }

            if (successButtons[i].backgroundImage.enabled)
            {
                successButtons[i].backgroundImage.enabled = false;
            }
        }
    }

    void ResetSuccessInformation()
    {
        successTitle.text = "";
        successDescription.text = "";
        successObjective.text = "";
        successXpReward.text = "";

        if (objectiveLabel.activeSelf)
            objectiveLabel.SetActive(false);
        if (rewardLabel.activeSelf)
            rewardLabel.SetActive(false);
    }

    public void DisplaySuccessInformation(int _successID)
    {
        if (Player_Success.instance.successDatabase.GetSuccessByID(_successID) == null)
        {
            Debug.Log("No success linked to this ID : " + _successID);
            return;
        }

        Success_Config currentSuccess = Player_Success.instance.successDatabase.GetSuccessByID(_successID);

        successTitle.text = currentSuccess.successTitle;
        successDescription.text = currentSuccess.successDescription;

        if (currentSuccess.isDone)
        {
            successObjective.text = "Accompli";

            if (currentSuccess.rewardGift)
            {
                if (rewardLabel.activeSelf)
                    rewardLabel.SetActive(false);
            }
            else
            {
                if (!rewardLabel.activeSelf)
                    rewardLabel.SetActive(true);

                if (!rewardButton.gameObject.activeSelf)
                    rewardButton.gameObject.SetActive(true);

                successXpReward.text = currentSuccess.xpReward.ToString() + " xp";

                rewardButton.onClick.RemoveAllListeners();
                rewardButton.onClick.AddListener(() => currentSuccess.rewardGift = true);
                rewardButton.onClick.AddListener(() => Player_Stats.instance.AddExperience(currentSuccess.xpReward));
                rewardButton.onClick.AddListener(() => DisplaySuccessInformation(currentSuccess.successID));

            }
        }
        else
        {
            successObjective.text = Player_Success.instance.successObjectives[_successID].ToString() + " / " + currentSuccess.successObjective.ToString();
            successXpReward.text = currentSuccess.xpReward.ToString() + " xp";

            if (!rewardLabel.activeSelf)
                rewardLabel.SetActive(true);

            if (rewardButton.gameObject.activeSelf)
                rewardButton.gameObject.SetActive(false);
        }

        if (!objectiveLabel.activeSelf)
            objectiveLabel.SetActive(true);

        for (int i = 0; i < successButtons.Length; i++)
        {
            if (successButtons[i].config.successID == _successID)
            {
                successButtons[i].backgroundImage.enabled = true;
            }
            else
            {
                if (successButtons[i].backgroundImage.enabled)
                    successButtons[i].backgroundImage.enabled = false;
            }
        }

    }
}
