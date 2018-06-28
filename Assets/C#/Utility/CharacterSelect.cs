using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class CharacterSelect : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject.Find (PlayerPrefs.GetString ("Character")).GetComponent<Toggle> ().isOn = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Hero (bool ee){
		if (ee) {
			PlayerPrefs.SetString ("Character", "Hero");
		}
	}
	public void Fish (bool ee){
		if (ee) {
			PlayerPrefs.SetString ("Character", "Fish");
		}
	}
	public void Ghost (bool ee){
		if (ee) {
			PlayerPrefs.SetString ("Character", "Ghost");
		}
	}
	public void Apple (bool ee){
		if (ee) {
			PlayerPrefs.SetString ("Character", "Apple");
		}
	}
}
