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
    [SerializeField] private Transform danceStepRequirementsParent;
    [SerializeField] private DanceStepScriptableObject[] danceStepRequirements;
    [SerializeField] private Transform currentDanceStepsAcquiredParent;

    [Header("Music")]
    [SerializeField] private AudioClip[] audioClips;
    private DanceStepScriptableObject[] currentDanceStepsAcquired;

    [Header("Popup")]
    [SerializeField] private GameObject popupPrefab;

    [Header("Sounds")]
    [SerializeField] private AudioSource soundsManagerAudioSrc;
    [SerializeField] private AudioClip greatSound;
    [SerializeField] private AudioClip[] perfectSounds;
    public Transform popupEndPoint;
    private int currentStepIndex = 0;
    private int currentCombo = 0;

    protected override void Awake()
    {
        base.Awake();
        InitMusic();
        InitDanceStepsRequirement();
        InitCurrentDanceSteps();
        StartCoroutine(WaitToSpawnDanceStep());
    }

    private void InitMusic()
    {
        GetComponent<AudioSource>()?.PlayOneShot(audioClips[Random.Range(0, audioClips.Length - 1)]);
    }

    private void InitCurrentDanceSteps()
    {
        currentDanceStepsAcquired = danceStepRequirements;
        foreach (Transform item in danceStepRequirementsParent.transform)
        {
            var gameObject = Instantiate(item, currentDanceStepsAcquiredParent);
            gameObject.GetComponent<Image>().enabled = false;
        }
    }

    private void InitDanceStepsRequirement()
    {
        foreach (var item in danceStepRequirements)
        {
            var gameObject = Instantiate(danceStepPrefab, danceStepRequirementsParent);
            gameObject.GetComponent<Image>().sprite = item.icon;
            gameObject.GetComponent<DanceStep>().danceStepSORef = item;
            Destroy(gameObject.GetComponent<FollowTarget>());
        }
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

        var followTargetComp = danceStep.AddComponent<FollowTarget>();
        followTargetComp.SelectTarget(GameManager.Instance.endPoint);
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
            circlePoint.sprite = circlePointFilledSprite;
            StartCoroutine(WaitToChangeCirclePointToDefaultSprite());

            GameObject danceStepInsideTheCirlePoint = GetDanceStepInsideTheCirlePoint();

            if(danceStepInsideTheCirlePoint == null)
            {
                return;
            }

            if (danceStepInsideTheCirlePoint.GetComponent<DanceStep>().isChecked)
            {
                return;
            }

            if (danceStepInsideTheCirlePoint != null)
            {
                Dance(danceStepInsideTheCirlePoint);
                CheckDanceStepRequired(danceStepInsideTheCirlePoint.GetComponent<DanceStep>());
                danceStepInsideTheCirlePoint.GetComponent<DanceStep>().isChecked = true;
                return;
            }
        }
    }

    private void CheckDanceStepRequired(DanceStep danceStepInsideCirlePoint)
    {
        bool isPerfect = true;

        foreach (Transform item in currentDanceStepsAcquiredParent.transform)
        {
            DanceStep danceStep = item.GetComponent<DanceStep>();

            if (!danceStep.isActivated && danceStepInsideCirlePoint.danceStepSORef != danceStep.danceStepSORef)
            {
                isPerfect = false;
                currentCombo = 0;
            }

            if (!danceStep.isActivated && danceStepInsideCirlePoint.danceStepSORef == danceStep.danceStepSORef)
            {
                AccomplishStepDance(danceStep, danceStepInsideCirlePoint.gameObject);

                if(isPerfect)
                {
                    currentCombo++;
                    SpawnPopup("Perfect X" + currentCombo);

                    if(currentCombo >= perfectSounds.Length)
                    {
                        soundsManagerAudioSrc.PlayOneShot(perfectSounds[perfectSounds.Length - 1]);
                    }
                    else
                    {
                        soundsManagerAudioSrc.PlayOneShot(perfectSounds[currentCombo - 1]);
                    }
                }
                else
                {
                    soundsManagerAudioSrc.PlayOneShot(greatSound);
                    SpawnPopup("Great");
                }

                return;
            }
        }
    }

    void PlayComboSound(int combo)
    {
        switch (combo)
        {
            case 1:
                soundsManagerAudioSrc.PlayOneShot(perfectSounds[combo-1]);
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;

            default:
                break;
        }
    }
    
    void SpawnPopup(string message)
    {
        var popup = Instantiate(popupPrefab, canvas.transform);
        popup.GetComponent<Popup>().Init(message);
    }

    private void AccomplishStepDance(DanceStep danceStepAquired, GameObject danceStepInsideCircle)
    {
        danceStepAquired.isActivated = true;
        FollowAnimationToDanceStepAquired(danceStepAquired.transform, danceStepInsideCircle.transform);
    }

    private void FollowAnimationToDanceStepAquired(Transform target, Transform danceStepInsideCircle)
    {
        var followTargetComp = danceStepInsideCircle.gameObject.GetComponent<FollowTarget>();
        followTargetComp.SelectTarget(target, 200, true);
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

    private GameObject GetDanceStepInsideTheCirlePoint()
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

    public void PauseTap()
    {
        Time.timeScale = 0;
        GetComponent<AudioSource>().Pause();
    }

    public void UnpauseTap()
    {
        Time.timeScale = 1;
        GetComponent<AudioSource>().UnPause();
    }
}
