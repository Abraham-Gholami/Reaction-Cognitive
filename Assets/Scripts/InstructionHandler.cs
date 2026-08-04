using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionHandler : MonoBehaviour
{
    [SerializeField] Sprite [] instructions;
    [SerializeField] Image instructionsImage,button;
    [SerializeField] GameObject instructionsPanel;
    [SerializeField] Sprite signUp;

    int index;
    private void Start() 
    {
        instructionsImage.sprite = instructions[index];
        instructionsPanel.gameObject.SetActive(true);

    }
    public void Next() {
        index ++;
        if(index >= instructions.Length)
        {
            instructionsPanel.gameObject.SetActive(false);
            return;
        }
        if(index + 1 >= instructions.Length)
        {
            button.sprite = signUp;
        }
        instructionsImage.sprite = instructions[index];
    }
}
