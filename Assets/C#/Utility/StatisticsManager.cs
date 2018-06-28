using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class StatisticsManager : MonoBehaviour {
	[SerializeField]
	Text trpamount;
	[SerializeField]
	Text coinamount;
	[SerializeField]
	Text gamePlayed;
	[SerializeField]
	Text maxTrophy;
	[SerializeField]
	Text first;
	[SerializeField]
	Text second;
	[SerializeField]
	Text third;
	[SerializeField]
	Text fourth;
	[SerializeField]
	Text off;
	// Use this for initialization
	void Start () {
		trpamount.text = ""+PlayerPrefs.GetInt ("curr_trophy", 100);
		coinamount.text = ""+PlayerPrefs.GetInt ("curr_coin", 100);
		gamePlayed.text = "Game Played:"+PlayerPrefs.GetInt ("st_ply", 0);
		maxTrophy.text = "Max Trophy: "+PlayerPrefs.GetInt ("max_trophy", 100);
		first.text = "1st place: "+PlayerPrefs.GetInt ("st_plc1", 0);
		second.text = "2nd place: "+PlayerPrefs.GetInt ("st_plc2", 0);
		third.text = "3rd place: "+PlayerPrefs.GetInt ("st_plc3", 0);
		fourth.text = "4th place: "+PlayerPrefs.GetInt ("st_plc4", 0);
		off.text = "Offline Game Played: "+PlayerPrefs.GetInt ("st_plyf", 0);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
