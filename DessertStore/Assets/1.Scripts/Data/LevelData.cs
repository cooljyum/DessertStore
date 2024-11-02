using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Order System/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Basic Info")]
    public int levelNumber;             // 가게 레벨
    public int maxMoney;                // 만점 기준 금액(?쓸지모르겠음)

    [Header("PuzzleData")]
    public PuzzleData[] puzzleDatas;    // 해당 라운드 퍼즐 목록
}