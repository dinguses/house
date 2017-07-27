using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour {

	public Slider musicVolumeSlider;
	public Slider masterVolumeSlider;
	public Button applyButton;
	public Button backButton;

	public AudioSource musicSource;
	public GameSettings gameSettings;

	void OnEnable() {
		gameSettings = new GameSettings ();

		musicVolumeSlider.onValueChanged.AddListener (delegate {OnMusicVolume(); });
		masterVolumeSlider.onValueChanged.AddListener (delegate {OnMasterVolume(); });

		applyButton.onClick.AddListener (delegate {OnApplyButtonClicked(); });
		backButton.onClick.AddListener (delegate {OnBackButtonClicked(); });

		LoadSettings ();
	}

	public void OnMusicVolume() {
		musicSource.volume = gameSettings.musicVolume = musicVolumeSlider.value;
	}

	public void OnMasterVolume() {
		musicSource.volume = gameSettings.masterVolume = masterVolumeSlider.value;
	}

	public void OnApplyButtonClicked() {
		SaveSettings ();
	}

	public void OnBackButtonClicked() {
		SceneManager.LoadScene ("Menu");
	}

	public void SaveSettings() {
		string jsonData = JsonUtility.ToJson (gameSettings, true);
		File.WriteAllText (Application.persistentDataPath + "/gamesettings.json", jsonData);
	}

	public void LoadSettings() {
		gameSettings = JsonUtility.FromJson<GameSettings> (File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

		musicVolumeSlider.value = gameSettings.musicVolume;
		masterVolumeSlider.value = gameSettings.masterVolume;
	}

	public GameSettings GetSettings() {
		LoadSettings ();

		return gameSettings;
	}
}
