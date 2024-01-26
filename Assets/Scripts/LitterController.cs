using UnityEngine;

public class LitterController : MonoBehaviour {

	private Transform robotPos;

	private Rigidbody rb;

	public RobotController script;

	private float dist, timer;

	// Use this for initialization
	void Start () {
		robotPos = GameObject.Find ("Wall-E/Audio Source").transform;
		rb = GetComponent<Rigidbody> ();
		script = GameObject.Find("Wall-E").GetComponent<RobotController> ();
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		dist = Vector3.Distance (robotPos.position, transform.position);

		if (transform.position.y < -250) {
			tag = "Untagged";
			script.LitterCount ();
			Destroy (gameObject);
		}

		if (rb.velocity.magnitude > 20 && tag == "Litter") {
			tag = "Untagged";
			script.LitterCount ();
		}

		if (dist < 20) {
			// Si le déchet vient de se faire attraper
			if (dist < 9) {
				rb.useGravity = true;
				rb.AddForce((robotPos.position - transform.position) * 3);
				if (tag == "Litter") {
					tag = "Untagged";
					script.LitterCount ();
				}
			} else if (timer < Time.fixedTime && !IsInvoking() && script.target == gameObject) {
				Fly ();
			}
		} else {
			if (tag != "Litter" && rb.velocity.magnitude < 30) {
				tag = "Litter";
				script.LitterCount ();
			}
			// On ralentit le déchet, puisqu'il est loin du robot, pas trop lent, pas trop rapide
			if (rb.velocity.magnitude < 10 && rb.velocity.magnitude > 0.5f) {
				rb.velocity *= 0.9f;
			}
		}
	}

	/**
	 * On fait léviter le déchet.
	 */
	void Fly () {
		rb.useGravity = false;
		rb.velocity = rb.angularVelocity = Vector3.zero;
		transform.position = transform.position + Vector3.up;
		Invoke (nameof(Fall), 5f);
	}

	void Fall () {
		rb.useGravity = true;
		timer = Time.fixedTime + 2;
	}
}
