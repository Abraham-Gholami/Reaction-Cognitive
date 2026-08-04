using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{

    Button button;
    // Start is called before the first frame update
    void Start()
    {
        //button = GetComponent<Button>();
        //button.onClick.AddListener
        //(
        //    ()=> SFXPlayer.Instance.PlaySFX()
        //);
    }

    // Update is called once per frame
    void OnDestroy()
    {
//        button.onClick.RemoveAllListeners();
    }
}
