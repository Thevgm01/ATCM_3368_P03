using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebateController : MonoBehaviour
{
    public string[] truthBullets;

    [SerializeField] int activeText = -1;

    NonstopDebateText[] text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentsInChildren<NonstopDebateText>(true);
        foreach (var t in text) t.gameObject.SetActive(false);
        NextText();
    }

    void NextText()
    {
        if(activeText >= 0) text[activeText].Finished -= NextText;
        activeText++;
        text[activeText].Finished += NextText;
        text[activeText].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
