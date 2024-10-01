using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CommentManager : MonoBehaviour
{
    [SerializeField] private Text _commentPrefab;  //댓글 프리팹
    [SerializeField] private Transform _commentParent;  // 댓글 표시될 부모 포지션
    [SerializeField] private float _commentSpeed = 50f;  // 댓글 올라가는 속도
    [SerializeField] private float _displayInterval = 2f;  //댓글 표시 간격
    [SerializeField] private int _commentsPerBatch = 5;   //한 번에 스폰할 댓글 개수

    private List<CommentData> _comments = new List<CommentData>();  // 댓글 데이터 리스트
    private float _timer = 0f;

    [SerializeField] private float _rating = 3.0f; //평점
    [SerializeField] private float _ratingRange = 1.0f; //평점 오차범위

    private void Start()
    {
        LoadCommentsFromCSV("csv/Comments");  // CSV에서 댓글 데이터 로드
        ShowBatchOfComments(_rating - _ratingRange, _rating + _ratingRange); 
    }

    private void Update()
    {
        // 타이머를 이용해 일정 시간 간격으로 댓글 표시
        _timer += Time.deltaTime;
        if (_timer >= _displayInterval)
        {
            _timer = 0f;
            ShowBatchOfComments(_rating - _ratingRange, _rating + _ratingRange); //평점 오차 범위 -1 ~ +1
        }
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
                if (row.Length < 2) continue; // 데이터가 충분한지 확인

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
    private void ShowBatchOfComments(float minRating, float maxRating)
    {
        if (_comments.Count == 0) return;

        for (int i = 0; i < _commentsPerBatch; i++)
        {
            CommentData selectedComment = GetWeightedRandomComment(minRating, maxRating);
            if (selectedComment != null)
            {
                // 댓글을 화면에 생성하고 위로 스크롤하는 애니메이션
                Text newComment = Instantiate(_commentPrefab, _commentParent);

                // 댓글의 텍스트 설정
                newComment.text = selectedComment.commentText;

                // Y 위치를 다르게 조정하여 겹치지 않게 설정
                RectTransform rectTransform = newComment.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -i * 100f); // 30f는 각 댓글 간격, 필요에 따라 조정 가능

                StartCoroutine(MoveComment(newComment));
            }
        }
    }


    // 평점 범위에 따라 무작위로 댓글을 선택하는 함수
    private CommentData GetWeightedRandomComment(float minRating, float maxRating)
    {
        float totalWeight = 0f;
        List<CommentData> filteredComments = new List<CommentData>();

        // 평점 범위 따라 필터링
        foreach (CommentData comment in _comments)
        {
            if (comment.rating >= minRating && comment.rating <= maxRating)
            {
                filteredComments.Add(comment);
                totalWeight += comment.rating;  // 가중치 계산
            }
        }

        if (filteredComments.Count == 0)
        {
            Debug.LogWarning("해당 평점 범위에 댓글이 없습니다.");
            return null;  // 필터링된 댓글이 없는 경우
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        // 필터링된 댓글 중에서 가중치에 따라 무작위 선택
        foreach (CommentData comment in filteredComments)
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
    private IEnumerator MoveComment(Text comment)
    {
        RectTransform rectTransform = comment.GetComponent<RectTransform>();
        RectTransform parentRectTransform = _commentParent.GetComponent<RectTransform>();
        int outOfBoundsCounter = 0; // 화면을 벗어난 횟수 카운터

        while (true)
        {
            rectTransform.anchoredPosition += new Vector2(0, _commentSpeed * Time.deltaTime);

            // 화면을 벗어난 경우
            if (rectTransform.anchoredPosition.y > parentRectTransform.anchoredPosition.y + (parentRectTransform.rect.height / 2)+800.0f)
            {
                outOfBoundsCounter++; // 카운터 증가
            }

            // 카운터가 3이 되면 댓글 삭제
            if (outOfBoundsCounter >= 3)
            {
                Destroy(comment.gameObject);
                yield break; // 코루틴 종료
            }

            yield return null; // 다음 프레임까지 대기
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
