using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public AudioClip thrownSound;
    public AudioClip hitSound;
    public AudioClip homeRunSound;
    
    private bool _isHit;
    private bool _isDetermined;
    private bool _isFair;
    private bool _isHomeRun;

    private int _multiplier;

    private AudioSource _audio;
    private GameController _gameController;
    private Transform _transform;
    private Rigidbody _rigidbody;

    private static int BAT_LAYER;
    private static int GROUND_LAYER;

    private const string BAT_TAG  = "Bat";
    private const string FAIR_TAG = "Fair";
    private const string FOUL_TAG = "Foul";
    private const string HOME_TAG = "HomeRun";
    private const string GROUND_TAG = "Ground";
    private const string GREEN_ZONE_TAG = "GreenZone";
    private const string YELLOW_ZONE_TAG = "YellowZone";
    private const string RED_ZONE_TAG = "RedZone";

    private const float GREEN_MULTIPLIER = 0.75f;
    private const float YELLOW_MULTIPLIER = 0.3f;
    private const float RED_MULTIPLIER = 0.1f;

    void Awake() {
        if (GROUND_LAYER <= 0) {
            GROUND_LAYER = LayerMask.NameToLayer(GROUND_TAG);
            BAT_LAYER    = LayerMask.NameToLayer(BAT_TAG);
        }
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        _audio = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision other) {
        if (!_isHit && other.gameObject.layer == BAT_LAYER) {
            _isHit = true;

            var multiplier = 0.01f;
            if (other.gameObject.tag.Equals(GREEN_ZONE_TAG)) {
                multiplier = GREEN_MULTIPLIER;
            } else if (other.gameObject.tag.Equals(YELLOW_ZONE_TAG)) {
                multiplier = YELLOW_MULTIPLIER;
            } else if (other.gameObject.tag.Equals(RED_ZONE_TAG)) {
                multiplier = RED_MULTIPLIER;
            }
            
            _rigidbody.AddForce(other.impulse * multiplier, ForceMode.Impulse);
            _audio.PlayOneShot(hitSound);
        } else if (_isHit && !_isDetermined) {
            DeterimeBall(other.gameObject.tag);
        }

        if (other.gameObject.layer != GROUND_LAYER) return;
        
        _gameController.OnBallFinished(_isHit ? _transform.position.magnitude : 0, _multiplier, _isFair, _isHomeRun);
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.detectCollisions = false;
        _rigidbody.useGravity = false;
    }

    void OnTriggerEnter(Collider other) {
        if (!_isHit || _isDetermined) return;
        
        DeterimeBall(other.gameObject.tag);
    }

    private void DeterimeBall(string otherTag) {
        var homeRun = otherTag.Equals(HOME_TAG);
        var fair    = otherTag.Equals(FAIR_TAG);
        var foul    = otherTag.Equals(FOUL_TAG);

        if (!homeRun && !fair && !foul) return;

        if (homeRun) {
            _audio.PlayOneShot(homeRunSound);
            _gameController.OnHomeRunTriggerd();
        }

        _isHomeRun    = homeRun;
        _isFair       = homeRun || fair;
        _isDetermined = true;
    }

    public void InitializeBall(GameController controller, int multiplier) {
        _gameController = controller;
        _multiplier = multiplier;
    }

    public void ResetBall(Vector3 position, Vector3 force) {
        gameObject.SetActive(true);
        _isHit = false;
        _isDetermined = false;
        _isFair = true;

        _transform.position = position;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.detectCollisions = true;
        _rigidbody.useGravity = true;
        
        _rigidbody.AddForce(force, ForceMode.Impulse);
        
        _audio.PlayOneShot(thrownSound);
    }
}
