using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridRecipeSizeData
{
    public int gridRecipeCount;           // 해당 사이즈 아이템의 개수
    public GridRecipeData gridRecipeData; // 해당 사이즈 아이템 목록
}


[CreateAssetMenu(fileName = "New Puzzle Data", menuName = "Order System/Puzzle Data")]
public class PuzzleData : ScriptableObject
{
    public List<GridRecipeSizeData> gridRecipeDatas;    // 해당 아이템 목록
}