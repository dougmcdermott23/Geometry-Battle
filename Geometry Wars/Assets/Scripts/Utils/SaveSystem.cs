using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";
    private const string SAVE_EXTENSION = ".txt";

    private static bool isInit = false;

    public static void Init()
    {
        if (!isInit)
        {
            isInit = true;
            // Test if Save Folder exists
            if (!Directory.Exists(SAVE_FOLDER))
            {
                // Create Save Folder
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }
    }

    public static void Save(string fileName, string saveString, bool overwrite)
    {
        Init();

        string saveFileName = fileName;

        if (!overwrite)
        {
            // Make sure save number is unique so it doesn't overwrite a previous save file
            int saveNumber = 1;
            while (File.Exists(SAVE_FOLDER + fileName + "_" + saveNumber + SAVE_EXTENSION))
            {
                saveNumber++;
                saveFileName = fileName + "_" + saveNumber;
            }
        } 

        File.WriteAllText(SAVE_FOLDER + saveFileName + SAVE_EXTENSION, saveString);
    }

    public static string Load(string fileName)
    {
        Init();

        if (File.Exists(SAVE_FOLDER + fileName + SAVE_EXTENSION))
            return File.ReadAllText(SAVE_FOLDER + fileName + SAVE_EXTENSION);
        else
            return null;
    }

    public static string LoadMostRecent()
    {
        Init();

        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);
        FileInfo[] saveFiles = directoryInfo.GetFiles("*" + SAVE_EXTENSION);
        FileInfo mostRecentFile = null;

        // Cycle through all save files and identify the most recent one
        foreach (FileInfo fileInfo in saveFiles)
        {
            if (mostRecentFile == null)
                mostRecentFile = fileInfo;
            else if (fileInfo.LastWriteTime > mostRecentFile.LastWriteTime)
                mostRecentFile = fileInfo;
        }

        if (mostRecentFile != null)
            return File.ReadAllText(mostRecentFile.FullName);
        else
            return null;
    }

    public static void SaveObject(object saveObject)
    {
        SaveObject("save", saveObject, false);
    }

    public static void SaveObject(string fileName, object saveObject, bool overwrite)
    {
        Init();

        string json = JsonUtility.ToJson(saveObject);
        Save(fileName, json, overwrite);
    }

    public static TSaveObject LoadObject<TSaveObject>(string fileName)
    {
        Init();

        string saveString = Load(fileName);

        if (saveString != null)
            return JsonUtility.FromJson<TSaveObject>(saveString);
        else
            return default(TSaveObject);
    }

    public static TSaveObject LoadMostRecentObject<TSaveObject>()
    {
        Init();

        string saveString = LoadMostRecent();

        if (saveString != null)
            return JsonUtility.FromJson<TSaveObject>(saveString);
        else
            return default(TSaveObject);
    }
}
