using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(BoostInfo))]
public class BoostInfoPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty price = property.FindPropertyRelative("price");
        SerializedProperty damageAddition = property.FindPropertyRelative("damageAddition");
        SerializedProperty cooldownReducer = property.FindPropertyRelative("cooldownReducer");

        Rect labelPosition = new Rect(position.x, position.y, position.width, position.height);
        GUIContent textLabel = new GUIContent("Boost");
        position = EditorGUI.PrefixLabel(labelPosition, EditorGUIUtility.GetControlID(FocusType.Passive), textLabel);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        float widthSize = Mathf.Clamp(position.width / 4, 40, 100) / 2f;
        float offsetSize = 10;
        float offsetPosX = 30;

        Rect pos1 = new Rect(position.x - offsetPosX, position.y, widthSize +offsetSize, position.height);
        
        Rect pos2 = new Rect(position.x - offsetPosX + widthSize * 2 + 30, position.y, widthSize + offsetSize, position.height);
        Rect pos3 = new Rect(position.x - offsetPosX + widthSize * 4 + 110, position.y, widthSize + offsetSize, position.height);

        // Texto
        GUIContent textoPrecio = new GUIContent("Precio mejora");
        GUIContent textoDanio = new GUIContent("Daño extra");
        GUIContent textoCooldown = new GUIContent("Reducción cooldown");

        EditorGUI.PropertyField(pos1, price, GUIContent.none);
        EditorGUI.PropertyField(pos2, damageAddition, GUIContent.none);
        EditorGUI.PropertyField(pos3, cooldownReducer, GUIContent.none);

        
        Rect pos1_1 = new Rect(pos1.x - 80, position.y, position.width, position.height);
        Rect pos2_1 = new Rect(pos2.x - 65, position.y, position.width, pos2.height);
        Rect pos3_1 = new Rect(pos3.x - 120, position.y, position.width, pos3.height);
        EditorGUI.LabelField(pos1_1, textoPrecio);
        EditorGUI.LabelField(pos2_1, textoDanio);
        EditorGUI.LabelField(pos3_1, textoCooldown);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
