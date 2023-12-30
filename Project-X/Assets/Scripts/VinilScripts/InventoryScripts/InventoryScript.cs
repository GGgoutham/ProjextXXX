using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryScript : MonoBehaviour
{
    public GameObject inventory;
    public List<SlotScript> InventorySlots = new List<SlotScript>();
    public Image crosshair;
    public TMP_Text itemhoverText;

    public float raycastDistance=5f;
    public LayerMask itemLayer;


    public void Start()
    {
        ToggleIventory(false);
        foreach(SlotScript uiSlot in InventorySlots)
        {
            uiSlot.InitialzeSlot();
        }

    }
    public void Update()
    {
       
        ItemRayCast(Input.GetKeyDown(KeyCode.E));
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleIventory(!inventory.activeInHierarchy);
        }
    }
    //Performs a raycast from the cameras position toward the crosshair to detect items.
//If  it is hasClicked is true  to add the item to the inventory otherwise it will  update the itemhoverText with the item name.
    private void ItemRayCast(bool hasClicked=false) 
    {
        itemhoverText.text = "";
        Ray ray = Camera.main.ScreenPointToRay(crosshair.transform.position);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,raycastDistance,itemLayer))
        {
            if(hit.collider != null)
            {
                if(hasClicked)
                {
                    ItemScript newItem = hit.collider.GetComponent<ItemScript>();
                    if(newItem)
                    {
                        addItemToInventory(newItem);
                    }
                }
                else
                {
                  ItemScript newItem= hit.collider.GetComponent<ItemScript>();
                    if (newItem)
                    {
                        itemhoverText.text= newItem.name;
                    }
                }
            }
        }
    }
    //adds the specific item to the inventory slots manage quantity and stacking of items within slots//
    private void addItemToInventory(ItemScript itemToAdd)
    {
        int leftoverQuantity = itemToAdd.currentQuantity;
        SlotScript openSlot = null;
        for(int i=0;i<InventorySlots.Count;i++)
        {
            ItemScript held = InventorySlots[i].GetItem();
            if (held!=null&&itemToAdd.name==held.name)
            {
                int freeSpaceInSlot = held.maxQuantity - held.currentQuantity;
                if(freeSpaceInSlot>=leftoverQuantity)
                {
                    held.currentQuantity = leftoverQuantity;
                    Destroy(itemToAdd.gameObject);
                    return;
                }
                else
                {
                    held.currentQuantity = held.maxQuantity;
                    leftoverQuantity -= freeSpaceInSlot;

                }
            }
            else if(held==null)
            {
                if(!openSlot)
                openSlot = InventorySlots[i];
            }
        }
        if(leftoverQuantity>0&&openSlot)
        {
            openSlot.SetItem(itemToAdd);
                itemToAdd.currentQuantity = leftoverQuantity;
            itemToAdd.gameObject.SetActive(false);
        }
        else
        {
            itemToAdd.currentQuantity = leftoverQuantity;
        }
    }
    //Toggles the visibility of the inventory ui and adjust the cursor lock state and visibilty

    private void ToggleIventory(bool enable)
    {
        inventory.SetActive(enable);
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;


    }    
}
