using PoschPlus.CraftingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemToCraft : MonoBehaviour
{
    [SerializeField] private StartGame startGame;

    [SerializeField]
    private UnityEvent WinGameLevel;

    [SerializeField] 
    private UnityEvent SetupNewLevel; 

    [SerializeField]
    private CreateItem createItem;

    private Ingredient ingredient;

    [SerializeField]
    public Transform itemContainerTransform;

    public static Action<string> itemIsCorrect;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject instance = Instantiate(itemThatNeedsToBeCrafted);

        ingredient = startGame.startingRecipe.CraftedItems;
        createItem.CreateNewItem(ingredient, itemContainerTransform, false, true);
        
    }

    private void OnEnable()
    {
        itemIsCorrect += CheckForCorrectItem;
    }

    private void OnDisable()
    {
        itemIsCorrect -= CheckForCorrectItem;
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
