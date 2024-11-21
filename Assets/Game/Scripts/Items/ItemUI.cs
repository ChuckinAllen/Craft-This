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
        [SerializeField] public Transform itemOrImageTransform;
        [SerializeField] public TextMeshProUGUI itemName;
    }
}