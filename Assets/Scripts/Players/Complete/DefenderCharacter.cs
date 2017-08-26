using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;

public class DefenderCharacter : ICharacter
{
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private Transform cameraSocket;

    [SerializeField]
    private float bulletInitialSpeed = 50f;

    private Transform cameraTransform;
    
    private void Start()
    {
        if(!hasAuthority)
            return;

		cameraTransform = Camera.main.transform.parent;

		cameraTransform.position = cameraSocket.position;
    }
    
    private void Update()
    {
        if (!hasAuthority)
            return;

		if (GameManager.instance.CurrentGameState != GameState.Playing)
			return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
			CmdSpawnBullet(Camera.main.transform.position, Camera.main.transform.rotation);
        }
    }

    [Command]
    private void CmdSpawnBullet(Vector3 position, Quaternion rotation)
    {
        GameObject newBullet = Instantiate(bulletPrefab, position, rotation) as GameObject;
        newBullet.GetComponent<Rigidbody>().velocity = rotation * Vector3.forward * bulletInitialSpeed;

        NetworkServer.Spawn(newBullet);
    }
}
