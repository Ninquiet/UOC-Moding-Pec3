using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace WhoIsUsingThis.Editor
{
    [InitializeOnLoad]
    public class CheckUsage
    {
        static CheckUsage()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUIHandler;
            EditorApplication.hierarchyChanged += HierarchyChangedHandler;
        }

        private static void Reset()
        {
            IsUsingOtherObjectLut.Clear();
            _usageCheckedObject = null;
        }

        private static void HierarchyChangedHandler()
        {
            Reset();
        }

        private static void HierarchyWindowItemOnGUIHandler(int instanceId, Rect selectionRect)
        {
            if (!Settings.IsPluginEnabled)
                return;

            var currentObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (!currentObject)
                return;

            if (selectionRect.Contains(Event.current.mousePosition) &&
                UsageRenderer.DrawNotSelectedPinButton(selectionRect))
            {
                if (_usageCheckedObject == currentObject)
                {
                    Reset();
                }
                else
                {
                    _usageCheckedObject = currentObject;
                    CheckUsageForObject(_usageCheckedObject);
                }
            }

            if (!_usageCheckedObject)
                return;


            if (IsUsingOtherObjectLut.TryGetValue(instanceId, out var value))
            {
                var (isUsing, hasChildUsing) = value;
                if (isUsing)
                {
                    UsageRenderer.DrawAsUsing(selectionRect);
                }

                if (hasChildUsing)
                {
                    UsageRenderer.DrawBadge(selectionRect);
                }                
            }
            
            if (currentObject == _usageCheckedObject)
            {
                UsageRenderer.DrawSelectedPinButton(selectionRect);
            }
        }

        private static void CheckUsageForObject(GameObject gameObject)
        {
            var usageCheckedScene = gameObject.scene;
            var rootGameObjects = usageCheckedScene.GetRootGameObjects();

            IsUsingOtherObjectLut.Clear();
            foreach (var rootGameObject in rootGameObjects)
            {
                var allChildren = rootGameObject.GetComponentsInChildren<Transform>();
                CacheAllUsageAndParentBadgeCheck(allChildren);
            }
        }

        private static void CacheAllUsageAndParentBadgeCheck(IEnumerable<Transform> transforms)
        {
            foreach (var transform in transforms)
            {
                var gameObject = transform.gameObject;
                var isUsing = IsCurrentObjectUsesOther(gameObject, _usageCheckedObject);

                if (isUsing)
                {
                    //mark all parents
                    var parent = transform.parent;
                    while (parent)
                    {
                        if (IsUsingOtherObjectLut.TryGetValue(parent.gameObject.GetInstanceID(), out var value))
                        {
                            IsUsingOtherObjectLut[parent.gameObject.GetInstanceID()] = (value.isUsing, true);
                        }

                        parent = parent.parent;
                    }
                }

                IsUsingOtherObjectLut[gameObject.GetInstanceID()] = (isUsing, false);
            }
        }


        private static bool IsCurrentObjectUsesOther(GameObject current, GameObject other)
        {
            var components = current.GetComponents<Component>();
            var serializedObjects = components
                .Where(c => c)
                .Select(c => new SerializedObject(c));
            foreach (var serializedObject in serializedObjects)
            {
                var iterator = serializedObject.GetIterator();
                while (iterator.NextVisible(true))
                {
                    if (iterator.propertyType != SerializedPropertyType.ObjectReference)
                        continue;

                    switch (iterator.objectReferenceValue)
                    {
                        case GameObject go when go == other:
                        case Component co when co && co.gameObject == other:
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        private static readonly Dictionary<int, ( bool isUsing, bool hasChildUsing)> IsUsingOtherObjectLut =
            new Dictionary<int, (bool, bool)>();

        private static GameObject _usageCheckedObject;
    }
}
#endif