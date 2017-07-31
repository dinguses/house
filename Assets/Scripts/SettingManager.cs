using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

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

		BackLabel = GameObject.Find ("BackBtnText").GetComponent<Text> ();
		BackHighLabel = GameObject.Find ("BackHigh").GetComponent<Text> ();
		BackArrow = GameObject.Find ("BackArrow").GetComponent<Text> ();

		gameSettings = new GameSettings ();

		musicSource = GameObject.FindGameObjectWithTag ("musictrack").GetComponent<AudioSource>();

		LoopingAudio.loop = true;
		LoopingAudio.volume = 0.0f;
		LoopingAudio.clip = Resources.Load("tapeloop") as AudioClip;
		LoopingAudio.Play ();

		white0 = new Color32 (255, 255, 255, 0);
		white1 = new Color32 (255, 255, 255, 255);
		gray0 = new Color32 (148, 148, 148, 0);
		gray1 = new Color32 (148, 148, 148, 255);

		HighlightOption ();

		LoadSettings ();

		MasterVolLevel.text = ((int)Mathf.Ceil ((gameSettings.masterVolume * 10))).ToString ();
		MasterVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.masterVolume * 10))).ToString ();
		MusicVolLevel.text = ((int)Mathf.Ceil ((gameSettings.musicVolume * 10))).ToString ();
		MusicVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.musicVolume * 10))).ToString ();
		EffectsVolLevel.text = ((int)Mathf.Ceil ((gameSettings.effectsVolume * 10))).ToString ();
		EffectsVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.effectsVolume * 10))).ToString ();
	}

	void Update() {

		if (selectedIndex == 3) {
			LoopingAudio.volume = gameSettings.masterVolume * gameSettings.effectsVolume;
			musicSource.volume = .2f;
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

		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if (selectedIndex != 4) {
				selectedIndex++;
			}
			HighlightOption ();
		}

		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			RightArrow ();
		}

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			LeftArrow ();
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (selectedIndex != 0) {
				selectedIndex--;
			}
			HighlightOption ();
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			toFadeImage = true;
		}

		if (Input.GetKeyDown (KeyCode.Return)) {

			if (selectedIndex == 4) {
				SaveSettings ();
				toFadeImage = true;
			}
		}
	}

	public void RightArrow() {

		int temp = 0;

		switch (selectedIndex) {
		case 0:
			break;
		case 1:
			temp = (int)Mathf.Ceil (gameSettings.masterVolume * 10);
			if (temp < 10) {
				temp += 1;
				gameSettings.masterVolume = ((float)temp) / 10;
				MasterVolLevel.text = ((int)Mathf.Ceil ((gameSettings.masterVolume * 10))).ToString ();
				MasterVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.masterVolume * 10))).ToString ();
				SaveSettings ();
			}
			break;
		case 2:
			temp = (int)Mathf.Ceil (gameSettings.musicVolume * 10);
			if (temp < 10) {
				temp += 1;
				gameSettings.musicVolume = ((float)temp) / 10;
				MusicVolLevel.text = ((int)Mathf.Ceil ((gameSettings.musicVolume * 10))).ToString ();
				MusicVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.musicVolume * 10))).ToString ();
				SaveSettings ();
			}
			break;
		case 3:
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
			break;
		case 1:
			temp = (int)Mathf.Ceil (gameSettings.masterVolume * 10);
			if (temp > 0) {
				temp -= 1;
				gameSettings.masterVolume = ((float)temp) / 10;
				MasterVolLevel.text = ((int)Mathf.Ceil ((gameSettings.masterVolume * 10))).ToString ();
				MasterVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.masterVolume * 10))).ToString ();
				SaveSettings ();
			}
			break;
		case 2:
			temp = (int)Mathf.Ceil (gameSettings.musicVolume * 10);
			if (temp > 0) {
				temp -= 1;
				gameSettings.musicVolume = ((float)temp) / 10;
				MusicVolLevel.text = ((int)Mathf.Ceil ((gameSettings.musicVolume * 10))).ToString ();
				MusicVolHighLevel.text = ((int)Mathf.Ceil ((gameSettings.musicVolume * 10))).ToString ();
				SaveSettings ();
			}
			break;
		case 3:
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
			MasterVolLabel.color = MasterVolLevel.color = MasterVolRight.color = MasterVolLeft.color = white0;
			MasterVolArrow.color = MasterVolHighLabel.color = MasterVolHighLevel.color = MasterVolHighLeft.color = MasterVolHighRight.color = gray1;
			break;
		case 2:
			MusicVolLabel.color = MusicVolLevel.color = MusicVolRight.color = MusicVolLeft.color = white0;
			MusicVolArrow.color = MusicVolHighLabel.color = MusicVolHighLevel.color = MusicVolHighLeft.color = MusicVolHighRight.color = gray1;
			break;
		case 3:
			EffectsVolLabel.color = EffectsVolLevel.color = EffectsVolRight.color = EffectsVolLeft.color = white0;
			EffectsVolArrow.color = EffectsVolHighLabel.color = EffectsVolHighLevel.color = EffectsVolHighLeft.color = EffectsVolHighRight.color = gray1;
			break;
		case 4:
			BackLabel.color = white0;
			BackHighLabel.color = BackArrow.color = gray1;
			break;
		}
	}

	public void ResetAllButtons() {
		FullscreenArrow.color = FullscreenHighLabel.color = FullscreenHighLevel.color = FullscreenHighLeft.color = FullscreenHighRight.color = gray0;
		FullscreenLabel.color = FullscreenLevel.color = FullscrenRight.color = FullscreenLeft.color = white1;

		MasterVolArrow.color = MasterVolHighLabel.color = MasterVolHighLevel.color = MasterVolHighLeft.color = MasterVolHighRight.color = gray0;
		MasterVolLabel.color = MasterVolLevel.color = MasterVolRight.color = MasterVolLeft.color = white1;

		MusicVolArrow.color = MusicVolHighLabel.color = MusicVolHighLevel.color = MusicVolHighLeft.color = MusicVolHighRight.color = gray0;
		MusicVolLabel.color = MusicVolLevel.color = MusicVolRight.color = MusicVolLeft.color = white1;

		EffectsVolArrow.color = EffectsVolHighLabel.color = EffectsVolHighLevel.color = EffectsVolHighLeft.color = EffectsVolHighRight.color = gray0;
		EffectsVolLabel.color = EffectsVolLevel.color = EffectsVolRight.color = EffectsVolLeft.color = white1;

		BackLabel.color = white1;
		BackHighLabel.color = BackArrow.color = gray0;
	}

	public void OnMusicVolume() {
		//musicSource.volume = gameSettings.musicVolume = musicVolumeSlider.value / 10;
	}

	public void OnMasterVolume() {
		//musicSource.volume = gameSettings.masterVolume = masterVolumeSlider.value / 10;
	}

	public void OnBackButtonClicked() {
		SaveSettings ();
		SceneManager.LoadScene ("Menu");
	}

	public void SaveSettings() {
		string jsonData = JsonUtility.ToJson (gameSettings, true);
		File.WriteAllText (Application.persistentDataPath + "/gamesettings.json", jsonData);
	}

	public void LoadSettings() {
		gameSettings = JsonUtility.FromJson<GameSettings> (File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

		//musicVolumeSlider.value = gameSettings.musicVolume * 10;
		//masterVolumeSlider.value = gameSettings.masterVolume * 10;
	}

	public GameSettings GetSettings() {
		LoadSettings ();

		return gameSettings;
	}
}
