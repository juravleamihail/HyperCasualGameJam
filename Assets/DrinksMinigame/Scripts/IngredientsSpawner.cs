using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientsSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> ingredients;
    [SerializeField] private DrinksMinigame minigame;

    private float[] startingX = { 3.5f, -3.5f };
    private float startingY = 3.5f;
    private float movementSpeed = 0.01f;

    GameObject newIngredient;
    int randomPositionValue = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!newIngredient)
        {
            int randomIngredientIndex = Random.Range(0, ingredients.Count);

            randomPositionValue = Random.Range(0, 2);
            newIngredient =
                Instantiate(ingredients[randomIngredientIndex], new Vector3(startingX[randomPositionValue], startingY, 0.0f), Quaternion.identity);
            minigame.SetIngredient(newIngredient);
        }
        else
        {
            newIngredient.transform.Translate(Vector3.right * (movementSpeed * -Mathf.Sign(startingX[randomPositionValue])));
            if (newIngredient.transform.position.x > 4f || newIngredient.transform.position.x < -4f)
            {
                Destroy(newIngredient);
                minigame.SetIngredient(null);
            }
        }
        
    }
}
