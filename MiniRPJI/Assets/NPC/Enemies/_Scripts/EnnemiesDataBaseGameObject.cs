/* EnnemiesDataBaseGameObject.cs
 * 
 * Simply use because i found no other place to nicely put on the EnnemiesDataBase.
 * 
 * Make this singleton and persistent
 * 
 * */

using UnityEngine;

public class EnnemiesDataBaseGameObject : MonoBehaviour
{
    public static EnnemiesDataBaseGameObject instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public EnnemiesDataBase database;
}
