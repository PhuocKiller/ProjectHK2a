using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputName: MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TextMeshProUGUI displayNameText;
    

    void Start()
    {
      
    }
    
    public void DisplayInputText(InputField input)
    {
        string inputName = input.text;
        displayNameText.text =  inputName;
    }
}
