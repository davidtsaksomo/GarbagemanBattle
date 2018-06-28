using UnityEngine;
using System.Collections;

public class AIAttack : MonoBehaviour {
	//-----------------------------------------------------------------------------------------------------------------//


	private AIControl AICtrl;		// Reference to the AIControl script.
	private Animator anim;					// Reference to the Animator component.

	//---------------------------------------------------------------------------------------------------------------//

	int touchCount;

	//[HideInInspector]
	public bool isAttacking = false;
	public float attackrange = 1f;
	public bool debugControl;
	public float timeBetweenAttack = 0.3f;
	float lastAttackTime = 0f;
	[SerializeField] float wornOutTime = 10f;
	[HideInInspector]
	public int playerID;
	[HideInInspector]
	public bool hitSuccess = false;
	[SerializeField] GameObject jetpack;
	//WeaponinHeld
	bool isHoldingWeapon;
	string weaponInHeld;
	//reference to the weapons
	GameObject weapon;
	[SerializeField] GameObject sword;
	[SerializeField] GameObject bazooka;
	[SerializeField] GameObject baseball;
	RaycastHit2D hitrange;
	PhotonView photonView;
	AILogic ailo;
	AttackedProperties atcpro;
	//blinking var
	float blinkingTime = 0.05f;
	public bool isJetpack;
	GameObject[] players;
	//------------------------------------------------------------------------------------------------------------------------
	void Awake()
	{
		// Setting up the references.
		photonView = GetComponent<PhotonView> ();

			anim = transform.root.gameObject.GetComponent<Animator> ();
			AICtrl = transform.root.GetComponent<AIControl> ();
		atcpro = GetComponent<AttackedProperties> ();
		ailo = GetComponent<AILogic> ();
			//Set to default weapon: BareHand
			weaponInHeld = "BareHand";

	}

	// Use this for initialization
	void Start () {
		players = GameObject.FindGameObjectsWithTag ("Player");

		
	}

	// Update is called once per frame
	void Update () {
		//use for debugging. DONT FORGET TO DEACTIVATE IN PROD
	if (weaponInHeld == "Bazooka") {
			foreach (GameObject ply in players) {
				if (ply != gameObject) {
					if (Vector2.Distance (ply.transform.position, transform.position) < 20f) {
						attack ();
					}
				}
			}

		} 
		


	}


	public void attack(){
		if(Time.time > lastAttackTime + timeBetweenAttack && atcpro.isVurnerable){
			//set the attack animation trigger
			anim.SetTrigger (weaponInHeld);
			if (weaponInHeld == "Bazooka") { 
				bazooka.GetComponent<Gun> ().fire ();
			} else {
				isAttacking = true;
				Invoke ("attackEnd", timeBetweenAttack);
			}



			//upthe last attack time
			lastAttackTime = Time.time;
		}

	}


	void attackEnd(){
		isAttacking = false;
	}
	//End of attacking function
	//--------------------------------------------------------------------------------------------------

	//Weapon Activator
	void OnCollisionEnter2D (Collision2D col){

		if (col.gameObject.tag == "Pickable") {


			if (col.gameObject.name == "Jetpack(Clone)" || col.gameObject.name == "Jetpack") {
				if (!isJetpack) {
					photonView.RPC ("TakeJetpack", PhotonTargets.All);
					col.gameObject.GetComponent<PickableObject> ().DestroySelf ();

				}

			} else {
				if (!isHoldingWeapon) {

					string weaponName = col.gameObject.name;
					photonView.RPC ("TakeWeapon", PhotonTargets.All, weaponName);
					col.gameObject.GetComponent<PickableObject> ().DestroySelf ();

				} else {
					attack ();
				}

			}

		}

	//	if (col.gameObject.tag == "Player") {
	//		attack();
	////	}

	}

	[PunRPC]
	void WeaponOut(){
		StopCoroutine ("Blinking");
		weapon.GetComponent<SpriteRenderer> ().color = Color.white;

		isHoldingWeapon = false;

		weapon.SetActive (false);
		timeBetweenAttack = 0.3f;
		weaponInHeld = "BareHand";
		attackrange = 1f;
	}
	[PunRPC] 
	void TakeWeapon(string weaponName){

		string weapToHold = "";
		switch (weaponName) {
		case "Sword(Clone)":
		case "Sword":
			//GetSwordAsWeapon
			weapToHold = "Sword";
			weapon = sword;
			attackrange = 3f;
			timeBetweenAttack = 0.3f;
			break;
		case "Baseball(Clone)":
		case "Baseball":
			//GetSwordAsWeapon
			weapToHold = "Baseball";
			weapon = baseball;
			timeBetweenAttack = 0.6f;
			attackrange = 3f;
			break;
		case "Bazooka(Clone)":
		case "Bazooka":
			//GetSwordAsWeapon
			weapToHold = "Bazooka";
			weapon = bazooka;
			timeBetweenAttack = 0.8f;
			break;

		default:
			break;
		}
		weapon.SetActive (true);
		weaponInHeld = weapToHold;
		isHoldingWeapon = true;
		ailo.PathEnd ();
		Invoke ("ToCallWeaponOut", wornOutTime);
		StartCoroutine (Blinking (wornOutTime - 2f));

	}
	void ToCallWeaponOut(){
		photonView.RPC ("WeaponOut", PhotonTargets.All);
	}

	IEnumerator Blinking(float delay) {
		//clear color every sprite renderer
		if (delay > 0) {
			yield return new WaitForSeconds (delay);
		}
		SpriteRenderer sprite = weapon.GetComponent<SpriteRenderer> ();
		sprite.color = Color.clear;


		//wait 0.1 s
		yield return new WaitForSeconds(blinkingTime);
		//turn normal again
		sprite.color = Color.white;


		//wait again
		yield return new WaitForSeconds(blinkingTime);

		//repeat until vurnerable again
		if(isHoldingWeapon)
		StartCoroutine (Blinking(0f));

	}
	[PunRPC]
	void TakeJetpack(){
		isJetpack = true;
		jetpack.SetActive (true);
		Invoke ("ToCallJetpackOut", wornOutTime);
		StartCoroutine (JetpackBlinking (wornOutTime - 2f));

	}

	void ToCallJetpackOut()
	{
		photonView.RPC ("JetpackOut", PhotonTargets.All);

	}

	[PunRPC]
	void JetpackOut(){
		StopCoroutine ("JetpackBlinking");
		isJetpack = false;
		jetpack.GetComponent<SpriteRenderer> ().color = Color.white;
		jetpack.SetActive (false);

	}

	IEnumerator JetpackBlinking(float delay) {
		//clear color every sprite renderer
		if (delay > 0) {
			yield return new WaitForSeconds (delay);
		}
		SpriteRenderer sprite = jetpack.GetComponent<SpriteRenderer> ();
		sprite.color = Color.clear;


		//wait 0.1 s
		yield return new WaitForSeconds(blinkingTime);
		//turn normal again
		sprite.color = Color.white;


		//wait again
		yield return new WaitForSeconds(blinkingTime);

		//repeat until vurnerable again
		if(isJetpack)
			StartCoroutine (JetpackBlinking(0f));

	}

}
