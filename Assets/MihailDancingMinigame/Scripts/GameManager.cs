using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SimpleSingletoneGeneric<GameManager>
{
    [SerializeField] public Transform spawnPoint;
    [SerializeField] public Transform endPoint;
    [SerializeField] private GameObject danceStepPrefab;
    [SerializeField] private Transform danceStepParent;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector2 spawnInterval;
    [SerializeField] private DanceStepScriptableObject[] danceSteps;
    [SerializeField] private Image circlePoint;
    [SerializeField] private Sprite circlePointEmptySprite;
    [SerializeField] private Sprite circlePointFilledSprite;
    [SerializeField] private Animator dancer;
    private int currentStepIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        SpawnDanceStep();
        StartCoroutine(WaitToSpawnDanceStep());
    }

    IEnumerator WaitToSpawnDanceStep()
    {
        yield return new WaitForSeconds(GetRandomSpawnTime());
        SpawnDanceStep();
        currentStepIndex++;
        StartCoroutine(WaitToSpawnDanceStep());
    }

    void SpawnDanceStep()
    {
        GameObject danceStep = Instantiate(danceStepPrefab, danceStepParent);
        InitDanceStep(ref danceStep);
    }

    void InitDanceStep(ref GameObject danceStep)
    {
        if(currentStepIndex >= danceSteps.Length)
        {
            currentStepIndex = 0;
        }

        danceStep.transform.position = spawnPoint.position;
        danceStep.GetComponent<Image>().sprite = danceSteps[currentStepIndex].icon;
        danceStep.GetComponent<DanceStep>().danceStepSORef = danceSteps[currentStepIndex];
    }

    private float GetRandomSpawnTime()
    {
        return Random.Range(spawnInterval.x, spawnInterval.y);
    }

    private void Update()
    {
        // Input for PC and mobile
        if(Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            Dance(GetDanceStepAroundTheCirlePoint());
            circlePoint.sprite = circlePointFilledSprite;
            StartCoroutine(WaitToChangeCirclePointToDefaultSprite());
        }
    }

    void Dance(GameObject gameObject)
    {
        DanceStepScriptableObject danceStepSORef = gameObject.GetComponent<DanceStep>().danceStepSORef;
        dancer.runtimeAnimatorController = danceStepSORef.animatorController;
    }

    private IEnumerator WaitToChangeCirclePointToDefaultSprite()
    {
        yield return new WaitForSeconds(0.1f);
        circlePoint.sprite = circlePointEmptySprite;
    }

    private GameObject GetDanceStepAroundTheCirlePoint()
    {
        foreach (Transform danceStep in danceStepParent)
        {
            if(RectOverlaps(danceStep.GetComponent<RectTransform>(), circlePoint.GetComponent<RectTransform>()))
            {
                return danceStep.gameObject;
            }
        }

        return null;
    }

    bool RectOverlaps(RectTransform rectTrans1, RectTransform rectTrans2)
    {
        Rect rect1 = new Rect(rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width, rectTrans1.rect.height);
        Rect rect2 = new Rect(rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width, rectTrans2.rect.height);

        return rect1.Overlaps(rect2);
    }
}
