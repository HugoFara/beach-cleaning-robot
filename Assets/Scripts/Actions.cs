using UnityEngine;
using UnityEngine.UI;

public class Actions : MonoBehaviour {
	
	private GameObject robot;

	//private Text Defeat;

	private Text Defeat, auto;

	public bool autoB;

	// Use this for initialization
	void Start () {
		robot = GameObject.FindWithTag ("Player");
		Defeat = GameObject.Find("OptionsCanvas/Defeat").GetComponent<Text> ();
		Defeat.text = "";
		auto = GameObject.Find ("OptionsCanvas/Auto/Label").GetComponent<Text> ();
		auto.text = "Auto\nPilot\n(Désactivé)";
		autoB = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (robot.transform.position.y < -250) {
			Defeat.text = "Engin perdu, merci de recommencer.";
			Application.Quit ();
			Time.timeScale = 0;
		}
	}

	public void AutoPilotController() {
		autoB = !autoB;
		auto.text = "Auto\nPilot\n(" + ((autoB) ? "A" : "Désa") + "ctivé)";
	}
}