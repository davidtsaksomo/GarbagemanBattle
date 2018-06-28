using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class OfflineGameManager : MonoBehaviour {



	public GameObject[] spawner;
	GameObject[] fortSpawner;
	GameObject flagPrefab;
	int round = 1;
	Room room;
	[SerializeField]
	Text roundInfo;
	[SerializeField]
	Text[] playerInfo;
	[SerializeField]
	Text[] scoreInfo;
	[SerializeField]
	Abaaba aba;
	int flagInt;
	int fortInt;
	PhotonView photonView;
	bool complete = false;
	PhotonPlayer winnerr;
	public GameObject winnerrr = null;
	bool scoreAdded = false;
	PhotonHashTable roomProp;
	GameObject[] player;
	public bool callbyply = false;
	bool StatUpd = true;
	int cmp;
	//--------------------------------------------------------------------------------------------------
	// Use this for initialization
	void Awake(){
		if (!PhotonNetwork.offlineMode) {
			Destroy (this);
		}
		photonView = GetComponent<PhotonView> ();
		room = PhotonNetwork.room;
	}

	void Start () {
		PhotonNetwork.Instantiate (PlayerPrefs.GetString("Character","Hero"), new Vector3(Random.Range(132f,145f),5.5f,0), Quaternion.identity, 0);

		PhotonNetwork.Instantiate ("AI"+ PlayerPrefs.GetString("AI1","Hero"), new Vector3 (132f,5.5f,0), Quaternion.identity, 0);
		PhotonNetwork.Instantiate ("AI"+ PlayerPrefs.GetString("AI2","Hero"), new Vector3 (135f,5.5f,0), Quaternion.identity, 0);
		PhotonNetwork.Instantiate ("AI"+ PlayerPrefs.GetString("AI3","Hero"), new Vector3 (140f,5.5f,0), Quaternion.identity, 0);

		player = GameObject.FindGameObjectsWithTag ("Player");
		AssignName ();

		spawner = GameObject.FindGameObjectsWithTag ("FlagSpawner");
		fortSpawner = GameObject.FindGameObjectsWithTag ("FortSpawner");
		//Assign the player properties hashtable
		roomProp = new PhotonHashTable();
		//Add Character property with it default value "Hero"

		round = (int)room.customProperties ["Round"];
		roomProp.Add ("Round", round);		
		aba.StartRound();
		if (PhotonNetwork.isMasterClient) {
			Invoke ("SpawnFlag", 3f);
		}

		InvokeRepeating("updateHudInfo",0f,5f);
	if (PlayerPrefs.GetInt ("OwnerID", -9999) != -9999) {
			//Invoke ("Reconnect",0.5f);
		}
	}

	void Update(){
		

	}
	//---------------------------------------------------------------------------------------------------------------


	//...............................................................................................................
	int FlagToFortInt(int flag){
		int fort;

		if (flag % 2 == 0) {
			fort = Random.Range (2, fortSpawner.Length+1);
			if (fort % 2 == 1) {
				fort = fort -1;
			}
		} else {
			fort = Random.Range (2, fortSpawner.Length+1);
			if (fort % 2 == 0) {
				fort = fort - 1;
			}
		}
		return fort;

	}
	void AssignName(){

		int i = 1;
		foreach (GameObject plyr in player) {
			AIControl aictrl = plyr.GetComponent<AIControl> ();
			if (aictrl) {
				aictrl.name = PlayerPrefs.GetString ("Name" + i);
				aictrl.playerName.text = aictrl.name;
				i++;
			}
		}
		print ("lengtj" + player.Length);

	}
	//..............................................................................................................
	void SpawnFlag (){

		if (PhotonNetwork.isMasterClient) {
			flagInt = Random.Range (1, spawner.Length+1);
			if (GameObject.FindGameObjectWithTag ("Flag") == null) {
				PhotonNetwork.InstantiateSceneObject ("Flag", spawner [flagInt-1].transform.position, Quaternion.identity, 0, null);
			}
		}
	}

	//......................................................................................................................

	public void SpawnBase(){
		if (PhotonNetwork.isMasterClient) {
			if (GameObject.FindGameObjectWithTag ("Fort")==null) {
				fortInt = FlagToFortInt (flagInt);
				PhotonNetwork.InstantiateSceneObject ("Fort", fortSpawner [fortInt - 1].transform.position, fortSpawner [fortInt - 1].transform.rotation, 0, null);
			}
		}
	}

	//......................................................................................................................
	void updateHudInfo(){
		int i = 0;
		foreach (GameObject plyr in player)
		{
			AIControl aictrl = plyr.GetComponent<AIControl> ();
			if (aictrl) {
				scoreInfo [i].text = ""+aictrl.score;
				playerInfo [i].text = aictrl.name;
			} else {
				scoreInfo [i].text = ""+PhotonNetwork.player.GetScore ();
				playerInfo [i].text = PhotonNetwork.player.name;
			}
			i++;
		}
		round = (int)room.customProperties ["Round"];
		roundInfo.text =""+round;

	}
	//...................................................................................
	public void CallEndRound(PhotonPlayer winner){
		if (!complete) {

			winnerr = winner;
			GetComponent<PhotonView> ().RPC ("EndRound", PhotonTargets.AllViaServer, winner);

		}
	}

	[PunRPC]
	public void EndRound(PhotonPlayer winner){

		if (!complete) {
			if (PhotonNetwork.isMasterClient && !scoreAdded) {
				if (callbyply) {
					winner.AddScore (1);
				} else {
					winnerrr.GetComponent<AIControl> ().score++;
				}
				callbyply = false;
				scoreAdded = true;
				photonView.RPC ("UpdateScore", winner);

			}
			PlayerPrefs.SetInt ("Score", PhotonNetwork.player.GetScore ());
			if ((round < 10 || Tie()) &&Reachable() ) {
				StartCoroutine ("NewRound");
				updateHudInfo ();
				aba.gameObject.SetActive (true);
				aba.NextRound (Tie() && round >=10);
			} else {
				if (PhotonNetwork.isMasterClient) {

					updateHudInfo ();
					aba.gameObject.SetActive (true);

					string[] tabstring= GetStanding ();
					PlayerPrefs.SetString ("std_off_1", tabstring [0]);
					PlayerPrefs.SetString ("std_off_2", tabstring [1]);
					PlayerPrefs.SetString ("std_off_3", tabstring [2]);
					PlayerPrefs.SetString ("std_off_4", tabstring [3]);
					UpdateStats ();

					EndGame ();

				}

			}
			complete = true;
		}
	}

	[PunRPC]
	void DestroyFort(){
		GameObject fortd = GameObject.FindGameObjectWithTag ("Fort");
		if (fortd)
			fortd.GetComponentInChildren<Animator> ().SetTrigger ("destroy");			
	}

	IEnumerator NewRound(){

		yield return new WaitForSeconds (2.5f);
		if (PhotonNetwork.isMasterClient) {
			round = round + 1;
			roomProp ["Round"] = round;
			room.SetCustomProperties (roomProp);

			photonView.RPC ("DestroyFort", PhotonTargets.All);
			PhotonView flag = GameObject.FindGameObjectWithTag ("Flag").GetComponent<PhotonView> ();
		
			if (!flag.isMine)
				flag.RequestOwnership ();
			PhotonNetwork.Destroy (flag);




		}
		yield return new WaitForSeconds (0.5f);
		if (PhotonNetwork.isMasterClient) {
			PhotonView fort = GameObject.FindGameObjectWithTag ("Fort").GetComponent<PhotonView> ();
			if (!fort.isMine)
				fort.RequestOwnership ();
			PhotonNetwork.Destroy (fort);
			Invoke ("SpawnFlag", 0.5f);
		}
		updateHudInfo ();
		complete = false;
		scoreAdded = false;


	}

	void UpdateState(){
		roomProp ["Round"] =  (int)room.customProperties ["Round"];
		room.SetCustomProperties (roomProp);

	}

	[PunRPC]
	void EndGame (){
		updateHudInfo ();

		aba.gameObject.SetActive (true);

		//print (maxScore);
		aba.EndGame ();
		Invoke ("Disconnect", 5f);
		if (PhotonNetwork.isMasterClient) {
			UpdateState ();
		}

	}

	void Disconnect(){
		
		PlayerPrefs.SetInt ("IsOnRoom", 0);
		PlayerPrefs.SetInt ("OwnerID", -9999);
		PlayerPrefs.SetInt ("Score", 0);
		PlayerPrefs.SetInt ("Round", 1);
		Application.LoadLevelAsync ("Result");
	}

	void OnDisconnectedFromPhoton(){


	}



	[PunRPC]
	void UpdateScore(){
		PlayerPrefs.SetInt ("Score", PhotonNetwork.player.GetScore ());
	}


	void OnPhotonPlayerDisconnected(PhotonPlayer player){
		if (PhotonNetwork.isMasterClient) {
			Invoke ("ReSpawnFlag", 0.1f);
		}
	}

	void ReSpawnFlag(){
		GameObject flagg = GameObject.FindGameObjectWithTag ("Flag");
		//print (flagg);
		if (!flagg) {
			PhotonNetwork.InstantiateSceneObject ("Flag", spawner [flagInt].transform.position, Quaternion.identity, 0,null);
			GameObject.FindGameObjectWithTag ("Flag").GetComponent<Flag> ().isvirgin = false;
			PhotonHashTable ownerProp = new PhotonHashTable ();
			ownerProp.Add ("Owner", -9999);
			room.SetCustomProperties (ownerProp);
		}
	}

	void OnMasterClientSwitched(PhotonPlayer newMasterClient) { 

	}

	void OnPhotonCustomRoomPropertiesChanged(PhotonHashTable propertiesThatChanged){
		if (propertiesThatChanged.ContainsKey ("Round")) {
			round = (int)room.customProperties ["Round"];
		}
	}


	public void DisconnectButton(){
		PhotonNetwork.Disconnect ();
	}

	//Pemrosesan array nilai
	bool Reachable(){
		int[] TabInt;
		TabInt = new int[5];
	
		foreach (GameObject plyr in player)
		{
			AIControl aictrl = plyr.GetComponent<AIControl> ();
			int x;
			if (aictrl) {
				x = aictrl.score;
			} else {
				x = PhotonNetwork.player.GetScore ();
			}

			int i = 3;
			while (i >= 0 && x > TabInt [i]) {
				TabInt [i+1] = TabInt [i];
				i--;
			}
			i++;
			if (x > TabInt [i]) {
				TabInt [i + 1] = TabInt [i];
				TabInt [i] = x;
			} else {
				TabInt [i] = x;
			}

		}
	
		return ((TabInt[0] - TabInt[1]) <= (10-round));
	}
	bool Tie (){
		int[] TabInt;
		TabInt = new int[5];

		foreach (GameObject plyr in player)
		{
			AIControl aictrl = plyr.GetComponent<AIControl> ();
			int x;
			if (aictrl) {
				x = aictrl.score;
			} else {
				x = PhotonNetwork.player.GetScore ();
			}

			int i = 3;
			while (i >= 0 && x > TabInt [i]) {
				TabInt [i+1] = TabInt [i];
				i--;
			}
			i++;
			if (x > TabInt [i]) {
				TabInt [i + 1] = TabInt [i];
				TabInt [i] = x;
			} else {
				TabInt [i] = x;
			}
		}
		return (TabInt[0] == TabInt[1]);
	}
	string[] GetStanding(){
		bool ply;
		int[] TabInt;
		TabInt = new int[5];
		string[] TabString;
		TabString = new string[5];
		for(int i = 0; i<=3; i++){
			TabInt [i] = -1;
			}

		foreach (GameObject plyr in player)
		{	
			ply = false;
			AIControl aictrl = plyr.GetComponent<AIControl> ();
			int x;
			string y;
			if (aictrl) {
				x = aictrl.score;
				y = aictrl.name;
			} else {
				x = PhotonNetwork.player.GetScore ();
				y = PhotonNetwork.player.name;
				ply = true;
			}

			int i = 3;
			while (i >= 0 && x >= TabInt [i]) {
				if (cmp == i) {
					cmp = cmp + 1;
				}
				TabInt [i+1] = TabInt [i];
				TabString [i + 1] = TabString [i];

				i--;
			}
			i++;
			if (x >= TabInt [i]) {
				if (cmp == i) {
					cmp = cmp + 1;
				}
				TabInt [i + 1] = TabInt [i];
				TabString [i + 1] = TabString [i];
				TabInt [i] = x;
				TabString [i] = y;
				if (ply) {
					cmp = i;
				}
			} else {
				TabInt [i] = x;
				TabString [i] = y;
				if (ply) {
					cmp = i;
				}
			}
		}
		cmp = cmp + 1;

		for(int i = 0; i<=3; i++){
			print ("Tabstring"+i+" "+TabString [i]);
		}
		return TabString;
	}

	void UpdateStats(){
		if (StatUpd) {
			
			int y = 0;
			switch (cmp) {
			case 1:
				y = 30;
				break;
			case 2:
				y = 20;
				break;
			case 3:
				y = 10;
				break;
			case 4:
				y = 5;
				break;

			}
			Debug.LogError ("cmp" + cmp);
			y += y * (PlayerPrefs.GetInt ("curr_trophy", 100) / 200);
			PlayerPrefs.SetInt ("curr_coin", PlayerPrefs.GetInt ("curr_coin", 100) + y);
			PlayerPrefs.SetInt ("dlt_trophy", 0);
			PlayerPrefs.SetInt ("dlt_coin", y);


			PlayerPrefs.SetInt ("st_plyf", PlayerPrefs.GetInt ("st_plyf", 0) + 1);
			StatUpd = false;
		}

	}
}
//------------------------------------------------------------------------------------------------------------------
