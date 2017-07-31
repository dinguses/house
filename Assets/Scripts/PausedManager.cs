using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class PausedManager : MonoBehaviour {

	public AudioSource pauseTrack;
	public GameSettings gameSettings;
	public Image fadeImage;
	bool toFadeImage = false;
	public Canvas canvas;
	public Canvas optionsCanvas;

	public Text ResumeText;
	public Text ResumeHigh;
	public Text ResumeArrow;
	public Text OptionsText;
	public Text OptionsHigh;
	public Text OptionsArrow;
	public Text MenuText;
	public Text MenuHigh;
	public Text MenuArrow;
	public Text QuitText;
	public Text QuitHigh;
	public Text QuitArrow;

	Color white0;
	Color white1;
	Color gray0;
	Color gray1;

	int selectedIndex = 0;

	// Use this for initialization
	void Start () {
		gameSettings = JsonUtility.FromJson<GameSettings> (File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

		white0 = new Color32 (255, 255, 255, 0);
		white1 = new Color32 (255, 255, 255, 255);
		gray0 = new Color32 (148, 148, 148, 0);
		gray1 = new Color32 (148, 148, 148, 255);


		HighlightOption ();
	}
	
	// Update is called once per frame
	void Update () {
		if (toFadeImage) {
			Color currColor = fadeImage.color;

			if (fadeImage.color.a < 1.0f) {
				currColor.a = currColor.a += .1f;
				fadeImage.color = currColor;
			} else {
				toFadeImage = false;
				switch (selectedIndex) {
				case 1:
					optionsCanvas.gameObject.SetActive (true);
					canvas.gameObject.SetActive (false);
					break;
				case 2:
					SceneManager.LoadScene ("Menu");
					break;
				case 3:
					Application.Quit ();
					break;
				}
			}
		} 

		/*if (canvas.gameObject.activeInHierarchy && !toFadeImage) {
			Color currColor = fadeImage.color;
			currColor.a = 1.0f;
			fadeImage.color = currColor;
		}*/

		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if (selectedIndex != 3) {
				selectedIndex++;
			}
			HighlightOption ();
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (selectedIndex != 0) {
				selectedIndex--;
			}
			HighlightOption ();
		}

		if (Input.GetKeyDown (KeyCode.Return)) {
			if (selectedIndex == 0) {
				if (!canvas.gameObject.activeInHierarchy) {
					canvas.gameObject.SetActive (true);
				} else {
					canvas.gameObject.SetActive (false);
				}
			} else {
				toFadeImage = true;
			}
		}
	}

	public void HighlightOption () {

		ResetAllButtons ();

		switch (selectedIndex) {
		case 0:
			ResumeText.color = white0;
			ResumeHigh.color = ResumeArrow.color = gray1;
			break;
		case 1:
			OptionsText.color = white0;
			OptionsHigh.color = OptionsArrow.color = gray1;
			break;
		case 2:
			MenuText.color = white0;
			MenuHigh.color = MenuArrow.color = gray1;
			break;
		case 3:
			QuitText.color = white0;
			QuitHigh.color = QuitArrow.color = gray1;
			break;
		}
	}

	public void ResetAllButtons() {
		ResumeText.color = OptionsText.color = MenuText.color = QuitText.color = white1;
		ResumeHigh.color = OptionsHigh.color = MenuHigh.color = QuitHigh.color = gray0;
		ResumeArrow.color = OptionsArrow.color = MenuArrow.color = QuitArrow.color = gray0;
	}
}
