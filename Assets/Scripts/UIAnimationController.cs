using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class UIAnimationController : GenericSingleton<UIAnimationController>
{
    [SerializeField]
    Animator comboAnimator;
    [SerializeField]
    TMP_Text comboText;
    string comboExtention = "X";
    void Start()
    {
        SettingsManager.Instance.settingPageClosed += OnSettingMenuClosed;
        shark.gameObject.SetActive(false);
        fish.gameObject.SetActive(false);
        comboText.text = "";
    }
    public Image fish,shark,diamond,fishCounter;
    [SerializeField]
    float duration;
    List<GameObject> fishList = new List<GameObject>();
    int difference;
    public void Animate(bool isFish,Vector3 goal,int counter = 0,bool wasClickedOn = true)
    {
        Vector3 startingPos;
        startingPos = fish.transform.position;
        if(isFish && wasClickedOn)
        {
            if(SettingsManager.Instance.useCombo)
            {
                if(counter > 0)
                {
                    if(counter > fishList.Count)
                    {
                        for (int i = 0; i < counter - fishList.Count; i++)
                        {
                            var fishGO = Instantiate(fish,startingPos,fish.transform.rotation);
                            fishGO.transform.SetParent(fish.transform.parent);
                            fishGO.transform.localScale = new Vector3(1,1,1);
                            fishList.Add(fishGO.gameObject);
                        }
                    }
                    else if(counter < fishList.Count)
                    {
                        for (int i = 0; i < fishList.Count - counter ; i++)
                        {
                            Destroy(fishList[fishList.Count - 1]);
                            fishList.RemoveAt(fishList.Count - 1);
                        }
                    }

                }
                else 
                {
                    foreach (var item in fishList)
                    {
                        Destroy(item);
                    }   
                    fishList.Clear();
                }
                if(counter > 0 && fishList.Count > 0)
                {
                    float diff = 0;
                    foreach (var item in fishList)
                    {
                        diff += 0.2f;
                        item.gameObject.SetActive(true);
                        item.transform.DOMove(goal,duration + diff).OnComplete(()=> SetBackAnimTransform(startingPos,item));
                    }
                }
                else 
                {
                    fish.gameObject.SetActive(true);
                    fish.transform.DOMove(goal,duration).OnComplete(()=> SetBackAnimTransform(startingPos,fish.gameObject));
                }
            }
            else 
            {
                fish.gameObject.SetActive(true);
                fish.transform.DOMove(goal,duration).OnComplete(()=> SetBackAnimTransform(startingPos,fish.gameObject));
            }
        }
        else if(isFish && !wasClickedOn)
        {
            if(SettingsManager.Instance.useFeedBack) 
            {
                StartCoroutine(LerpFunction(Color.red, 0.5f));
                 if(RandomButtonGenerator.Instance.level.isTraining)
                    wrong.Play();
            }
        }
        else 
        {
            foreach (var item in fishList)
            {
                Destroy(item);
            }   
            fishList.Clear();
            shark.gameObject.SetActive(true);
            startingPos = shark.transform.position;
            shark.transform.DOMove(goal,duration).OnComplete(()=> SetBackAnimTransform(startingPos,shark.gameObject));;
            if(SettingsManager.Instance.useFeedBack) 
            {
                StartCoroutine(LerpFunction(Color.red, 0.5f));
                if(RandomButtonGenerator.Instance.level.isTraining)
                    wrong.Play();
            }
        }
        AnimateCombo(counter);
    }
    [SerializeField]
    AudioSource wrong;
    void AnimateCombo(int combo)
    {
        if(combo > 0)
        {
            comboText.text = combo + comboExtention;
            comboAnimator.Play("Combo");
        }
        else 
        {
            comboText.text = "";
        }
    }
    void SetBackAnimTransform(Vector3 pos,GameObject go)
    {
        go.SetActive(false);
        go.transform.position = pos;
        ReactionManager.Instance.UpdateText();
    }
    public void AnimateDiamond(Vector3 goal)
    {
        diamond.gameObject.SetActive(true);
        var startingPos = diamond.transform.position;
        diamond.transform.DOMove(goal,duration).OnComplete(()=> SetBackAnimTransform(startingPos,diamond.gameObject));;
    }
    public void AnimateSlider()
    {

    }
    void OnSettingMenuClosed()
    {

        var isActive = SettingsManager.Instance.useCombo;
        comboText.gameObject.SetActive(isActive);
    }
    private void OnDestroy() 
    {
        SettingsManager.Instance.settingPageClosed -= OnSettingMenuClosed;
    }
    IEnumerator LerpFunction(Color endValue, float duration)
    {
        float time = 0;
        Color startValue = fishCounter.color;
        while (time < duration)
        {
            fishCounter.color = Color.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        fishCounter.color = endValue;
        time = 0;
        while (time < duration)
        {
            fishCounter.color = Color.Lerp(endValue, startValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        fishCounter.color = Color.white;
    }


}
