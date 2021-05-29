using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    static private ScoreManager _scoreManager;

    static public int ScoreFromPrevRound = 0;
    static public int HighScore = 0;

    
    private int _chain = 0;
    private int _scoreRun = 0;
    private int _score = 0;

    static public int Chain { get { return _scoreManager._chain; } }
    static public int Score { get { return _scoreManager._score; } }
    static public int ScoreRun { get { return _scoreManager._scoreRun; } }

    private void Awake()
    {
        if (_scoreManager == null)
        {
            _scoreManager = this;
        }
        else
        {
            Debug.LogError("Error: ScoreManager.Awake(): _scoreManager is already set!");
        }

        if (PlayerPrefs.HasKey("ProspectorHighScore"))
        {
            HighScore = PlayerPrefs.GetInt("ProspectorHighScore");
        }

        _score += ScoreFromPrevRound;
        ScoreFromPrevRound = 0;
    }

    static public void ScoreEvent(eScoreEvent evt)
    {
        try
        {
            _scoreManager.Event(evt);
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogError("ScoreManager: ScoreEvent() called while _scoreManager = null\n" + e);
        }
    }

    private void Event(eScoreEvent cardState)
    {
        switch (cardState)
        {
            case eScoreEvent.draw:
            case eScoreEvent.gameWin:
            case eScoreEvent.gameLose:
                _chain = 0;
                _score += _scoreRun;
                _scoreRun = 0;
                break;

            case eScoreEvent.mine:
                _chain++;
                _scoreRun += _chain;
                break;
        }

        switch (cardState)
        {
            case eScoreEvent.gameWin:          
                ScoreFromPrevRound = _score;
                //print("You won this round! Round score: " + ScoreFromPrevRound);
                break;

            case eScoreEvent.gameLose:
                if (HighScore <= _score)
                {
                    //print("New higt score: " + _score);
                    HighScore = _score;
                    PlayerPrefs.SetInt("ProspectorHighScore", _score);
                }
                //else
                //{
                //    print("Final score: " + _score);
                //}
                break;

            default:
                //print($"score: {_score} scoreRun: {_scoreRun} chain: {_chain}");
                break;
        }
    }
}
