using UnityEngine;
using UnityEngine.UI;

public class UI_Cheats : MonoBehaviour
{
    public GameObject UI;
    [SerializeField] Text cheatText;

    private void Start()
    {
        cheatText.text = "";

        UI.SetActive(false);
    }

    public void UpdateCheatText(string newText)
    {
        cheatText.text = newText;
    }
}
