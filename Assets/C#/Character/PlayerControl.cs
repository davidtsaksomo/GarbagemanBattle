using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class PlayerControl : MonoBehaviour
{	
	//-----------------------------------------------------------------------------------------------------//
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.


	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	public AudioClip[] jumpClips;
	public float flyForce = 50f;// Array of clips for when the player jumps.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.
	public AudioClip[] taunts;				// Array of clips for when the player taunts.
	public float tauntProbability = 50f;	// Chance of a taunt happening.
	public float tauntDelay = 1f;			// Delay for when the taunt should happen.


	private int tauntIndex;					// The index of the taunts array indicating the most recent taunt.
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.
	private Animator anim;					// Reference to the player's animator component.
	PhotonView photonView;
	//debug properties to control
	public bool ToControl;
	[SerializeField]
	GameObject playerName;
	[SerializeField]
	GameObject arrow; // GreenArrowThingy

	//-----------------------------------------------------------------------------------------------------------//

	bool isMine;
	bool jumping;
	float h;
	AttackedProperties atcpro;
	float maxvelo =25f;
	//==============================================================================================================//

	void Awake()
	{	
		//checking if isMIne
		isMine = GetComponent<PhotonView>().isMine;
		photonView = GetComponent<PhotonView> ();
		atcpro = GetComponent<AttackedProperties> ();
		//setting camera target
		if (isMine) {
			GameObject.Find ("mainCamera").GetComponent<CameraFollow> ().player = this.transform;
			playerName.SetActive (false);
		
			// Setting up references.
			groundCheck = transform.Find ("groundCheck");
			anim = GetComponent<Animator> ();
			PhotonNetwork.player.SetScore (PlayerPrefs.GetInt ("Score", 0));
			if (PhotonNetwork.offlineMode) {
				PhotonNetwork.player.SetScore (0);
			
			}
		}
		if (playerName) {
			playerName.GetComponent<Text> ().text = photonView.owner.name;
		}


	}
	void Start(){
		if (isMine) {
			PlayerPrefs.SetInt ("OwnerID", photonView.viewID);
		}
	}

	//------------------------------------------------------------------------------------------------------------//
	void Update()
	{	
		//Limit the max velocity
		if (isMine) {

			if (atcpro.isVurnerable)
				h = ButtonManager.horAxis;
			else
				h = 0;
			#if UNITY_EDITOR
			if (atcpro.isVurnerable )
				h = Input.GetAxis ("Horizontal");
			else
				h = 0;
			#endif




			if (GetComponent<Rigidbody2D> ().velocity.magnitude > maxvelo) {
				float velx, vely;
				velx = GetComponent<Rigidbody2D> ().velocity.x / GetComponent<Rigidbody2D> ().velocity.magnitude * maxvelo;
				vely = GetComponent<Rigidbody2D> ().velocity.y / GetComponent<Rigidbody2D> ().velocity.magnitude * maxvelo;
				GetComponent<Rigidbody2D> ().velocity = new Vector2 (velx, vely);
			}

			if (GetComponent<Rigidbody2D> ().velocity.y > 25) {
				GetComponent<Rigidbody2D> ().velocity =  new Vector2 (0f, 23);

			}

		

		//debug so you can edit which you control via inspector
		//isMine = ToControl;
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.


		// If the jump button is pressed and the player is grounded then the player should jump.
			if (atcpro.isVurnerable) {
				if (GetComponent<PlayerAttack> ().isJetpack) {
					if ((Input.GetButton ("Jump") || ButtonManager.jumpHold)) {
						Fly ();

					}
				} else if ((Input.GetButtonDown ("Jump") || ButtonManager.jump)) {
			

					if (grounded) {
						Jump ();
					} else {
						jumping = true;
						Invoke ("NotJump", 0.25f);

					}
			
				}
				if (grounded && jumping && !GetComponent<PlayerAttack> ().isJetpack) {
					Jump ();
					jumping = false;
				}
			}
				ButtonManager.jump = false;

		}
	}
	void NotJump(){jumping = false;}
	//---------------------------------------------------------------------------------------------------------------//
	void FixedUpdate ()
	{	if (isMine) {
			
			grounded = (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")))/* || (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Player")))*/;  
			// Input Code--------------------------------------------------------------------------------------------//


			//Android Control


			//------------------------------------------------------------------------------------------------------//


			// The Speed animator parameter is set to the absolute value of the horizontal input.
			anim.SetFloat ("Speed", Mathf.Abs (h));

			// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
			if (h * GetComponent<Rigidbody2D> ().velocity.x < maxSpeed)
			// ... add a force to the player.
			GetComponent<Rigidbody2D> ().AddForce (Vector2.right * h * moveForce);

			// If the player's horizontal velocity is greater than the maxSpeed...
			if (Mathf.Abs (GetComponent<Rigidbody2D> ().velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (Mathf.Sign (GetComponent<Rigidbody2D> ().velocity.x) * maxSpeed, GetComponent<Rigidbody2D> ().velocity.y);

			// If the input is moving the player right and the player is facing left...
			if (h > 0 && !facingRight)
			// ... flip the player.
			Flip ();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (h < 0 && facingRight)
			// ... flip the player.
			Flip ();

			// If the player should jump...

		}
		else //!isMine
		{ 
			if(facingRight)
			{
				if (GetComponent<Rigidbody2D> ().velocity.x < -1f) {
					Flip ();
				}
			}
			else //facingleft
			{
			if (GetComponent<Rigidbody2D> ().velocity.x > 1f) {
				Flip ();
			}
			}

		}
	}

	void Jump(){
		
			// Set the Jump animator trigger parameter.

			anim.SetTrigger ("Jump");

			// Play a random jump audio clip.
			int i = Random.Range (0, jumpClips.Length);
			AudioSource.PlayClipAtPoint (jumpClips [i], transform.position);

			// Add a vertical force to the player.
		GetComponent<Rigidbody2D> ().AddForce(new Vector2 (0f, jumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;

	}

	void Fly(){
		GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0f, flyForce));



	}
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;

		Vector3 arrowScale = arrow.transform.localScale;
		theScale.x *= -1;
		arrowScale.x *= -1;
	//	arrowScale.y *= -1;
	//	arrowScale.z *= -1;
		transform.localScale = theScale;
		arrow.transform.localScale = arrowScale;

	}


	public IEnumerator Taunt()
	{
		// Check the random chance of taunting.
		float tauntChance = Random.Range(0f, 100f);
		if (tauntChance > tauntProbability) {
			// Wait for tauntDelay number of seconds.
			yield return new WaitForSeconds (tauntDelay);

			// If there is no clip currently playing.
			if (!GetComponent<AudioSource> ().isPlaying) {
				// Choose a random, but different taunt.
				tauntIndex = TauntRandom ();

				// Play the new taunt.
				GetComponent<AudioSource> ().clip = taunts [tauntIndex];
				GetComponent<AudioSource> ().Play ();
			}
		} 

	}


	int TauntRandom()
	{
		// Choose a random index of the taunts array.
		int i = Random.Range(0, taunts.Length);

		// If it's the same as the previous taunt...
		if(i == tauntIndex)
			// ... try another random taunt.
			return TauntRandom();
		else
			// Otherwise return this index.
			return i;
	}
	void OnDisconnectedFromPhoton(){
		Destroy (gameObject);

	}
	[PunRPC]
	public void StateChange(int st){
	}
}
