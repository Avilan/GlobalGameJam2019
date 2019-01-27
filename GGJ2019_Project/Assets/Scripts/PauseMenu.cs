using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

	public static PauseMenu Instance { get; private set; }

	public bool IsOpen => gameObject.activeSelf;

	[SerializeField] float mouseSensitivityMin;
	[SerializeField] float mouseSensitivityMax;
	[SerializeField] Slider mouseSensitivitySlider;
	[SerializeField] Text mouseSensitivityText;
	[SerializeField] Text keybindsText;

	List<IPauseObserver> observers;

    void Awake () {
        Cursor.visible = true;

        Instance = this;
	    observers = new List<IPauseObserver>();
    }

    void Start () {
        Cursor.visible = true;

        PlayerController player = PlayerController.Instance;
		float playerMouseSensitivity = player.mouseSensitivity;
		mouseSensitivityText.text = $"{playerMouseSensitivity:F1}";
		mouseSensitivitySlider.minValue = mouseSensitivityMin;
		mouseSensitivitySlider.maxValue = mouseSensitivityMax;
		mouseSensitivitySlider.value = playerMouseSensitivity;
		keybindsText.text = player.Keybinds.ToString();
    }

    public void AddObserver (IPauseObserver observer) {
		observers.Add(observer);
    }

    public void Open () {
        Cursor.visible = true;

        gameObject.SetActive(true);
		Time.timeScale = 0f;
		MessageObserversAboutNewState(true);
    }

    public void Close ()
    {
        Cursor.visible = false;

		gameObject.SetActive(false);
		Time.timeScale = 1f;
		MessageObserversAboutNewState(false);
    }

    public void QuitGame () {
		Application.Quit();
	}

    public void MouseSensitivitySliderChanged (float newValue) {
		PlayerPrefs.SetFloat("mouseSensitivity", newValue);
		mouseSensitivityText.text = $"{newValue:F1}";
    }

    void MessageObserversAboutNewState (bool newState) {
		foreach(IPauseObserver observer in observers) {
			observer.OnPauseStateChanged(newState);
		}
    }

}

