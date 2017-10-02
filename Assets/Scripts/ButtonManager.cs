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
	bool fadeInSplash;
	bool fadeOutSplash = false;
	bool fadeInMenu = false;
	bool menuFadedIn = false;
	bool upKeyHeld = false;
	bool downKeyHeld = false;
	float keyHeldTimer = 1.0f;
	float KEY_HOLD_INTERVAL_DEFAULT = 0.03f;
	float keyHoldInterval;

	public Image splashFadeImage;

	public GameObject[] musicTracks;
	public GameObject[] splashChecks;

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

	public Transform MenuCanvas;
	public Transform SplashCanvas;
	public Transform splashCheck;

	public void Start() {

		if (!System.IO.File.Exists(Application.persistentDataPath + "/gamesettings.json"))
		{
			defaultSettings = new GameSettings ();
			defaultSettings.fullscreen = true;

			//double ratio = Screen.currentResolution.width / Screen.currentResolution.height;

			//int width = Screen.currentResolution.width;
			//int height = Screen.currentResolution.height;

			int width = Screen.width;
			int height = Screen.height;

			float ratio = (width * 1.0f /height * 1.0f);

			if (ratio == 16.0f / 9.0f) {
				defaultSettings.resolutionW = 1920;
				defaultSettings.resolutionH = 1080;
			} else if (ratio == 8.0f / 5.0f) {
				defaultSettings.resolutionW = 1920;
				defaultSettings.resolutionH = 1200;
			} else if (ratio == 4.0f / 3.0f) {
				defaultSettings.resolutionW = 1600;
				defaultSettings.resolutionH = 1200;
			} else if (ratio == 5.0f / 4.0f) {
				defaultSettings.resolutionW = 1280;
				defaultSettings.resolutionH = 720;
			} else {
				defaultSettings.resolutionW = 1920;
				defaultSettings.resolutionH = 1200;
			}

			//defaultSettings.resolutionW = Screen.currentResolution.width;
			//defaultSettings.resolutionH = Screen.currentResolution.height;
			defaultSettings.masterVolume = 1.0f;
			defaultSettings.musicVolume = 1.0f;
			defaultSettings.effectsVolume = 1.0f;

			string jsonData = JsonUtility.ToJson (defaultSettings, true);
			File.WriteAllText (Application.persistentDataPath + "/gamesettings.json", jsonData);
		}

		keyHoldInterval = KEY_HOLD_INTERVAL_DEFAULT;

		musicTracks = GameObject.FindGameObjectsWithTag ("musictrack");
		splashChecks = GameObject.FindGameObjectsWithTag ("splashcheck");

		gameSettings = JsonUtility.FromJson<GameSettings> (File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

		Screen.fullScreen = gameSettings.fullscreen;
		Screen.SetResolution (gameSettings.resolutionW, gameSettings.resolutionH, Screen.fullScreen);
		//Screen.fullScreen = gameSettings.fullscreen;

		if (splashChecks.Length == 1) {
			fadeInSplash = true;
			MenuCanvas.gameObject.SetActive (false);
			SplashCanvas.gameObject.SetActive (true);
		} else {
			Destroy (splashCheck.gameObject);
			fadeInSplash = false;
			MenuCanvas.gameObject.SetActive (true);
			SplashCanvas.gameObject.SetActive (false);
			fadeInMenu = true;
		}

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

	public IEnumerator WaitOnSplash() {
		fadeOutSplash = false;
		yield return new WaitForSeconds(3.0f); // waits 3 seconds
		fadeOutSplash = true; // will make the update method pick up 
	}

	public void FixedUpdate() {
		if (fadeInSplash) {
			Color currColor = splashFadeImage.color;

			if (splashFadeImage.color.a > 0.0f) {
				currColor.a = currColor.a -= .01f;
				splashFadeImage.color = currColor;
			} else {
				fadeInSplash = false;

				StartCoroutine(WaitOnSplash());
			}
		}

		if (fadeInMenu) {
			Color currColor = fadeImage.color;

			if (fadeImage.color.a > 0.0f) {
				currColor.a = currColor.a -= .1f;
				fadeImage.color = currColor;
			} else {
				fadeInMenu = false;
				menuFadedIn = true;
			}
		}

		if (fadeOutSplash) {
			Color currColor = splashFadeImage.color;

			if (splashFadeImage.color.a < 1.0f) {
				currColor.a = currColor.a += .01f;
				splashFadeImage.color = currColor;
			} else {
				fadeOutSplash = false;

				MenuCanvas.gameObject.SetActive (true);
				SplashCanvas.gameObject.SetActive (false);

				fadeInMenu = true;
			}
		}

		if (toFadeOut) {
			Color currColor = fadeImage.color;

			if (fadeImage.color.a < 1.0f) {
				currColor.a = currColor.a += .1f;
				fadeImage.color = currColor;
			} else {
				toFadeOut = false;

				DontDestroyOnLoad (splashCheck);

				if (screenToLoad == "Options") {
					DontDestroyOnLoad (musicTrack);
				} else {
					var toDestroy = GameObject.FindGameObjectWithTag ("musictrack");
					Destroy (toDestroy);
				}
				SceneManager.LoadScene (screenToLoad);
			}
		}

		if (downKeyHeld) {
			if (keyHeldTimer > 0.0f) {
				keyHeldTimer -= keyHoldInterval;
			} 

			else {
				if (selectedIndex == 3) {
					selectedIndex = 0;
				} else {
					selectedIndex++;
				}

				HighlightOption ();

				keyHoldInterval = 0.1f;
				keyHeldTimer = 1.0f;
			}
		}

		if (upKeyHeld) {
			if (keyHeldTimer > 0.0f) {
				keyHeldTimer -= keyHoldInterval;
			} 

			else {
				if (selectedIndex == 0) {
					selectedIndex = 3;
				} else {
					selectedIndex--;
				}

				HighlightOption ();

				keyHoldInterval = 0.1f;
				keyHeldTimer = 1.0f;
			}
		}
	}

	public void Update() {
		if (menuFadedIn) {
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				if (selectedIndex == 3) {
					selectedIndex = 0;
				} else {
					selectedIndex++;
				}

				HighlightOption ();

				downKeyHeld = true;
			}

			if (Input.GetKeyDown (KeyCode.UpArrow)) {

				if (selectedIndex == 0) {
					selectedIndex = 3;
				} else {
					selectedIndex--;
				}
					
				HighlightOption ();

				upKeyHeld = true;
			}

			if (Input.GetKeyUp (KeyCode.DownArrow)) {
				downKeyHeld = false;
				keyHoldInterval = KEY_HOLD_INTERVAL_DEFAULT;
				keyHeldTimer = 1.0f;
			}

			if (Input.GetKeyUp (KeyCode.UpArrow)) {
				upKeyHeld = false;
				keyHoldInterval = KEY_HOLD_INTERVAL_DEFAULT;
				keyHeldTimer = 1.0f;
			}

			if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.KeypadEnter)) {
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

	public void NewGameBtnHover() {
		selectedIndex = 0;
		HighlightOption ();
	}

	public void ExitGameBtn() {
		Application.Quit ();
	}

	public void ExitGameBtnHover() {
		selectedIndex = 3;
		HighlightOption ();
	}

	public void OptionsBtn() {
		toFadeOut = true;
		screenToLoad = "Options";
	}

	public void OptionsBtnHover() {
		selectedIndex = 1;
		HighlightOption ();
	}

	public void CreditsBtn() {
		toFadeOut = true;
		screenToLoad = "Credits";
	}

	public void CreditsBtnHover() {
		selectedIndex = 2;
		HighlightOption ();
	}

	public void MainMenuBtn() {
		SceneManager.LoadScene ("Menu");
	}
}
