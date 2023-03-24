using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CutsceneOverlayUI : MonoBehaviour
{
    
    private const string CutsceneOverlay = "CutsceneOverlayUI";
    public static CutsceneOverlayUI Singleton { get; private set; }

    public VisualElement cutsceneOverlay;
    
    void Start()
    {
        Singleton = this;
        
        var root = GetComponent<UIDocument>().rootVisualElement;
        cutsceneOverlay = root.Q<VisualElement>(CutsceneOverlay);
    }
    
    public void ShowCutsceneOverlayUI()
    {
        cutsceneOverlay.style.display = DisplayStyle.Flex;
    }

    public void HideCutsceneOverlayUI()
    {
        cutsceneOverlay.style.display = DisplayStyle.None;
    }
}
