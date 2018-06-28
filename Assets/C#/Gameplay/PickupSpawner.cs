using UnityEngine;
using System.Collections;

public class PickupSpawner : MonoBehaviour
{	
	GameObject[] pickupSpawner;
	PhotonView photonView;
	[SerializeField] string[] pickups;

	bool isMasterClient;
	GameObject[] players;
	// Use this for initialization
	void Awake(){

		photonView = GetComponent<PhotonView> ();
		pickupSpawner = GameObject.FindGameObjectsWithTag ("PickupSpawner");
	
	}
	void Start ()
	{
		isMasterClient = PhotonNetwork.isMasterClient;
		if (isMasterClient) {
			StartCoroutine (SpawnPickup ());
		}
		players = GameObject.FindGameObjectsWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	IEnumerator SpawnPickup(){
		float delay = Random.Range (10f, 15f);
		yield return new WaitForSeconds (delay);
		int indexPickup = Random.Range (0, pickups.Length);
		int indexSpawner = Random.Range (0, pickupSpawner.Length);
		PhotonNetwork.InstantiateSceneObject (pickups [indexPickup], pickupSpawner [indexSpawner].transform.position, Quaternion.Euler(0,0,50), 0, null);
	//	foreach (GameObject ply in players) {
	//		AILogic ailog = ply.GetComponent<AILogic> ();
	//if (ailog) {
	//			
	//		}
	//	}
		StartCoroutine (SpawnPickup ());

	}
	void OnMasterClientSwitched(){
		if (PhotonNetwork.isMasterClient) {
			StartCoroutine (SpawnPickup ());
		}
	}
}

