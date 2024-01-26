using UnityEngine;

public class LitterSwallower : MonoBehaviour {

	private GameObject centerCal, rightCal, collid;

	private Rigidbody rb;

	public float angle;

	void Start () {
		// On assigne des variables aux objets qui serviront, pour des raisons de performances
		centerCal = GameObject.Find ( "Wall-E/Other" );
		rightCal = GameObject.Find ( "Wall-E/Other/Right Calibrator" );
		collid = GameObject.Find ("Wall-E/Colliders/RollerColliders/BladeBox");
		rb = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody> ();
	}
	// Update is called once per frame
	void Update () {
		// Permet de faire tourner la pale à l'avant, manuellement
		if (Input.GetKey (KeyCode.Space)) {
			Rotate ();
		}
	}

	/**
	 * Fonction qui fait tourner la pale
	 */
	public void Rotate () {
		/// axe de rotation
		Vector3 axis = rightCal.transform.position - centerCal.transform.position;
		transform.RotateAround (transform.position, axis, angle * Time.timeScale);
		//collid.transform.RotateAround (collid.transform.position, axis, angle * Time.timeScale);
		collid.transform.RotateAround (collid.transform.position, axis, rb.velocity.magnitude * Time.timeScale);
	}
}
