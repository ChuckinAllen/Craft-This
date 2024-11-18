using PoschPlus.CraftingSystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] public Recipe startingRecipe;
    [SerializeField] private CreateItem createItem;
    [SerializeField] private GameObject craftingItem_UI;

    [HorizontalGroup("Dev")]
    [SerializeField] private List<Ingredient> startingIngredentList
        = new List<Ingredient>(); //CreateStartingIngredentList

    [HorizontalGroup("Dev")]
    [ToggleLeft]
    [SerializeField] private bool startValueAtZero = false;
    [ShowIf("startValueAtZero")]
    [SerializeField] private List<Ingredient> startingIngredentListAtZero
        = new List<Ingredient>(); //CreateStartingIngredentList

    [SerializeField] int spawnPosOffset = 20;

    private float offSet;

    // Start is called before the first frame update
    void Start()
    {
        /*Spawn Item to craft
         * Set the item in the craft pos
         * Spawn the starting items
        */
        //GameObject instance = Instantiate(craftingItem_UI);
        //instance.transform.SetParent(transform);
        //instance.transform.position = Vector3.zero;

        foreach (var ingredent in startingIngredentList)
        {
            var pos = transform.position.x + spawnPosOffset;

            offSet += pos;

            Vector2 positionOffset = new Vector2(offSet,0);

            //createItem.CreateNewItem(ingredent, transform);
            createItem.CreateNewItem(ingredent, positionOffset, true);
            //item.CreateNewItem(ingredent, pos);
            //GameObject craftableItemsInstance = Instantiate(ingredent);
            //craftableItemsInstance.transform.SetParent(transform);
            //craftableItemsInstance.transform.position += new Vector3(spawnPosOffset, 0,0);
        }

        if (startValueAtZero)
        {
            foreach (var ingredent in startingIngredentListAtZero)
            {
                var pos = transform.position.x + spawnPosOffset;

                offSet += pos;

                Vector2 posOffset = new Vector2(offSet,0);

                createItem.CreateNewItem(ingredent, posOffset, true);
                //item.CreateNewItem(ingredent, pos);
                //GameObject craftableItemsInstance = Instantiate(ingredent);
                //craftableItemsInstance.transform.SetParent(transform);
                //craftableItemsInstance.transform.position += new Vector3(spawnPosOffset, 0,0);
            }
        }
    }
}
