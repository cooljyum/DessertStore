using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GridRecipeData", menuName = "Order System/GridRecipeData")]
public class GridRecipeData : ScriptableObject
{
    public Vector2Int recipeSize;     // 아이템의 크기 (width, height)
    public RecipeData[] recipeDatas;  // 해당 크기의 레시피들 데이터
}
