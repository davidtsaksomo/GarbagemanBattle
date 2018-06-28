using UnityEngine;
using System.Collections;

public class PickableObject : MonoBehaviour {
	float blinkingTime = 0.05f;
	[SerializeField] [Range(3f, 20f)]  float destroyTime;
	// Use this for initialization
	void Start () {
		Invoke ("DestroySelf", destroyTime);
		StartCoroutine (Blinking(destroyTime - 3f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	[PunRPC]
	void Destroying(){
		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.Destroy (this.gameObject);
		}
	}
	public void DestroySelf(){
		gameObject.GetPhotonView ().RPC ("Destroying", PhotonTargets.All);
		StartCoroutine (Blinking(0f));
	}

	IEnumerator Blinking(float delay) {
		//clear color every sprite renderer
		if (delay > 0) {
			yield return new WaitForSeconds (delay);
		}
		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
		sprite.color = Color.clear;


		//wait 0.1 s
		yield return new WaitForSeconds(blinkingTime);
		//turn normal again
			sprite.color = Color.white;


		//wait again
		yield return new WaitForSeconds(blinkingTime);

		//repeat until vurnerable again
		StartCoroutine (Blinking(0f));

	}

}
