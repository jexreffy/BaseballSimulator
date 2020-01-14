using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    private bool _isHit;
    private bool _isDetermined;
    private bool _isFair;

    private Transform _transform;
    private Rigidbody _rigidbody;

    private const string BAT_TAG  = "Bat";
    private const string FAIR_TAG = "Fair";
    private const string FOUL_TAG = "Foul";
    private const string HOME_TAG = "HomeRun";

    void Awake() {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision other) {
        if (_isHit && _isDetermined) return;

        if (!_isHit && other.gameObject.tag.Equals(BAT_TAG)) {
            _isHit = true;
        } else if (_isHit) {
            var homeRun = other.gameObject.tag.Equals(HOME_TAG);
            var fair    = homeRun || other.gameObject.tag.Equals(FAIR_TAG);
            var foul    = other.gameObject.tag.Equals(FOUL_TAG);
            
            _isDetermined = true;
            _isFair       = fair && !foul;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (_isDetermined || !_isHit) return;

        var homeRun = other.gameObject.tag.Equals(HOME_TAG);
        var fair    = homeRun || other.gameObject.tag.Equals(FAIR_TAG);
        var foul    = other.gameObject.tag.Equals(FOUL_TAG);
        
        _isDetermined = true;
        _isFair       = fair && !foul;
    }

    public void ResetBall(Vector3 position, Vector3 force) {
        gameObject.SetActive(true);
        _isHit = false;
        _isDetermined = false;
        _isFair = false;

        _transform.position = position;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }
}
