using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AimAssist.Editor
{
    [CustomEditor(typeof(AimAssistInput))]
    public class AimAssistEditor : UnityEditor.Editor
    {
        private Collider2D[] _targets = new Collider2D[32]; 
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            var rect = EditorGUILayout.GetControlRect(false, 300);
            
            EditorGUI.DrawRect(rect, Color.black);
                
            float yMin = 0; 
            float yMax = 1;
            rect.xMin += EditorGUIUtility.singleLineHeight/2f;
            rect.xMax -= EditorGUIUtility.singleLineHeight/2f;
            rect.yMin += EditorGUIUtility.singleLineHeight/2f;
            rect.yMax -= EditorGUIUtility.singleLineHeight/2f;

            if (rect.width < 0)
                return;
            float step = 1 / rect.width;
            
            EditorGUI.DrawRect(rect, new Color(0.025f, 0.025f, 0.025f));

            var xPositions = new float[Mathf.FloorToInt(rect.width)];
            for (int i = 0; i < xPositions.Length; i++)
                xPositions[i] = i / rect.width;
            
            var aimAssist = (AimAssistInput)target;
            
            Vector2 prevPos = new Vector2(0, 0);
        
            int numTargets = Physics2D.OverlapCircleNonAlloc(aimAssist.transform.position, aimAssist.Radius, _targets);
            for (float t = step; t < 1; t += step) {
                Vector3 pos = new Vector3(t,  aimAssist.GetPointOnGraph(t, false, _targets, numTargets), 0);
                Handles.DrawLine(
                    new Vector3(rect.xMin + prevPos.x * rect.width, rect.yMax - ((prevPos.y - yMin) / (yMax - yMin)) * rect.height, 0), 
                    new Vector3(rect.xMin + pos.x * rect.width, rect.yMax - ((pos.y - yMin) / (yMax - yMin)) * rect.height, 0));

                prevPos = pos;
            }
        }

        public static GUIStyle Style(Color color, bool stretchWidth = false, RectOffset padding = null)
        {
            var currentStyle = new GUIStyle(GUI.skin.box) {border = new RectOffset(-2, -2, -2, -2)};

            var pix = new Color[1];
            pix[0] = color;
            var bg = new Texture2D(1, 1);
            bg.SetPixels(pix);
            bg.Apply();


            currentStyle.normal.background = bg;
            currentStyle.stretchWidth = stretchWidth;
            if (padding != null)
                currentStyle.padding = padding;
            return currentStyle;
        }

        public static GUIStyle Style(Color color, float width, RectOffset padding = null)
        {
            var currentStyle = new GUIStyle(GUI.skin.box) {border = new RectOffset(-2, -2, -2, -2)};

            var pix = new Color[1];
            pix[0] = color;
            var bg = new Texture2D(1, 1);
            bg.SetPixels(pix);
            bg.Apply();

            currentStyle.normal.background = bg;
            currentStyle.fixedWidth = width;
            if (padding != null)
                currentStyle.padding = padding;
            return currentStyle;
        }

        public static GUIContent Content(Color color)
        {
            var pix = new Color[1];
            pix[0] = color;
            var bg = new Texture2D(1, 1);
            bg.SetPixels(pix);
            bg.Apply();
        
            return new GUIContent(bg);
        }
    }
}