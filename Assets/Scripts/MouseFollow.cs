using UnityEngine;
using UnityEngine.VR;

public class MouseFollow : MonoBehaviour
{
	[SerializeField] private float mouseSensitivity = 100.0f;
	[SerializeField] private float clampAngle = 80.0f;

    [SerializeField]
    private bool hideCursor = false;

	private Vector3 currentRotation;
    
    private void Start ()
    {
		currentRotation = transform.localRotation.eulerAngles;
    }
    
    private void Update ()
    {
        if (hideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (VRSettings.enabled)
			Destroy (this);
		
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        
		currentRotation.y += mouseX * mouseSensitivity * Time.deltaTime;
		currentRotation.x += mouseY * mouseSensitivity * Time.deltaTime;
        
		currentRotation.x = Mathf.Clamp(currentRotation.x, -clampAngle, clampAngle);
        
		Quaternion localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0.0f);
        transform.rotation = localRotation;
    }
}
