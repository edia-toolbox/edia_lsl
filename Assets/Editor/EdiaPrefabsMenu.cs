using UnityEngine;
using UnityEditor;

namespace Edia.Lsl.Editor {
    
}

public class EdiaPrefabsMenu : MonoBehaviour
{
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
        var prefab = Resources.Load<GameObject>($"{prefabName}");
        if (prefab == null) {
            Debug.LogError($"Prefab '{prefabName}' not found in Resources");
            return;
        }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        GameObjectUtility.SetParentAndAlign(instance, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(instance, $"Create {prefabName}");
        Selection.activeObject = instance;
    }
}
