using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private GameObject robot;

	// Use this for initialization
	void Start () {
		robot = GameObject.FindWithTag ("Player");
		transform.position = new Vector3 (robot.transform.position.x, 150, robot.transform.position.z);
	}
	
	// Cette fonction est appellée après chaque image
	void LateUpdate () {
		// Les coordonnées sont choisies pour que la caméra revienne sur le robot si celle-ci sort de l'écran
		// Essayer de moderniser le calcul de distance, pour le rendre plus lisible
		if ( Mathf.Sqrt(Mathf.Pow(transform.position.x, 2) + Mathf.Pow(robot.transform.position.x, 2) ) > 50) {
			transform.position += (robot.transform.position.x - transform.position.x) * Vector3.right / 175;
		} 
		if (Mathf.Sqrt(Mathf.Pow(transform.position.z, 2) + Mathf.Pow(robot.transform.position.z, 2) ) > 20 ) {
			// Trouver la bonne fonction
			transform.position += (robot.transform.position.z - transform.position.z) * Vector3.forward / 175;
		} 
		if (transform.position.y - 100 > robot.transform.position.y) {
			transform.position += 0.4f * Vector3.down;
		}
		// Permet de zoomer
		transform.Translate (Vector3.forward * Input.GetAxis ("Mouse ScrollWheel") * 10);
		// Permet de regarder en permanance le joueur, effet un peu psychélique, à garder ?
		//transform.LookAt (robot.transform.position);
	}
}