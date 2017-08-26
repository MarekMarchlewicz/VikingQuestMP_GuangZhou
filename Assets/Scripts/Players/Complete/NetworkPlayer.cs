using UnityEngine;
using UnityEngine.Networking;

public enum TeamType
{
    Attackers,
    Defenders
}

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField]
    private GameObject attackerPlayerObject;

    [SerializeField]
    private GameObject defenderPlayerObject;
    
    private Color playerColor;
    private TeamType playerTeam;
    private string playerName;

    private int health;
        
    private Transform spawnPoint = null;
    
    private GameObject playerObject;

    public void Initialize(TeamType newTeam, Color newColor, string newName)
    {
        playerTeam = newTeam;
        playerColor = newColor;
        playerName = newName;

		Debug.Log (playerColor.ToString());
    }

    private void Start()
    {
        if (!isServer)
            return;

        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        switch(newState)
        {
            case GameState.Respawn:
                TryToRespawn();
                break;
            case GameState.Playing:
                break;
            case GameState.Gameover:
                break;
        }
    }

    private void TryToRespawn()
    {
        if (spawnPoint == null)
        {
            Cmd_SpawnCharacter();
        }
        else
        {
            playerObject.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        }

        health = 100;

        playerObject.GetComponent<ICharacter>().Rpc_HealthChanged(health);
    }

    [Command]
    private void Cmd_SpawnCharacter()
    {
        spawnPoint = SpawnPointManager.GetSpawnPoint(playerTeam);

        if (playerTeam == TeamType.Attackers)
        {
            playerObject = Instantiate(attackerPlayerObject, spawnPoint.position, spawnPoint.rotation) as GameObject;
        }
        else if (playerTeam == TeamType.Defenders)
        {
            playerObject = Instantiate(defenderPlayerObject, spawnPoint.position, spawnPoint.rotation) as GameObject;
        }

        playerObject.name = playerName + "Character";
        playerObject.GetComponent<ICharacter>().OnTakenDamage += OnCharacterTakenDamage;

        GameManager.instance.Cmd_PlayerJoined(playerTeam);

        NetworkServer.SpawnWithClientAuthority(playerObject, gameObject);
    }

    private void OnCharacterTakenDamage(int damage)
    {
        if (!isServer)
            return;

        health -= damage;

        if (health < 0)
            health = 0;

        ICharacter character = playerObject.GetComponent<ICharacter>();

        character.Rpc_HealthChanged(health);

        if (health == 0)
        {
            character.Rpc_Die();

            GameManager.instance.Cmd_PlayerDied(playerTeam);
        }
    }
}
