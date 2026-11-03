using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AdjustTransform))]
public class AdjustTransformEditor : SelfAdaptionPropertyDrawer
{
    public override string GetSpreadName(SerializedProperty property)
    {
        return "AdjustTransform";
    }
    public override void InitialProperty(SerializedProperty property)
    {
        SerializedProperty scale = property.FindPropertyRelative("scale");
        scale.vector3Value = new Vector3(1, 1, 1);
    }
    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty offset = property.FindPropertyRelative("offset");
        SerializedProperty rotation = property.FindPropertyRelative("rotation");
        SerializedProperty scale = property.FindPropertyRelative("scale");

        ShowPropertyVertical(offset);
        ShowPropertyVertical(rotation);
        ShowPropertyVertical(scale);
    }
}
