using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {
	//-----------------------------------------------------------------------------------------------------------------//


	private PlayerControl playerCtrl;		// Reference to the PlayerControl script.
	private Animator anim;					// Reference to the Animator component.

	//---------------------------------------------------------------------------------------------------------------//

	RaycastHit2D[] hitinfo;
	bool isMine;
	int touchCount;

	//[HideInInspector]
	public bool isAttacking = false;

	public bool debugControl;
	public float timeBetweenAttack = 0.3f;
	float lastAttackTime = 0f;
	[SerializeField] float wornOutTime = 10f;
	[HideInInspector]
	public int playerID;
	[HideInInspector]
	public bool hitSuccess = false;
	//WeaponinHeld
	bool isHoldingWeapon;
	string weaponInHeld;
	//reference to the weapons
	GameObject weapon;
	[SerializeField] GameObject sword;
	[SerializeField] GameObject bazooka;
	[SerializeField] GameObject baseball;
	[SerializeField] GameObject jetpack;
	AttackedProperties atcpro;
	PhotonView photonView;


	[HideInInspector] public bool isJetpack =false;
	//blinking var
	float blinkingTime = 0.05f;

	//------------------------------------------------------------------------------------------------------------------------
	void Awake()
	{
		// Setting up the references.
		photonView = GetComponent<PhotonView> ();

		isMine = photonView.isMine;
		if (isMine) {
			anim = transform.root.gameObject.GetComponent<Animator> ();
			playerCtrl = transform.root.GetComponent<PlayerControl> ();
			atcpro = GetComponent<AttackedProperties> ();

			//Set to default weapon: BareHand
			weaponInHeld = "BareHand";
		}
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//use for debugging. DONT FORGET TO DEACTIVATE IN PROD
		//isMine = debugControl;


		//----------------------------------------------------------------------------------------------------------
		//check the button input
		#if UNITY_EDITOR
		// If the fire button is pressed...
		if(Input.GetButtonDown("Fire1")&& isMine && Input.touchCount == 0)
		{
			//If theres button input choose to attack

			//but check the time first
			if(Time.time > lastAttackTime + timeBetweenAttack&& atcpro.isVurnerable){
			attack ();

			}
		}
		#endif
		//Android button input
		if (ButtonManager.fire && isMine) {
			if(Time.time > lastAttackTime + timeBetweenAttack&& atcpro.isVurnerable){
				attack ();
			}
			ButtonManager.fire = false;
		}
	

		//----------------------------------------------------------------------------------------------------------------
	
	
	
	}
		

	void attack(){

		//set the attack animation trigger
		anim.SetTrigger (weaponInHeld);
		if (weaponInHeld == "Bazooka") { 
			bazooka.GetComponent<Gun> ().fire ();
		} else {
			isAttacking = true;

		}

		Invoke ("attackEnd", timeBetweenAttack);


		//upthe last attack time
		lastAttackTime = Time.time;
	}


	void attackEnd(){
		isAttacking = false;
	}
	//End of attacking function
	//--------------------------------------------------------------------------------------------------

	//Weapon Activator
	void OnCollisionEnter2D (Collision2D col){

		if (col.gameObject.tag == "Pickable" && isMine) {
				

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

				}
			}

		}
		
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
					break;
				case "Baseball(Clone)":
				case "Baseball":
							//GetSwordAsWeapon
					weapToHold = "Baseball";
					weapon = baseball;
					timeBetweenAttack = 0.6f;
					break;
				case "Bazooka(Clone)":
				case "Bazooka":
							//GetSwordAsWeapon
					weapToHold = "Bazooka";
					weapon = bazooka;
					timeBetweenAttack = 0.3f;
					break;

				default:
					break;
		}
		weapon.SetActive (true);
		weaponInHeld = weapToHold;
		isHoldingWeapon = true;

		Invoke ("ToCallWeaponOut", wornOutTime);
		StartCoroutine (Blinking (wornOutTime - 2f));

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

	void ToCallWeaponOut(){
		photonView.RPC ("WeaponOut", PhotonTargets.All);
	}

	[PunRPC]
	void WeaponOut(){
		StopCoroutine ("Blinking");
		weapon.GetComponent<SpriteRenderer> ().color = Color.white;

		isHoldingWeapon = false;

		weapon.SetActive (false);
		timeBetweenAttack = 0.3f;
		weaponInHeld = "BareHand";
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
