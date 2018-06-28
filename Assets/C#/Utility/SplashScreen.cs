using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SplashScreen : MonoBehaviour {
	public float x;
	public float y;
	public float t;
	Vector3 tujuan;
	//	RectTransform CanvasRect = Canvas.GetComponent<RectTransform>();
	// Use this for initialization
	void Start () {
		x = x * Screen.height / 480;
		y = y * Screen.height / 800;
		tujuan = new Vector3 (transform.position.x - x, transform.position.y - y, 0);
		Invoke ("LoadLevel", 2f);
		//		tujuan = new Vector3 (((transform.position.x*CanvasRect.sizeDelta.x)-(CanvasRect.sizeDelta.x*0.5f)), ((transform.position.y*CanvasRect.sizeDelta.y)-(CanvasRect.sizeDelta.y*0.5f)), 0);
	}

	// Update is called once per frame
	void Update () {
		
			
		transform.position = (Vector3.Slerp (transform.position, tujuan, t));
		


	}
	void LoadLevel(){
		if (PlayerPrefs.HasKey ("playerName")) {
			Application.LoadLevel ("Start");
		} else {
			Application.LoadLevel("Register");
		}
	}


}
