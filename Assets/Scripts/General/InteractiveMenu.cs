using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InteractiveMenu : MonoBehaviour {

	public GameObject[] scenePanels;

	private GameObject cellsPanel;

	private int panel, serial, derivating;

	private RawImage battery;

	private RectTransform size;

	private Text capa, mass, volt, amp, auto;

	// Initialisation
	void Start () {
		scenePanels = new GameObject[2];
		scenePanels [0] = GameObject.Find ("MainMenuCanvas/DunesPanel");
		scenePanels [1] = GameObject.Find ("MainMenuCanvas/TestScenePanel");
		scenePanels [0].SetActive (true);
		scenePanels [1].SetActive (false);

		panel = 1;

		size = GameObject.Find ("CellsCanvas/ImagePanel/RawImage").GetComponent<RectTransform> ();
		battery = GameObject.Find ("CellsCanvas/ImagePanel/RawImage").GetComponent<RawImage> ();
		capa = GameObject.Find ("CellsCanvas/ResultPanel/CapacityText").GetComponent<Text> ();
		mass = GameObject.Find ("CellsCanvas/ResultPanel/MassText").GetComponent<Text> ();
		volt = GameObject.Find ("CellsCanvas/ResultPanel/VoltText").GetComponent<Text> ();
		amp = GameObject.Find ("CellsCanvas/ResultPanel/AmpText").GetComponent<Text> ();
		auto = GameObject.Find ("CellsCanvas/ResultPanel/AutoText").GetComponent<Text> ();

		serial = 4;
		derivating = 1;

		cellsPanel = GameObject.Find ("CellsCanvas");
		cellsPanel.SetActive (false);
	}

	public void ChangePanel () {
		panel = (panel + 1) % 2;
		scenePanels [panel].SetActive (false);
		scenePanels [1 - panel].SetActive (true);
	}

	public void LoadScene () {
		switch (panel) {
		case 0:
			SceneManager.LoadScene ("Test Scene");
			break;
		case 1:
			SceneManager.LoadScene ("Dunes");
			break;
		default:
			SceneManager.LoadScene ("Test Scene");
			break;
		}
	}

	public void OpenChooseCells () {
		cellsPanel.SetActive (true);
	}

	public void CloseChooseCells () {
		PlayerPrefs.SetFloat ("Tension", serial * 1.5f);
		PlayerPrefs.SetFloat ("Intensity", derivating);
		cellsPanel.SetActive (false);
	}

	public void Tension (float number) {
		serial = Mathf.FloorToInt(number);
		battery.uvRect = new Rect (1, 0, number, battery.uvRect.height);
		volt.text = "- " + number * 1.5f + " V";
		SetImage ();
		MassAndCapa ();
	}

	public void Current (float number) {
		derivating = Mathf.FloorToInt(number);
		battery.uvRect = new Rect (1, 0, battery.uvRect.height, number);
		amp.text = "- " + number + " A";
		SetImage ();
		MassAndCapa ();
	}

	public void MassAndCapa () {
		// Nombre de cellules * masse d'une cellule
		mass.text = "- " + serial * derivating * 23 + " g";
		// Nombre de cellules * capacité d'une
		capa.text = "- " + serial * derivating * 2.85f + " A.h";
		/* Autonomie = capacité totale / intensité totale = nombre de cellules * capacité d'une / intensité totale
		 * Autonomie = cellules en série * cellules en dérivation * capacité / (intensité d'une * cellules en dérivation)
		 * Autonomie = cellules en série * capacité d'une cellule / intensité d'une cellule (1 A)
		 */
		auto.text = "- " + serial * 2.85f + " heures\nde fonctionnement";
	}

	void SetImage () {

		if (serial < 9) {
			size.localScale = new Vector3 (serial, size.localScale.y, size.localScale.z);
		} else {
			size.localScale = new Vector3 (8, size.localScale.y, size.localScale.z);
		}

		if (derivating < 4) {
			size.localScale = new Vector3 (size.localScale.x, derivating, size.localScale.z);
		} else {
			size.localScale = new Vector3 (size.localScale.x, 3, size.localScale.z);
		}
	}
}
