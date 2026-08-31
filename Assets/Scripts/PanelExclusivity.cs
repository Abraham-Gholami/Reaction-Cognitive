using System.Collections.Generic;
using UnityEngine;

// While a full-screen panel is up - a tutorial / voice-over page, the "sending to
// server" wait, or the completed / failed result - nothing from the game underneath may
// show through or take a tap. The panel art is not opaque edge to edge (the tutorial
// scroll and the result panels both leave transparent margins), so the HUD counters, the
// oxygen tank, the stimuli and the menu were all visible around them, and the invisible
// full-screen InputGetters were still collecting taps behind the panel.
//
// Hiding is done with CanvasGroup.alpha, never SetActive. Deactivating Stimules would
// fire Bubble.OnDisable, and that hook is what writes a row to the export - so switching
// those objects off to tidy the screen would forge trial rows. A CanvasGroup makes them
// invisible and unclickable without running any lifecycle code.
//
// Nothing is wired in the inspector: the panels are whatever lives under the Panels
// container, and the things to hide are every other direct child of this canvas. Derived
// from the hierarchy so it cannot fall out of date, and so a lost reference cannot
// silently turn the whole thing off.
[RequireComponent(typeof(Canvas))]
public class PanelExclusivity : MonoBehaviour
{
    [Tooltip("Container whose children are the exclusive panels. Found by name if unset.")]
    [SerializeField] Transform panelsRoot;
    [SerializeField] string panelsRootName = "Panels";

    readonly List<GameObject> panels = new List<GameObject>();
    readonly List<CanvasGroup> groups = new List<CanvasGroup>();
    bool? lastState;

    void Awake()
    {
        if (panelsRoot == null) panelsRoot = transform.Find(panelsRootName);
        if (panelsRoot == null)
        {
            Debug.LogError("PanelExclusivity: no '" + panelsRootName + "' under " + name);
            enabled = false;
            return;
        }

        foreach (Transform panel in panelsRoot)
            panels.Add(panel.gameObject);

        foreach (Transform child in transform)
        {
            if (child == panelsRoot) continue;
            // The editor-only skip button lives on this canvas and has to stay usable
            // while a tutorial panel is up - that is the whole point of it.
            if (child.GetComponent<TutorialSkipButton>() != null) continue;

            var group = child.GetComponent<CanvasGroup>();
            if (group == null) group = child.gameObject.AddComponent<CanvasGroup>();
            groups.Add(group);
        }
    }

    // LateUpdate, so a panel opened during this frame's Update is already accounted for
    // and the game underneath is never drawn for a frame behind it.
    void LateUpdate()
    {
        var panelUp = false;
        for (var i = 0; i < panels.Count; i++)
        {
            if (panels[i] != null && panels[i].activeInHierarchy) { panelUp = true; break; }
        }

        if (lastState == panelUp) return;
        lastState = panelUp;

        for (var i = 0; i < groups.Count; i++)
        {
            var group = groups[i];
            if (group == null) continue;
            group.alpha = panelUp ? 0f : 1f;
            group.interactable = !panelUp;
            group.blocksRaycasts = !panelUp;
        }
    }
}
