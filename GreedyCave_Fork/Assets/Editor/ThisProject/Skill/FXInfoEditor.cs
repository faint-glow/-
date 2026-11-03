using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FXInfo))]
public class FXInfoEditor : SelfAdaptionPropertyDrawer
{
    public override float GetLabelWidth()
    {
        return 100;
    }

    public override void InitialProperty(SerializedProperty property)
    {
        SerializedProperty existTime = property.FindPropertyRelative("existTime");
        existTime.floatValue = 5.0f;
    }

    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty delayTime = property.FindPropertyRelative("delayTime");
        SerializedProperty isAutioOffsetDir = property.FindPropertyRelative("isAutioOffsetDir");
        SerializedProperty isAutioFlipDir = property.FindPropertyRelative("isAutioFlipDir");

        SerializedProperty autoFlipX = property.FindPropertyRelative("autoFlipX");
        SerializedProperty autoFlipY = property.FindPropertyRelative("autoFlipY");
        SerializedProperty followType = property.FindPropertyRelative("followType");
        SerializedProperty fireType = property.FindPropertyRelative("fireType");
        SerializedProperty originalRotate = property.FindPropertyRelative("originalRotate");
        SerializedProperty startPosType = property.FindPropertyRelative("startPosType");
        SerializedProperty targetPosType = property.FindPropertyRelative("targetPosType");
        SerializedProperty startTransform = property.FindPropertyRelative("startTransform");
        SerializedProperty targetOffset = property.FindPropertyRelative("targetOffset");

        SerializedProperty fxObj = property.FindPropertyRelative("fxObj");
        SerializedProperty isNeverDie = property.FindPropertyRelative("isNeverDie");
        SerializedProperty existTime = property.FindPropertyRelative("existTime");


        ShowPropertyHorizontal(delayTime, 150,"延迟时间");
        ShowPropertyHorizontal(isAutioOffsetDir, 30, "自动正向偏移",-80);
        ShowPropertyHorizontal(isAutioFlipDir, 30, "自动正向翻转");

        //ShowPropertyHorizontal(autoFlipX, 30, "自动水平反转");
        //ShowPropertyHorizontal(autoFlipY, 30, "自动竖直反转");
        ShowPropertyVertical(followType,"是否跟随父物体");
        ShowPropertyVertical(originalRotate,"初始方向");
        ShowPropertyVertical(fireType,"特效触发类型");
        if (fireType.enumValueIndex == (int)FX_FirType.Stay)
        {
            AddHideProperty(targetPosType);
            AddHideProperty(targetOffset);
        }
        ShowPropertyVertical(startPosType);
        ShowPropertyVertical(targetPosType);
        ShowPropertyVertical(startTransform);
        ShowPropertyVertical(targetOffset);
        ShowPropertyVertical(fxObj);
        ShowPropertyVertical(isNeverDie);
        if (isNeverDie.boolValue == true)
            AddHideProperty(existTime);
        ShowPropertyVertical(existTime);
    }
}
