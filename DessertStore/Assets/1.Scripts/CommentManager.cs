using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CommentManager : MonoBehaviour
{
    public Text commentPrefab;  // ����� ǥ���� UI ������ (�ؽ�Ʈ UI)
    public Transform commentParent;  // ����� ǥ�õ� �θ� ������Ʈ
    public float commentSpeed = 50f;  // ����� �ö󰡴� �ӵ�
    public float displayInterval = 2f;  // ��� ǥ�� ����
    public int commentsPerBatch = 5;   // �� ���� ������ ��� ����

    private List<CommentData> comments = new List<CommentData>();  // ��� ������ ����Ʈ
    private float timer = 0f;

    void Start()
    {
        LoadCommentsFromCSV("csv/Comments.csv");  // CSV���� ��� ������ �ε�
    }

    void Update()
    {
        // Ÿ�̸Ӹ� �̿��� ���� �ð� �������� ����� ǥ��
        timer += Time.deltaTime;
        if (timer >= displayInterval)
        {
            timer = 0f;
            ShowBatchOfComments();
        }
    }

    // ��� �����͸� CSV���� �ε��ϴ� �Լ�
    void LoadCommentsFromCSV(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(filePath))
        {
            string[] data = File.ReadAllLines(filePath);
            for (int i = 1; i < data.Length; i++)  // ù ���� ����̹Ƿ� ����
            {
                string[] row = data[i].Split(',');
                string commentText = row[0];
                float rating = float.Parse(row[1]);
                comments.Add(new CommentData(commentText, rating));
            }
        }
        else
        {
            Debug.LogError("CSV ������ ã�� �� �����ϴ�.");
        }
    }

    // ����� �� ���� ���� ���� ǥ���ϴ� �Լ�
    void ShowBatchOfComments()
    {
        if (comments.Count == 0) return;

        for (int i = 0; i < commentsPerBatch; i++)
        {
            CommentData selectedComment = GetWeightedRandomComment();
            if (selectedComment != null)
            {
                // ����� ȭ�鿡 �����ϰ� ���� ��ũ���ϴ� �ִϸ��̼�
                Text newComment = Instantiate(commentPrefab, commentParent);
                newComment.text = selectedComment.commentText;

                StartCoroutine(MoveComment(newComment));
            }
        }
    }

    // ������ ���� �������� ����� �����ϴ� �Լ� (������ �������� ����ġ �ο�)
    CommentData GetWeightedRandomComment()
    {
        float totalWeight = 0f;

        // ����ġ ��� (������ �������� ����ġ�� Ŀ��)
        foreach (CommentData comment in comments)
        {
            totalWeight += comment.rating;
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        // �������� ����ġ�� ���� ��� ����
        foreach (CommentData comment in comments)
        {
            cumulativeWeight += comment.rating;
            if (randomValue <= cumulativeWeight)
            {
                return comment;
            }
        }

        return null;  // �������� ���
    }

    // ����� ���� �����̴� �ڷ�ƾ
    IEnumerator MoveComment(Text comment)
    {
        RectTransform rectTransform = comment.GetComponent<RectTransform>();

        while (rectTransform.anchoredPosition.y < Screen.height)
        {
            rectTransform.anchoredPosition += new Vector2(0, commentSpeed * Time.deltaTime);
            yield return null;
        }

        // ȭ�鿡�� ����� ����
        Destroy(comment.gameObject);
    }
}

// ��� ������ ����ü
[System.Serializable]
public class CommentData
{
    public string commentText;
    public float rating;

    public CommentData(string commentText, float rating)
    {
        this.commentText = commentText;
        this.rating = rating;
    }
}
