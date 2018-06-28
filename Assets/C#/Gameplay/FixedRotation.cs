using UnityEngine;
using System.Collections;
// This class controls the movement of green arrow thingy on player's character
public class FixedRotation : MonoBehaviour {


	Vector3 dir;
	float angle;
	GameObject fort;
	GameObject flag;
	public bool isHandling;

	void Awake()
	{	//inizialitation
		fort = GameObject.FindGameObjectWithTag("Fort");
	}

	void LateUpdate()
	{	
		//Check for fort
		fort = GameObject.FindGameObjectWithTag("Fort");
		if (fort && isHandling) {
			//Point towards fort
			if (transform.localScale.x > 0) {
				dir = fort.transform.position - transform.position;
				angle = (Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg) - 180;
			} else {
				dir = fort.transform.position - transform.position;
				angle = ((Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg)-180);
			}
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		} else {
			//Search for flag
			flag = GameObject.FindGameObjectWithTag ("Flag");
			if (flag) {
				//Points toward flag
				if (transform.localScale.x > 0) {
					dir = flag.transform.position - transform.position;
					angle = (Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg) - 180;
				} else {
					dir = flag.transform.position - transform.position;
					angle = ((Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg)-180);
				}
				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}
				
			
		}
		
	}
}
