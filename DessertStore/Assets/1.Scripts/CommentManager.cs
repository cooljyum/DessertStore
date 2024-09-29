using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CommentManager : MonoBehaviour
{
    public Text commentPrefab;  // 댓글을 표시할 UI 프리팹 (텍스트 UI)
    public Transform commentParent;  // 댓글이 표시될 부모 오브젝트
    public float commentSpeed = 50f;  // 댓글이 올라가는 속도
    public float displayInterval = 2f;  // 댓글 표시 간격
    public int commentsPerBatch = 5;   // 한 번에 스폰할 댓글 개수

    private List<CommentData> comments = new List<CommentData>();  // 댓글 데이터 리스트
    private float timer = 0f;

    void Start()
    {
        LoadCommentsFromCSV("csv/Comments.csv");  // CSV에서 댓글 데이터 로드
    }

    void Update()
    {
        // 타이머를 이용해 일정 시간 간격으로 댓글을 표시
        timer += Time.deltaTime;
        if (timer >= displayInterval)
        {
            timer = 0f;
            ShowBatchOfComments();
        }
    }

    // 댓글 데이터를 CSV에서 로드하는 함수
    void LoadCommentsFromCSV(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(filePath))
        {
            string[] data = File.ReadAllLines(filePath);
            for (int i = 1; i < data.Length; i++)  // 첫 줄은 헤더이므로 무시
            {
                string[] row = data[i].Split(',');
                string commentText = row[0];
                float rating = float.Parse(row[1]);
                comments.Add(new CommentData(commentText, rating));
            }
        }
        else
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다.");
        }
    }

    // 댓글을 한 번에 여러 개씩 표시하는 함수
    void ShowBatchOfComments()
    {
        if (comments.Count == 0) return;

        for (int i = 0; i < commentsPerBatch; i++)
        {
            CommentData selectedComment = GetWeightedRandomComment();
            if (selectedComment != null)
            {
                // 댓글을 화면에 생성하고 위로 스크롤하는 애니메이션
                Text newComment = Instantiate(commentPrefab, commentParent);
                newComment.text = selectedComment.commentText;

                StartCoroutine(MoveComment(newComment));
            }
        }
    }

    // 평점에 따라 무작위로 댓글을 선택하는 함수 (평점이 높을수록 가중치 부여)
    CommentData GetWeightedRandomComment()
    {
        float totalWeight = 0f;

        // 가중치 계산 (평점이 높을수록 가중치가 커짐)
        foreach (CommentData comment in comments)
        {
            totalWeight += comment.rating;
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        // 무작위로 가중치에 따라 댓글 선택
        foreach (CommentData comment in comments)
        {
            cumulativeWeight += comment.rating;
            if (randomValue <= cumulativeWeight)
            {
                return comment;
            }
        }

        return null;  // 예외적인 경우
    }

    // 댓글을 위로 움직이는 코루틴
    IEnumerator MoveComment(Text comment)
    {
        RectTransform rectTransform = comment.GetComponent<RectTransform>();

        while (rectTransform.anchoredPosition.y < Screen.height)
        {
            rectTransform.anchoredPosition += new Vector2(0, commentSpeed * Time.deltaTime);
            yield return null;
        }

        // 화면에서 벗어나면 삭제
        Destroy(comment.gameObject);
    }
}

// 댓글 데이터 구조체
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
