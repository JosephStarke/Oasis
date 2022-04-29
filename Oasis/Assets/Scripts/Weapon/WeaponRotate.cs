using UnityEngine;

public class WeaponRotate : MonoBehaviour
{
    //[SerializeField] private GameObject player;
    private void Awake()
    {
        //player = transform.parent.gameObject; //find the childs parent
    }

    private void FixedUpdate()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; //Position of arm pivot
        difference.Normalize();

        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ); //Circle rotation

        //make pivot flip when mouse is on the other side of the body

        if (rotationZ < -90 || rotationZ > 90) //between 90-270 deg, left side unit circle
        {
            transform.localRotation = Quaternion.Euler(180, 0, -rotationZ);
        }
    }
}
