using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //  the mouse is currently hovering over the slot or not
    public bool hovered;

    // Ref to the item currently held in the slot
    private ItemScript held;

    // Colors used for transperncy and toher stuff
    private Color opq = new Color(1, 1, 1, 1);          
    private Color transparent = new Color(1, 1, 1, 0);  

    // Reference to the Image component of the slot
    private Image SlotImage;

    // set the initial item to null
    public void InitialzeSlot()
    {
        // Get the Image component attached to the slot GameObject
        SlotImage = gameObject.GetComponent<Image>();

        // Set the sprite to null and make the slot transparent
        SlotImage.sprite = null;
        SlotImage.color = transparent;

        // Set the held item to null
        SetItem(null);
    }

    //  to set the item in the slot and update itsrepresentation
    public void SetItem(ItemScript item)
    {
        // Update the reference to the held item
        held = item;

        // If there is an item in the slot
        if (item != null)
        {
           
            SlotImage.sprite = held.icon;

            SlotImage.color = opq;
        }
        else
        {
            // If there is no item set the sprite to null and make the slot transparent
            SlotImage.sprite = null;
            SlotImage.color = transparent;
        }
    }

   
    public ItemScript GetItem()
    {
        return held;
    }

    // Called when the mouse pointer enters the slot area
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
       
        hovered = true;
    }

    // Called when the mouse pointer exits the slot area
    public void OnPointerExit(PointerEventData pointerEventData)
    {
       
        hovered = false;
    }
}
