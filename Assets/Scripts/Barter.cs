using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Barter : MonoBehaviour
{
    private Inventory _inventoryPlayer;

    private void Start()
    {
        _inventoryPlayer = GetComponent<Inventory>();
    }
}