using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputField : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TextMeshProUGUI displayNameText;
    internal string text;
    internal object onValueChanged;

    // Start is called before the first frame update
    public void DisplayInputText()
    {
        string inputName = nameInputField.text;
        displayNameText.text = inputName;
    }
}
