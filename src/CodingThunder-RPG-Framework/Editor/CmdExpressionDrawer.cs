#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
    [CustomPropertyDrawer(typeof(CmdExpression))]
    public class CmdExpressionDrawer : PropertyDrawer
    {
        private const int TextAreaHeight = 75;  // Height for the text box (adjustable)
        private const int LabelPadding = 5;     // Padding between the label and the text box
        private const int LineHeight = 16;      // Approximate height for each line of text
        private const float TextAreaWidthPercentage = 0.8f;  // The desired width of the text area (80%)
        private const float LineThickness = 2f; // Thickness for the black border lines
        private const float ThinLineThickness = 1f; // Thickness for the line between label and text box

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Define colors for the lines
            Color blackLineColor = Color.black;
            Color thinLineColor = Color.gray;

            // Draw the top black line
            EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, LineThickness), blackLineColor);

            // Adjust the position for content (move down past the black line)
            position.y += LineThickness + LabelPadding;

            // Find the 'expression' property
            SerializedProperty expressionProperty = property.FindPropertyRelative("expression");

            // Process the string: replace ':' with '\n\n'
            string processedString = expressionProperty.stringValue.Replace(":", "\n\n");

            // Measure the height of the processed string
            int lineCount = processedString.Split(new[] { '\n' }, System.StringSplitOptions.None).Length;
            float labelHeight = lineCount * LineHeight;

            // Draw the label with the processed text
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width, labelHeight), processedString, EditorStyles.wordWrappedLabel);

            // Move the position below the label for the thinner line
            position.y += labelHeight + LabelPadding;

            // Draw a thin line between the label and the text box
            EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, ThinLineThickness), thinLineColor);

            // Move the position down past the thin line
            position.y += ThinLineThickness + LabelPadding;

            // Calculate the new width (80% of original width) and the x position to center it
            float newWidth = position.width * TextAreaWidthPercentage;
            float xOffset = (position.width - newWidth) / 2;

            // Draw the centered and resized text area
            expressionProperty.stringValue = EditorGUI.TextArea(
                new Rect(position.x + xOffset, position.y, newWidth, TextAreaHeight),
                expressionProperty.stringValue,
                EditorStyles.textArea
            );

            // Move the position down past the text area
            position.y += TextAreaHeight + LabelPadding;

            // Draw the bottom black line
            EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, LineThickness), blackLineColor);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Calculate the label height based on the number of lines in the processed string
            SerializedProperty expressionProperty = property.FindPropertyRelative("expression");
            string processedString = expressionProperty.stringValue.Replace(":", "\n\n");
            int lineCount = processedString.Split(new[] { '\n' }, System.StringSplitOptions.None).Length;

            // Return the total height: top line + label + padding + thin line + text area + padding + bottom line
            return LineThickness   // Top black line
                 + (lineCount * LineHeight) // Label height
                 + LabelPadding   // Padding
                 + ThinLineThickness // Thin line between label and text box
                 + LabelPadding   // Padding between thin line and text box
                 + TextAreaHeight // Text box height
                 + LabelPadding   // Padding after the text box
                 + LineThickness; // Bottom black line
        }
    }
}

#endif