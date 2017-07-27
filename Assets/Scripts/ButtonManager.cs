using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class ButtonManager : MonoBehaviour {
	
	//public Canvas canvas;
	public AudioSource musicTrack;
	public GameSettings gameSettings;

	public void Start() {
		AudioClip titleTheme = Resources.Load("track6") as AudioClip;
		gameSettings = JsonUtility.FromJson<GameSettings> (File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

		musicTrack.loop = true;
		musicTrack.clip = titleTheme;

		musicTrack.volume = gameSettings.masterVolume;

		musicTrack.Play ();
	}

	public void NewGameBtn() {

		/*var alpha = canvas.GetComponent<CanvasGroup> ().alpha;

		if (alpha > 0.0f) {
			alpha -= .1f;
			canvas.GetComponent<CanvasGroup> ().alpha = alpha;
			NewGameBtn ("Main");
		} */
		//else {
			SceneManager.LoadScene ("Main");
		//}
	}

	public void ExitGameBtn() {
		Application.Quit ();
	}

	public void OptionsBtn() {
		SceneManager.LoadScene ("Options");
	}

	public void CreditsBtn() {
		SceneManager.LoadScene ("Credits");
	}

	public void MainMenuBtn() {
		SceneManager.LoadScene ("Menu");
	}
}
