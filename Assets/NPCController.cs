using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class NPCTextCollection
{
    public List<NPCText> Collection = new List<NPCText>();
}

[Serializable]
public class NPCText {
    public int id;
    public List<string> dialogue;
}

public class NPCController : MonoBehaviour
{
    private NPCText nPCText;
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
        //SaveText();
        LoadText();
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

            if (speechIndex < nPCText.dialogue.Count - 1)
            {
                speechIndex += 1;
                var textToRender = WrapText(nPCText.dialogue[speechIndex], maxCharactersOnLine);
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

    private void LoadText() {
        //Read the text from directly from the test.txt file

        StreamReader reader = new StreamReader(path);
        var json = reader.ReadToEnd();
        reader.Close();
        var nPCTextCollection = JsonUtility.FromJson<NPCTextCollection>(json);
        nPCText = nPCTextCollection.Collection.First(x => x.id == textId);
    }

    public int textId = 1;
    private void SaveText()
    {
        var npcText = new NPCText();
        npcText.id = 1;
        npcText.dialogue = this.nPCText.dialogue;
        var nPCTextCollection = new NPCTextCollection();
        nPCTextCollection.Collection.Add(npcText);
        string json = JsonUtility.ToJson(nPCTextCollection);

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.Write(json);
        writer.Close();
    }

    private string path = "Assets/NPCText.json";
}
