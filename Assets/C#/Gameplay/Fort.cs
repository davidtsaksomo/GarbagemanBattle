using UnityEngine;
using System.Collections;

public class Fort : MonoBehaviour {
	GameObject[] fortSpawner;
	// Use this for initialization
	void Awake(){
		fortSpawner = GameObject.FindGameObjectsWithTag ("FortSpawner");
		foreach (GameObject spawner in fortSpawner) {
			if (transform.position == spawner.transform.position) {
				transform.parent = spawner.transform.parent;
			}
		}
	}
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
