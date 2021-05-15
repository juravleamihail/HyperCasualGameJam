using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinksMinigame : MonoBehaviour
{
    private float zOffset = 10;
    private GameObject ingredientOnScreen;

    [SerializeField] private TextMesh tempScore;
    [SerializeField] private TextMesh tempScore2;

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
                    if (ingredientPosition.y < 4.0f && ingredientPosition.y > 3.0f &&
                        (ingredientPosition.x < 1 && ingredientPosition.x > -1)) //up down green circle & left right green circle
                    {
                        tempScore.text = "perfect";
                        sliced = true;
                    }
                    else
                    {
                        tempScore.text = "great";
                        sliced = true;
                    }
                }
                else
                {
                    tempScore.text = "good";
                    sliced = true;
                }
            }

            if (sliced)
            {
                Destroy(ingredientOnScreen);
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
}