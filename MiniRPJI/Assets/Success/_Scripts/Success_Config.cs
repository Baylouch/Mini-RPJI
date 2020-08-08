using UnityEngine;

[CreateAssetMenu(fileName = "SucessConfig", menuName = "ScriptableObjects/Success/SuccessConfig", order = 1)]
public class Success_Config : ScriptableObject
{
    public int successID;

    public Sprite successSprite;

    public string successTitle;

    [TextArea]
    public string successDescription;

    public bool isDone = false;
    public bool rewardGift = false;

    public int successObjective;

    public int xpReward;
}
