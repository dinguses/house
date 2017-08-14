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

	public AudioSource SoundFX;
	public AudioSource AmbientFX;
	public AudioSource LoopingSoundFX;
	public AudioSource MusicTrack;
	public AudioSource KnockingTrack;
	public AudioSource ActionTrack;
	public AudioSource LossTrack;

	// Use this for initialization
	void Start () {
		gameSettings = JsonUtility.FromJson<GameSettings> (File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

		white0 = new Color32 (255, 255, 255, 0);
		white1 = new Color32 (255, 255, 255, 255);
		gray0 = new Color32 (148, 148, 148, 0);
		gray1 = new Color32 (148, 148, 148, 255);

		SoundFX = GameObject.Find("SoundFX").GetComponent<AudioSource>();
		AmbientFX = GameObject.Find("AmbientFX").GetComponent<AudioSource>();
		LoopingSoundFX = GameObject.Find("LoopingSoundFX").GetComponent<AudioSource>();
		MusicTrack = GameObject.Find("MusicTrack").GetComponent<AudioSource>();
		KnockingTrack = GameObject.Find("KnockingTrack").GetComponent<AudioSource>();
		ActionTrack = GameObject.Find("ActionTrack").GetComponent<AudioSource>();
		LossTrack = GameObject.Find("LossTrack").GetComponent<AudioSource>();

		HighlightOption ();
	}

	void OnEnable() {
		selectedIndex = 0;
		ResetAllButtons ();
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
				int oldSelIndex = selectedIndex;
				selectedIndex = 0;
				ResetAllButtons ();
				HighlightOption ();
				switch (oldSelIndex) {
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
			HighlightOption ();
		}

		if (Input.GetKeyDown (KeyCode.Return)) {
			if (selectedIndex == 0) {
				if (!canvas.gameObject.activeInHierarchy) {
					canvas.gameObject.SetActive (true);
				} else {
					gameSettings = JsonUtility.FromJson<GameSettings> (File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
					MusicTrack.volume = ActionTrack.volume = LossTrack.volume = gameSettings.masterVolume * gameSettings.musicVolume;
					KnockingTrack.volume = LoopingSoundFX.volume = AmbientFX.volume = SoundFX.volume = gameSettings.masterVolume * gameSettings.effectsVolume;
					pauseTrack.Pause ();
					SoundFX.UnPause ();
					AmbientFX.UnPause ();
					LoopingSoundFX.UnPause ();
					MusicTrack.UnPause ();
					KnockingTrack.UnPause ();
					ActionTrack.UnPause ();
					LossTrack.UnPause ();
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
