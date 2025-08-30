using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public int playerLevel;
    public float playerHealth;

    // Player Memory Tilemaps Data
    public bool[][] playerMemory;
}

public static class SaveManager
{
    public static bool loadingData;
    public static int levelLoading;
    private static string saveFilePath = Application.persistentDataPath + "/save.json";
    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved to: " + saveFilePath);
    }

    public static SaveData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data;
        }
        else
        {
            Debug.LogWarning("No save file found!");
            return null;
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
        }
    }
}