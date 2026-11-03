using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//对使用自定义的Preview这个属性的Sprite展现缩略图
[CustomPropertyDrawer(typeof(Preview))]
public class AttributesLearn : PropertyDrawer
{
    //调整整体高度
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 64f;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(position, property, label);

        // Preview
        Texture2D previewTexture = GetAssetPreview(property);
        if (previewTexture != null)
        {
            Rect previewRect = new Rect()
            {
                x = position.x + GetIndentLength(position),
                y = position.y + EditorGUIUtility.singleLineHeight,
                width = position.width,
                height = 64
            };
            GUI.Label(previewRect, previewTexture);
        }
        EditorGUI.EndProperty();
    }

    public static float GetIndentLength(Rect sourceRect)
    {
        Rect indentRect = EditorGUI.IndentedRect(sourceRect);
        float indentLength = indentRect.x - sourceRect.x;

        return indentLength;
    }

    Texture2D GetAssetPreview(SerializedProperty property)
    {
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            if (property.objectReferenceValue != null)
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);
                return previewTexture;
            }
            return null;
        }
        return null;
    }
}


