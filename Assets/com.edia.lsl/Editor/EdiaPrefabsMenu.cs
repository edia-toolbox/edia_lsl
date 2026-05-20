using UnityEngine;
using UnityEditor;

namespace Edia.Lsl.Editor {

    public static class EdiaPrefabsMenu {
        [MenuItem("GameObject/EDIA/LSL/Extended-PositionRotationOutlet", false, 0)]
        private static void CreatePrefabExtendedPosRotOutlet(MenuCommand menuCommand) {
            InstantiatePrefab("LSL-Extended-PositionRotationOutlet", menuCommand);
        }

        [MenuItem("GameObject/EDIA/LSL/MarkerOutlet", false, 0)]
        private static void CreatePrefabMarkerOutlet(MenuCommand menuCommand) {
            InstantiatePrefab("LSL-MarkerOutlet", menuCommand);
        }

        [MenuItem("GameObject/EDIA/LSL/CustomFloatOutlet", false, 0)]
        private static void CreatePrefabCustomFloatOutlet(MenuCommand menuCommand) {
            InstantiatePrefab("LSL-CustomFloatOutlet", menuCommand);
        }

        private static void InstantiatePrefab(string prefabName, MenuCommand command) {

            string[] prefabBasePaths = {
                "Packages/com.edia.lsl/Runtime/Prefabs/",
                "Assets/com.edia.lsl/Runtime/Prefabs/"
            };

            GameObject prefab = null;
            string failedPaths = "";

            foreach (var prefabPath in prefabBasePaths) {
                string fullPath = $"{prefabPath}{prefabName}.prefab";
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
                if (prefab != null) {
                    break;
                }
                failedPaths += $" - {prefabPath}\n";
            }

            if (prefab == null) {
                Debug.LogError($"Prefab '{prefabName}' not found. Tried:\n{failedPaths}");
                return;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            GameObjectUtility.SetParentAndAlign(instance, command.context as GameObject);
            Undo.RegisterCreatedObjectUndo(instance, $"Create {prefabName}");
            Selection.activeObject = instance;
        }
    }

}
