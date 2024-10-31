using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Order System/Recipe Data")]
public class RecipeData : ScriptableObject
{
    public string recipeName;                       // 메뉴 이름
    public List<ItemData> packagingComponents;      // 포장에 필요한 아이템들 (드래그 순서)
    public ItemData additionalItem;                 // 추가로 필요한 아이템 (ex. 포크, 빨대)
}