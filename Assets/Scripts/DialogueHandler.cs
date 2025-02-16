using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour
{
    public List<Button> buttons = new();
    public GameObject next_button;
    
    public TMP_Text dialogueText;

    private Dialogue dialogue;
    private DialogueEventDispatcher dialogueEventDispatcher = new();
    
    public List<MonsterDialogue> monsterDialogue = new();

    public int currentMonster = 0;
    
    //Martin things
    public GameObject cool_shirt;
    public GameObject normal_shirt;
    
    public List<GameObject> martin_faces = new List<GameObject>();
    
    private static IAsk currentAsk;

    private float rep;
    
    private void Start()
    {
        rep = monsterDialogue[currentMonster].base_rep;
        var textasset = Resources.Load<TextAsset>(monsterDialogue[currentMonster].file_name);
        dialogue = new(textasset.text.Split('\n').ToList());
        dialogueEventDispatcher.TextCallback.AddListener((dialogue1, text) => dialogueText.text = text.Text);
        dialogueEventDispatcher.LabelCallback.AddListener(SkipNode);
        dialogueEventDispatcher.JumpByCallback.AddListener(SkipNode);
        dialogueEventDispatcher.JumpToCallback.AddListener(SkipNode);
        dialogueEventDispatcher.DynamicEventCallback.AddListener(SkipNode);
        dialogueEventDispatcher.StartCallback.AddListener(SkipNode);
        dialogueEventDispatcher.EndCallback.AddListener(SkipNode);
        dialogueEventDispatcher.ErrorCallback.AddListener(SkipNode);
        
        dialogueEventDispatcher.AddDynamicEventListener("change_rep", args =>
        {
            if (!args.TryGet(0, out float rep_delta)) return;
            rep += rep_delta;
            var face_index = (int) Mathf.Lerp(0, martin_faces.Count, rep);
            for (int i = 0; i < martin_faces.Count; i++)
            {
                martin_faces[i].SetActive(i == face_index);
            }
        });
        
        dialogueEventDispatcher.AddDynamicEventListener("change_clothes", args =>
        {
            cool_shirt.SetActive(false);
            normal_shirt.SetActive(true);
            
        });
        
        dialogueEventDispatcher.AskCallback.AddListener((dialogue1, ask) =>
        {
            currentAsk = ask;
            
            for (var i = 0; i < ask.Answers.Length; i++)
            {
                buttons[i].GetComponentInChildren<TMP_Text>().text = ask.Answers[i];
                buttons[i].gameObject.SetActive(true);
            }
            
            next_button.SetActive(false);
        });
        
        ProgresEvent();
    }

    public void ProgresEvent()
    {
        foreach (var t in buttons) t.gameObject.SetActive(false);

        next_button.SetActive(true);
        
        dialogue.Next(out var node);
        dialogueEventDispatcher.TranslateAndDispatch(dialogue,node);
    }

    public void SkipNode(Dialogue dialogue, INode node)
    {
        ProgresEvent();
    }
    
    public void setDigIndex(int index)
    {
        currentAsk.AnswerIndex = index;
    }
}
