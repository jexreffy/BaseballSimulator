using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour {

    public GameObject whiteBallPrefab;
    public GameObject goldBallPrefab;
    public GameObject redBallPrefab;

    public Vector3 pitchPoint;

    public TextMeshPro BallCount;
    public TextMeshPro LastCount;
    public TextMeshPro HomeRunsCount;
    public TextMeshPro ScoreCount;
    public List<TextMeshPro> HighScores = new List<TextMeshPro>();
    
    public List<ParticleSystem> confettiShooters = new List<ParticleSystem>();

    public float whiteChance = 0.6f;
    public float goldChance = 0.2f;

    public int whiteMultiplier = 1;
    public int goldMultiplier = 2;
    public int redMultiplier = -1;

    public int homeRunScore = 50;
    
    public AudioClip homeRunSound;
    public AudioClip homeBadSound;
    public AudioClip highScoreSound;

    private bool _hasStarted;
    private bool _hasFinished;
    private float _pitchTime = -10f;
    private int _ballsCounted;
    private int _currentScore;
    private int _currentHomeRuns;

    private SaveState _saveState;
    
    private List<Ball> _whiteBalls = new List<Ball>();
    private List<Ball> _goldBalls = new List<Ball>();
    private List<Ball> _redBalls = new List<Ball>();
    
    private Queue<Ball> _ballsToPitch = new Queue<Ball>();

    private AudioSource _audio;

    private int NUM_BALLS = 30;
    private float PITCH_DELAY = 5f;

    void Awake() {
        _audio = GetComponent<AudioSource>();
        
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
        
        BallCount.SetText("0");
        LastCount.SetText("0");
        HomeRunsCount.SetText("0");
        ScoreCount.SetText("0");
        
        _saveState = SaveState.Load();
        
        for (var i = 0; i < HighScores.Count; i++) {
            if (i < _saveState.highScores.Count) HighScores[i].SetText(_saveState.highScores[i].ToString());
        }
    }

    void Update() {
        if (!_hasStarted) return;

        if (Time.time < _pitchTime) return;
        
        var ball = _ballsToPitch.Dequeue();
        ball.ResetBall(transform.position + pitchPoint, new Vector3(0, 0.45f, -1.5f));

        BallCount.SetText(_ballsToPitch.Count.ToString());
        
        _hasStarted = _ballsToPitch.Count > 0;
        _pitchTime = Time.time + PITCH_DELAY;
    }

    public bool HasStarted => _hasStarted;
    public bool HasFinished => _hasFinished;

    public void OnStartGame() {
        _hasStarted = true;
        _hasFinished = false;
        _currentScore = 0;
        _currentHomeRuns = 0;
        _ballsCounted = 0;
        _pitchTime = Time.time + PITCH_DELAY;
        
        _ballsToPitch.Clear();
        
        for (var i = 0; i < NUM_BALLS; i++) {
            _whiteBalls[i].gameObject.SetActive(false);
            _goldBalls[i].gameObject.SetActive(false);
            _redBalls[i].gameObject.SetActive(false);
        }

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
        
        BallCount.SetText(NUM_BALLS.ToString());
        LastCount.SetText("0");
        HomeRunsCount.SetText("0");
        ScoreCount.SetText("0");
    }

    public void OnHomeRunTriggerd(int multiplier) {
        _currentScore += homeRunScore;
        HomeRunsCount.SetText(_currentHomeRuns.ToString());
        ScoreCount.SetText(_currentScore.ToString());
        
        _audio.PlayOneShot(multiplier > 0 ? homeRunSound : homeBadSound);
        
        for (var i = 0; i < confettiShooters.Count; i++) {
            confettiShooters[i].Clear();
            confettiShooters[i].Play();
        }
    }

    public void OnBallFinished(float distance, int multiplier, bool fair, bool homeRun) {
        Debug.Log($"{Time.time} {multiplier} Ball went {distance} units {(homeRun ? "and was a home run " : fair ? "and was fair " : "and was foul")}");
        _ballsCounted++;
        var newScore = 0;
        if (fair) {
            newScore = Mathf.FloorToInt(distance * multiplier);
            _currentScore = Mathf.Max(_currentScore + newScore, 0);
            if (homeRun) _currentHomeRuns++;
            LastCount.SetText(newScore.ToString());
            HomeRunsCount.SetText(_currentHomeRuns.ToString());
            ScoreCount.SetText(_currentScore.ToString());
        } else {
            LastCount.SetText(newScore.ToString());
        }

        if (_ballsToPitch.Count <= 0 && _ballsCounted >= NUM_BALLS) ResolveHighScores();
    }

    private void ResolveHighScores() {
        _hasStarted = false;
        _hasFinished = true;

        var found = false;
        for (var i = 0; i < _saveState.highScores.Count; i++) {
            if (_saveState.highScores[i] >= _currentScore) continue;
            found = true;
            _saveState.highScores.Insert(i, _currentScore);

            if (i < HighScores.Count) {
                _audio.PlayOneShot(highScoreSound);
        
                for (var j = 0; j < confettiShooters.Count; j++) {
                    confettiShooters[j].Clear();
                    confettiShooters[j].Play();
                }
            }
            break;
        }

        if (!found) {
            _saveState.highScores.Add(_currentScore);
            
            if (_saveState.highScores.Count - 1 < HighScores.Count) {
                _audio.PlayOneShot(highScoreSound);
        
                for (var j = 0; j < confettiShooters.Count; j++) {
                    confettiShooters[j].Clear();
                    confettiShooters[j].Play();
                }
            }
        }

        for (var i = 0; i < HighScores.Count; i++) {
            if (i < _saveState.highScores.Count) HighScores[i].SetText(_saveState.highScores[i].ToString());
        }
       
        _saveState.Save();
    }
}
