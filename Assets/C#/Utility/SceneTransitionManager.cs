using UnityEngine;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadStart(){
		Application.LoadLevelAsync ("Start");
	}

	public void LoadRoomRegister(){
		Application.LoadLevelAsync ("RoomRegister");

	}
	public void LoadRoomRegisterC(){
		Application.LoadLevelAsync ("RoomRegisterC");

	}
	public void LoadRoomStart(){
		Application.LoadLevelAsync ("RoomStart");
	}
	public void LoadGameStart(){
		Application.LoadLevelAsync ("GameStart");
	}
	public void LoadCharacterSelect(){
		Application.LoadLevelAsync ("CharacterSelect");
	}
	public void Quit(){
		Application.Quit ();
	}
	public void LoadStatistic(){
		Application.LoadLevelAsync ("Statistics");
	}
}
