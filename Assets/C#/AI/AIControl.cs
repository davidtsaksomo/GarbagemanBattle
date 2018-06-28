using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Seeker))]
public class AIControl : MonoBehaviour
{	
	//-----------------------------------------------------------------------------------------------------//

	//FLAGS
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.
	private bool grounded = false;			// Whether or not the player is grounded.
	private bool ceilingCheck =false;
	private bool isJumping;
	[HideInInspector]
	public bool pathIsEnded = false;
    
	//ADJUSTABLE
	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.
	public float updateRate = 2f;			// How many times each second we will update our path
	public float maxhigh = 3f;
	float maxvelo =25f;
	public float nextWaypointDistance = 1;
	float flyForce = 50f;

	//REFERENCES
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	[SerializeField]
	private Animator anim;					// Reference to the player's animator component.
	PhotonView photonView;
	[SerializeField]
	public Text playerName;
	[SerializeField]
	GameObject arrow;
	public Transform target;
	private Seeker seeker;
	private Rigidbody2D rb;
	AttackedProperties atcpro;
	AILogic ailog;
	//OTHERS
	public Path path;
	float h;
	[SerializeField]
	private int currentWaypoint = 0;
	Vector3 prevPos;
	int countMov=0;
	public string name;
	public int score;
	void Awake()
	{	
		//checking if isMIne
		photonView = GetComponent<PhotonView> ();
		score = 0;

			// Setting up references.
		groundCheck = transform.Find ("groundCheck");
		anim = GetComponent<Animator> ();
	
		playerName.text = name;

	}
	void Start(){
	//	if (isMine) {
	//		PlayerPrefs.SetInt ("OwnerID", photonView.viewID);
	//		print (gameObject.name + " is mine");
	//	}
		GetComponent<FlagHandling> ().isAI =true;
		atcpro = GetComponent<AttackedProperties> ();
		ailog = GetComponent<AILogic> ();

		atcpro.isAI = true;
	//	InvokeRepeating ("AIRandomMove", 0f, 2f);

		seeker = GetComponent<Seeker>();
		rb = GetComponent<Rigidbody2D>();
		StartCoroutine ("CheckError");
		if (target == null) {
			//Debug.LogError ("No Player found? PANIC!");
			return;
		}


	}

	//------------------------------------------------------------------------------------------------------------//
	void Update()
	{	
		//Limit the max velocity
		if (GetComponent<Rigidbody2D> ().velocity.magnitude > maxvelo) {
			float velx, vely;
			velx = GetComponent<Rigidbody2D> ().velocity.x / GetComponent<Rigidbody2D> ().velocity.magnitude * maxvelo;
			vely = GetComponent<Rigidbody2D> ().velocity.y / GetComponent<Rigidbody2D> ().velocity.magnitude * maxvelo;
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (velx, vely);
		}


		// If the jump button is pressed and the player is grounded then the player should jump.
		//........................................................................................................AI DECISION//

	
	}

	public void UpdatePath () {
		if (target == null) {
			ailog.PathEnd ();

		} else {
			seeker.StartPath (transform.position, target.position, OnPathComplete);
		}
	
	}



	//---------------------------------------------------------------------------------------------------------------//
	void FixedUpdate ()
	{	


		
		grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));  
		ceilingCheck = Physics2D.Linecast (transform.position, new Vector2 (transform.position.x, transform.position.y + 5), 1 << LayerMask.NameToLayer ("Ground"));
	
		if (grounded && !jump && isJumping) {
			isJumping = false;
			CancelInvoke ("NotJumoing");
		}


		//	Physics2D.cast
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


	
		if (facingRight) {
			if (GetComponent<Rigidbody2D> ().velocity.x < -1f) {
				Flip ();
			}
		} else { //facingleft
			if (GetComponent<Rigidbody2D> ().velocity.x > 1f) {
				Flip ();
			}
		}
			


		//=================================================================================================================================
		if (target == null) {
			//TODO: Insert a player search here.
			return;
		}

		//TODO: Always look at player?

		if (path == null)
			return;

		if (currentWaypoint >= path.vectorPath.Count) {
			Vector3 rdir = target.position - transform.position;
			if (Mathf.Abs (rdir.x) < 1f && Mathf.Abs (rdir.y) < 1f) {
				if (rdir.x > 0)
					MoveRight ();
				if (rdir.x < 0)
					MoveLeft ();
			} else {
				if (Mathf.Abs (rdir.x) < 1f && Mathf.Abs (rdir.y) < 3f) {
					if (grounded && !isJumping) {

						Jump ();
					}
				}
					
			}
	
			//	Debug.Log ("End of path reached.");
			pathIsEnded = true;
			ailog.PathEnd ();
			return;
		}
		pathIsEnded = false;

		//Direction to the next waypoint
		Vector3 dir = (path.vectorPath [currentWaypoint] - transform.position);
		if (Mathf.Abs(dir.y) > 6f || Mathf.Abs(dir.x) > 6f) {
			UpdatePath ();
			return;
		}
	
		else
			if (GetComponent<AIAttack> ().isJetpack && !ceilingCheck && dir.y >0.1) {
				Fly ();
			}

			else
			if (!GetComponent<AIAttack>().isJetpack && dir.y > maxhigh &&!ceilingCheck) {

			if (grounded && !isJumping) {
				Jump ();

				rb.velocity = new Vector2 (0, rb.velocity.y);
				if (Mathf.Abs (dir.x) > 2f) {
					if (dir.x > 0)
						MoveRight ();
					if (dir.x < 0)
						MoveLeft ();
				} else {
					if (dir.x > 0)
						Invoke ("MoveRight", (4f - dir.x*2) * 0.15f);
					if (dir.x < 0)
						Invoke ("MoveLeft", (4f - dir.x*2)*0.15f);
					
				}
			} 
		} else if( !isJumping) {
			if (dir.x > 0)
				MoveRight ();
			if (dir.x < 0)
				MoveLeft ();
		}

		Vector3 dist = transform.position - path.vectorPath[currentWaypoint];
		if (Mathf.Abs(dist.x) < nextWaypointDistance &&Mathf.Abs(dist.y) < nextWaypointDistance*2f) {
			currentWaypoint++;
			Stop ();

			return;
		}
			
	}

	IEnumerator CheckError(){
		if (countMov > 10 ||transform.position == prevPos ) {
			UpdatePath ();
			if (transform.position == prevPos) {
				h = -h;
			}
		}


		prevPos = transform.position;
		countMov = 0;
		yield return new WaitForSeconds(0.5f);
		StartCoroutine ("CheckError");
	}

	void MoveLeft(){
		if(atcpro.isVurnerable )
		h = -1;
	
	}
	void MoveRight(){
		if(atcpro.isVurnerable )
		h = 1;

	}
	public void Stop(){
		
		h = 0;

	}
	void Jump(){

		// Set the Jump animator trigger parameter.
	
		if (!jump &&atcpro.isVurnerable) {
			anim.SetTrigger ("Jump");

			// Add a vertical force to the player.
			GetComponent<Rigidbody2D> ().velocity=  new Vector2(rb.velocity.x, 0);
			GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0f, jumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = true;
			Invoke ("NotJump", 0.1f);
			Invoke ("NotJumping", 3f);
			isJumping = true;
		}
	}
	void NotJump (){
		jump = false;
	}
	void NotJumping(){
		isJumping = false;
	}
	void Fly(){
		GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0f, flyForce));



	}

	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		countMov++;
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

	public void OnPathComplete (Path p) {
		if (!p.error) {
			path = p;

				currentWaypoint = 0;
			
		}
	}



	void OnDisconnectedFromPhoton(){
		Destroy (gameObject);

	}


}
