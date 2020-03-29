using UnityEngine;
using UnityEngine.UI;

public class Loading_Bar : MonoBehaviour
{
    [SerializeField] RectTransform loadingBar;

    [SerializeField] Text loadingText;

    public void SetLoadingBar(float percentage)
    {
        loadingBar.sizeDelta = new Vector2(percentage, loadingBar.sizeDelta.y);
    }

    public void SetLoadingText(float percentage)
    {
        loadingText.text = Mathf.RoundToInt(percentage).ToString() + " %";
    }
}
