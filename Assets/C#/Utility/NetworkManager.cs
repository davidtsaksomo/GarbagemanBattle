using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

//This Script placed at Main Camera Object in Start Scene, control the matchmaking, joining into room, character select
public class NetworkManager : MonoBehaviour {
	
	//Reference to Debug text
	public GameObject debugText;
	public GameObject debugText2;

	//Properties for room
	private string roomName;
	private RoomInfo[] roomsList;
	private RoomOptions roomopt;
	[SerializeField]
	//To check if player already connected
	bool ready = false;
	[SerializeField] InputField nameinput;
	//To set Player properties
	PhotonHashTable playerProp;

	void Start () {
		ready = PhotonNetwork.insideLobby;
		PhotonNetwork.offlineMode = false;

		//Assign the player properties hashtable
		playerProp = new PhotonHashTable();
		//Add Character property with it default value "Hero"
		playerProp.Add ("Character", "");
		PhotonNetwork.player.name = PlayerPrefs.GetString ("playerName", "Tanpa nama");
		//Set auto cleanpup player objects to remove object when player dusconnect
		if(!PhotonNetwork.inRoom)
		PhotonNetwork.autoCleanUpPlayerObjects = true;
		
		debugText2.GetComponent<Text> ().color = Color.yellow;
		debugText2.GetComponent<Text> ().text = "Connecting...";

	
		//Automatically sinc scene with master
		PhotonNetwork.automaticallySyncScene = true;
		//Connect to the server
		if (!PhotonNetwork.connected|| !ready) {
			PhotonNetwork.ConnectUsingSettings ("0.1");
			debugText2.GetComponent<Text> ().color = Color.yellow;
			debugText2.GetComponent<Text> ().text = "Connecting...";
		} else {
			debugText2.GetComponent<Text> ().color = Color.green;

			debugText2.GetComponent<Text> ().text = "Connected";
		}

		//Set the room options
		roomopt = new RoomOptions(){isVisible = true, maxPlayers = 4};
		roomopt.publishUserId = true;
		//Assign the palyerName Playerprefs
		//PhotonNetwork.playerName = PlayerPrefs.GetString ("playerName", "Player");
	
	}

	void Update () {
	
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit();
		}
	}
		
	//Updating the room
	void UpdateRoom()
	{
		roomsList = PhotonNetwork.GetRoomList();
	}

	//Mengecek apakkah sebelumnya diskonek paksa
	void checkReconnect(){
		roomsList = PhotonNetwork.GetRoomList();

		if (PlayerPrefs.GetInt ("IsOnRoom", 0) == 1) {
			if (searchRoom ()) {
				debugText.GetComponent<Text> ().color = Color.green;
				debugText.GetComponent<Text> ().text = "Click match to reconnect";

			} else {
				PlayerPrefs.SetInt ("IsOnRoom", 0);
				PlayerPrefs.SetInt ("Score", 0);
				PlayerPrefs.SetInt ("Round", 1);
			}
		}
	}

	bool searchRoom(){
		bool found = false;
		string connectedRoomName;
		roomsList = PhotonNetwork.GetRoomList();
		connectedRoomName = PlayerPrefs.GetString ("RoomIsIn");
		if (roomsList.Length != 0) {
			foreach (RoomInfo room in roomsList) {

				if (room.name == connectedRoomName) {
					found = true;
					break;
				}
			}
		}
		return found;
	}

	void OnJoinedRoom()
	{	
//		//Set player properties dengan character yang dipilih
		PhotonNetwork.SetPlayerCustomProperties(playerProp);
		playerProp["Character"] = PlayerPrefs.GetString("Character","Hero");

		PhotonNetwork.player.SetCustomProperties(playerProp, PhotonNetwork.player.customProperties, false);
		//Pindah level
		PhotonNetwork.LoadLevel("RoomStart");

		//Update playerpref 
		roomName = PhotonNetwork.room.name;
		PlayerPrefs.SetInt ("IsOnRoom", 1);

		PlayerPrefs.SetString ("RoomIsIn", roomName);
	}

	//Kallo gagal masuk room
	void OnPhotonJoinRoomFailed(){
		debugText.GetComponent<Text> ().text = "Can't enter room";
		PlayerPrefs.SetInt ("IsOnRoom", 0);
		PlayerPrefs.SetInt ("Score", 0);
		PlayerPrefs.SetInt ("Round", 1);

	}

	void OnPhotonJoinOrCreateRoomFailed(){
		debugText.GetComponent<Text> ().text = "Can't enter room";
	
	}

	//Gagal buat room
	void OnPhotonCreateRoomFailed(){
		debugText.GetComponent<Text> ().text = "Can't create room";
	}

	//Masuk ke lobby
	void OnJoinedLobby(){
		ready = true;
		debugText2.GetComponent<Text> ().color = Color.green;
		debugText2.GetComponent<Text> ().text = "Connected";
		Invoke("checkReconnect",0.1f);

	}
		
	//Jika connect ready = true
	void OnConnectedToMaster(){
		ready = true;

	}

	//fungsi untuk mencari apakah room ada di room listKalo diskonek
	void OnDisconnectedFromPhoton(){

		PhotonNetwork.ConnectUsingSettings ("0.1");
		debugText2.GetComponent<Text> ().color = Color.yellow;
		debugText2.GetComponent<Text> ().text = "Connecting...";
	}

	void OnConnectionFail(DisconnectCause cause) { 	debugText2.GetComponent<Text> ().color = Color.red;
		debugText2.GetComponent<Text> ().text = "Failed to connect";}
	//Gagal konek
	void OnFailedToConnectToPhoton(DisconnectCause cause){
		debugText2.GetComponent<Text> ().color = Color.red;
		debugText2.GetComponent<Text> ().text = "Failed to connect";
	}
		
	public void RoomButton()

	{	
		//Get The room list
		roomopt.customRoomProperties = new PhotonHashTable() { { "Round", 1 },{"State",0},{"FlagOwner",-9999}, {"Map","City"} , {"Mode","N"}, {"Friend", "N"}};

		roomsList = PhotonNetwork.GetRoomList();
		PhotonNetwork.offlineMode = false;
		//Joinning Lobby
		if (!ready) {
			PhotonNetwork.JoinLobby();
			debugText.GetComponent<Text> ().color = Color.red;
			debugText.GetComponent<Text> ().text = "You're not connected yet";
		} 
		else 
			if (!PhotonNetwork.connected) {
				debugText.GetComponent<Text> ().color = Color.red;
				debugText.GetComponent<Text> ().text = "You're not connected yet";
				if (PlayerPrefs.GetInt ("IsOnRoom", 0) == 1) {
					PhotonNetwork.Reconnect();
					debugText2.GetComponent<Text> ().color = Color.yellow;
					debugText2.GetComponent<Text> ().text = "Connecting...";

				}else {
					PhotonNetwork.ConnectUsingSettings ("0.1");
					debugText2.GetComponent<Text> ().color = Color.yellow;
					debugText2.GetComponent<Text> ().text = "Connecting...";

				}

				//Or reconnect if player Disconnect from a room before
			} else if (PlayerPrefs.GetInt ("IsOnRoom", 0) == 1) {
				bool found = searchRoom ();
				if (!found) {
					PlayerPrefs.SetInt ("IsOnRoom", 0);
					PlayerPrefs.SetInt ("Score", 0);
					PlayerPrefs.SetInt ("Round", 1);
					debugText.GetComponent<Text> ().color = Color.red;
					debugText.GetComponent<Text> ().text = "Can't Reconnect. Click again to start new game";
				} else {
					PhotonNetwork.JoinRoom (PlayerPrefs.GetString ("RoomIsIn"));

				}

			} 
		//If no room, create room
			else if (roomsList.Length == 0) {
				// Create Room
				roomName = PhotonNetwork.player.name + Random.Range(0,10).ToString() + Random.Range(0,10).ToString() +Random.Range(0,10).ToString()+Random.Range(0,10).ToString()+Random.Range(0,10).ToString()+Random.Range(0,10).ToString()+Random.Range(0,10).ToString()+Random.Range(0,10).ToString()+Random.Range(0,10).ToString();
				PhotonNetwork.JoinOrCreateRoom (roomName, roomopt, TypedLobby.Default);
				debugText.GetComponent<Text> ().color = Color.green;
				debugText.GetComponent<Text> ().text = "Creating room...";
			}
		// Join Room
			else 
			{



				PhotonNetwork.JoinRandomRoom();
				debugText.GetComponent<Text> ().color = Color.green;
				debugText.GetComponent<Text> ().text = "Entering room...";
			}

	}

	public void OfflineButton(){
		roomopt.customRoomProperties = new PhotonHashTable() { { "Round", 1 },{"State",0},{"FlagOwner",-9999}, {"Map","City"} , {"Mode","N"}, {"Friend", "O"}};

		PhotonNetwork.Disconnect ();
		PhotonNetwork.offlineMode = true;
		//Joinning Lobby


		PhotonNetwork.CreateRoom ("Offline Room", roomopt, TypedLobby.Default);

	}

	public void FriendButton(){
			
			
			roomopt = new RoomOptions(){isVisible = false, maxPlayers = 4};
			roomopt.publishUserId = true;
		roomopt.customRoomProperties = new PhotonHashTable() { { "Round", 1 },{"State",0},{"FlagOwner",-9999}, {"Map","City"} , {"Mode","N"}, {"Friend", "Y"}};

			//Get The room list
		//	roomsList = PhotonNetwork.GetRoomList();
			PhotonNetwork.offlineMode = false;
			//Joinning Lobby
			if (!ready) {
				PhotonNetwork.JoinLobby();
				debugText.GetComponent<Text> ().color = Color.red;
				debugText.GetComponent<Text> ().text = "You're not connected yet";
			} 
			else 
				if (!PhotonNetwork.connected) {
					debugText.GetComponent<Text> ().color = Color.red;
					debugText.GetComponent<Text> ().text = "You're not connected yet";
					PhotonNetwork.ConnectUsingSettings ("0.1");
					debugText2.GetComponent<Text> ().color = Color.yellow;
					debugText2.GetComponent<Text> ().text = "Connecting...";



				}
			//If no room, create room
			else  {
				// Create Room
				if (nameinput.text == "") {

					debugText.GetComponent<Text> ().color = Color.red;
					debugText.GetComponent<Text> ().text = "Name Invalid";
				} 
				else {

					roomName = nameinput.text;
					PhotonNetwork.JoinOrCreateRoom (roomName, roomopt, TypedLobby.Default);
					debugText.GetComponent<Text> ().color = Color.green;
					debugText.GetComponent<Text> ().text = "Joining room...";
				}
			}

	}

	public void CoopButton(){
		roomopt.customRoomProperties = new PhotonHashTable() { { "Round", 1 },{"State",0},{"FlagOwner",-9999}, {"Map","City"} , {"Mode","C"}, {"Friend", "N"}};
		roomsList = PhotonNetwork.GetRoomList();
		PhotonNetwork.offlineMode = false;
		//Joinning Lobby
		if (!ready) {
			PhotonNetwork.JoinLobby();
			debugText.GetComponent<Text> ().color = Color.red;
			debugText.GetComponent<Text> ().text = "You're not connected yet";
		} 
		else 
			if (!PhotonNetwork.connected) {
				debugText.GetComponent<Text> ().color = Color.red;
				debugText.GetComponent<Text> ().text = "You're not connected yet";
				if (PlayerPrefs.GetInt ("IsOnRoom", 0) == 1) {
					PhotonNetwork.Reconnect();
					debugText2.GetComponent<Text> ().color = Color.yellow;
					debugText2.GetComponent<Text> ().text = "Connecting...";

				}else {
					PhotonNetwork.ConnectUsingSettings ("0.1");
					debugText2.GetComponent<Text> ().color = Color.yellow;
					debugText2.GetComponent<Text> ().text = "Connecting...";

				}

				//Or reconnect if player Disconnect from a room before
			} else if (PlayerPrefs.GetInt ("IsOnRoom", 0) == 1) {
				bool found = searchRoom ();
				if (!found) {
					PlayerPrefs.SetInt ("IsOnRoom", 0);
					PlayerPrefs.SetInt ("Score", 0);
					PlayerPrefs.SetInt ("Round", 1);
					debugText.GetComponent<Text> ().color = Color.red;
					debugText.GetComponent<Text> ().text = "Can't Reconnect. Click again to start new game";
				} else {
					PhotonNetwork.JoinRoom (PlayerPrefs.GetString ("RoomIsIn"));

				}

			} 
		//If no room, create room
			else if (roomsList.Length == 0) {
				// Create Room
				roomName = PhotonNetwork.player.name + Random.Range(0,10).ToString() + Random.Range(0,10).ToString() +Random.Range(0,10).ToString()+Random.Range(0,10).ToString()+Random.Range(0,10).ToString()+Random.Range(0,10).ToString()+Random.Range(0,10).ToString()+Random.Range(0,10).ToString()+Random.Range(0,10).ToString();
				PhotonNetwork.JoinOrCreateRoom (roomName, roomopt, TypedLobby.Default);
				debugText.GetComponent<Text> ().color = Color.green;
				debugText.GetComponent<Text> ().text = "Creating room...";
			}
		// Join Room
			else 
			{



				PhotonNetwork.JoinRandomRoom();
				debugText.GetComponent<Text> ().color = Color.green;
				debugText.GetComponent<Text> ().text = "Entering room...";
			}
		

	}

	public void FriendCoop(){


		roomopt = new RoomOptions(){isVisible = false, maxPlayers = 4};
		roomopt.publishUserId = true;
		roomopt.customRoomProperties = new PhotonHashTable() { { "Round", 1 },{"State",0},{"FlagOwner",-9999}, {"Map","City"} , {"Mode","C"}, {"Friend", "Y"}};

		//Get The room list
		//	roomsList = PhotonNetwork.GetRoomList();
		PhotonNetwork.offlineMode = false;
		//Joinning Lobby
		if (!ready) {
			PhotonNetwork.JoinLobby();
			debugText.GetComponent<Text> ().color = Color.red;
			debugText.GetComponent<Text> ().text = "You're not connected yet";
		} 
		else 
			if (!PhotonNetwork.connected) {
				debugText.GetComponent<Text> ().color = Color.red;
				debugText.GetComponent<Text> ().text = "You're not connected yet";
				PhotonNetwork.ConnectUsingSettings ("0.1");
				debugText2.GetComponent<Text> ().color = Color.yellow;
				debugText2.GetComponent<Text> ().text = "Connecting...";



			}
		//If no room, create room
			else  {
				// Create Room
			if (nameinput.text == "") {

					debugText.GetComponent<Text> ().color = Color.red;
					debugText.GetComponent<Text> ().text = "Name Invalid";
			} 
				else {

					roomName = nameinput.text;
					PhotonNetwork.JoinOrCreateRoom (roomName, roomopt, TypedLobby.Default);
					debugText.GetComponent<Text> ().color = Color.green;
					debugText.GetComponent<Text> ().text = "Joining room...";
			}
			}
	}
}
