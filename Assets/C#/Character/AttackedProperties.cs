using UnityEngine;
using System.Collections;

public class AttackedProperties : MonoBehaviour {

	public bool isVurnerable = true;
	public float hurtForce = 10f;
	public float afterAttackTime =  2f;
	float blinkingTime = 0.05f;


	//-----------------------------------------------------------------------------------------------------------
	//collision properties
	GameObject attacker;
	Collider2D[] ownchildcols;
	Collider2D[] owncols;
	Collider2D[] attackercols;
	Collider2D[] attackerchildcols;
	GameObject parentAttacker;
	PhotonView photonView;
	public bool isAI = false;
	AIControl aictrl;
	bool ismine;
	GameObject attbutt;
	// Use this for initialization

	//-----------------------------------------------------------------------------------------------------------
	void Start(){
		attbutt = GameObject.Find ("fireButton");
		photonView = GetComponentInParent<PhotonView> ();
		aictrl = GetComponent<AIControl> ();
		ismine = GetComponent<PhotonView> ().isMine;
	}
	//player was attacked

	public void wasAttacked(int attackerID){
			//player is vurnerable
			if (isVurnerable) {
			//Attacked (attackerID);
			photonView.RPC ("Attacked", PhotonTargets.All, attackerID, false);
			isVurnerable = false;
			if (ismine && !isAI)
				attbutt.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0.5f);
			if (isAI) {
				aictrl.Stop ();
			}
			}

		
	}
	public void wasAttacked(bool rocket){
		//player is vurnerable
		if (isVurnerable) {
			//Attacked (attackerID);
			photonView.RPC ("Attacked", PhotonTargets.All, -5, rocket);
			isVurnerable = false;
			if(ismine && !isAI)
				attbutt.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0.5f);
			if (isAI) {
				aictrl.Stop ();
			}

		}


	}
	//-------------------------------------------------------------------------------------------
	[PunRPC]

	public void Attacked(int attackerID, bool rocket){
		//send this player flying
		if (!rocket) {
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
			foreach (GameObject player in players) {
				if (player.GetComponent<PhotonView> ().viewID == attackerID) {
					attacker = player;

				}
			}
		} else {
			attacker = gameObject;
		}
			if (attacker) {
			if (photonView.isMine) {
				Vector3 hurtVector = transform.position - attacker.transform.position + Vector3.up * 5f;
				GetComponent<Rigidbody2D> ().AddForce (hurtVector * hurtForce * 10);
			}
				//........................................................................................
			if (!rocket) {
				//store collision in variables
				ownchildcols = GetComponentsInChildren<Collider2D> ();
				owncols = GetComponents<Collider2D> ();
				attackercols = attacker.GetComponents<Collider2D> ();
				attackerchildcols = attacker.GetComponentsInChildren<Collider2D> ();

				//now ignore the collision between attacker and attacked
				foreach (Collider2D col in owncols) {
					foreach (Collider2D cols in attackercols) {
						Physics2D.IgnoreCollision (col, cols);
					}
					foreach (Collider2D cols in attackerchildcols) {
						Physics2D.IgnoreCollision (col, cols);
					}

				}
				foreach (Collider2D col in ownchildcols) {
					foreach (Collider2D cols in attackercols) {
						Physics2D.IgnoreCollision (col, cols);
					}
					foreach (Collider2D cols in attackerchildcols) {
						Physics2D.IgnoreCollision (col, cols);
					}

				}
			}
				//..............................................................................
				//flag properties
				if(GetComponent<FlagHandling>().isHandlingFlag == true){
				photonView.RPC ("releasingFlag", PhotonTargets.AllViaServer, attackerID, hurtForce);
				}

				//................................................................................
				isVurnerable = false;
			if(ismine && !isAI)
				attbutt.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0.5f);;
			if (isAI) {
				aictrl.Stop ();
			}
			if (!rocket) {

				Invoke ("AfterAttack", afterAttackTime);

			} else {
				Invoke ("AfterAttack2", afterAttackTime);

			}
				StartCoroutine ("Blinking");
			}
		

	}


	void AfterAttack(){
		//now reset the collision between attacker and attacked
		foreach (Collider2D col in owncols) {
			foreach (Collider2D cols in attackercols) {
				Physics2D.IgnoreCollision (col, cols, false);
			}
			foreach (Collider2D cols in attackerchildcols) {
				Physics2D.IgnoreCollision (col, cols, false);
			}

		}
		foreach (Collider2D col in ownchildcols) {
			foreach (Collider2D cols in attackercols) {
				Physics2D.IgnoreCollision (col, cols, false);
			}
			foreach (Collider2D cols in attackerchildcols) {
				Physics2D.IgnoreCollision (col, cols, false);
			}

		}
		//become vurnerable again
		isVurnerable = true;
		if(ismine && !isAI)
			attbutt.GetComponent<SpriteRenderer> ().color =  new Color (1, 1, 1, 0.9f);
	}
	void AfterAttack2(){
		
		isVurnerable = true;
		if(ismine && !isAI)
			attbutt.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0.9f);
	}

	IEnumerator Blinking() {
		//clear color every sprite renderer
		SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer sprite in sprites) {
			sprite.color = Color.clear;
			 
		}
		//wait 0.1 s
		yield return new WaitForSeconds(blinkingTime);
		//turn normal again
		foreach (SpriteRenderer sprite in sprites) {
			sprite.color = Color.white;

		}
		//wait again
		yield return new WaitForSeconds(blinkingTime);

		//repeat until vurnerable again
		if (!isVurnerable) {
			StartCoroutine ("Blinking");
		}

	}
}

