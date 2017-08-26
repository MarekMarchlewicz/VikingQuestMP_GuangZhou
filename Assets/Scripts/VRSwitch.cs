using UnityEngine;
using UnityEngine.VR;

public class VRSwitch : MonoBehaviour 
{
	[SerializeField] private bool enableVR;

	private void Start()
	{
		VRSettings.enabled = enableVR;
	}
}
