using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class CoopGameManager : MonoBehaviour {

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

	[SerializeField]
	GameObject[] unusedBox;

	void Awake(){

		photonView = GetComponent<PhotonView> ();
		room = PhotonNetwork.room;
	}

	void Start () {
		if (PhotonNetwork.offlineMode || (string)PhotonNetwork.room.customProperties ["Mode"] != "C") {
			Destroy (this);
		} else {
			GameObject.Find ("A*").SetActive (false);
			foreach (GameObject unuse in unusedBox) {
				unuse.SetActive (false);
			}
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
				roomProp.Add ("B", 0);
				roomProp.Add ("R", 0);
				room.SetCustomProperties (roomProp);
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

	void AddScoreBlue(){
		roomProp ["B"] = (int)roomProp ["B"] + 1;

		room.SetCustomProperties (roomProp);


	}

	void AddScoreRed(){

		roomProp ["R"] = (int)roomProp ["R"] + 1;

		room.SetCustomProperties (roomProp);
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
		int blue = (int)room.customProperties ["B"];
		int red = (int)room.customProperties ["R"];

		return (Mathf.Abs(blue - red) <= (10-round));
	}

	bool Tie (){

		int blue = (int)room.customProperties ["B"];
		int red = (int)room.customProperties ["R"];

		return (blue == red);
	}

	void GetStanding(){
		int blue = (int)room.customProperties ["B"];
		int red = (int)room.customProperties ["R"];
		if(blue > red)
		roomProp.Add ("C", "B");
		else if(red > blue)
			roomProp.Add ("C", "B");
		room.SetCustomProperties (roomProp);
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

		print(room.customProperties.ToStringFull ());
		scoreInfo [0].text =""+(int) room.customProperties["B"];
		playerInfo [0].text = "Team Blue";
		scoreInfo [1].text =""+(int) room.customProperties["R"];
		playerInfo [1].text = "Team Red";
	
		round = (int)room.customProperties ["Round"];
		roundInfo.text = ""+round;

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
		if ( propertiesThatChanged.ContainsKey ("Round")) {
			round = (int)room.customProperties ["Round"];
		}
		if (propertiesThatChanged.ContainsKey ("C")) {
			UpdateStats ();
		}
		if (propertiesThatChanged.ContainsKey ("B") || propertiesThatChanged.ContainsKey ("R")) {
			updateHudInfo ();
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
				if (winner.GetTeam() == PunTeams.Team.red) {
					AddScoreRed ();
				}
				else
					if (winner.GetTeam() == PunTeams.Team.blue) {
					AddScoreBlue ();
				}
				scoreAdded = true;

			}
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
					GetStanding();
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
			bool win = false;
			if ((string)room.customProperties ["C"] == "B") {
				if (PhotonNetwork.player.GetTeam () == PunTeams.Team.blue) {
					win = true;
				} else {
					win = false;
				}
			}
			else
				if ((string)room.customProperties ["C"] == "R") {
				if (PhotonNetwork.player.GetTeam () == PunTeams.Team.red) {
					win = true;
				} else {
					win = false;
				}
			}

			int x = 0;
			int y = 0;

			if (win) {
				x = 20;
				y = 40;
			} else {
				x = -15;
				y = 10;
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
			if (win)
				PlayerPrefs.SetInt ("st_coopwin", PlayerPrefs.GetInt ("st_coopwin", 0) + 1);
			else
				PlayerPrefs.SetInt ("st_cooplose", PlayerPrefs.GetInt ("st_cooplose", 0) + 1);
			StatUpd = false;


			if (PhotonNetwork.isMasterClient) {

				photonView.RPC ("EndGame",PhotonTargets.AllViaServer);

			}
		
		}

	}
}
//------------------------------------------------------------------------------------------------------------------
