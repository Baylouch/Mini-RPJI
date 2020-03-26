using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Load_Button : MonoBehaviour
{
    [SerializeField] GameObject dataImage; // To display if there is a save on slot
    [SerializeField] GameObject noDataText; // To display if there isn't save on slot

    [SerializeField] Text playerLevelText;
    [SerializeField] Image playerMoneyImage;
    [SerializeField] Text playerMoneyText;

    public void SetNoDataButton()
    {
        noDataText.SetActive(true);
        dataImage.SetActive(false);

        if (playerLevelText)
            playerLevelText.gameObject.SetActive(false);
        if (playerMoneyImage)
            playerMoneyImage.gameObject.SetActive(false);
    }

    public void SetDataButton(int playerLevel, int playerMoney)
    {
        noDataText.SetActive(false);
        dataImage.SetActive(true);

        if (playerLevelText)
        {
            playerLevelText.text = "Lvl : " + playerLevel.ToString();
            playerLevelText.gameObject.SetActive(true);
        }
        
        if (playerMoneyText)
        {
            playerMoneyText.text = playerMoney.ToString();
            playerMoneyImage.gameObject.SetActive(true);
        }       
    }
}
