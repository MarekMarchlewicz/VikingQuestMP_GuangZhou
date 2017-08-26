using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    private static SpawnPointManager instance;

    [SerializeField]
    private List<Transform> attackersSpawnPoints;

    [SerializeField]
    private List<Transform> defendersSpawnPoints;

    [SerializeField]
    private Transform defaultSpawnPoint;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Destroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }

    public static Transform GetSpawnPoint(TeamType teamtype)
    {
        Transform spawnPoint = instance.defaultSpawnPoint;

        if(teamtype == TeamType.Attackers)
        {
            if(instance.attackersSpawnPoints.Count > 0)
            {
                int choosenPoint = Random.Range(0, instance.attackersSpawnPoints.Count);

                spawnPoint = instance.attackersSpawnPoints[choosenPoint];

                instance.attackersSpawnPoints.RemoveAt(choosenPoint);
            }
        }
        else if(teamtype == TeamType.Defenders)
        {
            int choosenPoint = Random.Range(0, instance.defendersSpawnPoints.Count);

            spawnPoint = instance.defendersSpawnPoints[choosenPoint];

            instance.defendersSpawnPoints.RemoveAt(choosenPoint);
        }

        return spawnPoint;
    }
}
