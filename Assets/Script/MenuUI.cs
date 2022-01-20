using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("InfoText")]
    [SerializeField] GameObject helpText;

    #region InfoText
    public void OnPointerEnter(PointerEventData eventData)
    {
        helpText.SetActive(true);
        Text test = helpText.GetComponent<Text>();
        switch (eventData.pointerEnter.name)
        {
        case "TextPlay":
            test.text = "Pressing Play will find a random lobby to join.";//Play button help text
            break;
        case "TextCreate":
            test.text = "Press create to create a own lobby.";//Inputfield id help text
            break;
        case "InputField":
            test.text = "Fill this textbox with the room ID that you want to join. \n The create button automatically change to join if the ID is correct.";//Inputfield id help text
            break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        helpText.GetComponent<Text>().text = "";
        helpText.SetActive(false);
    }
    #endregion
}
