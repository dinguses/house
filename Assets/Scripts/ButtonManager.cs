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
	public Image fadeImage;
	bool toFadeOut;
	string screenToLoad;

	public GameObject[] musicTracks;

	public GameSettings defaultSettings;

	public Text NGText;
	public Text NGHigh;
	public Text NGArrow;
	public Text OptionsText;
	public Text OptionsHigh;
	public Text OptionsArrow;
	public Text CreditsText;
	public Text CreditsHigh;
	public Text CreditsArrow;
	public Text QuitText;
	public Text QuitHigh;
	public Text QuitArrow;

	Color white0;
	Color white1;
	Color gray0;
	Color gray1;

	int selectedIndex = 0;

	public void Start() {

		if (!System.IO.File.Exists(Application.persistentDataPath + "/gamesettings.json"))
		{
			defaultSettings = new GameSettings ();
			defaultSettings.fullscreen = true;
			defaultSettings.resolutionW = Screen.currentResolution.width;
			defaultSettings.resolutionH = Screen.currentResolution.height;
			defaultSettings.masterVolume = 1.0f;
			defaultSettings.musicVolume = 1.0f;
			defaultSettings.effectsVolume = 1.0f;

			string jsonData = JsonUtility.ToJson (defaultSettings, true);
			File.WriteAllText (Application.persistentDataPath + "/gamesettings.json", jsonData);
		}

		musicTracks = GameObject.FindGameObjectsWithTag ("musictrack");


		gameSettings = JsonUtility.FromJson<GameSettings> (File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

		Screen.fullScreen = gameSettings.fullscreen;
		Screen.SetResolution (gameSettings.resolutionW, gameSettings.resolutionH, Screen.fullScreen);
		//Screen.fullScreen = gameSettings.fullscreen;

		if (musicTracks.Length == 1) {

			AudioClip titleTheme = Resources.Load ("track6") as AudioClip;
			musicTrack.loop = true;
			musicTrack.clip = titleTheme;

			musicTrack.volume = gameSettings.masterVolume * gameSettings.musicVolume;
			musicTrack.Play ();
		} 

		else {
			Destroy (musicTrack);
		}

		toFadeOut = false;
		screenToLoad = "";

		white0 = new Color32 (255, 255, 255, 0);
		white1 = new Color32 (255, 255, 255, 255);
		gray0 = new Color32 (148, 148, 148, 0);
		gray1 = new Color32 (148, 148, 148, 255);

		HighlightOption ();
	}

	public void Update() {
		if (toFadeOut) {
			Color currColor = fadeImage.color;

			if (fadeImage.color.a < 1.0f) {
				currColor.a = currColor.a += .1f;
				fadeImage.color = currColor;
			} else {
				toFadeOut = false;
				if (screenToLoad == "Options") {
					DontDestroyOnLoad (musicTrack);
				} else {
					var toDestroy = GameObject.FindGameObjectWithTag ("musictrack");
					Destroy (toDestroy);
				}
				SceneManager.LoadScene (screenToLoad);
			}
		}

		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if (selectedIndex == 3) {
				selectedIndex = 0;
			}
			else {
				selectedIndex++;
			}

			HighlightOption ();
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {

			if (selectedIndex == 0) {
				selectedIndex = 3;
			}
			else {
				selectedIndex--;
			}

			if (selectedIndex != 0) {
			}
			HighlightOption ();
		}

		if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
			switch (selectedIndex) {
			case 0:
				NewGameBtn ();
				break;
			case 1:
				OptionsBtn ();
				break;
			case 2:
				CreditsBtn ();
				break;
			case 3:
				ExitGameBtn ();
				break;
			}
		}

		if (Input.GetKeyDown (KeyCode.Alpha0)) {
			NGText.font = Resources.Load ("VIDEOPHREAK") as Font;
			NGHigh.font = Resources.Load ("VIDEOPHREAK") as Font;
			OptionsText.font = Resources.Load ("VIDEOPHREAK") as Font;
			OptionsHigh.font = Resources.Load ("VIDEOPHREAK") as Font;
			QuitText.font = Resources.Load ("VIDEOPHREAK") as Font;
			QuitHigh.font = Resources.Load ("VIDEOPHREAK") as Font;
			CreditsText.font = Resources.Load ("VIDEOPHREAK") as Font;
			CreditsHigh.font = Resources.Load ("VIDEOPHREAK") as Font;
		}

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			NGText.font = Resources.Load ("LazenbyCompLiquid") as Font;
			NGHigh.font = Resources.Load ("LazenbyCompLiquid") as Font;
			OptionsText.font = Resources.Load ("LazenbyCompLiquid") as Font;
			OptionsHigh.font = Resources.Load ("LazenbyCompLiquid") as Font;
			QuitText.font = Resources.Load ("LazenbyCompLiquid") as Font;
			QuitHigh.font = Resources.Load ("LazenbyCompLiquid") as Font;
			CreditsText.font = Resources.Load ("LazenbyCompLiquid") as Font;
			CreditsHigh.font = Resources.Load ("LazenbyCompLiquid") as Font;
		}

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			NGText.font = Resources.Load ("johnny") as Font;
			NGHigh.font = Resources.Load ("johnny") as Font;
			OptionsText.font = Resources.Load ("johnny") as Font;
			OptionsHigh.font = Resources.Load ("johnny") as Font;
			QuitText.font = Resources.Load ("johnny") as Font;
			QuitHigh.font = Resources.Load ("johnny") as Font;
			CreditsText.font = Resources.Load ("johnny") as Font;
			CreditsHigh.font = Resources.Load ("johnny") as Font;
		}

		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			NGText.font = Resources.Load ("Para") as Font;
			NGHigh.font = Resources.Load ("Para") as Font;
			OptionsText.font = Resources.Load ("Para") as Font;
			OptionsHigh.font = Resources.Load ("Para") as Font;
			QuitText.font = Resources.Load ("Para") as Font;
			QuitHigh.font = Resources.Load ("Para") as Font;
			CreditsText.font = Resources.Load ("Para") as Font;
			CreditsHigh.font = Resources.Load ("Para") as Font;
		}
	}

	public void HighlightOption () {

		ResetAllButtons ();

		switch (selectedIndex) {
		case 0:
			NGText.color = white0;
			NGHigh.color = gray1;
			NGArrow.color = gray1;
			break;
		case 1:
			OptionsText.color = white0;
			OptionsHigh.color = gray1;
			OptionsArrow.color = gray1;
			break;
		case 2:
			CreditsText.color = white0;
			CreditsHigh.color = gray1;
			CreditsArrow.color = gray1;
			break;
		case 3:
			QuitText.color = white0;
			QuitHigh.color = gray1;
			QuitArrow.color = gray1;
			break;
		}
	}

	public void ResetAllButtons() {
		NGText.color = OptionsText.color = CreditsText.color = QuitText.color = white1;
		NGHigh.color = OptionsHigh.color = CreditsHigh.color = QuitHigh.color = gray0;
		NGArrow.color = OptionsArrow.color = CreditsArrow.color = QuitArrow.color = gray0;
	}

	public void NewGameBtn() {
		toFadeOut = true;
		screenToLoad = "Main";
	}

	public void ExitGameBtn() {
		Application.Quit ();
	}

	public void OptionsBtn() {
		toFadeOut = true;
		screenToLoad = "Options";
	}

	public void CreditsBtn() {
		toFadeOut = true;
		screenToLoad = "Credits";
	}

	public void MainMenuBtn() {
		SceneManager.LoadScene ("Menu");
	}
}
