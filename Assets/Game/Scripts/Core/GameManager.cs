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
    private Ingredient ingredent;

    private float offSet;

    public static Action<string> itemIsCorrect;

    public List<Recipe> GetLevelRecipes()
    {
        return LevelRecipes;
    }

    //Starts the game once called from Device checker
    public void StartTheGame()
    {
        foreach (var ingredent in startingIngredentList)
        {
            SetupItems(0, true, ingredent);
        }
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

            //SetupItemToCraft(0);
        }

        
    }

    private void SetupItems(int recipeNumber, bool randomPos, Ingredient ingredent)
    {
        var pos = transform.position.x + spawnPosOffset;

        offSet += pos;


        ItemToCraft.OnCreateItemToCreate?.Invoke(recipeNumber);

        CreateItem.OnCreateNewItem?.Invoke(ingredent, randomPos);
    }

    private void SetupItemToCraft(int recipeNumber)
    {
        this.ingredent = GetLevelRecipes()[recipeNumber].CraftedItems;
        CreateItem.OnCreateNewItemToCraft?.Invoke(this.ingredent);
    }

    public void CheckForCorrectItem(string itemName)
    {
        if (itemName == ingredent.name)
        {
            Debug.Log("Level Completed");
            WinGameLevel?.Invoke();
        }
    }
}
