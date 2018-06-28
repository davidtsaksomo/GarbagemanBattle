using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class ButtonManager : MonoBehaviour {

	public static int horAxis = 0;
	public static bool jump = false;
	public static bool fire = false;
	public static bool jumpHold = false;
	RaycastHit2D[] hitinfo;
	public int index = 9;
	public LayerMask touchmask;
	// Use this for initialization
	void Start () {
		hitinfo = new RaycastHit2D[5];
	}

	public static void SetJumpFalse(){
		jump = false;

	}
	// Update is called once per frame
	void Update () {


			#if UNITY_ANDROID
			if (Input.touchCount > 0) {
				for (int i = 0; i < Input.touchCount; i++){
				if (Input.GetTouch (i).phase == TouchPhase.Began || Input.GetTouch(i).phase == TouchPhase.Moved) {

						hitinfo[i] = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch (i).position), Vector2.zero, 100f,touchmask);
						if(hitinfo[i]){
							if ((hitinfo[i].transform.gameObject.name == "jumpButton"&&Input.GetTouch (i).phase == TouchPhase.Began)) {
								// Gerakan melompat

								jump = true;
								jumpHold = true;
							}

							if ((hitinfo[i].transform.gameObject.name == "fireButton")&&Input.GetTouch (i).phase == TouchPhase.Began) {
								// Gerakan Ke kiri
								fire = true;							
							}
							if ((hitinfo[i].transform.gameObject.name == "leftButton")) {
								// Gerakan Ke kiri
								horAxis = -1;
							}
							
							else if ((hitinfo[i].transform.gameObject.name == "rightButton")) {
								// Gerakan Ke kanan
								horAxis = 1;
							}
					// RaycastHit2D can be either true or null, but has an implicit conversion to bool, so we can use it like this
						}
				
				//----------------------------------------------------------------------------------------

				}
					if(Input.GetTouch(i).phase == TouchPhase.Ended){
					if(hitinfo[i]){
								if(hitinfo[i].transform.gameObject.name == "rightButton" || hitinfo[i].transform.gameObject.name == "leftButton")
									horAxis=0;
								if(hitinfo[i].transform.gameObject.name == "jumpButton")
									jumpHold = false;
						}
				}

				//--------------------------------------------------------------------------------

			}
			}
		else{
			horAxis = 0;
			jumpHold = false;
		}
		
			#endif
	}
}
