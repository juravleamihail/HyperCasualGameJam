using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager Instance { get; private set; }
    public enum HitType{ None=0, GoodHit=1,GreatHit=2,PerfectHit=3};

    [SerializeField] EndGameUI _endGamePrefab;

    private Dictionary<HitType, float> _scoreByHitType;

    private List<HitType> _hitHistory;

    private float _score;

    public float Score { get { return _score; } }



    public event Action<float> OnScoreUpdate = delegate { };
    private void Awake()
    {

        if(Instance==null)
        {
            Instance = this;
        }
        else if(Instance!=this)
        {
            Destroy(this);
        }

        _hitHistory = new List<HitType>();

        _scoreByHitType = new Dictionary<HitType, float>();
        _scoreByHitType.Add(HitType.GoodHit,10000);
        _scoreByHitType.Add(HitType.GreatHit, 20000);
        _scoreByHitType.Add(HitType.PerfectHit, 40000);

        
    }

     IEnumerator Start()
      {
          yield return new WaitForSeconds(1);
          //AddScore(HitType.GoodHit); AddScore(HitType.GoodHit); AddScore(HitType.GoodHit);
          //ddScore(HitType.GreatHit); AddScore(HitType.GreatHit); AddScore(HitType.GreatHit);
          AddScore(HitType.PerfectHit); 
          AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit);
          AddScore(HitType.PerfectHit);
          AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit);
          AddScore(HitType.PerfectHit);
          AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit);
          AddScore(HitType.PerfectHit);
          AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit);
          AddScore(HitType.PerfectHit);
          AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit);
          AddScore(HitType.PerfectHit);
          AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit); AddScore(HitType.PerfectHit);
        EngGame(); 
} 

    public void AddScore(HitType hitType)
    {
        _hitHistory.Add(hitType);
        int combo = GetHitChain(HitType.PerfectHit);

        _score += _scoreByHitType[hitType] + CalculateCombo(hitType, combo);

        OnScoreUpdate?.Invoke(_score);
    }

    int GetHitChain(HitType hitType)
    {
        int count = 0;
        for(int i=_hitHistory.Count-1;i>=0;i--)
        {
            if(_hitHistory[i] == hitType)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    float CalculateCombo(HitType hitType, int combo)
    {
        float percentage = 5 * Mathf.Clamp(combo-1,0,int.MaxValue);

        return (_scoreByHitType[hitType] * percentage) / 100;
    }

    public void EngGame()
    {
        Canvas canvas = FindObjectOfType<Canvas>();

        EndGameUI endGame = Instantiate<EndGameUI>(_endGamePrefab);
        RectTransform rect = endGame.GetComponent<RectTransform>();

        rect.transform.parent = canvas.transform;
        rect.localScale = Vector3.one;
        rect.anchorMax = Vector2.one;
        rect.anchorMin = Vector2.one;

        endGame.EndGame(_score);
    }

}
