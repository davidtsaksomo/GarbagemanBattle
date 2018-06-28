using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DatabaseControl;

public class NameInput : MonoBehaviour {

	public GameObject inputFieldText;

	[SerializeField]
	Text debug;
	[SerializeField]
	Button proceed;
	string uname;
	string temp;
	// Use this for initialization
	void Start () {
		//inputFieldText.GetComponent<InputField>().text = PlayerPrefs.GetString ("playerName", "Player");
	}
	
	// Update is called once per frame
	public void nameInput(string name){
		//PlayerPrefs.SetString ("playerName", name);
		//PhotonNetwork.playerName = name;

		proceed.interactable = false;
		debug.color = Color.red;
		if (name.Contains (" ") || name.Contains ("*")) {
			debug.text = "Username not valid";

		} else {

			StartCoroutine(Login(name.ToLower(),"aa"));
			temp = name;
			debug.text = "Checking username....";
		}
		}



	public void editName(){
		proceed.interactable = false;
	}
		
	IEnumerator Login(string username, string password) {
		IEnumerator e = DC.Login(username, password);
		while(e.MoveNext()) {
		yield return e.Current;
		}
		WWW returned = e.Current as WWW;
	
		if (returned.text == "incorrectUser") {
		//Username not found in database
		//Do Stuff
			debug.text = "Username available";
			debug.color = Color.green;
			uname = temp;
			proceed.interactable = true;
		}
		if (returned.text == "incorrectPass") {
		//Username found but password incorrect
		//Do Stuff
			debug.text = "Username not Available";
			//proceed.interactable = false;
		}
		if (returned.text == "ContainsUnsupportedSymbol") {
		//One of the parameters contained a “-“ symbol
		//Do Stuff
			debug.text = "Username not Valid";
		}
		if (returned.text == "Error") {
		//Should not login as another error occurred
		//Do Stuff
			debug.text = "Error";
		}
		}

	IEnumerator Register(string username, string password, string data) {
		IEnumerator e = DC.RegisterUser(username, password, data);
		while(e.MoveNext()) {
			yield return e.Current;
		}
		WWW returned = e.Current as WWW;
		if (returned.text == "Success") {
			PlayerPrefs.SetString ("playerName", uname);
			Application.LoadLevelAsync ("Start");
			//Account Created Successfully
			//Do Stuff
		}
		if (returned.text == "usernameInUse") {
			//Account Not Created due to username being used on another account
			//Do Stuff
			debug.text = "Username not Available";
			proceed.interactable = false;
		}
		if (returned.text == "ContainsUnsupportedSymbol") {
			//Account Not Created as one of the parameters contained a “-“ symbol
			//this will not be returned in this example as parameters were “a” “b” and “c”
			//Do Stuff
			debug.text = "Username not Valid";
			proceed.interactable = false;
		}
		if (returned.text == "Error") {
			//Account Not Created, another error occurred
			//Do Stuff
			debug.text = "Error";
			proceed.interactable = false;
		}
	}

	public void ProceedButton(){
		StartCoroutine(Register(uname.ToLower(),"",""));

	}
}
