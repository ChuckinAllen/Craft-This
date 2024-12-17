using PoschPlus.CraftingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemToCraft : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField]
    private UnityEvent WinGameLevel;

    [SerializeField] 
    private UnityEvent SetupNewLevel;

    [SerializeField]
    private CreateItem createItem;

    private Ingredient ingredient;

    [SerializeField]
    public Transform itemContainerTransform;

    //public static Action<string> itemIsCorrect;

    //public static Action<int> OnCreateItemToCreate;

    private void OnEnable()
    {
        //OnCreateItemToCreate += CreatesItemToCraft;
        //itemIsCorrect += CheckForCorrectItem;
    }

    private void OnDisable()
    {
        //OnCreateItemToCreate -= CreatesItemToCraft;
        //itemIsCorrect -= CheckForCorrectItem;
    }

    private void CreatesItemToCraft(int recipeNumber)
    {
        //ingredient = gameManager.GetLevelRecipes()[recipeNumber].CraftedItems;

        //createItem.CreateNewItemToCraft(ingredient);
    }

    public void CheckForCorrectItem(string itemName)
    {
        if(itemName == ingredient.name)
        {
            Debug.Log("Level Completed");
            WinGameLevel?.Invoke();
        }
    }
}
