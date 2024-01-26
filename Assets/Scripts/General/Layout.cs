using UnityEngine;
//using System.Collections;
/* La caméra doit suivre wall-E s'il tombe
 * Ajouter des commentaires partout
 * Afficher "Wall-E nous a quitté" si chute
 * Modifier le terrain (ajouter la mer, le désert, la plage, la benne, des dunes)
 * Modifier Wall-E
 * Rendre Wall-E autonome
 * Faire un calcul de trajectoire, abandon si trop long
 * Particules de sable
 * Résistance à l'avancement
 * Déplacer la caméra (ou automatique ? )
 * Modifier les déchets
 * Ajouter des bruits de vieux tracteur
 * Calcul de force : aus chenilles doit s'appliquer une FORCE
 ** On a entrée la puissance électrique des piles (power)
 ** La puissance mécanique du moteur est (power - puissance perdu par Joule - force électromotrice) * rendement
 ** On continue jusqu'à trouvé la force du moteur (couple moteur / rayon chenille)
 * Bruit de tracteur à : https://www.sounddogs.com/results.asp?Type=1&CategoryID=1022&SubcategoryID=7
 * F = Nb / r (N force normale, b coefficient de résistance au roulement et r rayon de la roue)
 * Pour une roue normale sur du sable, b = 0.3
 * 0.5 MJ/kg
 */
public class Layout : MonoBehaviour {
	// Cette classe est utilisée pour placer les différents objets au bon endroit

	//private GameObject robot; 

	private GameObject main_camera, map;

	private Actions robot;

	[Range(0, 10)]
	public float timeScale;

	void Start () {

		// Reglage de la position de la caméra
		main_camera = GameObject.Find ("Main Camera");
		main_camera.transform.position = new Vector3 (0, 150, 0);
		main_camera.transform.rotation = new Quaternion (90, 0, 0, 90);

		//robot = GameObject.FindWithTag ("Player");
		//robot.transform.position = new Vector3 (0, 10, 0);

		// Reglage de la position du terrain, selon le nom de ce terrain
		if ( GameObject.Find ("Terrain") ) {
			
			map = GameObject.Find ("Terrain");
			map.transform.position = new Vector3 (-20, 0, -20);

		} else if ( GameObject.Find ("Plane") ){
			
			map = GameObject.Find ("Plane");
			map.transform.position = Vector3.zero;

		}

		robot = GetComponent<Actions> ();
	}

	void Update() {
		if (Time.timeScale != timeScale && !robot.autoB) Time.timeScale = timeScale;
	}
}

	