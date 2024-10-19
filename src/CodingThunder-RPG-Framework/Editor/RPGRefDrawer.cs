using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using CodingThunder.RPGUtilities.DataManagement;

[CustomPropertyDrawer(typeof(RPGRefUnity))]
public class RPGRefDrawer : PropertyDrawer
{
	private string searchFilter = string.Empty; // Store the user's input for filtering

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		var referenceIdProp = property.FindPropertyRelative("ReferenceId");
		var typeNameProp = property.FindPropertyRelative("TypeName");

		// Draw ReferenceId
		var referenceRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		EditorGUI.PropertyField(referenceRect, referenceIdProp, new GUIContent("Reference ID"));

		// Draw Search Field
		var searchRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
		searchFilter = EditorGUI.TextField(searchRect, "Type Filter", searchFilter);

		// Draw Type Selector
		var typeRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 2, position.width, EditorGUIUtility.singleLineHeight);
		var type = System.Type.GetType(typeNameProp.stringValue) ?? typeof(int);

		// Get all available types (you can filter this list as needed)
		List<Type> types = new List<Type> { typeof(int), typeof(float), typeof(string) };
		types.AddRange(typeof(RPGRefUnity).Assembly.GetTypes().Where(t => t.IsClass));// && t.Namespace == "CodingThunder"));

		// Filter types based on the user's input
		var filteredTypes = types
			.Where(t => string.IsNullOrEmpty(searchFilter) || t.Name.ToLower().Contains(searchFilter.ToLower()))
			.ToList();

		var typeNames = filteredTypes.Select(x => x.Name).ToArray();

		int selectedIndex = filteredTypes.IndexOf(type);
		int newIndex = EditorGUI.Popup(typeRect, selectedIndex, typeNames);

		if (newIndex != selectedIndex && newIndex >= 0)
		{
			typeNameProp.stringValue = filteredTypes[newIndex].AssemblyQualifiedName;
		}

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		// Adjust for three lines: Reference ID, Search Filter, and Type Dropdown
		return EditorGUIUtility.singleLineHeight * 3 + 6;
	}
}
