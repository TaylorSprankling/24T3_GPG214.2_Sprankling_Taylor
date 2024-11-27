using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MetadataFile))]
public class MetadataScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MetadataFile metadata = (MetadataFile)target;

        if (GUILayout.Button("Export metadata as Json"))
        {
            ExportToJson(metadata);
            EditorUtility.SetDirty(metadata); // Show something has changed
        }
    }

    private void ExportToJson(MetadataFile metadata)
    {
        MetadataFileContents metadataFile = new MetadataFileContents
        {
            version = metadata.version,
            fileLink = metadata.associatedFileLink
        };

        string json = JsonUtility.ToJson(metadataFile, true);

        Directory.CreateDirectory(Application.streamingAssetsPath);

        File.WriteAllText(metadata.LocalMetadataFilePath, json);

        Debug.Log($"Metadata exported to json at: {metadata.LocalMetadataFilePath}");
    }
}
