using PoschPlus.CraftingSystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredent Book", menuName = "Create Ingredent Book")]
public class IngredientBook : ScriptableObject
{
    [ShowInInspector] public List<Ingredient> ingredientList = new List<Ingredient>();
}
