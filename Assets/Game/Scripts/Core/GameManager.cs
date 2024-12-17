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
    [SerializeField] private List<Recipe> LevelRecipes = new List<Recipe>();

    [SerializeField] private List<Ingredient> startingIngredentList
        = new List<Ingredient>(); //CreateStartingIngredentList

    [SerializeField] private List<IngredientBook> ingredientBook = new List<IngredientBook>();

    [SerializeField] private int spawnPosOffset = 20;

    [SerializeField]
    private UnityEvent WinGameLevel;

    [SerializeField]
    private UnityEvent SetupNewLevel;

    private int currentLevelIndex = 0;
    private float offSet;

    public static Action<string> itemIsCorrect;

    private Ingredient ingredient;

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
        foreach (var ingredent in startingIngredentList)
        {
            SetupItems(0, true, ingredent);
        }
        CreatesItemToCraft(0);
    }



    public void StartNextLevel()
    {
        foreach (Ingredient ingredient in ingredientBook[currentLevelIndex].ingredientList)
        {
            if (ingredientBook == null || ingredientBook.Count == 0)
            {
                Debug.LogWarning("Ingredient book is empty!");
                return;
            }

            // Move to the next index
            currentLevelIndex = (currentLevelIndex + 1) % ingredientBook.Count;

            SetupItems(currentLevelIndex, true, ingredient);
        }

        CreatesItemToCraft(currentLevelIndex);

        CreateItem.OnRemoveOldItemToCraft?.Invoke();
    }


    private void CreatesItemToCraft(int recipeNumber)
    {
        ingredient = LevelRecipes[recipeNumber].CraftedItems;

        CreateItem.OnCreateNewItemToCraft?.Invoke(ingredient);
    }

    private void SetupItems(int recipeNumber, bool randomPos, Ingredient ingredient)
    {
        var pos = transform.position.x + spawnPosOffset;

        offSet += pos;

        CreateItem.OnCreateNewItem?.Invoke(ingredient, randomPos);
    }

    public void CheckForCorrectItem(string itemName)
    {
        if (itemName == ingredient.name)
        {
            Debug.Log("Level Completed");
            WinGameLevel?.Invoke();
        }
    }
}
