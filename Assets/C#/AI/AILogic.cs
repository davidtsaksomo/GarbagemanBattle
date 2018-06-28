using UnityEngine;
using System.Collections;

public class AILogic : MonoBehaviour {

	enum States{SearchFlag, GoToFlag, GuardFlag, RetrieveFlag}//Kalo ganti jangan lupa ganti juga parameter yg di flag handiling
	[SerializeField]
	States state;
	AIControl aictrl;
	GameObject flag;
	GameObject[] flagspawner;
	GameObject fort;
	public bool isguardingfort;
	// Use this for initialization
	void Start () {
		aictrl = GetComponent<AIControl> ();
		flag = GameObject.FindGameObjectWithTag ("Flag");
		flagspawner = GameObject.FindGameObjectsWithTag ("FlagSpawner");
		StateChange ((int)States.SearchFlag);
		aictrl.UpdatePath ();
		StartCoroutine ("ScanPickups");

	}
	// Update is called once per frame
	void Update () {
		if (!flag) {
			state = States.SearchFlag;
			flag = GameObject.FindGameObjectWithTag ("Flag");
		} else if (state == States.SearchFlag) {
			
			if (Mathf.Abs (flag.transform.position.x - transform.position.x) < 15 && Mathf.Abs (flag.transform.position.x - transform.position.x) < 10) {
		
				StateChange ((int)States.GoToFlag);
			}
		}
		else
		if (isguardingfort) {
			if(Vector2.Distance(flag.transform.position,transform.position) < 5F){
				aictrl.target = flag.transform;
				aictrl.UpdatePath ();
			}
		}



	}

	public void PathEnd(){
		StateChange ((int)state);
	}

	[PunRPC]
	public void StateChange(int st){
		state = (States)st;
		isguardingfort = false;
		if (state ==States.SearchFlag) {
			//Make sure flag location traversed only once;
		
			aictrl.target = flagspawner[ Random.Range (0, flagspawner.Length)].transform;
		}

		if (state ==States.GoToFlag) {
			aictrl.target = flag.transform;
		}
		if (state ==States.GuardFlag) {
			fort = GameObject.FindGameObjectWithTag ("Fort");
			flag = GameObject.FindGameObjectWithTag ("Flag");
			if (Vector2.Distance (fort.transform.position, transform.position) < Vector2.Distance (flag.transform.position, transform.position) / 0.75f) {
				
				aictrl.target = fort.transform;
				isguardingfort = true;
			} else {
				aictrl.target = flag.transform;
			}
		}
		if (state ==States.RetrieveFlag) {
			fort = GameObject.FindGameObjectWithTag ("Fort");
			if (fort)
			aictrl.target = fort.transform;
		}

		aictrl.UpdatePath ();
	}


	IEnumerator ScanPickups(){
		GameObject[] pickups = GameObject.FindGameObjectsWithTag ("Pickable");
		if (state == States.GuardFlag) {
			
			foreach (GameObject pickup in pickups) {
				if (Vector2.Distance (pickup.transform.position, transform.position) < 10f) {
					aictrl.target = pickup.transform;
					break;
				}
			}
		}
	
		yield return new WaitForSeconds (3f);
		StartCoroutine ("ScanPickups");
	}
	public void Attacked(GameObject attacker){
		if (state == States.GuardFlag && isguardingfort) {
			isguardingfort = false;
			aictrl.target = flag.transform;
			aictrl.UpdatePath();
		}
	}
}
