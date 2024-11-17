using PoschPlus.CraftingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RecipeBook", menuName = "Create RecipeBook")]
public class RecipeBook : ScriptableObject
{
    [SerializeField] Recipe[] Recipes;
}
