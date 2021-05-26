using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingScore : MonoBehaviour
{
    [Header("Set Dynamically")]
    public eFSState State = eFSState.idle;

    [SerializeField] protected int _score = 0;
    public string ScoreString;

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
            GetComponent<Text>().text = ScoreString;
        }
    }

    public List<Vector2> BezierPts;
    public List<float> FontSizes;

    public float TimeStart = -1f;
    public float TimeDuration = 1f;
    public string EasingCurve = Easing.InOut;

    public GameObject ReportFinishTo = null;

    private RectTransform RectTransform;
    private Text Txt;

    public void Init(List<Vector2> ePts, float eTimeS = 0, float eTimeD = 1)
    {
        RectTransform = GetComponent<RectTransform>();
        RectTransform.anchoredPosition = Vector2.zero;

        Txt = GetComponent<Text>();

        BezierPts = new List<Vector2>(ePts);

        if (ePts.Count.Equals(1))
        {
            transform.position = ePts[0];
            return;
        }

        if (eTimeS == 0)
        {
            eTimeS = Time.time;
        }

        TimeStart = eTimeS;
        TimeDuration = eTimeD;

        State = eFSState.pre;
    }

    public void FSCallback(FloatingScore floatingScore)
    {
        Score += floatingScore.Score;
    }

    private void Update()
    {
        if (State.Equals(eFSState.idle))
        {
            return;
        }

        float u = (Time.time - TimeStart) / TimeDuration;

        float uC = Easing.Ease(u, EasingCurve);

        if (u < 0)
        {
            State = eFSState.pre;
            Txt.enabled = false;
        }
        else
        {
            if (u >= 1)
            {
                uC = 1;
                State = eFSState.post;

                if (ReportFinishTo != null)
                {
                    ReportFinishTo.SendMessage("FSCallback", this);
                    Destroy(gameObject);
                }
                else
                {
                    State = eFSState.idle;
                }
            }
            else
            {
                State = eFSState.active;
                Txt.enabled = true;
            }

            Vector2 pos = Utils.Bezier(uC, BezierPts);

            RectTransform.anchorMin = RectTransform.anchorMax = pos;

            if (FontSizes != null && FontSizes.Count > 0)
            {
                int size = Mathf.RoundToInt(Utils.Bezier(uC, FontSizes));
                GetComponent<Text>().fontSize = size;
            }
        }
    }
}
