/* MakeProjectileRotate.cs
 * 
 * Simple script qui permet de faire tourner un projectile
 * 
 * 
 * 
 * */

using UnityEngine;

public class MakeProjectileRotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 50f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, 1 * rotationSpeed));
    }
}
