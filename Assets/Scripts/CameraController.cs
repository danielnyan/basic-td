using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 1f; 

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            transform.rotation *= Quaternion.AngleAxis(sensitivity * Input.GetAxis("Mouse X"), transform.up);
        }
    }
}
