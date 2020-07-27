using UnityEngine;
using UnityEngine.UI;

public class UI_Map : MonoBehaviour
{
    // Forest scenes range : 6 - 10
    // Beach scenes range : 16 - 21

    [SerializeField] Sprite gameMapForest;
    [SerializeField] Sprite gameMapBeach;
    //[SerializeField] Sprite gameMapVolcano;

    [SerializeField] GameObject noMapText;
    [SerializeField] GameObject gameMapGO;
    [SerializeField] Image gameMapImage;
    
    // Start is called before the first frame update
    void Start()
    {
        SetMap();
    }

    private void OnEnable()
    {
        SetMap();
    }

    void SetMap()
    {
        if (Scenes_Control.instance)
        {
            if (Scenes_Control.instance.GetCurrentSceneBuildIndex() >= 6 && Scenes_Control.instance.GetCurrentSceneBuildIndex() <= 10)
            {
                // Display forest map
                noMapText.SetActive(false);
                gameMapImage.sprite = gameMapForest;
                gameMapGO.SetActive(true);
            }
            else if (Scenes_Control.instance.GetCurrentSceneBuildIndex() >= 16 && Scenes_Control.instance.GetCurrentSceneBuildIndex() <= 21)
            {
                // Display beach map
                noMapText.SetActive(false);
                gameMapImage.sprite = gameMapBeach;
                gameMapGO.SetActive(true);
            }
            else
            {
                // Display text no map
                noMapText.SetActive(true);
                gameMapGO.SetActive(false);
            }
        }
    }
}
