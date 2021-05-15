using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class IngredientsSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> requiredIngredients;
    [SerializeField] private List<int> requirementQuantities;
    [SerializeField] private List<TextMesh> requirementTable;

    [SerializeField] private DrinksMinigame minigame;

    private float[] startingX = {3.5f, -3.5f};
    private float startingY = 3.5f;
    private float baseMovementSpeed = 1f;
    private float movementSpeed = 1f;

    GameObject newIngredient;
    int randomPositionValue = 0;
    private int lastIndex = -1;

    private void Start()
    {
        RefreshRequirementTable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!newIngredient)
        {
            int randomIngredientIndex = Random.Range(0, requiredIngredients.Count);
            lastIndex = randomIngredientIndex;

            randomPositionValue = Random.Range(0, 2);
            newIngredient =
                Instantiate(requiredIngredients[randomIngredientIndex],
                    new Vector3(startingX[randomPositionValue], startingY, 0.0f), Quaternion.identity);
            minigame.SetIngredient(newIngredient);
            movementSpeed = baseMovementSpeed * Random.Range(2, 6) * Time.deltaTime;
        }
        else
        {
            newIngredient.transform.Translate(Vector3.right *
                                              (movementSpeed * -Mathf.Sign(startingX[randomPositionValue])));
            if (newIngredient.transform.position.x > 4f || newIngredient.transform.position.x < -4f)
            {
                Destroy(newIngredient);
                minigame.SetIngredient(null);
            }
        }
    }

    private void RefreshRequirementTable()
    {
        for (int i = 0; i < requirementTable.Count; i++)
        {
            requirementTable[i].text = requirementQuantities[i].ToString();
        }
    }

    public void CutIngredient()
    {
        requirementQuantities[lastIndex] = requirementQuantities[lastIndex] > 0 ? requirementQuantities[lastIndex] - 1 : 0;
        RefreshRequirementTable();
        Destroy(newIngredient);
        minigame.SetIngredient(null);
    }

    public int GetRemainingIngredientCount()
    {
        return requirementQuantities.Sum();
    }
}