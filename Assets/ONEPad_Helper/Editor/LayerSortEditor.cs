using UnityEngine;
using UnityEditor;
using System.Collections;
#if UNITY_EDITOR
public class OnePadLayerSorterEditor : Editor
{
    [MenuItem("ONEPAD/Layer Sorter/Update Layer Sorter")]
    public static void LayerSorterChecker()
    {
        SpriteRenderer[] spriteRenderers = GameObject.FindObjectsOfType(typeof(SpriteRenderer)) as SpriteRenderer[];
        int count = spriteRenderers.Length;

        bool needUpdate = false;
        for (int i = 0; i < count; i++)
        {
            GameObject srGO = spriteRenderers[i].gameObject;
            OnePadLayerSorter layerSorter = srGO.GetComponent<OnePadLayerSorter>();
            if (layerSorter == null || layerSorter.SortLayerName != spriteRenderers[i].sortingLayerName)
            {
                needUpdate = true;
                break;
            }
        }
        if(needUpdate)
        {
            if (EditorUtility.DisplayDialog("ONEPad Layer Sorter", "You need update Layer sorter.", "Update now", "Avoid"))
            {
                int addCount = AutoAddLayerSorterSilent();
                EditorUtility.DisplayDialog("ONEPad Layer Sorter", addCount.ToString() + " Layer sorter added", "OK");
            }
        }else
        {
            EditorUtility.DisplayDialog("ONEPad Layer Sorter", "Aready All Layer sorter setted", "OK");
        }

    }

    public static int AutoAddLayerSorterSilent()
    {
        OnePadLayerSorterEditor.RemoveAllLayerSorterSilent();
        SpriteRenderer[] spriteRenderers = GameObject.FindObjectsOfType(typeof(SpriteRenderer)) as SpriteRenderer[];
        int count = spriteRenderers.Length;

        for (int i = 0; i < count; i++)
        {
            GameObject srGO = spriteRenderers[i].gameObject;
            OnePadLayerSorter layerSorter = srGO.AddComponent<OnePadLayerSorter>();
            layerSorter.SortLayerName = spriteRenderers[i].sortingLayerName;
        }

        EditorApplication.SaveScene();
        return count;
    }

    [MenuItem("ONEPAD/Layer Sorter/Remove All LayerSorter")]
    public static void RemoveAllLayerSorter()
    {
        OnePadLayerSorter[] layerSorters = GameObject.FindObjectsOfType(typeof(OnePadLayerSorter)) as OnePadLayerSorter[];
        int count = layerSorters.Length;

        if(count == 0)
        {
            EditorUtility.DisplayDialog("ONEPad Layer Sorter", "Any Layer sorter in this scene", "OK");
            return;
        }

        if (EditorUtility.DisplayDialog("ONEPad Layer Sorter", "Remove all Layer sorter? (" + count.ToString() + " layer sorters)", "Yes", "No"))
        {
            for (int i = 0; i < count; i++)
                DestroyImmediate(layerSorters[i]);
            AssetDatabase.Refresh();

            EditorApplication.SaveScene();
            EditorUtility.DisplayDialog("ONEPad Layer Sorter", "All layer sorter removed", "OK");
        }
    }

    public static void RemoveAllLayerSorterSilent()
    {
        OnePadLayerSorter[] layerSorters = GameObject.FindObjectsOfType(typeof(OnePadLayerSorter)) as OnePadLayerSorter[];
        int count = layerSorters.Length;

        for (int i = 0; i < count; i++)
            DestroyImmediate(layerSorters[i]);
        AssetDatabase.Refresh();

        EditorApplication.SaveScene();
    }

}
#endif