using UnityEngine;
using System.Collections;

public class DestroyParent : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "Weapon")
			transform.root.GetComponent<PickableObject> ().DestroySelf ();
	}
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Weapon")
			transform.root.GetComponent<PickableObject> ().DestroySelf ();
		
	}
}
