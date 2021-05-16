using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] Text _min;
    [SerializeField] Text _max;
    [SerializeField] Slider _scoreSlider;

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreUpdate += ChangeScore;
        }

        ChangeScore(0);
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreUpdate -= ChangeScore;
        }
    }

    void ChangeScore(float score)
    {
        int minVal = (int)(score / 1000000);
        int maxVal = (int)(minVal + 1);
        float sliderValue = (score % 1000001)/1000000;

        _min.text = minVal.ToString();
        _max.text = maxVal.ToString();
        _scoreSlider.value = sliderValue;
    }
}
