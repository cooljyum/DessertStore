using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public class DialogueEntry
    {
        public int Index;
        public string Speaker;
        public string Dialogue;
        public string CatEmotion;
        public string WitchEmotion;
    }

    private List<DialogueEntry> _dialogues = new List<DialogueEntry>();

    void Start()
    {
        LoadDialogueData("Tutorial");
    }

    private void LoadDialogueData(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>($"Csv/{fileName}");
        StringReader reader = new StringReader(csvFile.text);

        // 첫 번째 줄 건너뛰기
        reader.ReadLine();

        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            string[] fields = line.Split(',');

            DialogueEntry entry = new DialogueEntry
            {
                Index = int.Parse(fields[0]),
                Speaker = fields[1],
                Dialogue = fields[2],
                CatEmotion = fields[3],
                WitchEmotion = fields[4]
            };

            _dialogues.Add(entry);
        }
    }

    public DialogueEntry GetDialogueEntry(int index)
    {
        return _dialogues.Find(d => d.Index == index);
    }
}