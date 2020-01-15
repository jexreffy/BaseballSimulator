using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject whiteBallPrefab;
    public GameObject goldBallPrefab;
    public GameObject redBallPrefab;

    public Vector3 pitchPoint;

    public float whiteChance = 0.6f;
    public float goldChance = 0.2f;

    public int whiteMultiplier = 1;
    public int goldMultiplier = 2;
    public int redMultiplier = -1;

    private bool _hasStarted;
    private float _pitchTime = -10f;
    
    private List<Ball> _whiteBalls = new List<Ball>();
    private List<Ball> _goldBalls = new List<Ball>();
    private List<Ball> _redBalls = new List<Ball>();
    
    private Queue<Ball> _ballsToPitch = new Queue<Ball>();

    private int NUM_BALLS = 30;
    private float PITCH_DELAY = 5f;

    void Awake() {
        for (var i = 0; i < NUM_BALLS; i++) {
            var newBall = Instantiate(whiteBallPrefab, Vector3.zero, Quaternion.identity);
            newBall.SetActive(false);
            _whiteBalls.Add(newBall.GetComponent<Ball>());
            _whiteBalls[_whiteBalls.Count - 1].InitializeBall(this, whiteMultiplier);
            
            newBall = Instantiate(goldBallPrefab, Vector3.zero, Quaternion.identity);
            newBall.SetActive(false);
            _goldBalls.Add(newBall.GetComponent<Ball>());
            _goldBalls[_goldBalls.Count - 1].InitializeBall(this, goldMultiplier);
            
            newBall = Instantiate(redBallPrefab, Vector3.zero, Quaternion.identity);
            newBall.SetActive(false);
            _redBalls.Add(newBall.GetComponent<Ball>());
            _redBalls[_redBalls.Count - 1].InitializeBall(this, redMultiplier);
        }
    }

    void Start() {
        OnStartGame();
    }

    void Update() {
        if (!_hasStarted) return;

        if (Time.time < _pitchTime) return;
        
        var ball = _ballsToPitch.Dequeue();
        ball.ResetBall(transform.position + pitchPoint, new Vector3(0, 0.45f, -1.5f));

        _hasStarted = _ballsToPitch.Count > 0;
        _pitchTime = Time.time + PITCH_DELAY;
    }

    private void OnStartGame() {
        _hasStarted = true;
        _pitchTime = Time.time + PITCH_DELAY;
        
        _ballsToPitch.Clear();

        var whiteCount = 0;
        var goldCount = 0;
        var redCount = 0;

        var rand = 0f;
        for (var i = 0; i < NUM_BALLS; i++) {
            rand = Random.value;

            if (rand < whiteChance) {
                _ballsToPitch.Enqueue(_whiteBalls[whiteCount++]);
            } else if (rand < whiteChance + goldChance) {
                _ballsToPitch.Enqueue(_goldBalls[goldCount++]);
            } else {
                _ballsToPitch.Enqueue(_redBalls[redCount++]);
            }
        }
    }

    public void OnBallFinished(float distance, int multiplier, bool fair, bool homeRun) {
        Debug.Log($"{Time.time} {multiplier} Ball went {distance} units {(homeRun ? "and was a home run " : fair ? "and was fair " : "and was foul")}");
    }
}
