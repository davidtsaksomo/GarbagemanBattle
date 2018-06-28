using UnityEngine;
using System.Collections;

public class AIAttackCollider : MonoBehaviour {

	AIAttack AIAttack;
	public float hurtForce = 10f;
	PhotonView photonView;

	// Use this for initialization
	void Awake () {
		AIAttack = GetComponentInParent<AIAttack> ();
		photonView = GetComponentInParent<PhotonView> ();

	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter2D (Collider2D col)
	{
		// If the colliding gameobject is an Enemy...
	
		//&& col.gameObject != transform.parent.gameObject
		if (col.gameObject.tag == "Player" && col.gameObject != transform.parent.gameObject) {	//If the player is currently attackingg....
			if (AIAttack.isAttacking) {
				//send the col object flying
				// Create a vector that's from the enemy to the player with an upwards boost.
					col.gameObject.GetComponent<AttackedProperties> ().wasAttacked (photonView.viewID);
				PhotonNetwork.Instantiate ("rocketExplosion", transform.position, Quaternion.identity, 0);
					AILogic ailo = col.gameObject.GetComponent<AILogic> ();
					if (ailo) {
					ailo.Attacked (this.gameObject);
					}
					AIAttack.isAttacking = false;

				//Vector3 hurtVector =  col.gameObject.transform.position - GetComponentInParent<Transform>().position  + Vector3.up * 5f;
				// Add a force to the player in the direction of the vector and multiply by the hurtForce.
				//col.gameObject.GetComponent<Rigidbody2D>().AddForce(hurtVector * hurtForce*10);
			} else {
				AIAttack.attack ();
			}
		} else
			if (col.gameObject.tag == "Pickable") {	//Destroy the pickable
				if (AIAttack.isAttacking) {

					col.gameObject.GetComponent<PickableObject> ().DestroySelf ();
					AIAttack.isAttacking = false;

				}
			}
		
	}


}
