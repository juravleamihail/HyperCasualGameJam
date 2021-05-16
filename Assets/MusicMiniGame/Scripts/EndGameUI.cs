using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    Dictionary<ScoreManager.HitType, List<string>> _encouragements;
    [SerializeField] GameObject _encouragementPrefab;
    List<Color> colors;
    [SerializeField] Text _scoreLabel;
    [SerializeField] Text _scoreTxt;
    // Start is called before the first frame update
    void Awake()
    {
        _encouragements = new Dictionary<ScoreManager.HitType, List<string>>();
        _encouragements.Add(ScoreManager.HitType.GoodHit,new List<string>() {"Good!" });
        _encouragements.Add(ScoreManager.HitType.GreatHit, new List<string>() { "Great!" });
        _encouragements.Add(ScoreManager.HitType.PerfectHit, new List<string>() { "Perfect!","Flawless!","Excellent!","Splendid!","Superb!","Out-of-this-world!" });
        _encouragements.Add(ScoreManager.HitType.None, new List<string>() { "Try harder!", "Maybe next time" });

        colors = new List<Color>() { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta };
    }

    public void EndGame(float score)
    {
        ScoreManager.HitType hitType =  ScoreManager.HitType.None;
        if (score<300000)
        {
            hitType = ScoreManager.HitType.None;
        }
        else if (score > 1000000)
        {
            hitType = ScoreManager.HitType.PerfectHit;
        }
        else if (score > 6000000)
        {
            hitType = ScoreManager.HitType.GreatHit;
        }
        else if(score>300000)
        {
            hitType = ScoreManager.HitType.GoodHit;
        }
       

        AddScore(hitType, score);
        StartCoroutine(ShowEncouragements(10, hitType));

    }

    void AddScore(ScoreManager.HitType type,float score)
    {
        _scoreLabel.text = _encouragements[type][Random.Range(0, _encouragements[type].Count)];
        _scoreTxt.text = score.ToString();
    }
    IEnumerator ShowEncouragements(int count, ScoreManager.HitType type)
    {
        float time = 3;
        float timePerShow = time / count;

        for(int i=0;i<count;i++)
        {
            GameObject go = Instantiate(_encouragementPrefab,transform);
            go.GetComponentInChildren<Text>().text = _encouragements[type][Random.Range(0, _encouragements[type].Count)];
            go.transform.localScale = Vector3.one;
            RectTransform rect = go.GetComponent<RectTransform>();
            Text txt = go.GetComponentInChildren<Text>();
            txt.color = colors[Random.Range(0, colors.Count)];
            Vector2 screenLimit = new Vector2(Random.Range(rect.sizeDelta.x / 2, Screen.width-rect.sizeDelta.x/2), Random.Range(rect.sizeDelta.y / 2, Screen.height - rect.sizeDelta.y / 2));
            go.transform.position = screenLimit;
            yield return new WaitForSeconds(timePerShow);
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }
}
