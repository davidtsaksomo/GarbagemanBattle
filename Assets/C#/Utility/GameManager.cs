using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour {

	public GameObject[] spawner;
	GameObject[] fortSpawner;
	GameObject flagPrefab;
	int round = 1;
	Room room;
	PhotonPlayer[] players;

	[SerializeField]
	Text roundInfo;
	[SerializeField]
	Text[] scoreInfo;
	[SerializeField]
	Text[] playerInfo;
	[SerializeField]
	Abaaba aba;
	int flagInt;
	int fortInt;
	PhotonView photonView;
	bool complete = false;
	PhotonPlayer winnerr;
	bool scoreAdded = false;
	bool StatUpd = true;
	PhotonHashTable roomProp;


	void Awake(){
		
		photonView = GetComponent<PhotonView> ();
		room = PhotonNetwork.room;
	}

	void Start () {
		if (PhotonNetwork.offlineMode || (string)PhotonNetwork.room.customProperties ["Mode"] == "C") {
			Destroy (this);
		} else {
			GameObject.Find ("A*").SetActive (false);
		
			spawner = GameObject.FindGameObjectsWithTag ("FlagSpawner");
			fortSpawner = GameObject.FindGameObjectsWithTag ("FortSpawner");
			//Assign the player properties hashtable
			roomProp = new PhotonHashTable ();
			//Add Character property with it default value "Hero"

			round = (int)room.customProperties ["Round"];
			roomProp.Add ("Round", round);		
			players = PhotonNetwork.playerList;
			aba.StartRound ();
			if (PhotonNetwork.isMasterClient) {
				Invoke ("SpawnFlag", 3f);
			}
		
			InvokeRepeating ("updateHudInfo", 0f, 5f);
			PhotonNetwork.Instantiate (PlayerPrefs.GetString ("Character", "Hero"), new Vector3 (Random.Range (132f, 145f), 5.5f, 0), Quaternion.identity, 0);
		//	if (PlayerPrefs.GetInt ("OwnerID", -9999) != -9999) {
				//Invoke ("Reconnect",0.5f);
		//	}
		}
	}

	void Update(){

	}
		
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

	bool Reachable(){
		int[] TabInt;
		TabInt = new int[5];


		foreach (PhotonPlayer player in PhotonNetwork.playerList)
		{
			int x;
			x = player.GetScore ();
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

		foreach (PhotonPlayer player in PhotonNetwork.playerList)
		{
			int x;
			x = player.GetScore ();
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

	int[] GetStanding(){
		int[] TabInt;
		TabInt = new int[5];
		int[] TabID;
		TabID = new int[5];
		for(int i = 0; i<=3; i++){
			TabInt [i] = -1;
		}
		foreach (PhotonPlayer player in PhotonNetwork.playerList)
		{
			int x;
			int y;

			x = player.GetScore ();
			y = player.ID;


			int i = 3;
			while (i >= 0 && x >= TabInt [i]) {
				TabInt [i+1] = TabInt [i];
				TabID [i + 1] = TabID [i];
				i--;
			}
			i++;
			if (x >= TabInt [i]) {
				TabInt [i + 1] = TabInt [i];
				TabID [i + 1] = TabID [i];
				TabInt [i] = x;
				TabID [i] = y;
			} else {
				TabInt [i] = x;
				TabID [i] = y;
			}
		}
		for(int i = 0; i<=3; i++){
			print ("TabID"+i+" "+TabID [i]);
		}
		return TabID;
	}

	void SpawnFlag (){
		
		if (PhotonNetwork.isMasterClient) {
			flagInt = Random.Range (1, spawner.Length+1);
			if (GameObject.FindGameObjectWithTag ("Flag") == null) {
				GameObject fortt = GameObject.FindGameObjectWithTag ("Fort");
				if (fortt) {
					PhotonNetwork.Destroy (fortt);
				}
				PhotonNetwork.InstantiateSceneObject ("Flag", spawner [flagInt-1].transform.position, Quaternion.identity, 0, null);
			}
			}
	}
		
	public void SpawnBase(){
		if (PhotonNetwork.isMasterClient) {
			if (GameObject.FindGameObjectWithTag ("Fort")==null) {
				fortInt = FlagToFortInt (flagInt);
				PhotonNetwork.InstantiateSceneObject ("Fort", fortSpawner [fortInt - 1].transform.position, fortSpawner [fortInt - 1].transform.rotation, 0, null);
			}
		}
	}
		
	void ReSpawnFlag(){
		GameObject flagg = GameObject.FindGameObjectWithTag ("Flag");
		//print (flagg);
		if (!flagg) {
			PhotonNetwork.InstantiateSceneObject ("Flag", spawner [flagInt].transform.position, Quaternion.identity, 0,null);
			GameObject fortt = GameObject.FindGameObjectWithTag ("Fort");
			if (fortt) {
				GameObject.FindGameObjectWithTag ("Flag").GetComponent<Flag> ().isvirgin = false;

			}
			PhotonHashTable ownerProp = new PhotonHashTable ();
			ownerProp.Add ("Owner", -9999);
			room.SetCustomProperties (ownerProp);
		}
	}

	void updateHudInfo(){
		
	
		int i = 0;
		foreach (PhotonPlayer player in PhotonNetwork.playerList)
		{
			
				scoreInfo [i].text = ""+player.GetScore ();
				playerInfo [i].text = player.name;

			i++;
		}
		for (; i <= 3; i++) {
			scoreInfo [i].text = "";
			playerInfo [i].text ="";
		}
		round = (int)room.customProperties ["Round"];
		roundInfo.text = ""+round;

	}

	[PunRPC]
	void UpdateScore(){
		PlayerPrefs.SetInt ("Score", PhotonNetwork.player.GetScore ());
	}


	void Disconnect(){
		PlayerPrefs.SetInt ("IsOnRoom", 0);
		PlayerPrefs.SetInt ("OwnerID", -9999);
		PlayerPrefs.SetInt ("Score", 0);
		PlayerPrefs.SetInt ("Round", 1);
		if(PhotonNetwork.isMasterClient)
		Application.LoadLevelAsync ("Result");

	}

	public void DisconnectButton(){
		PhotonNetwork.Disconnect ();
	}


	void OnPhotonPlayerDisconnected(PhotonPlayer player){
		if (PhotonNetwork.isMasterClient) {
			Invoke ("ReSpawnFlag", 0.1f);
		}
  	}

	void OnMasterClientSwitched(PhotonPlayer newMasterClient) { 
		if(PhotonNetwork.isMasterClient){
	
			ReSpawnFlag ();
			GameObject forttt = GameObject.FindGameObjectWithTag ("Fort");

			if((int)room.customProperties["Owner"] != -9999 && !forttt)
			{
				SpawnBase ();
			}
		}
	}

	void OnPhotonCustomRoomPropertiesChanged(PhotonHashTable propertiesThatChanged){
		if (propertiesThatChanged.ContainsKey ("Round")) {
			round = (int)room.customProperties ["Round"];
		}
		if (propertiesThatChanged.ContainsKey ("C1")) {
			UpdateStats ();
		}
	}

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
				winner.AddScore (1);
				scoreAdded = true;
				photonView.RPC ("UpdateScore", winner);

			}
			PlayerPrefs.SetInt ("Score", PhotonNetwork.player.GetScore ());
			if ((round < 10 || Tie()) && Reachable()) {
				Invoke ("NewRound", 3f);
				updateHudInfo ();
				aba.gameObject.SetActive (true);
				aba.NextRound (Tie()&& round >=10);
			} else {
				if (PhotonNetwork.isMasterClient) {

					updateHudInfo ();
					aba.gameObject.SetActive (true);

					//Set winner decision here
					int[] TabID = GetStanding();
					roomProp.Add ("C1", TabID [0]);
					roomProp.Add ("C2", TabID [1]);
					roomProp.Add ("C3", TabID [2]);
					roomProp.Add ("C4", TabID [3]);
					room.SetCustomProperties (roomProp,room.customProperties);

					photonView.RPC ("EndGame",PhotonTargets.AllViaServer);

				}

			}
			complete = true;
		}
	}

	void NewRound(){


		if (PhotonNetwork.isMasterClient) {
			round = round + 1;
			roomProp ["Round"] = round;
			room.SetCustomProperties (roomProp);


			PhotonView flag = GameObject.FindGameObjectWithTag ("Flag").GetComponent<PhotonView> ();
			PhotonView fort = GameObject.FindGameObjectWithTag ("Fort").GetComponent<PhotonView> ();
			if (!fort.isMine)
				fort.RequestOwnership ();
			if (!flag.isMine)
				flag.RequestOwnership ();
			PhotonNetwork.Destroy (flag);
			PhotonNetwork.Destroy (fort);
			Invoke ("SpawnFlag" ,0.5f);


		}
		updateHudInfo ();
		complete = false;
		scoreAdded = false;


	}

	[PunRPC]
	void EndGame (){
		updateHudInfo ();

		aba.gameObject.SetActive (true);

		aba.EndGame ();
		Invoke ("Disconnect", 5f);
	

	}

	void UpdateStats(){
		if (StatUpd) {
			int c = 0;
			for (int i = 1; i <= 4; i++) {
				if ((int)room.customProperties ["C" + i] == PhotonNetwork.player.ID) {
					c = i;
					break;
				}
			}
			int x = 0;
			int y = 0;

			//not a game freind, increase coinn and thrpoy
			switch (c) {
			case 1:
				x = 30;
				y = 50;
				break;
			case 2:
				x = 10;
				y = 25;
				break;
			case 3:
				x = -5;
				y = 15;
				break;
			case 4:
				x = -25;
				y = 5;
				break;

			}

			if ((string)PhotonNetwork.room.customProperties ["Friend"] == "Y") {
				x = 0;
			}

			int prv = PlayerPrefs.GetInt ("curr_trophy", 100);
			int z = PlayerPrefs.GetInt ("curr_trophy", 100) + x;
			if (z < 0)
				z = 0;
			PlayerPrefs.SetInt ("curr_trophy", z);
			y += y * (PlayerPrefs.GetInt ("curr_trophy", 100) / 200);
			PlayerPrefs.SetInt ("curr_coin", PlayerPrefs.GetInt ("curr_coin", 100) + y);
			PlayerPrefs.SetInt ("dlt_trophy", z - prv);
			PlayerPrefs.SetInt ("dlt_coin", y);

			if (PlayerPrefs.GetInt ("curr_trophy", 100) > (PlayerPrefs.GetInt ("max_trophy", 100))) {
				PlayerPrefs.SetInt ("max_trophy", PlayerPrefs.GetInt ("curr_trophy", 100));
			}

			PlayerPrefs.SetInt ("st_ply", PlayerPrefs.GetInt ("st_ply", 0) + 1);
			PlayerPrefs.SetInt ("st_plc"+c, PlayerPrefs.GetInt ("st_plc"+c, 0) + 1);
			StatUpd = false;
		}

	}
}
//------------------------------------------------------------------------------------------------------------------