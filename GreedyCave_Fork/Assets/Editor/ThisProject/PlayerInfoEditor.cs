using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(PlayerInfo))]
public class PlayerInfoEditor : Editor
{
    SerializedProperty propertyFactors;
    private void OnEnable()
    {
        propertyFactors = serializedObject.FindProperty("propertyFactors");
        PropertyFactorInfo.InitialPropertys(serializedObject, propertyFactors);
        PropertyFactorInfo.SortPropertys(serializedObject, propertyFactors, PropertyFactorInfo.baseSort);
    }
}


public class PropertyFactorInfo
{

    public static Dictionary<PropertyType, int> baseSort = new Dictionary<PropertyType, int>()
    {
        { PropertyType.生命值上限,0 },
        { PropertyType.魔法值上限,1 },
        { PropertyType.经验值上限,2 },
        { PropertyType.攻击力,3 },
        { PropertyType.魔力,4 },

        { PropertyType.物理防御,5 },
        { PropertyType.魔法防御,6 },

        { PropertyType.攻速,7 },
    };

    public static void InitialPropertys(SerializedObject serializedObject, SerializedProperty propertyFactors)
    {
        InitialProperty(serializedObject, propertyFactors, PropertyType.生命值上限, 50,10);
        InitialProperty(serializedObject, propertyFactors, PropertyType.魔法值上限, 25,10);
        InitialProperty(serializedObject, propertyFactors, PropertyType.经验值上限, 100,100);

        InitialProperty(serializedObject, propertyFactors, PropertyType.攻击力, 10,2);
        InitialProperty(serializedObject, propertyFactors, PropertyType.魔力, 10,2);

        InitialProperty(serializedObject, propertyFactors, PropertyType.物理防御, 5,1);
        InitialProperty(serializedObject, propertyFactors, PropertyType.魔法防御, 5,1);

        InitialProperty(serializedObject, propertyFactors, PropertyType.攻速, 1,0);


    }

    private static void InitialProperty(SerializedObject serializedObject, SerializedProperty propertyFactors, PropertyType targetType, float baseValue,float growValue)
    {
        for (int i = 0; i < propertyFactors.arraySize; i++)
        {
            if (propertyFactors.GetArrayElementAtIndex(i).FindPropertyRelative("propertyType").enumValueIndex == (int)targetType)
                return;
        }
        serializedObject.Update();

        propertyFactors.arraySize += 1;
        SerializedProperty propertyType = propertyFactors.GetArrayElementAtIndex(propertyFactors.arraySize - 1).FindPropertyRelative("propertyType");
        SerializedProperty propertyBaseValue = propertyFactors.GetArrayElementAtIndex(propertyFactors.arraySize - 1).FindPropertyRelative("baseValue");
        SerializedProperty propertyGrowValue = propertyFactors.GetArrayElementAtIndex(propertyFactors.arraySize - 1).FindPropertyRelative("growValue");

        propertyType.enumValueIndex = (int)targetType;
        propertyBaseValue.floatValue = baseValue;
        propertyGrowValue.floatValue = growValue;

        serializedObject.ApplyModifiedProperties();
    }

    public static void SortPropertys(SerializedObject serializedObject, SerializedProperty propertyFactors, Dictionary<PropertyType, int> sortDic)
    {
        for (int i = 0; i < propertyFactors.arraySize; i++)
        {
            SerializedProperty propertyType = propertyFactors.GetArrayElementAtIndex(i).FindPropertyRelative("propertyType");
            PropertyType type = (PropertyType)propertyType.enumValueIndex;

            if (sortDic.ContainsKey(type))
            {
                int index = sortDic[type];

                if (index != i)
                {
                    Swap(serializedObject, propertyFactors, i, index);
                    i--;
                }
            }
        }

    }
    public static void Swap(SerializedObject serializedObject, SerializedProperty propertyFactors, int one, int two)
    {
        serializedObject.Update();

        SerializedProperty first = propertyFactors.GetArrayElementAtIndex(one);
        SerializedProperty propertyTypeOne = first.FindPropertyRelative("propertyType");
        SerializedProperty baseValueOne = first.FindPropertyRelative("baseValue");
        SerializedProperty growValueOne = first.FindPropertyRelative("growValue");

        SerializedProperty second = propertyFactors.GetArrayElementAtIndex(two);
        SerializedProperty propertyTypeTwo = second.FindPropertyRelative("propertyType");
        SerializedProperty baseValueTwo = second.FindPropertyRelative("baseValue");
        SerializedProperty growValueTwo = second.FindPropertyRelative("growValue");

        PropertyType tempType = (PropertyType)propertyTypeOne.enumValueIndex;
        float tempBaseValue = baseValueOne.floatValue;
        float tempGrowValue = growValueOne.floatValue;

        propertyTypeOne.enumValueIndex = propertyTypeTwo.enumValueIndex;
        baseValueOne.floatValue = baseValueTwo.floatValue;
        growValueOne.floatValue = growValueTwo.floatValue;

        propertyTypeTwo.enumValueIndex = (int)tempType;
        baseValueTwo.floatValue = tempBaseValue;
        growValueTwo.floatValue = tempGrowValue;

        serializedObject.ApplyModifiedProperties();
    }

}
