using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class StartManager : MonoBehaviour {

	public Text sambutan;
	// Use this for initialization
	void Start () {
		PhotonNetwork.ConnectUsingSettings ("0.1");

		sambutan.text = "Welcome, "+PlayerPrefs.GetString ("playerName", "Tanpa nama");
		if (!PlayerPrefs.HasKey ("curr_trophy")) {
			PlayerPrefs.SetInt ("curr_trophy", 100);
		}
		if (!PlayerPrefs.HasKey ("curr_coin")) {
			PlayerPrefs.SetInt ("curr_coin", 50);
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
