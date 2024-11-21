using PoschPlus.CraftingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [field: SerializeField] public Ingredient Data { get; private set; }

    public void UseItem()
    {
        //subtract item count
        //remove item
    }

    public void UpdateItemData(Ingredient data)
    {
        Data = data;
    }


    //spawn data from item like mesh and other things needed
}
