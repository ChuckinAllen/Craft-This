using PoschPlus.CraftingSystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingGridUI : MonoBehaviour
{
    public int gridX = 3;
    public int gridY = 3;

    [ShowInInspector]
    [HorizontalGroup(Width = 400)]
    [TableMatrix(HideRowIndices = true, HideColumnIndices = true, SquareCells = true)]
    public GameObject[,] Grid = new GameObject[3, 3];

    [SerializeField] private Inventory inventory;

    [ReadOnly]
    [ShowInInspector]
    private List<Inventory.Items> storedInventoryItems = new List<Inventory.Items>();

    //[ReadOnly]
    [ShowInInspector]
    private List<Inventory.Items> storedInventoryItemsInCraftingTable = new List<Inventory.Items>();

    [SerializeField] public int selectionIndex = 0;

    //[SerializeField] public 

    [Button]
    public void UpdateCraftingTableSize()
    {
        Grid = new GameObject[gridX, gridY];
    }

    public void AddItemToGrid(string itemName)
    {
        //Grid[selectionIndex] = inventory;
    }
}
