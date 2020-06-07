/* MiniMapSettings.cs
 * 
 * Permet l'affichage sur la minimap de gameobject qui ont tous la même représentation dessus (ex: les ennemies).
 * Attaché ce script sur chaque gameobject qui le nécessite, puis mettre un prefab représentant ce gameobject sur la minimap.
 * 
 * */

using UnityEngine;

public class MiniMapSettings : MonoBehaviour
{
    [SerializeField] GameObject gfxMinimap;

    // Start is called before the first frame update
    void Start()
    {
        if (gfxMinimap != null)
        {
            GameObject gfx = Instantiate(gfxMinimap, this.gameObject.transform);
        }
    }
}
