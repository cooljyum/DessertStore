using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CommentManager : MonoBehaviour
{
    [SerializeField] private Text _commentPrefab;  // 댓글 프리팹
    [SerializeField] private Transform _commentParent;  // 댓글 표시될 부모 포지션
    [SerializeField] private float _commentSpeed = 50f;  // 댓글 올라가는 속도
    [SerializeField] private int _initialCommentsCount = 6;  // 처음에 스폰할 댓글 개수
    [SerializeField] private int _subsequentCommentsCount = 6;  // 그 후 스폰할 댓글 개수
    [SerializeField] private float _rating = 3.0f;  // 평점
    [SerializeField] private float _ratingRange = 1.0f;  // 평점 오차범위
    [SerializeField] private float _positionOffset = 100.0f;  // 위치 offset

    private List<CommentData> _comments = new List<CommentData>();  // 댓글 데이터 리스트
    private Queue<Text> _activeComments = new Queue<Text>(); // 화면에 표시된 댓글
    private int _outOfBoundsCount = 0;  // 화면을 벗어난 댓글 수

    private void Start()
    {
        LoadCommentsFromCSV("csv/Comments");  // CSV에서 댓글 데이터 로드
        ShowBatchOfComments(_initialCommentsCount);  // 처음에 10개 스폰
    }

    // 댓글 데이터를 CSV에서 로드
    private void LoadCommentsFromCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName); // 확장자 없이 파일 이름만 사용
        if (csvFile != null)
        {
            string[] data = csvFile.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < data.Length; i++)  // 첫 줄은 헤더이므로 무시
            {
                string[] row = data[i].Split(',');
                if (row.Length < 2) continue;

                string commentText = row[0].Trim();
                float rating;
                if (float.TryParse(row[1], out rating))
                {
                    _comments.Add(new CommentData(commentText, rating));
                }
                else
                {
                    Debug.LogError("Invalid rating value: " + row[1]);
                }
            }
        }
        else
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다.");
        }
    }

    // 댓글을 한 번에 여러 개씩 표시하는 함수
    private void ShowBatchOfComments(int commentsToSpawn)
    {
        if (_comments.Count == 0) return;

        for (int i = 0; i < commentsToSpawn; i++)
        {
            CommentData selectedComment = GetWeightedRandomComment(_rating - _ratingRange, _rating + _ratingRange);
            if (selectedComment != null)
            {
                // 댓글을 화면에 생성하고 위로 스크롤하는 애니메이션
                Text newComment = Instantiate(_commentPrefab, _commentParent);

                // 댓글의 텍스트 설정
                newComment.text = selectedComment.commentText;

                // Y 위치를 다르게 조정하여 겹치지 않게 설정
                RectTransform rectTransform = newComment.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, (-i * 100f) - _positionOffset);

                // 생성된 댓글을 큐에 추가
                _activeComments.Enqueue(newComment);

                StartCoroutine(MoveComment(newComment));
            }
        }
    }

    // 평점 범위에 따라 무작위로 댓글을 선택하는 함수
    private CommentData GetWeightedRandomComment(float minRating, float maxRating)
    {
        float totalWeight = 0f;
        List<CommentData> filteredComments = new List<CommentData>();

        foreach (CommentData comment in _comments)
        {
            if (comment.rating >= minRating && comment.rating <= maxRating)
            {
                filteredComments.Add(comment);
                totalWeight += comment.rating;
            }
        }

        if (filteredComments.Count == 0)
        {
            Debug.LogWarning("해당 평점 범위에 댓글이 없습니다.");
            return null;
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        foreach (CommentData comment in filteredComments)
        {
            cumulativeWeight += comment.rating;
            if (randomValue <= cumulativeWeight)
            {
                return comment;
            }
        }

        return null;
    }

    // 댓글을 위로 움직이는 코루틴
    private IEnumerator MoveComment(Text comment)
    {
        RectTransform rectTransform = comment.GetComponent<RectTransform>();
        RectTransform parentRectTransform = _commentParent.GetComponent<RectTransform>();

        while (true)
        {
            rectTransform.anchoredPosition += new Vector2(0, _commentSpeed * Time.deltaTime);

            // 화면을 벗어난 경우
            if (rectTransform.anchoredPosition.y > parentRectTransform.anchoredPosition.y + (parentRectTransform.rect.height / 2) + 400.0f)
            {
                // 댓글을 화면 아래로 위치시켜 재사용
                rectTransform.anchoredPosition = new Vector2(0, -_positionOffset*4);
            }

            yield return null;
        }
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
