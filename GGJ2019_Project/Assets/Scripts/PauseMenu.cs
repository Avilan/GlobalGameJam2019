using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

	public static PauseMenu Instance { get; private set; }

	public bool IsOpen => gameObject.activeSelf;

	[SerializeField] float mouseSensitivityMin;
	[SerializeField] float mouseSensitivityMax;
	[SerializeField] Slider mouseSensitivitySlider;
	[SerializeField] Text mouseSensitivityText;

	List<IPauseObserver> observers;

    void Awake () {
	    Instance = this;
	    observers = new List<IPauseObserver>();
    }

    void Start () {
		float playerMouseSensitivity = PlayerController.Instance.MouseSensitivity;
		mouseSensitivityText.text = $"{playerMouseSensitivity:F1}";
		mouseSensitivitySlider.minValue = mouseSensitivityMin;
		mouseSensitivitySlider.maxValue = mouseSensitivityMax;
		mouseSensitivitySlider.value = playerMouseSensitivity;
    }

    public void AddObserver (IPauseObserver observer) {
		observers.Add(observer);
    }

    public void Open () {
		gameObject.SetActive(true);
		Time.timeScale = 0f;
		MessageObserversAboutNewState(true);
    }

    public void Close () {
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

