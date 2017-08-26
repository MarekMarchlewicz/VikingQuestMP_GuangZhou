//This script handles the logic and UI for our game. It controls how much time the player has,
//how many points they have scored, and it detects when the player has won or lost the game

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public enum GameState
{
    None,
    Respawn,
    Playing,
    Gameover
}

public class GameManager : NetworkBehaviour 
{
    public static event System.Action<GameState> OnGameStateChanged;

    public delegate void TeamHandler(TeamType teamType);
    [SyncEvent]
    public static event TeamHandler Event_OnTeamWon;

	//This class contains a public static reference to itself. This means that it 
	//will be accessible to other classes globally, even if they don't have a 
	//reference or link to it. 
	public static GameManager instance;

	[Header("Game Properties")]
    public float matchTime = 60f;//How long the player has to reach the goal

    [Header("UI Elements")]
	public Text messageText;			//The UI element that shows the amount of time

    private int attackersNum;
    private int defendersNum;

    private int attackersAlive = 0;
    private int defendersAlive = 0;

    private float timeAmount;

    //Note that method is called only when value changes
    [SyncVar(hook = "GameStateChanged")]
    private GameState currentGameState = GameState.None;

    public GameState CurrentGameState { get { return currentGameState; } }

    //Called from server (SyncVar)
    private void GameStateChanged(GameState newGameState)
    {
        currentGameState = newGameState;

        if (OnGameStateChanged != null)
        {
            OnGameStateChanged(newGameState);
        }

        switch(newGameState)
        {
            case GameState.Respawn:
                timeAmount = matchTime;
                messageText.text = "Respawn";
                break;
            case GameState.Playing:
                break;
            case GameState.Gameover:
                messageText.text = "GameOver";
                if (isServer)
                {
                    Start();
                }
                break;
        }
    }

    private void Awake()
	{
		//If there currently isn't a GameManager, make this the game manager. Otherwise,
		//destroy this object. We only want one GameManager
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}

    private void Start()
    {
        if(isServer)
            Invoke("Respawn", 2f);
    }

    private void Respawn()
    {
        currentGameState = GameState.None;
        currentGameState = GameState.Respawn;
        timeAmount = matchTime;

        Invoke("StartGame", 5f);
    }

    private void StartGame()
    {
        attackersAlive = attackersNum;
        defendersAlive = defendersNum;

        currentGameState = GameState.Playing;

        messageText.text = ((int)timeAmount).ToString();
    }

    private void GameFinished()
    {
        currentGameState = GameState.Gameover;
    }
    
	private void Update () 
	{
        if(currentGameState == GameState.Playing)
        {
            UpdateTime();
            CheckIfTeamWon();
        }
	}

    private void UpdateTime()
    {
        //Update the UI to show the remaining time
        messageText.text = ((int)timeAmount).ToString("00");

        timeAmount -= Time.deltaTime;

        if (timeAmount <= 0f)
        {
            timeAmount = 0f;

            if (isServer)
            {
                if (Event_OnTeamWon != null)
                {
                    Event_OnTeamWon(TeamType.Defenders);
                }

				Debug.Log ("Time's over");

                currentGameState = GameState.Gameover;
            }
        }
    }

    private void CheckIfTeamWon()
    {
        if(isServer)
        {
            if (attackersAlive <= 0)
            {
                if (Event_OnTeamWon != null)
                {
                    Event_OnTeamWon(TeamType.Defenders);
                }

				Debug.Log ("Attackers defeated");

                currentGameState = GameState.Gameover;
            }
            else if(defendersAlive <= 0)
            {
                if (Event_OnTeamWon != null)
                {
                    Event_OnTeamWon(TeamType.Attackers);
                }

				Debug.Log ("Defenders defeated");

                currentGameState = GameState.Gameover;
            }
        }
    }

    [Command]
    public void Cmd_PlayerJoined(TeamType team)
    {
        if (team == TeamType.Attackers)
            attackersNum++;
        else if (team == TeamType.Defenders)
            defendersNum++;
    }

    [Command]
    public void Cmd_PlayerDied(TeamType team)
    {
        if (team == TeamType.Attackers)
            attackersAlive--;
        else if (team == TeamType.Defenders)
            defendersAlive--;
    }
}
