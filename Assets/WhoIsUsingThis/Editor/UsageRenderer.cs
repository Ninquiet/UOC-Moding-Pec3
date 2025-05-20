using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace WhoIsUsingThis.Editor
{
    public static class UsageRenderer
    {
        static UsageRenderer()
        {
            BadgeIconDarkMode = Resources.Load<Texture2D>("BadgeDarkMode");
            BadgeIconLightMode = Resources.Load<Texture2D>("BadgeLightMode");
            PinIconDarkMode = Resources.Load<Texture2D>("PinDarkMode");
            PinIconLightMode = Resources.Load<Texture2D>("PinLightMode");
        }

        public static void DrawAsUsing(Rect rect)
        {
            EditorGUI.DrawRect(rect, BackgroundColor);
            DrawBorder(rect, 1, BorderColor);
        }

        public static void DrawBadge(Rect rect)
        {
            const float iconSize = 8;
            EditorGUIUtility.SetIconSize(new Vector2(iconSize, iconSize));
            var offset = new Vector2(-18, 0);
            var iconDrawRect = new Rect(rect.position + offset, Vector2.one * iconSize);

            var iconGUIContent = new GUIContent(BadgeIcon);
            EditorGUI.LabelField(iconDrawRect, iconGUIContent);
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        public static bool DrawNotSelectedPinButton(Rect rect)
        {
            return  DrawPinButton(rect);
        }
        
        public static bool DrawSelectedPinButton(Rect rect)
        {
            var originalPaddingReference = GUI.skin.button.padding;
            var originalPadding =  new RectOffset( originalPaddingReference.left, originalPaddingReference.right, originalPaddingReference.top, originalPaddingReference.bottom);
            GUI.skin.button.padding = new RectOffset(4, 4, 2, 2);
            var isButtonPressed = DrawPinButton(rect);

            GUI.skin.button.padding = originalPadding;
            return isButtonPressed;
        }


        private static bool DrawPinButton(Rect rect)
        {
            var (x, y) = GetPinPosition(rect);
            var position = new Vector2(x, y);
            var r = new Rect(position, Vector2.one * 16);
            return GUI.Button(r, PinIcon);
        }
        
        private static (float x, float y) GetPinPosition(Rect rect)
        {
            var x = Settings.IsLeftAligned ? rect.xMin : rect.xMax - 16;
            var y = rect.yMin;
            return (x, y);
        }

        private static void DrawBorder(Rect rect, float thickness, Color color)
        {
            const int padding = 1;
            rect.xMin += padding;
            rect.xMax -= padding;
            rect.yMin += padding;
            rect.yMax -= padding;
            
            
            DrawLine(rect.xMin, rect.yMin, rect.xMin, rect.yMax, thickness, color);
            DrawLine(rect.xMin, rect.yMax, rect.xMax, rect.yMax, thickness, color);
            DrawLine(rect.xMax, rect.yMax, rect.xMax, rect.yMin, thickness, color);
            DrawLine(rect.xMax, rect.yMin, rect.xMin, rect.yMin, thickness, color);
        }

        private static void DrawLine(float x1, float y1, float x2, float y2, float thickness, Color color)
        {
            var start = new Vector2(x1, y1);
            var end = new Vector2(x2, y2);
            var direction = end - start;
            Vector2 sideDirection = Vector3.Cross(direction, Vector3.forward);
            sideDirection.Normalize();


            var points = new Vector2[4];
            var halfWidth = sideDirection * (thickness / 2f);


            points[0] = start + halfWidth;
            points[1] = start - halfWidth;
            points[2] = end + halfWidth;
            points[3] = end - halfWidth;

            var minX = points.Min(p => p.x);
            var maxX = points.Max(p => p.x);
            var minY = points.Min(p => p.y);
            var maxY = points.Max(p => p.y);
            var rect = Rect.MinMaxRect(minX, minY, maxX, maxY);

            EditorGUI.DrawRect(rect, color);
        }

        private static Color BackgroundColor =>
            EditorGUIUtility.isProSkin ? UsingBackgroundColorDarkMode : UsingBackgroundColorLightMode;

        private static readonly Color UsingBackgroundColorDarkMode = new Color(0.22f, 0.78f, 0.56f, .05f);
        private static readonly Color UsingBackgroundColorLightMode = new Color(0f, 0.51f, 0.37f, .05f);

        private static Color BorderColor =>
            EditorGUIUtility.isProSkin ? UsingBorderColorDarkMode : UsingBorderColorLightMode;

        private static readonly Color UsingBorderColorDarkMode = new Color(0.22f, 0.78f, 0.56f, 1f);
        private static readonly Color UsingBorderColorLightMode = new Color(0f, 0.51f, 0.37f, 1f);

        private static Texture2D BadgeIcon => EditorGUIUtility.isProSkin ? BadgeIconDarkMode : BadgeIconLightMode;
        private static readonly Texture2D BadgeIconDarkMode;
        private static readonly Texture2D BadgeIconLightMode;

        private static Texture2D PinIcon => EditorGUIUtility.isProSkin ? PinIconDarkMode : PinIconLightMode;
        private static readonly Texture2D PinIconDarkMode;
        private static readonly Texture2D PinIconLightMode;

    }
}
#endif