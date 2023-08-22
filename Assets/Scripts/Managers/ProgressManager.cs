using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serialize;
using Levels;
using UnityEngine.Events;
using System;

public class ProgressManager : MonoBehaviour
{
    public int NumberOfLevels;

    static ProgressManager instance = null;
    private string levelJsonPath = "levels-data.json";

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (LevelsInfo.Levels.Count == 0)
        {
            try
            {
                LevelsInfo.Levels = Serializer.Deserialize<List<Level>>(levelJsonPath);

                if (NumberOfLevels != LevelsInfo.Levels.Count)
                {
                    for (int i = LevelsInfo.Levels.Count; i < (NumberOfLevels - LevelsInfo.Levels.Count) + LevelsInfo.Levels.Count; i++)
                    {
                        LevelsInfo.Levels.Add(new Level(i.ToString(), 0, 0, 0, 0, 0, false));
                    }
                }
            }
            catch
            {
                for (int i = 0; i < NumberOfLevels; i++)
                {
                    LevelsInfo.Levels.Add(new Level(i.ToString(), 0, 0, 0, 0, 0, false));
                }
            }
        }

        ProgressEventManager.SaveData += HandleSaveData;
        ProgressEventManager.RefreshData += HandleRefreshData;
    }

    private void HandleSaveData()
    {
        bool success = Serializer.Serialize(levelJsonPath, LevelsInfo.Levels);
        if (!success) Debug.Log("Unable to save level's progress.");
    }

    private void HandleRefreshData()
    {
        try
        {
            LevelsInfo.Levels = Serializer.Deserialize<List<Level>>(levelJsonPath);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}
