using System.Collections.Generic;
using UnityEngine;

public class CatManager : MonoBehaviour
{
    [SerializeField] private CharacterManager _characterManager; // �ϳ��� ĳ���� �Ŵ���
    [SerializeField] private List<CharacterActionData> _characterActionDataList; // ĳ���� �ൿ ������ ����Ʈ

    private void Awake()
    {
        // CharacterManager ������Ʈ �ʱ�ȭ (�� ���, ���� GameObject�� �ִٰ� ����)
        _characterManager = GetComponent<CharacterManager>();

        // CharacterActionData ����Ʈ �ʱ�ȭ
        // ���� ���, Resources �������� �ε��ϴ� ���
         _characterActionDataList = new List<CharacterActionData>(Resources.LoadAll<CharacterActionData>("ScriptableObject/CharacterAction/Main/Cat"));
    }

    public void PerformCharacterAction(CharacterState state)
    {
        // ���¿� �ش��ϴ� �׼� ������ ��������
        CharacterActionData actionData = _characterActionDataList.Find(data => data.characterState == state);

        if (actionData != null)
        {
            _characterManager.PerformAction(actionData); // ĳ���� �׼� ����
        }
        else
        {
            Debug.LogWarning("�ش� ���¿� ���� �׼� �����Ͱ� �����ϴ�.");
        }
    }
}
