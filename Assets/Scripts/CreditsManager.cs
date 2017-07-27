using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour {

	public Image credits;

	void Update () {
		credits.transform.Translate (new Vector3 (0f, 1f, 0f));

		if (credits.transform.position.y > 1650f) {
			SceneManager.LoadScene ("Menu");
		}
	}
}
