using UnityEngine;

public class SetDarkKingCreditText : MonoBehaviour
{
    [SerializeField] GameObject textPanel;

    // Start is called before the first frame update
    void Start()
    {
        if (textPanel.activeSelf)
            textPanel.SetActive(false);
    }

    public void ActiveTextPanel()
    {
        if (!textPanel.activeSelf)
            textPanel.SetActive(true);
    }
}
