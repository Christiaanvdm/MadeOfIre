using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;

public class NPCController : MonoBehaviour
{

    public List<string> dialogue = new List<string>();
    public int maxCharactersOnLine = 17;
    private int speechIndex = 0;
    private Transform speechBubble;
    private TextMesh speechText;
    // Start is called before the first frame update
    void Start()
    {
        speechBubble = transform.parent.Find("SpeechBubble");
        speechText = speechBubble.Find("SpeechText").GetComponent<TextMesh>();
        speechBubble.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player" && speechIndex == 0)
        {
            DisplaySpeechBubble();
        }
    }

    private void DisplaySpeechBubble()
    {
        speechBubble.gameObject.SetActive(true);
        speechIndex = -1;
        advanceStep();
    }

    private bool allowAdvance = true;
    private Coroutine currentRenderCoroutine;
    public void advanceStep()
    {
        if (allowAdvance)
        {
            if (currentRenderCoroutine != null)
            {
                StopCoroutine(currentRenderCoroutine);
            }
            allowAdvance = false;
            StartCoroutine(delayAdvanceStep());

            if (speechIndex < dialogue.Count - 1)
            {
                speechIndex += 1;
                var textToRender = WrapText(dialogue[speechIndex], maxCharactersOnLine);
                currentRenderCoroutine = StartCoroutine(renderTextToBubble(textToRender));
            }
        }
    }



    IEnumerator renderTextToBubble(string text) {
        int currentTextLength = 0;
        while (currentTextLength < text.Length) {
            currentTextLength += 1;
            speechText.text = text.Substring(0, currentTextLength);
            yield return new WaitForSeconds(0.03f);
        }
    }

    IEnumerator delayAdvanceStep() {
        yield return new WaitForSeconds(0.5f);
        allowAdvance = true;
    }

    private static string WrapText(string text, int lineCharacters)
    {
        var result = "";
        var words = text.Split(new string[] { " " }, StringSplitOptions.None);
        int currentLine = 0;
        var stringList = new List<string>();
        stringList.Add("");
        foreach (string word in words)
        {
            if (stringList[currentLine].Length + word.Length > lineCharacters)
            {
                stringList[currentLine] += Environment.NewLine;
                currentLine += 1;
                stringList.Add($"{word} ");
            }
            else
            {
                stringList[currentLine] += $"{word} ";
            }
        }
        foreach (var line in stringList) {
            result += line;
        }
        return result.ToString();
    }

    private void closeSpeechBubble()
    {
        speechIndex = 0;
        speechBubble.gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            closeSpeechBubble();
        }
    }
}
