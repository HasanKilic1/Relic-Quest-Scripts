using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TextColorer : MonoBehaviour
{
    public static string GetColorChangedTextViaItemType(string givenText)
    {
        string originalText = givenText;
        string[] targetWords = { "Common" , "Uncommon" , "Rare" , "Epic" ,"Legendary"};
        foreach (string word in targetWords )
        {
            if(givenText.Contains(word))
            {
                if(word == "Common") originalText = originalText.Replace(word, "<color=white>" + word + "</color>");
                if (word == "Uncommon") originalText = originalText.Replace(word, "<color=green>" + word + "</color>");
                if (word == "Rare") originalText = originalText.Replace(word, "<color=blue>" + word + "</color>");
                if (word == "Epic") originalText = originalText.Replace(word, "<color=purple>" + word + "</color>");
                if (word == "Legendary") originalText = originalText.Replace(word, "<color=orange>" + word + "</color>");
            }
        }
        return originalText;
    }  
}
