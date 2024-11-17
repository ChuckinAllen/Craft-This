using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PoschPlus.CraftingSystem
{
    //This is the function that the item will have
    public class ItemUI : MonoBehaviour
    {
        [SerializeField] public Transform itemImageTransform;
        [SerializeField] public TextMeshProUGUI itemName;
    }
}