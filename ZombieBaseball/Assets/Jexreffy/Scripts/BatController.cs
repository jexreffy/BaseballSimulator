using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class BatController : MonoBehaviour {

    public GameController gameController;
    public GameObject bat;
    public GameObject otherBat;
    public Canvas instructionCanvas;
    public Image radialProgress;

    private float _triggerHoldTime = -10f;
    
    private VRTK_ControllerEvents _events;

    private const float TRIGGER_HOLD_TIME = 2;
    
    void Awake() {
        _events = GetComponent<VRTK_ControllerEvents>();
    }

    void Update() {
        if (gameController.HasStarted || _triggerHoldTime <= 0) return;
        
        if (Time.time >= _triggerHoldTime) {
            gameController.OnStartGame();
            instructionCanvas.gameObject.SetActive(false);
            _triggerHoldTime = -10f;
            radialProgress.gameObject.SetActive(false);
            radialProgress.fillAmount = 0f;
        } else {
            radialProgress.fillAmount = 1 - (_triggerHoldTime - Time.time) / TRIGGER_HOLD_TIME;  
        }
    }
    
    void OnEnable() {
        _events.GripPressed += OnGripPressed;
        _events.TriggerPressed += OnTriggerPressed;
        _events.TriggerReleased += OnTriggerReleased;
    }
    
    void OnDisable() {
        _events.GripPressed -= OnGripPressed;
        _events.TriggerPressed -= OnTriggerPressed;
        _events.TriggerReleased -= OnTriggerReleased;
    }
    
    private void OnGripPressed(object sender, ControllerInteractionEventArgs e) {
        if (bat.activeSelf || gameController.HasStarted) return;
        
        bat.SetActive(!bat.activeSelf);
        otherBat.SetActive(!otherBat.activeSelf);
    }

    private void OnTriggerPressed(object sender, ControllerInteractionEventArgs e) {
        if (!bat.activeSelf || gameController.HasStarted) return;

        _triggerHoldTime = Time.time + TRIGGER_HOLD_TIME;
        radialProgress.gameObject.SetActive(true);
        radialProgress.fillAmount = 0f;
    }
    
    private void OnTriggerReleased(object sender, ControllerInteractionEventArgs e) {
        if (!bat.activeSelf || gameController.HasStarted) return;

        _triggerHoldTime = -10f;
        radialProgress.gameObject.SetActive(false);
        radialProgress.fillAmount = 0f;
    }
}
