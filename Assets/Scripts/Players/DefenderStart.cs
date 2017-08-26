using UnityEngine;
using UnityEngine.Networking;

public class DefenderStart : NetworkBehaviour
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
        cameraTransform = Camera.main.transform;

        cameraTransform.position = cameraSocket.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBullet(Camera.main.transform.position, Camera.main.transform.rotation);
        }
    }
    
    private void SpawnBullet(Vector3 position, Quaternion rotation)
    {
        GameObject newBullet = Instantiate(bulletPrefab, position, rotation) as GameObject;
        newBullet.GetComponent<Rigidbody>().velocity = rotation * Vector3.forward * bulletInitialSpeed;
    }
}
