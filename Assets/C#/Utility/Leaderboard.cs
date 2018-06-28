using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Leaderboard : MonoBehaviour {
	
	dreamloLeaderBoard dl;
	dreamloPromoCode pc;

	[SerializeField] Text nameField;
	[SerializeField] Text scoreField;
	// Use this for initialization

	void Start () {


		// get the reference here...
		this.dl = dreamloLeaderBoard.GetSceneDreamloLeaderboard();
		this.dl.LoadScores ();
		// get the other reference here
		nameField.text = "Loading....";
		InvokeRepeating ("Check",2f, 1f);
	}

	void Check(){
		List<dreamloLeaderBoard.Score> 	 scoreList = dl.ToListHighToLow();


		if (scoreList == null) {
			GUILayout.Label ("(loading...)");
		} else {

			nameField.text = "";
			scoreField.text = "";
			int maxToDisplay = 20;
			int count = 0;
			foreach (dreamloLeaderBoard.Score currentScore in scoreList) {
				count++;
				nameField.text = nameField.text + count + ". " + currentScore.playerName + "\n";
				scoreField.text = scoreField.text + currentScore.score.ToString () + "\n";

				if (count >= maxToDisplay)
					break;
			}

			count++;
			for (; count <= maxToDisplay; count++) {
				nameField.text = nameField.text + count + ". " + "\n";
			}
		}
	}
	// Update is called once per frame
	void Update () {

	}
}
