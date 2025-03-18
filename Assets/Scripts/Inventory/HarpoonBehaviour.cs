using System;
using Inventory;
using UnityEngine;

public class HarpoonBehaviour : MonoBehaviour, IItem {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool isHarpoonActive = false;
    [SerializeField] private GameObject harpoon;
    [SerializeField] private Item harpoonItem;
    private ItemManager itemManager;

    private void Awake()
    {
        itemManager = GetComponent<ItemManager>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrimaryUse() // Shoot
    {
        throw new System.NotImplementedException();
    }

    public void SecondaryUse() // Pull?
    {
        throw new System.NotImplementedException();
    }

    public void TertiaryUse() //Reload
    {
        throw new System.NotImplementedException();
    }
}
