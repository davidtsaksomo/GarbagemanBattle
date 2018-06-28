using UnityEngine;
using System.Collections;

public class CheckConnection : MonoBehaviour
{
	long res;
	bool isUnstabble;
	GameObject unstabble;
	// Use this for initialization
	void Start ()
	{	unstabble = GameObject.FindGameObjectWithTag ("Unstabble");
		unstabble.SetActive (false);
		InvokeRepeating ("CheckUnstable", 2f, 2f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	void CheckUnstable()
	{
		//Debug.LogWarning ("Resent: " + (PhotonNetwork.ResentReliableCommands - res));
		if (PhotonNetwork.ResentReliableCommands - res > 10) {
			Debug.LogWarning ("Connection unstable");
			unstabble.SetActive (true);
			isUnstabble = true;
		} else {
			unstabble.SetActive (false);
			isUnstabble = false;
		}
		//Debug.LogWarning (PhotonNetwork.ResentReliableCommands);
		res = PhotonNetwork.ResentReliableCommands;
		gameObject.GetPhotonView ().RPC ("A", gameObject.GetPhotonView ().owner);
	}
	[PunRPC]
	void A(){
	}

	void OnDisconnectedFromPhoton(){
		if (!isUnstabble && !PhotonNetwork.offlineMode) {
			int x = PlayerPrefs.GetInt ("curr_trophy", 100) - 10;
			if (x < 0)
				x = 0;
			PlayerPrefs.SetInt ("curr_trophy", x);
		}
	}
	void OnLeftRoom(){
		if (!isUnstabble && !PhotonNetwork.offlineMode) {
			int x = PlayerPrefs.GetInt ("curr_trophy", 100) - 10;
			if (x < 0)
				x = 0;
			PlayerPrefs.SetInt ("curr_trophy", x);
		}
		Application.LoadLevelAsync ("GameStart");
	}
}

