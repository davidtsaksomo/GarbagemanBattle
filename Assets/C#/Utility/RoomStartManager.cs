using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class RoomStartManager : MonoBehaviour {
	[SerializeField]
	Text debugText;
	[SerializeField]
	Text playerInfo;
	[SerializeField]
	Text curMapInfo;
	string map;
	[SerializeField]
	GameObject mapSel;
	[SerializeField]
	GameObject startButton;
	PhotonView photonView;
	string[] AIhero;
	int[] AIindex;
	PhotonHashTable roomProp;

	[SerializeField] GameObject[] change;
	// Use this for initialization
	void Start () {
		
		//Assign map with default value
		map = "City";
		AIhero = new string[4];
		AIindex = new int[4] ;
		//OFFLINE MODE
		if(PhotonNetwork.offlineMode){
			AIindex[0] = Random.Range (0, 4);
			AIindex[1] = Random.Range (0, 4);
			AIindex[2] = Random.Range (0, 4);
			int i = 0;
			foreach (int idx in AIindex) {
				switch (idx)
				{
				case 0:
					AIhero [i] = "Hero";
					break;
				case 1:
					AIhero [i] = "Ghost";
					break;
				case 2:
					AIhero [i] = "Fish";
					break;
				case 3:
					AIhero [i] = "Apple";
					break;
				default:
					AIhero [i] = "Hero";
					break;
				}
				i++;
			}
			PlayerPrefs.SetString ("AI1",AIhero[0]);
			PlayerPrefs.SetString ("AI2", AIhero[1]);
			PlayerPrefs.SetString ("AI3", AIhero[2]);


			AssignName ();
			debugText.text = "";
		}
		//Set Assignment
		photonView = GetComponent<PhotonView> ();
		//photonView.RPC ("UpdatePlayerInfo", PhotonTargets.All);
		StartCoroutine ("regularupdate");
		StartButtonActivation ();
		PlayerPrefs.SetInt ("IsOnRoom", 1);
		PlayerPrefs.SetString ("RoomIsIn", PhotonNetwork.room.name);

		//set the playerprop to send character properties
		PhotonHashTable playerProp = new PhotonHashTable ();
		playerProp.Add ("Character", PlayerPrefs.GetString ("Character", ""));
		PhotonNetwork.player.SetCustomProperties(playerProp);

		//make room propwerties
		roomProp = new PhotonHashTable ();
		roomProp.Add ("Map", map);

		//update curmap info
		curMapInfo.text = "Map: " + PhotonNetwork.room.customProperties["Map"];



		//Set Team
		if (CountBlue () > CountRed ()) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.red);
		} else {
			PhotonNetwork.player.SetTeam (PunTeams.Team.blue);

		}
		//master client can select the map
		if (PhotonNetwork.isMasterClient) {
			mapSel.SetActive (true);
		} 


	}

	void AssignName(){

		string[] names;
		int i = -1;
		int n = 2;
		names = new string[6];
		names [0] = "kepet";
		names [1] = "john";
		names [2] = "budi";
		names [3] = "michael";
		names [4] = "rawszozky";
		names [5] = "vladimir";
		for (int j = 1; j <= 3; j++) {
				i = Random.Range (i+1, 6 - n);
			PlayerPrefs.SetString ("Name" + j, names [i]);

				n--;

		}
			
		

	}

	// Update is called once per frame
	public void LeaveRoomButton () {
		PhotonNetwork.LeaveRoom ();
		debugText.text = "Leaving...";
		Application.LoadLevelAsync ("GameStart");

	}
	public void StartRoombutton(){
		if (PhotonNetwork.playerList.Length > 0) {
			debugText.text = "Starting...";
			PhotonNetwork.room.maxPlayers = PhotonNetwork.room.playerCount;
			//load map
			PhotonNetwork.LoadLevel (map);
		} else {
			debugText.text = "Not enough players";
		}
	}

	int CountRed(){
		int red = 0;
		foreach (PhotonPlayer player in PhotonNetwork.playerList) {
			if (player.GetTeam () == PunTeams.Team.red) {
				red++;
			}
		}
		return red;
	}

	int CountBlue(){
		int blue = 0;
		foreach (PhotonPlayer player in PhotonNetwork.playerList) {
			if (player.GetTeam () == PunTeams.Team.blue) {
				blue++;
			}
		}
		return blue;
	}
	void OnLeftRoom(){

		PlayerPrefs.SetInt ("IsOnRoom", 0);
		PlayerPrefs.SetInt ("OwnerID", -9999);
		PlayerPrefs.SetInt ("Score", 0);
		PlayerPrefs.SetInt ("Round", 1);
		Application.LoadLevelAsync ("GameStart");

	}
	IEnumerator regularupdate(){
		UpdatePlayerInfo ();
		yield return new WaitForSeconds(5f);
		StartCoroutine ("regularupdate");
	}
	[PunRPC]
	void UpdatePlayerInfo(){
		string playerlist;
		int number = 1;
		playerlist = "";
		foreach (GameObject but in change) {
			but.SetActive (false);
		}
		if (!PhotonNetwork.offlineMode) {
			int count = 0;
			foreach (PhotonPlayer player in PhotonNetwork.playerList) {
				string team = "";
				if ((string)PhotonNetwork.room.customProperties ["Mode"] == "C") {
					if (player.GetTeam () == PunTeams.Team.blue) {
						team = "(Team Blue)";
					} else if (player.GetTeam () == PunTeams.Team.red) {
						team = "(Team Red)";

					} else {
						team = "(No Team)";
					}
					if ((string)PhotonNetwork.room.customProperties ["Friend"] == "Y"  && PhotonNetwork.player.ID == player.ID)//if friend coop and is me
					{
						change [count].SetActive (true);
					}
				}
				if (player.isLocal) {
					playerlist = playerlist + (number + ". " + player.name + " (" + PlayerPrefs.GetString ("Character", "Hero") + ")" + " "+ team+"\n");
				} else {
					playerlist = playerlist + (number + ". " + player.name + " (" + (string)player.customProperties ["Character"] + ")" +  " "+ team+"\n");
				}
				number++;
			
				count++;
			}
		} else {//OFFLINE MODE
			playerlist = playerlist + ("1" + ". " + PhotonNetwork.player.name + " (" + PlayerPrefs.GetString ("Character", "Hero") + ")" + "\n");
			playerlist = playerlist + ("2. "+PlayerPrefs.GetString("Name1")+" (" + AIhero [0] + ")\n") + ("3. "+PlayerPrefs.GetString("Name2")+" (" + AIhero [1] + ")\n") +("4. "+PlayerPrefs.GetString("Name3")+" (" + AIhero [2] + ")\n");
		}
		playerInfo.text = playerlist;
	}
	void StartButtonActivation(){
		if (PhotonNetwork.isMasterClient) {
			if ((string)PhotonNetwork.room.customProperties ["Friend"] == "N") {

				if (PhotonNetwork.room.playerCount == 2) {
					StartRoombutton ();
				}

			} else if ((string)PhotonNetwork.room.customProperties ["Friend"] == "Y") {
				if ((string)PhotonNetwork.room.customProperties ["Mode"] == "N") {
					if (PhotonNetwork.room.playerCount > 1) {
						startButton.SetActive (true);
					} else {
						startButton.SetActive (false);
					}
				} else if ((string)PhotonNetwork.room.customProperties ["Mode"] == "C") {
					if (CountRed () >= 1 && CountBlue () >= 1) {
						startButton.SetActive (true);
					} else {
						startButton.SetActive (false);

					}
				}
			} else if ((string)PhotonNetwork.room.customProperties ["Friend"] == "O") {
				startButton.SetActive (true);
			}

		} 
		else{startButton.SetActive (false);}
	}
	void OnMasterClientSwitched(){
		StartButtonActivation ();
		if (PhotonNetwork.isMasterClient) {
			mapSel.SetActive (true);
		} 
	}

	public void ChangeMap(int x){
		if (x == 0) {
			map = "City";
		} else if (x == 1) {
			map = "Night";
		}
		roomProp ["Map"] = map;
		PhotonNetwork.room.SetCustomProperties (roomProp);
	}

	void OnPhotonCustomRoomPropertiesChanged(PhotonHashTable propertiesThatChanged){
		if (propertiesThatChanged.ContainsKey ("Map")) {
			curMapInfo.text = "Map: " + PhotonNetwork.room.customProperties["Map"];
		}
	

	}

	 void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps) {
        PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
        PhotonHashTable props = playerAndUpdatedProps[1] as PhotonHashTable;
		if (props.ContainsKey (PunTeams.TeamPlayerProp)) {
			UpdatePlayerInfo ();
			StartButtonActivation ();
		}
    }

	void OnPhotonPlayerConnected (PhotonPlayer newPlayer){
		UpdatePlayerInfo ();
		StartButtonActivation ();
	}
	void OnPhotonPlayerDisconnected (PhotonPlayer otherplayer){
		UpdatePlayerInfo ();
		StartButtonActivation ();
	}

	public	void Change(int idx){
		if (PhotonNetwork.player.GetTeam () == PunTeams.Team.blue) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.red);
		} else {
			PhotonNetwork.player.SetTeam (PunTeams.Team.blue);
						
		}
	}
}




