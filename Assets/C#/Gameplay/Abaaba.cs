using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Abaaba : MonoBehaviour {

	[SerializeField]
	float destroyTime;
	Text thisText;
	int i;
	int j;
	string winnername;
	// Use this for initialization
	void Awake(){	thisText = GetComponent<Text> ();}
	void Start () {

	}

	// Update is called once per frame
	public void StartRound(){
		thisText.text = "Round Start";
		Invoke ("disableOnDelay", 3f);
	}
	public void NextRound(bool Tie){
		if(!Tie)
			thisText.text = "Next Round";
		else
			thisText.text = "Tie Break";
		Invoke ("Count", 1f);
		Invoke ("disableOnDelay", 3f);
	}
	void Count(){
		i = 3;
		StartCoroutine ("Counting");

	}
	IEnumerator Counting(){
		thisText.text = ""+i;
		i--;
		yield return new WaitForSeconds (0.5f);
		if (i > 0) {
			StartCoroutine ("Counting");
		} else {
			thisText.text = "START!";
		}

	}
	void disableOnDelay(){
		this.gameObject.SetActive (false);
	}
	public void EndGame(){
		thisText.text = "Round Over";
	}
	IEnumerator winner(){
		thisText.text = "Winner: " + winnername + "\nDisconnect in " + j;
		j--;
		yield return new WaitForSeconds (1f);
		if (j > 0) {
			StartCoroutine ("winner");
		} else {
			
		}
	}

}
