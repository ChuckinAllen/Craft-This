using PoschPlus.CraftingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Starting Ingredent List", menuName = "Create Starting Ingredent List")]
public class CreateStartingIngredentList : ScriptableObject
{
    public List<Ingredient> recipeItems = new List<Ingredient>();
}
