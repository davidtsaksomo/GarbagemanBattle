using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{	
	//-----------------------------------------------------------------------------------------------------------------//
	public Rigidbody2D rocket;				// Prefab of the rocket.
	public float speed = 30f;				// The speed the rocket will fire at.

	[SerializeField] Transform launcher;
	private PlayerControl playerCtrl;		// Reference to the PlayerControl script.
	bool off = false;
	public AIControl aictrl;
	//---------------------------------------------------------------------------------------------------------------//

	void Awake()
	{
		// Setting up the references.
		speed = 30f;
		playerCtrl = transform.root.GetComponent<PlayerControl>();
		if (PhotonNetwork.offlineMode) {
			off= true;
			aictrl =  transform.root.GetComponent<AIControl>();
		}
	}


	void Update ()
	{	

	}

	public void fire(){
		// ... set the animator Shoot trigger parameter and play the audioclip.

		// If the player is facing right...
		if (!off || !aictrl) {
			if (playerCtrl.facingRight) {
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				GameObject bulletInstance = PhotonNetwork.Instantiate ("Bullet", launcher.position, Quaternion.Euler (new Vector3 (0, 0, 0)), 0);
				bulletInstance.GetComponent<Rigidbody2D> ().velocity = new Vector2 (speed, 0);
			} else {
				// Otherwise instantiate the rocket facing left and set it's velocity to the left.
				GameObject bulletInstance = PhotonNetwork.Instantiate ("Bullet", launcher.position, Quaternion.Euler (new Vector3 (0, 0, 180f)), 0);
				bulletInstance.GetComponent<Rigidbody2D> ().velocity = new Vector2 (-speed, 0);
			}
		} else {
			if (aictrl.facingRight) {
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				GameObject bulletInstance = PhotonNetwork.Instantiate ("Bullet", launcher.position, Quaternion.Euler (new Vector3 (0, 0, 0)), 0);
				bulletInstance.GetComponent<Rigidbody2D> ().velocity = new Vector2 (speed, 0);
			} else {
				// Otherwise instantiate the rocket facing left and set it's velocity to the left.
				GameObject bulletInstance = PhotonNetwork.Instantiate ("Bullet", launcher.position, Quaternion.Euler (new Vector3 (0, 0, 180f)), 0);
				bulletInstance.GetComponent<Rigidbody2D> ().velocity = new Vector2 (-speed, 0);
			}
		}
	}
}
