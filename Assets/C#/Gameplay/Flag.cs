using UnityEngine;
using System.Collections;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class Flag : MonoBehaviour {
	[SerializeField]
	GameObject gameManager;
	GameObject requester;
	public bool isHandled = false;
	public bool isvirgin = true;
	bool destroyable = true;
	float maxvelo =30f;
	bool mode;
	int stuck = 0;
	// Use this for initialization
	void Awake(){


	}
	void Start () {
		mode = (string)PhotonNetwork.room.customProperties ["Mode"] == "C";
		gameManager = GameObject.FindGameObjectWithTag ("MainCamera");
		destroyable = false;
		StartCoroutine ("AntiStuck");
		//this.gameObject.GetPhotonView ().RPC ("Destroyable", PhotonTargets.AllBuffered);

	}
	[PunRPC]
	void IsHandled(bool handled){
		isHandled = handled;
	}
	[PunRPC]
	void IsVirgin(bool virgin){
		isvirgin = virgin;
	}
	// Update is called once per frame
//	[PunRPC]
//	void Taked(bool handled, int requesterID){
//		if (PhotonNetwork.isMasterClient) {
//			if (handled && !isHandled) {
				
			//	GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

			//	foreach (GameObject player in players) {
			//		if (player.GetComponent<PhotonView> ().viewID == requesterID) {
			//			requester = player;

				//	}
			//	}
			//	if (requester)
			//	requester.GetPhotonView().RPC ("gainFlag", PhotonTargets.AllViaServer);

//				isHandled = true;
//				gameObject.GetPhotonView().RPC ("IsHandled", PhotonTargets.All, handled );
				//stayPosition ();
			

//			} else if (!handled && isHandled) {
	//			gameObject.GetPhotonView().RPC ("IsHandled", PhotonTargets.All, handled );
	//		}
//		}
//	}
	void OnCollisionStay2D(Collision2D st){
		stuck++;
	}
	void Update(){
		if (GetComponent<PhotonView> ().isMine&&GetComponent<Rigidbody2D> ().velocity.magnitude > maxvelo) {
			float velx, vely;
			velx = GetComponent<Rigidbody2D> ().velocity.x / GetComponent<Rigidbody2D> ().velocity.magnitude * maxvelo;
			vely = GetComponent<Rigidbody2D> ().velocity.y / GetComponent<Rigidbody2D> ().velocity.magnitude * maxvelo;
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (velx, vely);
		}
		if(isHandled && (int)PhotonNetwork.room.customProperties ["FlagOwner"] != -9999)
		{
			GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		}
	//	print (GetComponent<PhotonView> ().isMine);
	//	if (isHandled && GetComponent<Rigidbody2D> ().velocity.magnitude >0) {
	//		GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
	//	}

		
	}
	[PunRPC]
	void Destroyable(){
		destroyable = false;
	}
	void OnPhotonCustomRoomPropertiesChanged(PhotonHashTable propertiesThatChanged){
		if (propertiesThatChanged.ContainsKey ("FlagOwner")) {
			if ((int)PhotonNetwork.room.customProperties ["FlagOwner"] != -9999) {
				if (PhotonNetwork.isMasterClient && isvirgin) {
					gameObject.GetPhotonView ().RPC ("IsVirgin", PhotonTargets.All, false);
					if (!PhotonNetwork.offlineMode) {
						if(mode)
							gameManager.GetComponent<CoopGameManager> ().SpawnBase ();
						else
						gameManager.GetComponent<GameManager> ().SpawnBase ();
					} else {
						gameManager.GetComponent<OfflineGameManager> ().SpawnBase ();
					}

				}
			}
		}
	}
	IEnumerator AntiStuck(){
		if (stuck > 500) {
			gameObject.transform.position = new Vector3 (135f, 5.5f, 0);
		}
		stuck = 0;
		yield return new WaitForSeconds (5f);
		StartCoroutine ("AntiStuck");
	}
}
