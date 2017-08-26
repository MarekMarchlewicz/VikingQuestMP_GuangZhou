using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField]
    private float lifetime = 5f;

    private float startTime;

    private bool isLive = true;

    private const int damage = 50;

    private void Start()
    {
        if (!isServer)
            return;

        startTime = Time.time;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //If the shell isn't live, leave. 
        if (!isLive)
            return;

        //The shell is going to explode and is no longer live
        isLive = false;

        //Show visual explosion
        GameObject explosion = Instantiate(explosionEffect, collision.contacts[0].point, Quaternion.identity) as GameObject;
        Destroy(explosion, 3f);

        //If this is not the server, leave. The above code doesn't need to be 
        //run only on the server since it only deals with the graphical explosion. Since
        //the code below handles actually harming other tanks, it should only be run on
        //the server

        if (!isServer)
            return;

        AttackerCharacter attacker = collision.collider.GetComponent<AttackerCharacter>();
        if(attacker != null)
        {
            attacker.TakeDamage(damage);
        }

        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    private void Update()
    {
        //If the bullet has been alive too long...
        if (Time.time > startTime + lifetime)
        {
            //...Destroy it on the network
            NetworkServer.Destroy(gameObject);
        }
    }
}
