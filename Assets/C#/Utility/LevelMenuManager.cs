using UnityEngine;
using System.Collections;

public class LevelMenuManager : MonoBehaviour {
	bool isOnMenu;
	[SerializeField]
	GameObject Menu;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void MenuButton(){
		isOnMenu = !isOnMenu;
		if (isOnMenu) {
			Menu.SetActive (true);
			if (PhotonNetwork.offlineMode)
				Time.timeScale = 0f;
		}
		else {
			Menu.SetActive (false);
			if (PhotonNetwork.offlineMode)
				Time.timeScale = 1f;
		}
	}
	public void LeaveRoom(){
		Time.timeScale = 1f;
		PlayerPrefs.SetInt ("IsOnRoom", 0);
		PlayerPrefs.SetInt ("OwnerID", -9999);
		PlayerPrefs.SetInt ("Score", 0);
		PlayerPrefs.SetInt ("Round", 1);
		PhotonNetwork.LeaveRoom ();
	}
}
