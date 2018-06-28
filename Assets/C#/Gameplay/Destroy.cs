using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {
	[SerializeField]
	float time;
	// Use this for initialization
	void Start () {
		Invoke ("destroy", time);
	}
	
	// Update is called once per frame
	void Update () {

	}
	void destroy()
	{
		Destroy (this.gameObject);
	}
}
