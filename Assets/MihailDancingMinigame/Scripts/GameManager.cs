using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct Level
{
    public DanceStepScriptableObject[] danceSteps;
    public DanceStepScriptableObject[] danceStepRequirements;
    public int currentStepsAcquired;
    public int danceStepSpeed;
}

public class GameManager : SimpleSingletoneGeneric<GameManager>
{
    [SerializeField] public Transform spawnPoint;
    [SerializeField] public Transform endPoint;
    [SerializeField] private GameObject danceStepPrefab;
    [SerializeField] private Transform danceStepParent;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector2 spawnInterval;
    [SerializeField] private Image circlePoint;
    [SerializeField] private Sprite circlePointEmptySprite;
    [SerializeField] private Sprite circlePointFilledSprite;
    [SerializeField] private Animator dancer;
    [SerializeField] private Transform danceStepRequirementsParent;
    [SerializeField] private Transform currentDanceStepsAcquiredParent;

    [Header("Levels")]
    [SerializeField] private Level[] levels;
    [SerializeField] private GameObject reloadButton;
    [SerializeField] private GameObject congratsMessage;
    [SerializeField] private GameObject finishLevelButtonsParent;
    private int currentLevel = 0;
    private bool canSpawnDanceSteps = true;

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
        InitLevel();
    }

    private void InitLevel()
    {
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
        currentDanceStepsAcquired = levels[currentLevel].danceStepRequirements;
        foreach (Transform item in danceStepRequirementsParent.transform)
        {
            var gameObject = Instantiate(item, currentDanceStepsAcquiredParent);
            gameObject.GetComponent<Image>().enabled = false;
        }
    }

    private void InitDanceStepsRequirement()
    {
        foreach (var item in levels[currentLevel].danceStepRequirements)
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

        if(canSpawnDanceSteps)
        {
            StartCoroutine(WaitToSpawnDanceStep());
        }
    }

    void SpawnDanceStep()
    {
        if(!canSpawnDanceSteps)
        {
            return;
        }

        GameObject danceStep = Instantiate(danceStepPrefab, danceStepParent);
        InitDanceStep(ref danceStep);
    }

    void InitDanceStep(ref GameObject danceStep)
    {
        if(currentStepIndex >= levels[currentLevel].danceSteps.Length)
        {
            currentStepIndex = 0;
        }

        var followTargetComp = danceStep.AddComponent<FollowTarget>();
        followTargetComp.SelectTarget(GameManager.Instance.endPoint, levels[currentLevel].danceStepSpeed);
        danceStep.transform.position = spawnPoint.position;
        danceStep.GetComponent<Image>().sprite = levels[currentLevel].danceSteps[currentStepIndex].icon;
        danceStep.GetComponent<DanceStep>().danceStepSORef = levels[currentLevel].danceSteps[currentStepIndex];
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
                    ScoreManager.Instance.AddScore(ScoreManager.HitType.PerfectHit);
                }
                else
                {
                    soundsManagerAudioSrc.PlayOneShot(greatSound);
                    SpawnPopup("Great");
                    ScoreManager.Instance.AddScore(ScoreManager.HitType.GreatHit);
                }

                levels[currentLevel].currentStepsAcquired++;

                if (LevelIsFinished())
                {
                    currentLevel++;

                    if (IsGameFinished())
                    {
                        reloadButton.SetActive(true);
                    }

                    canSpawnDanceSteps = false;
                    StartCoroutine(WaitToShowLevelDone());
                }

                return;
            }
        }
    }

    private void CleanLevel()
    {
        DestroyAllChildsFromParent(danceStepRequirementsParent);
        DestroyAllChildsFromParent(danceStepParent);
        DestroyAllChildsFromParent(currentDanceStepsAcquiredParent);
    }

    private void DestroyAllChildsFromParent(Transform parent)
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
    }

    private IEnumerator WaitToShowLevelDone()
    {
        yield return new WaitForSeconds(2);
        ShowLevelDone();
        CleanLevel();
    }

    private void ShowLevelDone()
    {
        congratsMessage.SetActive(true);
        finishLevelButtonsParent.SetActive(true);
    }

    private bool IsGameFinished()
    {
        if(currentLevel >= levels.Length)
        {
            return true;
        }

        return false;
    }

    private bool LevelIsFinished()
    {
        if (levels[currentLevel].currentStepsAcquired >= levels[currentLevel].danceStepRequirements.Length)
        {
            return true;
        }

        return false;
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

    public void NextTap()
    {
        if(IsGameFinished())
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            NextLevel();
        }
    }

    private void NextLevel()
    {
        reloadButton.SetActive(false);
        congratsMessage.SetActive(false);
        finishLevelButtonsParent.SetActive(false);
        canSpawnDanceSteps = true;
        currentStepIndex = 0;
        currentCombo = 0;
        GetComponent<AudioSource>().Stop();
        InitLevel();
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
