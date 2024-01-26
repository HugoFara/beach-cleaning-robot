using UnityEngine;
using System.Collections;

public class RobotController : MonoBehaviour {

	//public Vector3 leftCatPos, rightCatPos;
	[Range(0f, 100f)]
	public float perf;

	public float elecPower;

	private float mecPower, blinded;

	private int i, j, foo;

	//private int[] sign = new int[2];

	//private bool wheel;

	private GameObject robot, leftCat, rightCat, centerCal, backwardCal, rightCal, hq;

	public GameObject target;

	//private GameObject[] wheels;

	// litters stocke les déchets par ordre de hiérarchie, distant par leur distance au centre de gravité
	public GameObject[] litters, distant;

	private AudioSource sound;

	private Transform robCenter;

	private Rigidbody rb;

	private LineRenderer line;

	private Vector3 movingForce;

	private Actions auto;

	private LitterSwallower blade;

	public Informations infos;

	void Start () {
		robot = GameObject.FindWithTag ("Player");
		// On assigne des variables aux objets qui serviront, pour des raisons de performances
		leftCat = GameObject.Find ( "Wall-E/Pieces/Left Caterpillar Container" );
		rightCat = GameObject.Find ( "Wall-E/Pieces/Right Caterpillar Container" );

		centerCal = GameObject.Find ("Wall-E/Other");
		backwardCal = GameObject.Find ("Wall-E/Other/Backward Calibrator");
		//rightCal = GameObject.Find ("Wall-E/Other/Right Calibrator");
		sound = GameObject.Find("Wall-E/Audio Source").GetComponent<AudioSource> ();
		robCenter = sound.transform;
		blinded = 0;

		hq = GameObject.Find ("Base");

		InvokeRepeating ("litterCount", 0f, 3f);

		// On règle la puissance electrique * 2 car on a deux boîtiers de pile
		elecPower = PlayerPrefs.GetFloat ("Tension", 6) * PlayerPrefs.GetFloat ("Intensity", 1) * 2;

		if (robot) {
			rb = robot.GetComponent<Rigidbody> ();
			line = robot.GetComponent<LineRenderer> ();
			line.SetVertexCount (GameObject.FindGameObjectsWithTag ("Litter").Length + 1);
			// Puissance / tension pour le nombre de piles, fois 2 pour 2 batteries, fois la masse d'une pile, puis passage en kilogrammes
			rb.mass = 2.8f + PlayerPrefs.GetFloat ("Tension", 6) * PlayerPrefs.GetFloat ("Intensity", 1) * 2 * 23 * Mathf.Pow(10, -2f) / 1.5f;
		}
		auto = GameObject.FindGameObjectWithTag ("MainMap").GetComponent<Actions> ();
		blade = GameObject.Find ("Wall-E/Pieces/Roller Pieces/Blade").GetComponent<LitterSwallower> ();
		infos = GameObject.Find ("InformationsCanvas").GetComponent<Informations> ();
	}

	void Update () {
		sound.volume = Mathf.Lerp (0.2f, 1f, Vector3.Magnitude (rb.velocity) / 20f);
		// Puissance mécanique = puissance absorbée * rendement (en %)
		mecPower = elecPower * perf * 0.01f;
		rb.WakeUp ();
		setLine ();
	}

	void FixedUpdate () {
		if (auto.autoB) {
			// Debug.Log ("dist" + Vector3.Distance (target.transform.position, sound.transform.position) + "\ntime " + Time.timeScale);
			if (blinded < Time.fixedTime) {
				// On modifie le temps pour passer les phases de déplacement entre deux déchets
				Time.timeScale = Mathf.FloorToInt(Mathf.Lerp (2f, 5f, Vector3.Distance (target.transform.position, sound.transform.position) * 0.005f));
			} else {
				Time.timeScale = 1;
			}
		}
	}

	/* Cette fonction est appelée à chaque fois qu'un objet est en contact (produit une collision) avec le robot
	 * Beaucoup de conditions imbriquées pour des raisons d'efficacité, dans l'ordre on va :
	 * Vérifier qu'il y a contact
	 * Que ce contact aie lieu avec le terrain, et que l'utilisateur presse une touche de direction
	 * Ce que doit faire le robot pour aller dans la direction choisie
	 */
	void OnCollisionStay (Collision collisionInfo) {
		if (collisionInfo.gameObject.CompareTag ("MainMap") ) {
			if (auto.autoB) {
				if (Time.fixedTime > blinded) {
					autoMove (target.transform.position, sound.transform.position, centerCal.transform.position - backwardCal.transform.position);
				} else {
					motorsController (mecPower, 0);
					blade.rotate ();
				}
			} else if (Input.GetAxis ("Horizontal") + Input.GetAxis ("Vertical") != 0) {
				userMove ();
			}
		}
	}

	void userMove() {

		// Si l'utilisateur a appuyé sur flèche droite ou gauche
		if (Input.GetAxis ("Horizontal") != 0) {
			/* mecPower															: donne la norme
			 * Mathf.Sign( Input.GetAxis ("Horizontal") )		 				: donne le sens
			 * Direction et point d'application sont déjà connus (il s'agit des chenilles)
			 */
			if (rb.angularVelocity.magnitude < 1.2f) {
				motorsController (mecPower, Mathf.Sign (Input.GetAxis ("Horizontal")));
			}
		}

		// Si l'utilisateur a appuyé sur flèche haut ou bas
		if (Input.GetAxis ("Vertical") != 0) {
			/* backwardCal.transform.position - centerCal.transform.position 	: donne la direction
				 * power 															: donne la norme
				 * Mathf.Sign( Input.GetAxis ("Horizontal") ) et sign 				: donnent le sens
				 * sign va donner le sens, soit en l'inversant soit en ne changeant rien
				 */
			motorsController (mecPower * Mathf.Sign (Input.GetAxis ("Vertical")), 0);
		}
	}

	void autoMove(Vector3 objective, Vector3 pos, Vector3 dir) {
		/*
		 * objecive : coordonnées de l'objectif
		 * pos : coordonnées de notre robot
		 * dir : orientation du robot
		 * On travaille en 2 dimensions, pour plus de simplicité
		 */
		Vector3 targetAxis = objective - pos;
		// On vérifie la distance
		if (Vector3.Distance (objective, pos) > 12f) {
			// On tourne d'abord, si nécessaire, par une projection orthogonale sur le plan x,z
			if (Vector2.Angle (ProjectOnPlan2D(dir), ProjectOnPlan2D(targetAxis)) > 2f) {
				if (rb.angularVelocity.magnitude < 2f) {
					motorsController (mecPower, Mathf.Sign (Vector3.Cross(dir, targetAxis).y));
				}
			} else {
				motorsController (mecPower, 0);
			}
		} else {
			/* La distance robot-déchet est très petite, on va alors passer en pilotage en aveugle.
			   Pendant 5 secondes, on avance lentement avec le rouleau.								*/
			blinded = Time.fixedTime + 5;
		}

	}

	void setLine() {

		line.SetPosition (0, sound.transform.position);
		// Le nombre de points est le nombre de déchets, plus un pour le robot et plus un pour la base
		line.SetVertexCount (litters.Length + 2);

		// Le dernier point est la base
		line.SetPosition (litters.Length + 1, hq.transform.position);
		
		if (litters.Length == 0) {
			autoMove (hq.transform.position, sound.transform.position, centerCal.transform.position - backwardCal.transform.position);
			return;
		}

		int[] targIndex = new int[2];
		targIndex [0] = closestIndex (robCenter.position, litters);
		// Le premier objectif est le déchet le plus proche du robot
		target = litters [targIndex [0]];
		line.SetPosition (1, target.transform.position);

		switch (litters.Length) {
		case 1:
			// Un déchet ? On ne fait rien
			break;
		case 2:
			// Deux déchets ? On vise le plus proche, et ensuite l'autre
			line.SetPosition (2, litters [
				1 - targIndex [0]
			].transform.position);
			break;
		case 3:
			// Trois déchets, pareil, de proche en proche
			// On regarde lequel des deux déchets restant est le plus proche de la cible
			if (Vector3.Distance (target.transform.position, litters [Mathf.Abs (targIndex [0] - 1)].transform.position) < Vector3.Distance (target.transform.position, litters [Mathf.Abs (targIndex [0] - 2)].transform.position)) {
				line.SetPosition (2, litters [Mathf.Abs (targIndex [0] - 1)].transform.position);
				line.SetPosition (3, litters [Mathf.Abs (targIndex [0] - 2)].transform.position);
			} else {
				line.SetPosition (3, litters [Mathf.Abs (targIndex [0] - 1)].transform.position);
				line.SetPosition (2, litters [Mathf.Abs (targIndex [0] - 2)].transform.position);
			}
			break;
		default:
			// Au-delà de trois déchets, on calcul de manière empirique le chemin le plus court
			/*i = 0;
			while (i < litters.Length) {
				// if (litters [i]) line.SetPosition (i + 1, litters [i].transform.position);
				for (j = 0; j < distant.Length; j++) {
					Debug.Log (i + "/" + j);
					//Debug.Break ();
					if (distant [j]) {
						if (GameObject.Equals (distant [j], litters [i])) {
							targIndex [1] = j;
						} else {
							// Si le déchet actuel est plus proche que les autres déchets
							if (Vector3.Distance (centerOfLitters, litters [i].transform.position) < Vector3.Distance (centerOfLitters, distant [j].transform.position)) {
								// On décale tous les éléments du tableau
								for (int k = distant.Length - 1; k > j + 1; k--) {
									distant [k] = distant [k - 1];
									//Debug.Log ("k: " + k);
								}
								distant [j] = litters [i];
								i++;
								break;
							}
						}
					} else {
						distant [j] = litters [i];
						i++;
						break;
					}
					i++;
					if (i >= litters.Length)
						break;
				}
			}

			if (targIndex [1] < 3) {
				for (i = 0; i < 3; i++) {
					//Debug.Log (distant [Mathf.Abs (targIndex [1] - i)] + "\nn: " + (targIndex [1] - i));
					line.SetPosition (i + 2, distant [Mathf.Abs (targIndex [1] - i)].transform.position);
				}
			} else {
				for (i = 0; i < 3; i++) line.SetPosition (i + 2, distant [i].transform.position);
			}*/
			break;
		}
	}

	// Fait avancer le robot
	void motorsController(float power, float rotate) {
		Vector3 direction = Vector3.ProjectOnPlane(centerCal.transform.position - backwardCal.transform.position, Vector3.up);
		movingForce = power * direction;
		/* On va tracer des vecteurs force avec
		 * direction 	: la direction de la force (norme de 1 m)
		 * power 		: l'intensité et le sens (valeur absolue en W)
		 * rotate 		: le sens
		 * movingForce donne donc la direction et la norme (W.m)
		 * 
		 * rotate est une variable a trois états
		 * -1 : tourner à gauche
		 * 0 : aller tout droit
		 * 1 : tourner à droite
		 */
		if (rotate == 0) {
			/* 
			 * On vérifie qu'on peut aller plus vite, deux conditions :
			 	* Si le robot est suffisamment lent
			 	* Si le travail du couple des moteurs est résistant
			 */
			Debug.Log ("Speed : " + rb.velocity.magnitude);
			// Si la vitesse est supérieur à 2/3 m/s : à 2/3 m/s, on a une force de 18 N (force maximale)
			if (rb.velocity.magnitude * 3 > 2) {
				// Force = puissance / vitesse
				rb.AddForceAtPosition (movingForce / rb.velocity.magnitude, leftCat.transform.position + Vector3.up * 0.8f, ForceMode.Force);
				rb.AddForceAtPosition (movingForce / rb.velocity.magnitude, rightCat.transform.position + Vector3.up * 0.8f, ForceMode.Force);
			} else {
				// Pour éviter de diviser par un nombre qui tend vers 0, on triche pour obtenir la force maximale
				rb.AddForceAtPosition (movingForce * 3 / 2, leftCat.transform.position + Vector3.up * 0.8f, ForceMode.Force);
				rb.AddForceAtPosition (movingForce * 3 / 2, rightCat.transform.position + Vector3.up * 0.8f, ForceMode.Force);
			}
		} else {
			Debug.Log ("Rotation : " + rb.angularVelocity.magnitude);
			// Vitesse angulaire nominale : 
			if (rb.angularVelocity.magnitude < 0.8f && Mathf.FloorToInt (rb.angularVelocity.magnitude * 100) != 0) {
				// Force = force * distance / vitesse angulaire
				rb.AddForceAtPosition (- movingForce * rotate / rb.angularVelocity.magnitude, rightCat.transform.position, ForceMode.Force);
				rb.AddForceAtPosition (movingForce * rotate / rb.angularVelocity.magnitude, leftCat.transform.position, ForceMode.Force);
			} else {
				rb.AddForceAtPosition (- movingForce * rotate * 2, rightCat.transform.position, ForceMode.Force);
				rb.AddForceAtPosition (movingForce * rotate * 2, leftCat.transform.position, ForceMode.Force);
			}
		}
	}

	// Permet de retracer la ligne répétitivement
	public void litterCount() {
		litters = new GameObject[GameObject.FindGameObjectsWithTag ("Litter").Length];
		litters = GameObject.FindGameObjectsWithTag ("Litter");
		distant = new GameObject[litters.Length];
		// On met à jour le nombre de déchets
		infos.NLitters (litters.Length);
	}

	int closestIndex(Vector3 point, GameObject[] list) {
		/* Parmi tous les objets de la liste "list",
		 	on trouve le plus proche du point "point" */
		int index = 0;
		float dist = Vector3.Distance (point, list [0].transform.position);
		if (list.Length > 1) {
			for (i = 1; i < list.Length; i++) {
				if (Vector3.Distance (point, list [i].transform.position) < dist) {
					dist = Vector3.Distance (point, list [i].transform.position);
					index = i;
				}
			}
		}
		return index;
	}
		
	Vector2 ProjectOnPlan2D(Vector3 vector3D) {
		return new Vector2 (vector3D.x, vector3D.z);
	}
}
