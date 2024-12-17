using PoschPlus.CraftingSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Title("Game Setup")]
    //[SerializeField] private List<Recipe> LevelRecipes = new List<Recipe>();

    //[SerializeField] private List<Ingredient> startingIngredentList
    //    = new List<Ingredient>(); //CreateStartingIngredentList

    [SerializeField] private List<RecipePage> recipePage = new List<RecipePage>();

    [SerializeField] private int spawnPosOffset = 20;

    [SerializeField]
    private UnityEvent WinGameLevel;

    [SerializeField]
    private UnityEvent SetupNewLevel;

    private int currentLevelIndex = 0;
    private float offSet;

    public static Action<string> itemIsCorrect;

    [SerializeField, ReadOnly]
    private Recipe recipe;

    private void OnEnable()
    {
        itemIsCorrect += CheckForCorrectItem;
    }
    private void OnDisable()
    {
        itemIsCorrect -= CheckForCorrectItem;
    }


    public void StartTheGame()
    {
        foreach (var ingredent in recipePage[0].Ingredients)
        {
            SetupItems(true, ingredent);
        }
        CreatesItemToCraft(0);

        Debug.LogWarning($"Level Index {currentLevelIndex}");
    }



    public void StartNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= recipePage.Count)
        {
            currentLevelIndex = 0;
        }

        Debug.LogWarning($"Level Index {currentLevelIndex}");

        List<Ingredient> ingredients = recipePage[currentLevelIndex].Ingredients;

        foreach (Ingredient ingredient in ingredients)
        {
            if (recipePage == null || recipePage.Count == 0)
            {
                Debug.LogWarning("Ingredient book is empty!");
                return;
            }

            SetupItems(true, ingredient);
        }

        

        // Move to the next index
        //currentLevelIndex = (currentLevelIndex + 1) % recipePage.Count;

        //CreateItem.OnRemoveOldItemToCraft?.Invoke();

        CreatesItemToCraft(currentLevelIndex);
    }

    private void SetupItems(bool randomPos, Ingredient ingredient)
    {
        var pos = transform.position.x + spawnPosOffset;

        offSet += pos;

        CreateItem.OnCreateNewItem?.Invoke(ingredient, randomPos);
    }

    private void CreatesItemToCraft(int recipeNumber)
    {
        recipe = recipePage[recipeNumber].Recipes;

        Crafting.OnUpdateRecipe?.Invoke(recipe);

        Debug.Log(recipe);

        CreateItem.OnCreateNewItemToCraft?.Invoke(recipe.CraftedItems);
    }



    public void CheckForCorrectItem(string itemName)
    {
        if (itemName == recipe.CraftedItems.name)
        {
            Debug.Log("Level Completed");
            WinGameLevel?.Invoke();
        }
    }
}
