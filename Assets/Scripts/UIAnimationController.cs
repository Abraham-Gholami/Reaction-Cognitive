using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
public class UIAnimationController : GenericSingleton<UIAnimationController>
{
    [SerializeField]
    Animator comboAnimator;
    [SerializeField]
    TMP_Text comboText;
    string comboExtention = "x";
    void Start()
    {
        SettingsManager.Instance.settingPageClosed += OnSettingMenuClosed;
        shark.gameObject.SetActive(false);
        fish.gameObject.SetActive(false);
        comboText.text = "";
        diamondStartingPos = diamond.transform.position;
        MakeFish();
    }
    [SerializeField] int maxFish;
    public void MakeFish()
    {
        fishList = new List<GameObject>();
        for (int i = 0; i < maxFish; i++)
            fishList.Add(NewFish());
    }
    GameObject NewFish()
    {
        var fishGO = Instantiate(fish,fish.transform.position,fish.transform.rotation);
        fishGO.transform.SetParent(fish.transform.parent);
        fishGO.transform.localScale = new Vector3(1,1,1);
        fishGO.gameObject.SetActive(false);
        return fishGO.gameObject;
    }
    public void AnimateFish(int howMany,Vector3 goal)
    {
        if(howMany <= 0) return;
        var startingPos = fish.transform.position;
        var released = 0;
        // Trials can be as little as ~1.1s apart while the last fish of a batch is
        // airborne for ~1.7s, so leftovers would be counted as part of this release.
        // Land them first — DOComplete runs their OnComplete, which hides them and
        // credits the counter — so each combo is shown on its own.
        for (int i = 0; i < fishList.Count; i++)
            if(fishList[i].activeSelf) fishList[i].transform.DOComplete();
        // Only fish that are not already in flight may be reused. Taking a busy one
        // restarts its tween while the old one is still running, and that old tween's
        // OnComplete then hides a fish belonging to the new combo — which is how the
        // number on screen drifted away from the multiplier.
        for (int i = 0; i < fishList.Count && released < howMany; i++)
        {
            if(fishList[i].activeSelf) continue;
            released ++;
            ReleaseFish(fishList[i],startingPos,goal,released);
        }
        // The whole pool is still airborne from an earlier combo, so add to it rather
        // than recycling a fish mid-flight and losing it from the count.
        while(released < howMany)
        {
            var extra = NewFish();
            fishList.Add(extra);
            released ++;
            ReleaseFish(extra,startingPos,goal,released);
        }
    }
    void ReleaseFish(GameObject item,Vector3 startingPos,Vector3 goal,int order)
    {
        item.transform.DOKill();
        item.transform.position = startingPos;
        item.SetActive(true);
        item.transform.DOMove(goal,duration + order * 0.2f).OnComplete(()=> SetBackAnimTransform(startingPos,item));
    }
    public Image fish,shark,diamond,fishCounter;
    [SerializeField]
    float duration,diamondDuration = 2;
    List<GameObject> fishList = new List<GameObject>();
    int difference;
    public void Animate(bool isFish,Vector3 goal,int counter = 0,bool wasClickedOn = true)
    {
        Vector3 startingPos;
        startingPos = fish.transform.position;
        if(isFish && wasClickedOn)
        {
            if(SettingsManager.Instance.useCombo)
                AnimateFish(counter,goal);
            else
                // No fish fly when the multiplier is off, and UpdateText only ran when a
                // fish landed — so the score display froze for the whole session.
                ReactionManager.Instance.UpdateText();
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
            comboText.text =  comboExtention + combo;
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
    Vector3 diamondStartingPos;
    public void AnimateDiamond(Vector3 goal,Action onComplete)
    {
        void Completed()
        {
            SetBackAnimTransform(diamondStartingPos,diamond.gameObject);
            onComplete?.Invoke();
        }
        diamond.gameObject.SetActive(true);
        diamond.transform.DOMove(goal,diamondDuration).OnComplete
        (
            ()=>  Completed()
        );
    }
    public void ClearCombo()
    {
        comboText.text = "";
    }
    void OnSettingMenuClosed()
    {

        var isActive = SettingsManager.Instance.useCombo;
        comboText.gameObject.SetActive(isActive);
    }
    private void OnDestroy() 
    {
        base.OnDestroy();
        // Never resurrect the settings singleton during teardown just to unsubscribe.
        var settings = SettingsManager.Instance;
        if (settings != null) settings.settingPageClosed -= OnSettingMenuClosed;
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
/*
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
*/
