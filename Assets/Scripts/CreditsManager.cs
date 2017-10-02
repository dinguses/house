using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class CreditsManager : MonoBehaviour {

	public Image credits;
	public bool toFadeOut = false;
	public Image fadeImage;
	public AudioSource musicTrack;
	public GameSettings gameSettings;
	public bool moveCredits = true;

	void Start () {
		gameSettings = JsonUtility.FromJson<GameSettings> (File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

		AudioClip creditsTheme = Resources.Load ("track8") as AudioClip;
		musicTrack.volume = gameSettings.masterVolume * gameSettings.musicVolume;
		musicTrack.loop = true;
		musicTrack.clip = creditsTheme;
		musicTrack.Play ();
	}

	void FixedUpdate() {
		if (moveCredits) {
			credits.transform.Translate (new Vector3 (0f, 1f, 0f));
		}

		if (toFadeOut) {
			Color currColor = fadeImage.color;

			if (fadeImage.color.a < 1.0f) {
				currColor.a = currColor.a += .1f;
				fadeImage.color = currColor;
			} else {
				SceneManager.LoadScene ("Menu");
			}
		}

		if (credits.transform.position.y > 3520.0f) {
			StartCoroutine(WaitOnYOU());
		}
	}

	public IEnumerator WaitOnYOU() {
		moveCredits = false;
		yield return new WaitForSeconds(1.0f); // waits 1 seconds
		toFadeOut = true; // will make the update method pick up 
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
			toFadeOut = true;
		}
	}
}
