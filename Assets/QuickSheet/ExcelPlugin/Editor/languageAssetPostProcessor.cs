using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

///
/// !!! Machine generated code !!!
///
public class languageAssetPostprocessor : AssetPostprocessor 
{
    private static readonly string filePath = "Assets/GameParty/Resources/player_language.xls";
    private static readonly string assetFilePath = "Assets/GameParty/Resources/language.asset";
    private static readonly string sheetName = "language";
    
    static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets) 
        {
            if (!filePath.Equals (asset))
                continue;
                
            language data = (language)AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(language));
            if (data == null) {
                data = ScriptableObject.CreateInstance<language> ();
                data.sheetName = filePath;
                data.worksheetName = sheetName;
                AssetDatabase.CreateAsset ((ScriptableObject)data, assetFilePath);
                //data.hideFlags = HideFlags.NotEditable;
            }
            
            //data.dataArray = new ExcelQuery(filePath, sheetName).Deserialize<languageData>().ToArray();		

            //ScriptableObject obj = AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(ScriptableObject)) as ScriptableObject;
            //EditorUtility.SetDirty (obj);

            ExcelQuery query = new ExcelQuery(filePath, sheetName);
            if (query != null && query.IsValid())
            {
                data.dataArray = query.Deserialize<languageData>().ToArray();
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(ScriptableObject)) as ScriptableObject;
                EditorUtility.SetDirty (obj);
            }
        }
    }
}
