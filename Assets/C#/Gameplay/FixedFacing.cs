using UnityEngine;
using System.Collections;

public class FixedFacing : MonoBehaviour {
	[SerializeField]
	Transform parent;
	bool facingRight;
	Vector3 nameScale;
	// Use this for initialization
	void Start () {
		facingRight = true;
		nameScale = transform.localScale;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (parent.localScale.x < 0 && facingRight) {
			facingRight = !facingRight;
			nameScale.x = -1;		
			transform.localScale = nameScale;



		}
		if (parent.localScale.x > 0 && !facingRight) {
			facingRight = !facingRight;
			nameScale.x = 1;
			transform.localScale = nameScale;

		}

	
	}
}
