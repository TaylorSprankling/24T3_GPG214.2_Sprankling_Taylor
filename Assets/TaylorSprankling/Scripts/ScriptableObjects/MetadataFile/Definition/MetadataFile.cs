using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMetadataFile", menuName = "Scriptable Objects/MetadataFile", order = 69)]
public class MetadataFile : ScriptableObject
{
    [Header("File Data")]
    public string fileName; // Default file name
    public string associatedFileExtension; // Extension type of the file
    public string version; // Current version of the file
    public string associatedFileLink; // Google drive link for actual file

    [Header("Metadata")]
    public string metadataExtension = ".json"; // Extension type
    public string metadataFileLink; // Google drive link to the meta data

    public string LocalFolderPath => Application.streamingAssetsPath;
    public string LocalFilePath => Path.Combine(LocalFolderPath, fileName + associatedFileExtension);
    public string LocalMetadataFilePath => Path.Combine(LocalFolderPath, fileName + metadataExtension);
    public string RemoteFileDownloadLink => GoogleDriveHelper.ConvertToDirectDownloadLink(associatedFileLink);
    public string DirectMetadataDownloadLink => GoogleDriveHelper.ConvertToDirectDownloadLink(metadataFileLink);

    public void CheckLocalVersion()
    {
        version = string.Empty;

        if (File.Exists(LocalMetadataFilePath))
        {
            // Read and store metadata content
            string localMetadataContent = File.ReadAllText(LocalMetadataFilePath).ToString();

            // Parse from Json
            MetadataFileContents localMetadata = JsonUtility.FromJson<MetadataFileContents>(localMetadataContent);

            //Debug.Log("Local metadata found");

            if (localMetadata != null)
            {
                // Grab local version
                version = localMetadata.version;
            }

            // Delete local metadata as it must be downloaded from Google Drive to check version each time
            File.Delete(LocalMetadataFilePath);
        }
        else
        {
            Debug.Log($"No metadata found for {fileName}{associatedFileExtension}, updating required");
            version = "-1";
        }
    }

    public bool FileNeedsUpdating()
    {
        Debug.Log($"Checking local metadata file path at: {LocalMetadataFilePath}");

        // Check if the metadata file exists
        if (!File.Exists(LocalMetadataFilePath))
        {
            Debug.Log("Metadata file does not exist, updating required");
            return true;
        }

        // Read metadata content
        string metadataContent = File.ReadAllText(LocalMetadataFilePath);

        // Check if metadata is empty
        if (string.IsNullOrEmpty(metadataContent))
        {
            Debug.Log("Metadata file is empty, updating required");
            return true;
        }

        // Parse from Json
        MetadataFileContents remoteMetadata = JsonUtility.FromJson<MetadataFileContents>(metadataContent);

        // Check if parse was successful
        if (remoteMetadata == null)
        {
            Debug.LogError("Failed to parse metadata content, updting required");
            return true;
        }

        // Compare versions
        if (version != remoteMetadata.version)
        {
            Debug.Log($"New version detected: {remoteMetadata.version}. Update from {version} required");
            version = remoteMetadata.version; // Show remote version
            return true;
        }

        // If we made it this far then the file probably doesn't need updating!! :)
        Debug.Log($"Local file {fileName}{associatedFileExtension} is up to date");
        return false;
    }

    public void DeleteLocalFile()
    {
        if (File.Exists(LocalFilePath))
        {
            File.Delete(LocalFilePath);
            Debug.Log($"Deleting outdated file at: {LocalFilePath}");
        }
    }
}
