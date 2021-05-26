using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    public static Scoreboard S;

    [Header("Set in Inspector")]
    public GameObject PrefabFloatingScore;

    [Header("Set Dynamically")]
    [SerializeField] private int _score = 0;
    [SerializeField] private string _scoreString;

    private Transform _canvasTrans;

    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            ScoreString = _score.ToString("N0");
        }
    }

    public string ScoreString
    {
        get
        {
            return _scoreString;
        }
        set
        {
            _scoreString = value;
            GetComponent<Text>().text = _scoreString;
        }
    }

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("ERROR: Scoreboard.Awake(): S is already set!");
        }
        _canvasTrans = transform.parent;
    }

    public void FSCallback(FloatingScore floatingScore)
    {
        Score += floatingScore.Score;
    }

    public FloatingScore CreateFloatingScore(int amt, List<Vector2> pts)
    {
        GameObject gmObject = Instantiate<GameObject>(PrefabFloatingScore);
        gmObject.transform.SetParent(_canvasTrans);
        FloatingScore floatingScore = gmObject.GetComponent<FloatingScore>();
        floatingScore.Score = amt;
        floatingScore.ReportFinishTo = this.gameObject;
        floatingScore.Init(pts);
        return floatingScore;
    }
}
