using UnityEngine;
using System.Collections;

public class AttackCollider : MonoBehaviour {

	PlayerAttack playerAttack;
	public float hurtForce = 10f;
	PhotonView photonView;

	// Use this for initialization
	void Awake () {
		playerAttack = GetComponentInParent<PlayerAttack> ();
		photonView = GetComponentInParent<PhotonView> ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		// If the colliding gameobject is an Enemy...

		if (photonView.isMine) {
			if (col.gameObject.tag == "Player") {	//If the player is currently attackingg....
				if (playerAttack.isAttacking) {
					//send the col object flying
					// Create a vector that's from the enemy to the player with an upwards boost.
					if (col.gameObject != transform.parent.gameObject) {
						col.gameObject.GetComponent<AttackedProperties> ().wasAttacked (photonView.viewID);
						PhotonNetwork.Instantiate ("rocketExplosion", transform.position, Quaternion.identity, 0);
				
						playerAttack.isAttacking = false;
					}
					//Vector3 hurtVector =  col.gameObject.transform.position - GetComponentInParent<Transform>().position  + Vector3.up * 5f;
					// Add a force to the player in the direction of the vector and multiply by the hurtForce.
					//col.gameObject.GetComponent<Rigidbody2D>().AddForce(hurtVector * hurtForce*10);
				}
			}
			if (col.gameObject.tag == "Pickable") {	//Destroy the pickable
				if (playerAttack.isAttacking) {
					
					col.gameObject.GetComponent<PickableObject> ().DestroySelf ();
					playerAttack.isAttacking = false;

				}
			}
		}
	}


}
