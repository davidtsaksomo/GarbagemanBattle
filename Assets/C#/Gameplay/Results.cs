using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Results : MonoBehaviour {
	[SerializeField]
	Text[] results;
	[SerializeField]
	Text coins;
	[SerializeField]
	Text trophy;
	dreamloLeaderBoard dl;


	// Use this for initialization
	void Start () {
		Invoke ("LeaveRoom", 5f);
		if (PhotonNetwork.offlineMode) {
			for (int i = 0; i <= 3; i++) {
				results [i].text = (i + 1)+". " + PlayerPrefs.GetString ("std_off_" + (i + 1));
			}
		}
		else{
			if ((string)PhotonNetwork.room.customProperties ["Mode"] == "C") { //Coop Mode
				if ((string)PhotonNetwork.room.customProperties ["C"] == "B") { //Coop Mode
					results [0].text = "1. Team Blue";
					results [1].text = "2. Team Red";

				} else {
					results [1].text = "2. Team Blue";
					results [0].text = "1. Team Red";
				}
			} else { //Non Coop
				int i;
				for (i = 0; i <= 3; i++) {
					foreach (PhotonPlayer ply in PhotonNetwork.playerList) {
						if ((int)PhotonNetwork.room.customProperties ["C" + (i + 1)] == ply.ID) {
							results [i].text = (i + 1) + ". " + ply.name;
						}
					}

				}
			}

			
			this.dl = dreamloLeaderBoard.GetSceneDreamloLeaderboard();
			this.dl.DelScore (PlayerPrefs.GetString("playerName", "Tanpa nama"));
			this.dl.AddScore (PlayerPrefs.GetString ("playerName", "Tanpa nama"), PlayerPrefs.GetInt ("curr_trophy", 0));

		}
		int x = PlayerPrefs.GetInt ("dlt_trophy", 0);
		if (x >= 0) {
			if(x>0)
				trophy.color = Color.green;
			trophy.text = PlayerPrefs.GetInt ("curr_trophy", 50) + " (+ " + PlayerPrefs.GetInt ("dlt_trophy", 0) + ")";
		} else {
			trophy.text = PlayerPrefs.GetInt ("curr_trophy", 50) + " ( " + PlayerPrefs.GetInt ("dlt_trophy", 0) + ")";
		}
		coins.color = Color.green;
		coins.text = PlayerPrefs.GetInt ("curr_coin", 50) + " (+ " + PlayerPrefs.GetInt ("dlt_coin", 0) + ")";


	}

	
	// Update is called once per frame
	void Update () {
	
	}
	public void LeaveRoom(){
		PhotonNetwork.LeaveRoom ();
	}
	void OnLeftRoom(){
		PlayerPrefs.SetInt ("IsOnRoom", 0);
		PlayerPrefs.SetInt ("OwnerID", -9999);
		PlayerPrefs.SetInt ("Score", 0);
		PlayerPrefs.SetInt ("Round", 1);
		Application.LoadLevelAsync ("GameStart");
	}
}
