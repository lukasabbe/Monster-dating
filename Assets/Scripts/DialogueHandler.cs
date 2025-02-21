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

    public List<FoodItem> foodItems = new();
    
    //Martin things
    public List<GameObject> shirts = new(); 

    private int shirtIndex = 1;
    
    public List<GameObject> martin_faces = new List<GameObject>();

    public List<string> foodComments = new();
    
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
        dialogueEventDispatcher.EndCallback.AddListener((dialogue2, text) => Application.Quit());
        dialogueEventDispatcher.ErrorCallback.AddListener(SkipNode);
        
        dialogueEventDispatcher.AddDynamicEventListener("change_rep", args =>
        {
            if (!args.TryGet(0, out float rep_delta)) return;
            rep += rep_delta;
            var face_index = (int) Mathf.Lerp(0, martin_faces.Count-1, rep);
            for (int i = 0; i < martin_faces.Count; i++)
            {
                martin_faces[i].SetActive(i == face_index);
            }
        });
        
        dialogueEventDispatcher.AddDynamicEventListener("change_clothes", args =>
        {
            shirts.ForEach(s => s.SetActive(false));
            shirts[shirtIndex].SetActive(true);
            shirtIndex++;

        });
        
        dialogueEventDispatcher.AddDynamicEventListener("food_comment", args =>
        {
            var success = true;
            var food_amount = 0;
            foreach (var foodItem in foodItems)
            {
                foreach (var reqFoodItem in monsterDialogue[currentMonster].req_food_items)
                {
                    if (reqFoodItem.type != foodItem.type) continue;
                    food_amount++;
                    if(foodItem.burned != reqFoodItem.burned) success = false;
                    if(foodItem.cooked != reqFoodItem.cooked) success = false;
                    if(foodItem.mixed != reqFoodItem.mixed) success = false;
                    if(foodItem.shopped != reqFoodItem.shopped) success = false;
                }
            }

            if (monsterDialogue[currentMonster].req_food_items.Count > food_amount) success = false;
            
            
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
        
        if(!dialogue.Next(out var node))
        {
            return;
        }

        dialogueEventDispatcher.TranslateAndDispatch(dialogue, node);
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
