using UnityEngine;

public class SendPlayerToCreditScene : MonoBehaviour
{
    private void Start()
    {
        if (Scenes_Control.instance)
        {
            if (Scenes_Control.instance.GetCurrentSceneBuildIndex() != Scenes_Control.finalLevelBuildIndex)
            {
                Destroy(this);
            }
        }
    }

    public void GoToCreditScene()
    {
        if (Scenes_Control.instance)
        {
            Scenes_Control.instance.StartTransitionToCreditScene();
        }
    }
}
