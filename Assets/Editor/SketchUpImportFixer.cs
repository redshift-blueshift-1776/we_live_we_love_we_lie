using UnityEditor;
using UnityEngine;

public class SketchUpImportFixer : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        if (!assetPath.EndsWith(".skp")) return;

        ModelImporter importer = (ModelImporter)assetImporter;

        // Only set it if it's still default
        if (Mathf.Approximately(importer.globalScale, 1f))
        {
            importer.globalScale = 3.28084f; // feet to Unity units
        }
    }
}
