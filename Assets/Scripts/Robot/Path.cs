using UnityEngine;
using System.Collections;

public class Path : MonoBehaviour {

	private LineRenderer line;

	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void OnGUI () {
		GL.Begin (GL.LINES);
		line.SetPosition (1, transform.position);
	}

	void FixedUpdate () {
		GL.End ();
	}
}
