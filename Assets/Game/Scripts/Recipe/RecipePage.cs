using ExternPropertyAttributes;
using PoschPlus.CraftingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RecipePage", menuName = "Create RecipePage")]
public class RecipePage : ScriptableObject
{
    [SerializeField] public Recipe Recipes;

    [InfoBox("Theses ingredients should have the recipy but might have the exact ones or less " +
        "depending on the page")]
    [SerializeField] public List<Ingredient> Ingredients = new List<Ingredient>();
}
