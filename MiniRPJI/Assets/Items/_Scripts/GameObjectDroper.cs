/* GameObjectDroper.cs
 * 
 * Permet de droper un ou plusieurs prefabs.
 * 
 * Exemple d'utilisation : Droper une abilitée(ancien systeme avant arbre d'abilités), un animal de compagnie...
 * 
 * 
 * */



using UnityEngine;

public class GameObjectDroper : MonoBehaviour
{
    [SerializeField] Transform[] dropZone = new Transform[2]; // 2 Transform child of this make a zone, then we use Random.Range between theirs 2 position to get randomly spawn position

    [SerializeField] GameObject[] prefabsToDrop;

    public void DropItems()
    {
        // Get the parent of all items gameobjects -> I simply let it because it'll clean up too and its ok to be in Items parent for dropped things.
        Transform parentGameObject = null;

        if (GameObject.Find("Items"))
        {
            parentGameObject = GameObject.Find("Items").transform;
        }

        // Drop each gameobjects
        for (int i = 0; i < prefabsToDrop.Length; i++)
        {
            if (prefabsToDrop[i] != null)
            {
                // Determine drop position
                Vector3 dropPose = new Vector3(Random.Range(dropZone[0].position.x, dropZone[1].position.x),
                                               Random.Range(dropZone[0].position.y, dropZone[1].position.y), 0f);

                GameObject droppedGO = Instantiate(prefabsToDrop[i], dropPose, Quaternion.identity);
            }         
        }  
    }
}
