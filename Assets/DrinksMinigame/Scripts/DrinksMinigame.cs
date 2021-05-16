using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DrinksMinigame : MonoBehaviour
{
    private float zOffset = 10;
    private GameObject ingredientOnScreen;
    private int ingredientCounter = 0;

    [SerializeField] private List<Sprite> blenders;
    [SerializeField] private SpriteRenderer inSceneBlender;
    [SerializeField] private IngredientsSpawner ingredientsSpawner;

    private void Awake()
    {
        SwipeDetector.OnSwipe += CutIngredient;
    }

    private void CutIngredient(SwipeData data)
    {
        Vector3[] positions = new Vector3[2];
        positions[0] = Camera.main.ScreenToWorldPoint(new Vector3(data.StartPosition.x, data.StartPosition.y, zOffset));
        positions[1] = Camera.main.ScreenToWorldPoint(new Vector3(data.EndPosition.x, data.EndPosition.y, zOffset));
        CheckScores(positions[0], positions[1]);
    }

    private void CheckScores(Vector3 pos1, Vector3 pos2)
    {
        if (ingredientOnScreen)
        {
            bool sliced = false;
            Vector3 ingredientPosition = ingredientOnScreen.transform.position;
            if (IsIngredientInSlice(ingredientPosition, pos1, pos2))
            {
                if ((ingredientPosition.y < 5.0f && ingredientPosition.y > 2.0f) &&
                    (ingredientPosition.x < 2 && ingredientPosition.x > -2)) //up down yellow circle & left right yellow circle
                {
                    if (ingredientPosition.y < 3.0f && ingredientPosition.y > 2.0f &&
                        (ingredientPosition.x < 1 && ingredientPosition.x > -1)) //up down green circle & left right green circle
                    {
                        ScoreManager.Instance.AddScore(ScoreManager.HitType.PerfectHit);
                        //tempScore.text = "perfect";
                        sliced = true;
                    }
                    else
                    {
                        //tempScore.text = "great";
                        ScoreManager.Instance.AddScore(ScoreManager.HitType.GreatHit);
                        sliced = true;
                    }
                }
                else
                {
                    //tempScore.text = "good";
                    ScoreManager.Instance.AddScore(ScoreManager.HitType.GoodHit);
                    sliced = true;
                }
            }

            if (sliced)
            {
                ingredientsSpawner.CutIngredient();
                switch (ingredientsSpawner.GetRemainingIngredientCount())
                {
                    case 7:
                        inSceneBlender.sprite = blenders[1];
                        break;
                    case 4:
                        inSceneBlender.sprite = blenders[2];
                        break;
                    case 0:
                        inSceneBlender.sprite = blenders[3];
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private bool IsIngredientInSlice(Vector3 ingredientPosition, Vector3 pos1, Vector3 pos2)
    {
        if (ingredientPosition.y - 1 < pos1.y && ingredientPosition.y + 1 > pos2.y ||
            ingredientPosition.y + 1 > pos1.y && ingredientPosition.y - 1 < pos2.y)
        {
            if (ingredientPosition.x - 1 < pos1.x && ingredientPosition.x + 1 > pos2.x ||
                ingredientPosition.x + 1 > pos1.x && ingredientPosition.x - 1 < pos2.x)
            {
                return true;
            }
        }

        return false;
    }

    public void SetIngredient(GameObject newIngredient)
    {
        ingredientOnScreen = newIngredient;
    }

    public void GoBack()
    {
        SceneManager.LoadScene(0);
    }
}