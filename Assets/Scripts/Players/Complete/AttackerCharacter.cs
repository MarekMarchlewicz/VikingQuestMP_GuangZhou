//This script is used to control the player with keyboard / controller mechanics. This differs from
//the Player_ControllerMove script in that this script also allows the player to jump

using UnityEngine;

public class AttackerCharacter : ICharacter
{
	[SerializeField]
    private float movementSpeed = 5.0f;	//The speed of the player
	[SerializeField]
    private float turnSpeed = 1000f;		//The turn speed of the player
	[SerializeField]
    private float jumpForce = 6f;			//The force that the player jumps with
	[SerializeField]
    private LayerMask whatIsGround;     //A layer mask defining what layers constitute the ground
    [SerializeField]
    private Transform attackTransform;

    private Animator anim;          //A reference to the player's animator component
    private Rigidbody rigidBody;    //A reference to the player's rigidbody component
    private Vector3 playerInput;    //A vector3 to store the player's x, y, and z controller input
    private bool grounded = true;   //Is the player currently on the ground?

    private Transform cameraTransform;

	private Vector3 startPosition;
    
    private void Start()
    {
        if (!hasAuthority)
            return;

        //Get references to the player's rigidbody and animator components
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        cameraTransform = Camera.main.transform;

		startPosition = transform.position;

        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        Debug.Log("OnGameStateChanged " + newState.ToString());

        if(newState == GameState.Respawn)
        {
            anim.SetBool("Dead", false);

			transform.position = startPosition;
        }
    }

    private void Update()
	{
        if (!hasAuthority)
            return;

        //Did the player press the "Jump" button AND are they on the ground? NOTE: we check
        //for GetButtonDown() in the regular Update() instad of FixedUpdate() since FixedUpdate() 
        //runs slower and thus can miss our input. 
        if (Input.GetButtonDown ("Jump") && grounded) 
		{
			//Add a Y-axis force to the character
			rigidBody.AddForce (new Vector3 (0f, jumpForce, 0f), ForceMode.Impulse);
			//Tell the animator to play the jumping animation
			anim.SetTrigger ("Jump");
			//We are no longer on the ground
			grounded = false;
		}

		//Update the animator by telling it whether or not we are currently on the ground
		anim.SetBool ("Grounded", grounded);
	}

    private void FixedUpdate()
	{
        if (!hasAuthority)
            return;

        //Generate an imaginary sphere at the character's feet and see if it collides with anything on the ground layer. 
        //If it does, then we are on the ground. Otherwise we are not on the ground
        if (Physics.CheckSphere (transform.position, .1f, whatIsGround))
			grounded = true;
		else
			grounded = false;
		
		//If the GameManager exists AND it tells us that the game is over, leave
		if (GameManager.instance != null && GameManager.instance.CurrentGameState == GameState.Gameover) 
		{
			//Go back to our Idle animation by dropping the speed to 0
			anim.SetFloat ("Speed", 0f);
			return;
		}

		//Get the horizontal and vertical input (up/down/left/right arrows, WASD keys, controller analog stick, etc),
		//and store that input in our playerInput variable (there won't be any "y" input)
		playerInput.Set(Input.GetAxis("Horizontal"), 0f, Input.GetAxis ("Vertical"));

        //Adjust to camera space
        playerInput = Quaternion.Euler(Vector3.up * cameraTransform.rotation.eulerAngles.y) * playerInput;

        bool canRun = anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Run") || anim.GetCurrentAnimatorStateInfo(0).IsName("Jump");

		if (GameManager.instance.CurrentGameState != GameState.Playing)
			canRun = false;

        //Tell the animator the "speed" of our movement based on the magnitude 
        //of the vector (the numerical "value" of the vector)
        if (canRun)
            anim.SetFloat("Speed", playerInput.sqrMagnitude);
        else
            anim.SetFloat("Speed", 0f);
        
        //Get Attack state
        if(Input.GetMouseButtonDown(0))
        {
            anim.SetBool("Attack", true);

            RaycastHit hit;

            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);

            Debug.DrawRay(ray.origin, ray.direction * 2f, Color.blue, 20f);

            if (Physics.Raycast(ray, out hit, 2f))
            {
                DefenderCharacter tower = hit.collider.GetComponent<DefenderCharacter>();
                if(tower != null)
                {
                    tower.TakeDamage(5);
                }
            }

            CancelInvoke("AttackCooldown");
            Invoke("AttackCooldown", 0.5f);
        }
        
        //If there is no input from the player, we're done here and can leave
        if (playerInput == Vector3.zero)
			return;

		//Use the "Quaternion" class to determine the rotation we need to face the direction we want to go
		Quaternion newRotation = Quaternion.LookRotation (playerInput);  

		//If we need to turn and face a new direction, use the RotateTowards() method to turn quickly, but not
		//instantly (which looks better)
		if(rigidBody.rotation != newRotation) 
			rigidBody.rotation = Quaternion.RotateTowards(rigidBody.rotation, newRotation, turnSpeed * Time.deltaTime);

        if (canRun)
        {
            //We take our input, multiply it by our speed, and then multiply Time.deltaTime. We then
            //add this amount to our current position to get the new desired position. NOTE: We "normalize"
            //our input so that the player won't move faster going diagonolly. NOTE: multiplying the value
            //with Time.deltaTime ensures that everyone has the same gameplay regardless of the speed of their
            //computers or the physics settings of the game
            Vector3 newPosition = transform.position + playerInput.normalized * movementSpeed * Time.deltaTime;

            //Use the rigidbody to move to the new position. This is better than Transform.Translate since it means
            //the player will mvoe with physics and force instead of just "teleporting" to the new spot
            rigidBody.MovePosition(newPosition);
        }
    }

    public void Attack()
    {
        //if (!isServer)
        //    return;

        Collider[] colliders = Physics.OverlapSphere(attackTransform.position, 3f);

        foreach(Collider overlappedCollider in colliders)
        {
            DefenderCharacter defender = overlappedCollider.GetComponent<DefenderCharacter>();
            if(defender != null)
            {
                defender.TakeDamage(5);
            }
        }
    }

    private void AttackCooldown()
    {
        anim.SetBool("Attack", false);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (!hasAuthority)
            return;

        anim.SetTrigger("Hit");
    }
    
    protected override void OnDied()
    {
        if (!hasAuthority)
            return;

        anim.SetBool("Dead", true);
    }
}
