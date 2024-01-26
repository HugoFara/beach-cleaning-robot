using UnityEngine;
using UnityEngine.UI;

public class Informations : MonoBehaviour {

	private GameObject robot;

	public Canvas infosCanvas;

	private Vector3 move;

	private float dist;

	private int n;

	private Text time, distance, nlitters;

	// Use this for initialization
	void Start () {
		robot = GameObject.FindGameObjectWithTag ("Player");
		move = robot.transform.position;
		dist = 0;
		// On initialise un nombre de déchets
		n = GameObject.FindGameObjectsWithTag("Litter").Length;

		// On asigne les différents éléments d'information (temps, distance, nombre de déchet ramasés)
		time = GameObject.Find ("InformationsCanvas/Informations/Time").GetComponent<Text> ();
		distance = GameObject.Find ("InformationsCanvas/Informations/Distance").GetComponent<Text> ();
		nlitters = GameObject.Find ("InformationsCanvas/Informations/Number").GetComponent<Text> ();
		nlitters.text = "Déchets ramassés :\n0";

		infosCanvas = GameObject.Find ("InformationsCanvas").GetComponent ("Canvas") as Canvas;
		infosCanvas.enabled = false;
	}
	
	// FixedUpdate est appelé à la fin d'une image
	void FixedUpdate () {
		// La distance augmente de l'écart de position entre deux images
		dist += Vector3.Distance (robot.transform.position, move);
		// On affiche à chaque fois une décimale
		time.text = "Temps écoulé (s) :\n" + Mathf.Floor(Time.time * 10) * 0.1f;
		distance.text = "Distance parcourue (m) :\n" + Mathf.Floor(dist * 0.1f);
		move = robot.transform.position;
	}

	public void NLitters (int change) {
		// change est le nombre actuel de déchets, donc déchets ramassés = déchets au début - déchets restants
		nlitters.text = "Déchets ramassés :\n" + (n - change);
	}

	public void CloseOptions () {
		infosCanvas.enabled = false;
	}

	public void OpenOptions () {
		infosCanvas.enabled = true;
	}
}
