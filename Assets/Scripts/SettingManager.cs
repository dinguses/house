using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SettingManager : MonoBehaviour {

	Color white0;
	Color white1;
	Color gray0;
	Color gray1;

	public Text FullscreenArrow;
	public Text FullscreenLabel;
	public Text FullscreenHighLabel;
	public Text FullscreenLevel;
	public Text FullscreenHighLevel;
	public Text FullscrenRight;
	public Text FullscreenLeft;
	public Text FullscreenHighLeft;
	public Text FullscreenHighRight;

	public Text ResolutionArrow;
	public Text ResolutionLabel;
	public Text ResolutionHighLabel;
	public Text ResolutionLevel;
	public Text ResolutionHighLevel;
	public Text ResolutionRight;
	public Text ResolutionLeft;
	public Text ResolutionHighLeft;
	public Text ResolutionHighRight;

	public Text MasterVolArrow;
	public Text MasterVolLabel;
	public Text MasterVolHighLabel;
	public Text MasterVolLevel;
	public Text MasterVolHighLevel;
	public Text MasterVolRight;
	public Text MasterVolLeft;
	public Text MasterVolHighLeft;
	public Text MasterVolHighRight;

	public Text MusicVolArrow;
	public Text MusicVolLabel;
	public Text MusicVolHighLabel;
	public Text MusicVolLevel;
	public Text MusicVolHighLevel;
	public Text MusicVolRight;
	public Text MusicVolLeft;
	public Text MusicVolHighLeft;
	public Text MusicVolHighRight;

	public Text EffectsVolArrow;
	public Text EffectsVolLabel;
	public Text EffectsVolHighLabel;
	public Text EffectsVolLevel;
	public Text EffectsVolHighLevel;
	public Text EffectsVolRight;
	public Text EffectsVolLeft;
	public Text EffectsVolHighLeft;
	public Text EffectsVolHighRight;

	public Text BackLabel;
	public Text BackHighLabel;
	public Text BackArrow;

	int selectedIndex = 0;
	bool upKeyHeld = false;
	bool downKeyHeld = false;
	bool rightKeyHeld = false;
	bool leftKeyHeld = false;
	float keyHeldTimer = 1.0f;
	float KEY_HOLD_INTERVAL_DEFAULT = 0.05f;
	float keyHoldInterval;

	public List<Resolution> resolutionsList;
	public Resolution[] resolutions;
	public int currentRes;

	public Image fadeImage;
	public bool toFadeImage = false;

	public AudioSource musicSource;
	public AudioSource LoopingAudio;
	public GameSettings gameSettings;

	void Start() {

		FullscreenArrow = GameObject.Find("FullscreenArrow").GetComponent<Text>();
		FullscreenLabel = GameObject.Find("FullscreenLabel").GetComponent<Text>();
		FullscreenHighLabel = GameObject.Find("FullscreenHighLabel").GetComponent<Text>();
		FullscreenLevel = GameObject.Find("FullscreenLevel").GetComponent<Text>();
		FullscreenHighLevel = GameObject.Find("FullscreenHighLevel").GetComponent<Text>();
		FullscrenRight = GameObject.Find("FullscreenRight").GetComponent<Text>();
		FullscreenLeft = GameObject.Find("FullscreenLeft").GetComponent<Text>();
		FullscreenHighLeft = GameObject.Find("FullscreenHighLeft").GetComponent<Text>();
		FullscreenHighRight = GameObject.Find("FullscreenHighRight").GetComponent<Text>();

		ResolutionArrow = GameObject.Find("ResolutionArrow").GetComponent<Text>();
		ResolutionLabel = GameObject.Find("ResolutionLabel").GetComponent<Text>();
		ResolutionHighLabel = GameObject.Find("ResolutionHighLabel").GetComponent<Text>();
		ResolutionLevel = GameObject.Find("ResolutionLevel").GetComponent<Text>();
		ResolutionHighLevel = GameObject.Find("ResolutionHighLevel").GetComponent<Text>();
		ResolutionRight = GameObject.Find("ResolutionRight").GetComponent<Text>();
		ResolutionLeft = GameObject.Find("ResolutionLeft").GetComponent<Text>();
		ResolutionHighLeft = GameObject.Find("ResolutionHighLeft").GetComponent<Text>();
		ResolutionHighRight = GameObject.Find("ResolutionHighRight").GetComponent<Text>();

		MasterVolArrow = GameObject.Find("MasterVolArrow").GetComponent<Text>();
		MasterVolLabel = GameObject.Find("MasterVolLabel").GetComponent<Text>();
		MasterVolHighLabel = GameObject.Find("MasterVolHighLabel").GetComponent<Text>();
		MasterVolLevel = GameObject.Find("MasterVolLevel").GetComponent<Text>();
		MasterVolHighLevel = GameObject.Find("MasterVolHighLevel").GetComponent<Text>();
		MasterVolRight = GameObject.Find("MasterVolRight").GetComponent<Text>();
		MasterVolLeft = GameObject.Find("MasterVolLeft").GetComponent<Text>();
		MasterVolHighLeft = GameObject.Find("MasterVolHighLeft").GetComponent<Text>();
		MasterVolHighRight = GameObject.Find("MasterVolHighRight").GetComponent<Text>();

		MusicVolArrow = GameObject.Find("MusicVolArrow").GetComponent<Text>();
		MusicVolLabel = GameObject.Find("MusicVolLabel").GetComponent<Text>();
		MusicVolHighLabel = GameObject.Find("MusicVolHighLabel").GetComponent<Text>();
		MusicVolLevel = GameObject.Find("MusicVolLevel").GetComponent<Text>();
		MusicVolHighLevel = GameObject.Find("MusicVolHighLevel").GetComponent<Text>();
		MusicVolRight = GameObject.Find("MusicVolRight").GetComponent<Text>();
		MusicVolLeft = GameObject.Find("MusicVolLeft").GetComponent<Text>();
		MusicVolHighLeft = GameObject.Find("MusicVolHighLeft").GetComponent<Text>();
		MusicVolHighRight = GameObject.Find("MusicVolHighRight").GetComponent<Text>();

		EffectsVolArrow = GameObject.Find("EffectsVolArrow").GetComponent<Text>();
		EffectsVolLabel = GameObject.Find("EffectsVolLabel").GetComponent<Text>();
		EffectsVolHighLabel = GameObject.Find("EffectsVolHighLabel").GetComponent<Text>();
		EffectsVolLevel = GameObject.Find("EffectsVolLevel").GetComponent<Text>();
		EffectsVolHighLevel = GameObject.Find("EffectsVolHighLevel").GetComponent<Text>();
		EffectsVolRight = GameObject.Find("EffectsVolRight").GetComponent<Text>();
		EffectsVolLeft = GameObject.Find("EffectsVolLeft").GetComponent<Text>();
		EffectsVolHighLeft = GameObject.Find("EffectsVolHighLeft").GetComponent<Text>();
		EffectsVolHighRight = GameObject.Find("EffectsVolHighRight").GetComponent<Text>();

		BackLabel = GameObject.Find ("BackText").GetComponent<Text> ();
		BackHighLabel = GameObject.Find ("BackHighText").GetComponent<Text> ();
		BackArrow = GameObject.Find ("BackArrow").GetComponent<Text> ();

		gameSettings = new GameSettings ();

		musicSource = GameObject.FindGameObjectWithTag ("musictrack").GetComponent<AudioSource>();

		resolutionsList = new List<Resolution> ();

		resolutionsList.Add (new Resolution{ width = 1024, height = 768 });
		resolutionsList.Add (new Resolution{ width = 1280, height = 720 });
		resolutionsList.Add (new Resolution{ width = 1280, height = 1024 });
		resolutionsList.Add (new Resolution{ width = 1440, height = 900 });
		resolutionsList.Add (new Resolution{ width = 1600, height = 1200 });
		resolutionsList.Add (new Resolution{ width = 1680, height = 1050 });
		resolutionsList.Add (new Resolution{ width = 1920, height = 1080 });
		resolutionsList.Add (new Resolution{ width = 1920, height = 1200 });
		resolutions = resolutionsList.ToArray ();

		//resolutions = Screen.resolutions;

		/*var trimmedResolutions = new List<Resolution> ();

		for (int i = 0; i < resolutions.Length; ++i) {

			bool isNewRes = false;

			for (int j = 0; j < trimmedResolutions.Count; ++j) {
				if (trimmedResolutions [j].height == resolutions [i].height && trimmedResolutions [j].width == resolutions [i].width) {
					if (trimmedResolutions [j].refreshRate < resolutions [i].refreshRate) {
						trimmedResolutions [j] = resolutions [i];
					}

					isNewRes = true;
				}
			}

			if (!isNewRes) {
				trimmedResolutions.Add (resolutions [i]);
			}
		}

		resolutions = trimmedResolutions.ToArray ();*/

		LoopingAudio.loop = true;
		LoopingAudio.volume = 0.0f;
		LoopingAudio.clip = Resources.Load("trapmaking") as AudioClip;
		LoopingAudio.Play ();

		white0 = new Color32 (255, 255, 255, 0);
		white1 = new Color32 (255, 255, 255, 255);
		gray0 = new Color32 (148, 148, 148, 0);
		gray1 = new Color32 (148, 148, 148, 255);

		keyHoldInterval = KEY_HOLD_INTERVAL_DEFAULT;

		HighlightOption ();

		LoadSettings ();

		for (int i = 0; i < resolutions.Length; ++i) {
			if (resolutions [i].width == gameSettings.resolutionW && resolutions[i].height == gameSettings.resolutionH) {
				currentRes = i;
			}
		}

		MasterVolLevel.text = ((int)Mathf.Ceil ((gameSettings.masterVolume * 10))).ToString ();
		MasterVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.masterVolume * 10))).ToString ();
		MusicVolLevel.text = ((int)Mathf.Ceil ((gameSettings.musicVolume * 10))).ToString ();
		MusicVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.musicVolume * 10))).ToString ();
		EffectsVolLevel.text = ((int)Mathf.Ceil ((gameSettings.effectsVolume * 10))).ToString ();
		EffectsVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.effectsVolume * 10))).ToString ();
		ResolutionLevel.text = ResolutionHighLevel.text = gameSettings.resolutionW + " X " + gameSettings.resolutionH;
		if (gameSettings.fullscreen) {
			FullscreenLevel.text = "YES";
			FullscreenHighLevel.text = "YES";
		} else {
			FullscreenLevel.text = "NO";
			FullscreenHighLevel.text = "NO";
		}
	}

	void FixedUpdate() {
		if (selectedIndex == 4) {
			LoopingAudio.volume = gameSettings.masterVolume * gameSettings.effectsVolume;
			musicSource.volume = 0.0f;
		} else {
			LoopingAudio.volume = 0.0f;
			musicSource.volume = gameSettings.masterVolume * gameSettings.musicVolume;
		}

		if (toFadeImage) {
			Color currColor = fadeImage.color;

			if (fadeImage.color.a < 1.0f) {
				currColor.a = currColor.a += .1f;
				fadeImage.color = currColor;
			} else {
				SceneManager.LoadScene ("Menu");
			}
		}

		if (downKeyHeld) {
			if (keyHeldTimer > 0.0f) {
				keyHeldTimer -= keyHoldInterval;
			} 

			else {
				if (selectedIndex == 5) {
					selectedIndex = 0;
				}
				else {
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
					selectedIndex = 5;
				}
				else {
					selectedIndex--;
				}
				HighlightOption ();

				keyHoldInterval = 0.1f;
				keyHeldTimer = 1.0f;
			}
		}

		if (rightKeyHeld) {
			if (keyHeldTimer > 0.0f) {
				keyHeldTimer -= keyHoldInterval;
			} 

			else {
				if (selectedIndex == 2 || selectedIndex == 3 || selectedIndex == 4) {
					RightArrow ();
				}

				keyHoldInterval = 0.1f;
				keyHeldTimer = 1.0f;
			}
		}

		if (leftKeyHeld) {
			if (keyHeldTimer > 0.0f) {
				keyHeldTimer -= keyHoldInterval;
			} 

			else {
				if (selectedIndex == 2 || selectedIndex == 3 || selectedIndex == 4) {
					LeftArrow ();
				}

				keyHoldInterval = 0.1f;
				keyHeldTimer = 1.0f;
			}
		}
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if (selectedIndex == 5) {
				selectedIndex = 0;
			}
			else {
				selectedIndex++;
			}
			HighlightOption ();

			downKeyHeld = true;
		}

		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			RightArrow ();

			rightKeyHeld = true;
		}

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			LeftArrow ();

			leftKeyHeld = true;
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (selectedIndex == 0) {
				selectedIndex = 5;
			}
			else {
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

		if (Input.GetKeyUp (KeyCode.RightArrow)) {
			rightKeyHeld = false;
			keyHoldInterval = KEY_HOLD_INTERVAL_DEFAULT;
			keyHeldTimer = 1.0f;
		}

		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			leftKeyHeld = false;
			keyHoldInterval = KEY_HOLD_INTERVAL_DEFAULT;
			keyHeldTimer = 1.0f;
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			toFadeImage = true;
		}

		if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {

			if (selectedIndex == 5) {
				SaveSettings ();
				toFadeImage = true;
			}
		}

		/*if (Screen.currentResolution.width != gameSettings.resolutionW || Screen.currentResolution.height != gameSettings.resolutionH || Screen.fullScreen != gameSettings.fullscreen) {
			Screen.SetResolution (gameSettings.resolutionW, gameSettings.resolutionH, gameSettings.fullscreen);
			Screen.fullScreen = gameSettings.fullscreen;
		}*/
	}

	public void RightArrow() {

		int temp = 0;

		switch (selectedIndex) {
		case 0:
			if (Screen.fullScreen) {
				FullscreenLevel.text = "NO";
				FullscreenHighLevel.text = "NO";
				gameSettings.fullscreen = false;
			} else {
				FullscreenLevel.text = "YES";
				FullscreenHighLevel.text = "YES";
				gameSettings.fullscreen = true;
			}
			Screen.fullScreen = !Screen.fullScreen;
			//gameSettings.fullscreen = Screen.fullScreen;
			SaveSettings ();
			break;
		case 1:
			if (currentRes == (resolutions.Length - 1)) {
				currentRes = 0;
			} else {
				currentRes++;
			}
			ResolutionLevel.text = ResolutionHighLevel.text = resolutions [currentRes].width + " X " + resolutions [currentRes].height;
			gameSettings.resolutionH = resolutions [currentRes].height;
			gameSettings.resolutionW = resolutions [currentRes].width;
			Screen.SetResolution (resolutions [currentRes].width, resolutions [currentRes].height, Screen.fullScreen);
			SaveSettings ();
			break;
		case 2:
			temp = (int)Mathf.Ceil (gameSettings.masterVolume * 10);
			if (temp < 10) {
				temp += 1;
				gameSettings.masterVolume = ((float)temp) / 10;
				MasterVolLevel.text = ((int)Mathf.Ceil ((gameSettings.masterVolume * 10))).ToString ();
				MasterVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.masterVolume * 10))).ToString ();
				SaveSettings ();
			}
			break;
		case 3:
			temp = (int)Mathf.Ceil (gameSettings.musicVolume * 10);
			if (temp < 10) {
				temp += 1;
				gameSettings.musicVolume = ((float)temp) / 10;
				MusicVolLevel.text = ((int)Mathf.Ceil ((gameSettings.musicVolume * 10))).ToString ();
				MusicVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.musicVolume * 10))).ToString ();
				SaveSettings ();
			}
			break;
		case 4:
			temp = (int)Mathf.Ceil (gameSettings.effectsVolume * 10);
			if (temp < 10) {
				temp += 1;
				gameSettings.effectsVolume = ((float)temp) / 10;
				EffectsVolLevel.text = ((int)Mathf.Ceil ((gameSettings.effectsVolume * 10))).ToString ();
				EffectsVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.effectsVolume * 10))).ToString ();
				SaveSettings ();
			}
			break;
		}
	}

	public void LeftArrow() {

		int temp = 0;

		switch (selectedIndex) {
		case 0:
			if (Screen.fullScreen) {
				FullscreenLevel.text = "NO";
				FullscreenHighLevel.text = "NO";
				gameSettings.fullscreen = false;
			} else {
				FullscreenLevel.text = "YES";
				FullscreenHighLevel.text = "YES";
				gameSettings.fullscreen = true;
			}
			Screen.fullScreen = !Screen.fullScreen;
			SaveSettings ();
			break;
		case 1:
			if (currentRes == 0) {
				currentRes = resolutions.Length - 1;
			} else {
				currentRes--;
			}
			ResolutionLevel.text = ResolutionHighLevel.text = resolutions[currentRes].width + " X " + resolutions[currentRes].height;
			gameSettings.resolutionH = resolutions [currentRes].height;
			gameSettings.resolutionW = resolutions [currentRes].width;
			Screen.SetResolution (resolutions [currentRes].width, resolutions [currentRes].height, Screen.fullScreen);
			SaveSettings ();
			break;
		case 2:
			temp = (int)Mathf.Ceil (gameSettings.masterVolume * 10);
			if (temp > 0) {
				temp -= 1;
				gameSettings.masterVolume = ((float)temp) / 10;
				MasterVolLevel.text = ((int)Mathf.Ceil ((gameSettings.masterVolume * 10))).ToString ();
				MasterVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.masterVolume * 10))).ToString ();
				SaveSettings ();
			}
			break;
		case 3:
			temp = (int)Mathf.Ceil (gameSettings.musicVolume * 10);
			if (temp > 0) {
				temp -= 1;
				gameSettings.musicVolume = ((float)temp) / 10;
				MusicVolLevel.text = ((int)Mathf.Ceil ((gameSettings.musicVolume * 10))).ToString ();
				MusicVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.musicVolume * 10))).ToString ();
				SaveSettings ();
			}
			break;
		case 4:
			temp = (int)Mathf.Ceil (gameSettings.effectsVolume * 10);
			if (temp > 0) {
				temp -= 1;
				gameSettings.effectsVolume = ((float)temp) / 10;
				EffectsVolLevel.text = ((int)Mathf.Ceil ((gameSettings.effectsVolume * 10))).ToString ();
				EffectsVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.effectsVolume * 10))).ToString ();
				SaveSettings ();
			}
			break;
		}
	}

	public void HighlightOption () {

		ResetAllButtons ();

		switch (selectedIndex) {
		case 0:
			FullscreenLabel.color = FullscreenLevel.color = FullscrenRight.color = FullscreenLeft.color = white0;
			FullscreenArrow.color = FullscreenHighLabel.color = FullscreenHighLevel.color = FullscreenHighLeft.color = FullscreenHighRight.color = gray1;
			break;
		case 1:
			ResolutionLabel.color = ResolutionLevel.color = ResolutionRight.color = ResolutionLeft.color = white0;
			ResolutionArrow.color = ResolutionHighLabel.color = ResolutionHighLevel.color = ResolutionHighLeft.color = ResolutionHighRight.color = gray1;
			break;
		case 2:
			MasterVolLabel.color = MasterVolLevel.color = MasterVolRight.color = MasterVolLeft.color = white0;
			MasterVolArrow.color = MasterVolHighLabel.color = MasterVolHighLevel.color = MasterVolHighLeft.color = MasterVolHighRight.color = gray1;
			break;
		case 3:
			MusicVolLabel.color = MusicVolLevel.color = MusicVolRight.color = MusicVolLeft.color = white0;
			MusicVolArrow.color = MusicVolHighLabel.color = MusicVolHighLevel.color = MusicVolHighLeft.color = MusicVolHighRight.color = gray1;
			break;
		case 4:
			EffectsVolLabel.color = EffectsVolLevel.color = EffectsVolRight.color = EffectsVolLeft.color = white0;
			EffectsVolArrow.color = EffectsVolHighLabel.color = EffectsVolHighLevel.color = EffectsVolHighLeft.color = EffectsVolHighRight.color = gray1;
			break;
		case 5:
			BackLabel.color = white0;
			BackHighLabel.color = BackArrow.color = gray1;
			break;
		}
	}

	public void ResetAllButtons() {
		FullscreenArrow.color = FullscreenHighLabel.color = FullscreenHighLevel.color = FullscreenHighLeft.color = FullscreenHighRight.color = gray0;
		FullscreenLabel.color = FullscreenLevel.color = FullscrenRight.color = FullscreenLeft.color = white1;

		ResolutionLabel.color = ResolutionLevel.color = ResolutionRight.color = ResolutionLeft.color = white1;
		ResolutionArrow.color = ResolutionHighLabel.color = ResolutionHighLevel.color = ResolutionHighLeft.color = ResolutionHighRight.color = gray0;

		MasterVolArrow.color = MasterVolHighLabel.color = MasterVolHighLevel.color = MasterVolHighLeft.color = MasterVolHighRight.color = gray0;
		MasterVolLabel.color = MasterVolLevel.color = MasterVolRight.color = MasterVolLeft.color = white1;

		MusicVolArrow.color = MusicVolHighLabel.color = MusicVolHighLevel.color = MusicVolHighLeft.color = MusicVolHighRight.color = gray0;
		MusicVolLabel.color = MusicVolLevel.color = MusicVolRight.color = MusicVolLeft.color = white1;

		EffectsVolArrow.color = EffectsVolHighLabel.color = EffectsVolHighLevel.color = EffectsVolHighLeft.color = EffectsVolHighRight.color = gray0;
		EffectsVolLabel.color = EffectsVolLevel.color = EffectsVolRight.color = EffectsVolLeft.color = white1;

		BackLabel.color = white1;
		BackHighLabel.color = BackArrow.color = gray0;
	}

	public void OnBackButtonClicked() {
		SaveSettings ();
		SceneManager.LoadScene ("Menu");
	}

	public void ButtonClick () {
		var btn = EventSystem.current.currentSelectedGameObject.name;

		if (btn.Contains ("Fullscreen")) {
			selectedIndex = 0;
		} else if (btn.Contains ("Resolution")) {
			selectedIndex = 1;
		} else if (btn.Contains ("Master")) {
			selectedIndex = 2;
		} else if (btn.Contains ("Music")) {
			selectedIndex = 3;
		} else if (btn.Contains ("Effects")) {
			selectedIndex = 4;
		} else {
			selectedIndex = 5;
			SaveSettings ();
			toFadeImage = true;
		}

		if (btn.Contains ("Left")) {
			LeftArrow ();
		} else {
			RightArrow ();
		}
	}

	public void FullscreenHover () {
		selectedIndex = 0;
		HighlightOption ();
	}

	public void ResolutionHover () {
		selectedIndex = 1;
		HighlightOption ();
	}

	public void MasterHover () {
		selectedIndex = 2;
		HighlightOption ();
	}

	public void MusicHover () {
		selectedIndex = 3;
		HighlightOption ();
	}

	public void EffectsHover () {
		selectedIndex = 4;
		HighlightOption ();
	}

	public void BackHover () {
		selectedIndex = 5;
		HighlightOption ();
	}

	public void SaveSettings() {
		string jsonData = JsonUtility.ToJson (gameSettings, true);
		File.WriteAllText (Application.persistentDataPath + "/gamesettings.json", jsonData);
	}

	public void LoadSettings() {
		gameSettings = JsonUtility.FromJson<GameSettings> (File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
	}

	public GameSettings GetSettings() {
		LoadSettings ();

		return gameSettings;
	}
}
