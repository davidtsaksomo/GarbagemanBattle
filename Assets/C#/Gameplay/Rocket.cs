using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour 
{
	PhotonView photonView;
	public GameObject explosion;		// Prefab of explosion effect.
	void Start () 
	{
		// Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
		Invoke("DestroySelf", 4f);
		photonView = gameObject.GetPhotonView ();
	}

	void DestroySelf(){
		if (photonView.isMine) {
			PhotonNetwork.Destroy (gameObject);
		}

	}
	void OnExplode()
	{
		// Create a quaternion with a random rotation in the z-axis.
		PhotonNetwork.Instantiate("ExplosionSmall",new Vector3(transform.position.x,transform.transform.position.y, -6f),transform.rotation,0);

	}
	
	void OnTriggerEnter2D (Collider2D col) 
	{
		// If it hits an enemy...
		if (photonView.isMine) {
			
			if (col.tag == "Player") {
				// ... find the Enemy script and call the Hurt function.
				//col.gameObject.GetComponent<Enemy>().Hurt();

				// Call the explosion instantiation.
				OnExplode ();

				// Destroy the rocket.
				col.gameObject.GetComponent<AttackedProperties> ().wasAttacked (true);
				DestroySelf ();
			}else 
		if (col.tag == "Pickable") {
				
					col.gameObject.GetComponent<PickableObject> ().DestroySelf ();
					OnExplode ();
					DestroySelf ();
				}
			if (col.tag == "ground") {

					OnExplode ();
					DestroySelf ();
				}
		}
	}
}
