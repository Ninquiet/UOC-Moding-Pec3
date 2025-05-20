using UnityEditor;

namespace WhoIsUsingThis.Editor
{
    public static class Settings
    {
        [MenuItem("Tools/Who Is Using This/Enable")]
        private static void EnablePlugin()
        {
            IsPluginEnabled = true;
        }

        [MenuItem("Tools/Who Is Using This/Enable", true)]
        private static bool ValidateEnablePlugin()
        {
            return !IsPluginEnabled;
        }

        [MenuItem("Tools/Who Is Using This/Disable")]
        private static void DisablePlugin()
        {
            IsPluginEnabled = false;
        }
        
        [MenuItem("Tools/Who Is Using This/Disable", true)]
        private static bool ValidateDisablePlugin()
        {
            return IsPluginEnabled;
        }
        
        [MenuItem("Tools/Who Is Using This/Toggle Alignment")]
        private static void ToggleAlignment()
        {
            IsLeftAligned = !IsLeftAligned;
        }
        
        internal static bool IsPluginEnabled
        {
            get
            {
                if (!EditorPrefs.HasKey(PluginPrefsEnabledKey))
                {
                    EditorPrefs.SetBool(PluginPrefsEnabledKey, true);
                }

                return EditorPrefs.GetBool(PluginPrefsEnabledKey);
            }
            set => EditorPrefs.SetBool(PluginPrefsEnabledKey, value);
        }

        internal static bool IsLeftAligned
        {
            get
            {
                if (!EditorPrefs.HasKey(PluginPrefsLeftAlignedKey))
                {
                    EditorPrefs.SetBool(PluginPrefsLeftAlignedKey, false);
                }

                return EditorPrefs.GetBool(PluginPrefsLeftAlignedKey);
            }
            set => EditorPrefs.SetBool(PluginPrefsLeftAlignedKey, value);
        }

        private const string PluginPrefsEnabledKey = "WhoIsUsingThisPluginEnabled";
        private const string PluginPrefsLeftAlignedKey = "WhoIsUsingThisPluginLeftAligned";
    }
}