using UnityEngine;
using System.Collections;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class FlagHandling : MonoBehaviour {


	public bool isHandlingFlag = false;
	public Transform flagPos;
	GameObject flag;
	GameObject attacker;
	GameObject gameManager;
	[SerializeField]
	GameObject arrow; //green arrow thingy
	PhotonView photonView;
	public GameObject flagPref;
	public bool isAI = false;

	bool isMine;
	PhotonHashTable expectedValue;
	PhotonHashTable valueToChange;
	[SerializeField]
	float distx;
	[SerializeField]
	float disty;
	[SerializeField]
	float distance;

	bool coop;
	// Use this for initialization
	void Awake(){
		
	}
	void Start () {
		flag = GameObject.FindGameObjectWithTag ("Flag");
		coop = (string)PhotonNetwork.room.customProperties ["Mode"] == "C";

		photonView = GetComponent<PhotonView> ();
		isMine = photonView.isMine;
		gameManager = GameObject.FindGameObjectWithTag ("MainCamera");
		valueToChange = new PhotonHashTable ();
		expectedValue = new PhotonHashTable ();
		valueToChange.Add ("FlagOwner", photonView.viewID);
		expectedValue.Add ("FlagOwner", -9999);
	}
	
	// Update is called once per frame
	void Update () {
		if (flag) {
			distx = Mathf.Abs (flag.transform.position.x - transform.position.x);
			disty = Mathf.Abs (flag.transform.position.y - transform.position.y);
			distance = Vector2.Distance (flag.transform.position, transform.position);
			if (gameObject.GetPhotonView().isMine && !isAI) {
				arrow.SetActive (true);
			}
		} else {
			if (photonView.isMine) {
				arrow.SetActive (false);
			}
			flag = GameObject.FindGameObjectWithTag ("Flag");
		}
	}

	void OnCollisionEnter2D (Collision2D col){
		//Col object is flag
		if (col.gameObject.tag == "Flag" && isMine &&  GetComponent<AttackedProperties>().isVurnerable) {
			flag = GameObject.FindGameObjectWithTag ("Flag");
			PhotonNetwork.room.SetCustomProperties (valueToChange, expectedValue, false);
			//if (!flag.GetComponent<Flag> ().isHandled) {
				//store in local variable

			//	flag.GetPhotonView().RPC ("Taked", PhotonTargets.AllViaServer, true, photonView.viewID);

			//}
		} 

	}
	void OnTriggerEnter2D (Collider2D col){
		if (col.gameObject.tag == "Fort" && isMine) {
			if (isHandlingFlag) {
				GetComponent<PhotonView>().RPC ("PutFlag", PhotonTargets.AllViaServer);

			}

		} 
	}
	[PunRPC]
	public void gainFlag(){
		flag = GameObject.FindGameObjectWithTag ("Flag");
		flag.GetComponent<Animator> ().SetBool ("Wavin", true);
			Collider2D[] cols = flag.GetComponents<Collider2D> ();

			//deactivate rigidbody component
			Rigidbody2D rb = flag.GetComponent<Rigidbody2D> ();
			rb.gravityScale = 0;
			rb.freezeRotation = false;
			rb.isKinematic = true;

			flag.transform.parent = flagPos.transform;

			foreach (Collider2D coll in cols) {
				coll.enabled = false;
			}
			PhotonView flagPhotonView = flag.GetComponent<PhotonView> ();

		if (gameObject.GetPhotonView().isMine) {
				flagPhotonView.RequestOwnership ();

			}
			flag.transform.localPosition = Vector3.zero;
			flag.transform.localRotation = Quaternion.Euler (Vector3.zero);
	
		if (PhotonNetwork.offlineMode) {

			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject ply in players) {
				AILogic ailog = ply.GetComponent<AILogic> ();
				if (ailog) {
					if (ply == this.gameObject) {
						ailog.StateChange (3);
					} else {
						ailog.StateChange (2);
					}
				}
			}

		}
		isHandlingFlag = true;
		arrow.GetComponent<FixedRotation> ().isHandling = true;
	}
	[PunRPC]
	void DestroyFlag(){
		GameObject[] flags;
		flags = GameObject.FindGameObjectsWithTag ("Flag");
		print (flags.Length);
		int x = 0;
		int i;
		for ( i = 1; i < flags.Length +x ; i++) {
	
			 
			//	if (!flags [i - 1].GetComponent<Rigidbody2D> ().isKinematic) {
					Destroy (flags [i - 1]);
					//print ("Destroy flag ke " + i);
			//	} else {
			//		x = 1;

			//	}


		}

	/*	photonView = gameObject.GetPhotonView ();
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject player in players) {
			if (player.GetComponent<FlagHandling> ().isHandlingFlag) {
			}
		}*/
			
	

	}

	[PunRPC]
	public void releasingFlag(int attackerID, float hurtForce){
		//make Flag normal again

		if (flag && isHandlingFlag) {
			flag.GetComponent<Animator> ().SetBool ("Wavin", false);

			if (attackerID != -5) {
				GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

				foreach (GameObject player in players) {
					if (player.GetComponent<PhotonView> ().viewID == attackerID) {
						attacker = player;

					}
				}
			} else {
				attacker = gameObject;
			}
			Rigidbody2D rb = flag.GetComponent<Rigidbody2D> ();
			rb.gravityScale = 1;
			rb.freezeRotation = true;
			rb.isKinematic = false;
			Collider2D[] cols = flag.GetComponents<Collider2D> ();
			foreach(Collider2D coll in cols) {
				coll.enabled = true;
			}
			//apply force to flag;
			if(photonView.isMine){
				photonView.RPC("NullParent",PhotonTargets.Others);
				flag.transform.parent = null;
			}
			if (flag.GetComponent<PhotonView> ().isMine) {
				Vector3 hurtVector = (transform.position - attacker.transform.position) / 2 + Vector3.up * 7f;

				flag.GetComponent<Rigidbody2D> ().AddForce (hurtVector * hurtForce * 10);
			}
		
			//flag.GetComponent<PhotonView> ().RPC ("Taked", PhotonTargets.AllViaServer, false,0);
			//if (photonView.isMine) {
			//	arrow.SetActive (false);
			//}
			if (PhotonNetwork.offlineMode) {
				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				foreach (GameObject ply in players) {
					AILogic ailog = ply.GetComponent<AILogic> ();
					if (ailog) {
							ailog.StateChange (1);
						
					}
				}
			}
			isHandlingFlag = false;
			arrow.GetComponent<FixedRotation> ().isHandling = false;

			PhotonNetwork.room.SetCustomProperties (expectedValue);
		}


	}
	[PunRPC]
	void NullParent (){
		if(flag)
		flag.transform.parent = null;
	}
	//-----------------------------------------------------------------------------------------------------//
	[PunRPC]
	void PutFlag(){
			
		//apply force to flag;
		if (!gameManager) {
			gameManager = GameObject.FindGameObjectWithTag ("MainCamera");
		}

		//gameManager.GetPhotonView ().RPC ("EndRound", PhotonTargets.AllViaServer, GetComponent<PhotonView>().owner);
		isHandlingFlag = false;
		arrow.GetComponent<FixedRotation> ().isHandling = false;


		if (flag) {
			GameObject fort = GameObject.FindGameObjectWithTag ("Fort");
			flag.transform.parent = fort.transform;

			flag.transform.localPosition = new Vector3(0,1.75f,0);
			flag.GetComponent<SpriteRenderer> ().sortingOrder = -2;
			flag.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
			//if (photonView.isMine) {
			//	arrow.SetActive (false);
			//}
		//Debug	flag.GetComponent<Flag> ().isHandled = false;
			flag.GetComponent<PhotonView> ().TransferOwnership (PhotonNetwork.masterClient);

			//GetComponent<PhotonView> ().RPC ("CallEndRound", PhotonTargets.MasterClient);
			isHandlingFlag = false;
			arrow.GetComponent<FixedRotation> ().isHandling = false;

			PhotonNetwork.room.SetCustomProperties (expectedValue);
			if (!PhotonNetwork.offlineMode) {
				if(coop)
					gameManager.GetComponent<CoopGameManager> ().CallEndRound (GetComponent<PhotonView> ().owner);
				else
					gameManager.GetComponent<GameManager> ().CallEndRound (GetComponent<PhotonView> ().owner);
			} else { 
				if (isAI)
					gameManager.GetComponent<OfflineGameManager> ().winnerrr = this.gameObject;
				else
					gameManager.GetComponent<OfflineGameManager> ().callbyply = true;
				
				gameManager.GetComponent<OfflineGameManager> ().CallEndRound (GetComponent<PhotonView> ().owner);

				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				foreach (GameObject ply in players) {
					AIControl aictrl = ply.GetComponent<AIControl> ();
					if (aictrl) {
						aictrl.target = null;

					}
				}
			}
		}
	}
	void LateUpdate(){
		
		if (isMine && isHandlingFlag) {
			flag.transform.localPosition = Vector3.zero;
			flag.transform.localRotation = Quaternion.Euler (Vector3.zero);
		}
	}

	void OnPhotonPlayerConnected (PhotonPlayer newPlayer){
		if (isMine) {
			if (isHandlingFlag) {
				photonView.RPC ("gainFlag", newPlayer);

			} 
			photonView.RPC ("DestroyFlag", newPlayer);
		}
		
	}
	void OnPhotonCustomRoomPropertiesChanged(PhotonHashTable propertiesThatChanged){
		if (propertiesThatChanged.ContainsKey ("FlagOwner")) {
			int id;
			if ((int)PhotonNetwork.room.customProperties ["FlagOwner"] != -9999) {
				id = (int)PhotonNetwork.room.customProperties ["FlagOwner"];
				GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
				foreach (GameObject player in players) {
					if (player.GetComponent<PhotonView> ().viewID == id) {
						player.GetComponent<FlagHandling> ().gainFlag ();

					}
				}
			}
		}
	}
	//[PunRPC]
	//void CallEndRound(){
		//gameManager.GetComponent<GameManager> ().EndRound (GetComponent<PhotonView>().owner);
	//}
}
