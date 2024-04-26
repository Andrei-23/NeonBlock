using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataSaver
{

    private static MapDataSaver _instance;
    public static MapDataSaver Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MapDataSaver();
            }
            return _instance;
        }
    }

    public bool saved = false;
    public List<MapPoint.Type> levelTypes;
    public LevelDataManager.LevelType curLevelDifficulty = LevelDataManager.LevelType.standart;
    public void Reset()
    {
        saved = false;
        levelTypes = new List<MapPoint.Type>();
        curLevelDifficulty = LevelDataManager.LevelType.standart;
    }
}
