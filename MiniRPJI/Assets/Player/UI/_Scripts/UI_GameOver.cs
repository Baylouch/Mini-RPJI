using UnityEngine;
using UnityEngine.UI;

public class UI_GameOver : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button backToMenuButton;

    private void OnEnable()
    {
        if (Scenes_Control.instance)
        {
            Time.timeScale = 0;

            if (continueButton)
            {
                Player_Health playerHealth = FindObjectOfType<Player_Health>();
                continueButton.onClick.AddListener(() => playerHealth.SetCurrentHealthPoints(playerHealth.GetTotalHealthPoints())); // Reset player health
                continueButton.onClick.AddListener(() => Scenes_Control.instance.SwitchToLevel1Start()); // Tp player to level_1.
                continueButton.onClick.AddListener(() => playerHealth.gameObject.SetActive(true)); // Active player
                continueButton.onClick.AddListener(() => FindObjectOfType<Player_Movement>().SetPlayerPosition(0)); // Set player position
                continueButton.onClick.AddListener(() => Camera.main.transform.position = playerHealth.transform.position); // Set camera on the player
                continueButton.onClick.AddListener(() => Camera.main.transform.SetParent(playerHealth.transform)); // Link camera to the player
                continueButton.onClick.AddListener(() => Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, - 10)); // Offset camera
                continueButton.onClick.AddListener(() => Camera.main.transform.localScale = Vector3.one); // Reset camera scale
                continueButton.onClick.AddListener(() => gameObject.SetActive(false)); // Hide Game Over UI
                // TODO maybe add a curse to the player ?
            }

            if (backToMenuButton)
            {
                backToMenuButton.onClick.AddListener(() => Scenes_Control.instance.ChangeLevel("Start_Menu"));              
            }           
        }

        int totalGameOverSounds = Sound_Manager.instance.asset.gameOver.Length;
        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.gameOver[Random.Range(0, totalGameOverSounds)]);
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}
