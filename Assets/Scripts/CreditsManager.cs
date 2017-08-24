using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour {

	public Image credits;
	public bool toFadeOut = false;
	public Image fadeImage;

	void Update () {
		credits.transform.Translate (new Vector3 (0f, 1f, 0f));

		if (toFadeOut) {
			Color currColor = fadeImage.color;

			if (fadeImage.color.a < 1.0f) {
				currColor.a = currColor.a += .1f;
				fadeImage.color = currColor;
			} else {
				SceneManager.LoadScene ("Menu");
			}
		}

		if (credits.transform.position.y > 1650f) {
			toFadeOut = true;
		}

		if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
			toFadeOut = true;
		}
	}
}
