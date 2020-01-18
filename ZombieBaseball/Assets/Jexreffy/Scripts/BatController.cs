using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class BatController : MonoBehaviour {

    public GameController gameController;
    public GameObject bat;
    public GameObject otherBat;
    public Canvas instructionCanvas;
    
    private float _triggerHoldTime = -10f;
    
    private VRTK_ControllerEvents _events;

    private const float TRIGGER_HOLD_TIME = 2;
    
    void Awake() {
        _events = GetComponent<VRTK_ControllerEvents>();
    }

    void Update() {
        if (gameController.HasStarted) return;
        
        if (_triggerHoldTime > 0 && Time.time >= _triggerHoldTime) {
            gameController.OnStartGame();
            instructionCanvas.gameObject.SetActive(false);
            _triggerHoldTime = -10f;
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
    }
    
    private void OnTriggerReleased(object sender, ControllerInteractionEventArgs e) {
        if (!bat.activeSelf || gameController.HasStarted) return;

        _triggerHoldTime = -10f;
    }
}
