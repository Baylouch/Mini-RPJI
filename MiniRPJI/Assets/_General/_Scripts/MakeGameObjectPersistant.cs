using UnityEngine;

public class MakeGameObjectPersistant : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
