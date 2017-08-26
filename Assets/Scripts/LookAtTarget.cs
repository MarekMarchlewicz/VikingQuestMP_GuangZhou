//This script is used to make UI elements face the player's camera

using UnityEngine;

public class LookAtTarget : MonoBehaviour 
{
	Transform target;	//The camera's transform


	void Start()
	{
		//Set the Main Camera as the target
		target = Camera.main.transform;
	}

	//Update after all other updates have run
	void LateUpdate()
	{
		//Apply the rotation needed to look at the camera. Note, since pointing a UI text element
		//at the camera makes it appear backwards, we are actually pointing this object
		//directly *away* from the camera.
		transform.rotation = Quaternion.LookRotation (transform.position - target.position);
	}
}
