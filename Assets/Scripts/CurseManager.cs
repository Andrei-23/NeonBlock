using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseManager : MonoBehaviour
{
    public static CurseManager Instance;

    public enum CurseType
    {
        sword = 0,
        apple,
        discount,
        binoculars,
        trashcan,
        battery,

    }

    [System.Serializable]
    public class CurseData
    {
        public CurseType type;
        public Sprite sprite;
        public string name = string.Empty;
        public string description = string.Empty;
        public bool show_value = false;
    }

    public List<CurseData> relicDataInput;
    [HideInInspector] public List<CurseData> curseData;
    [HideInInspector] public int count; // total amount of relic types


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        count = System.Enum.GetNames(typeof(CurseType)).Length;
        curseData = new List<CurseData>();
        for (int i = 0; i < count; i++)
        {
            curseData.Add(new CurseData());
        }

        foreach (CurseData cd in relicDataInput)
        {
            int id = (int)cd.type;
            if (id < 0 || id >= count)
            {
                Debug.LogError("Incorrect enum");
                continue;
            }
            curseData[id] = cd;
            if (cd.name == string.Empty)
            {
                curseData[id].name = EnumToString(cd.type);
            }
        }
    }
    //private void OnDestroy()
    //{
    //    Instance = null;
    //}

    private string EnumToString(CurseType type)
    {
        string str = Enum.GetName(type.GetType(), type);
        if (str != string.Empty)
        {
            char[] letters = str.ToCharArray();
            letters[0] = char.ToUpper(letters[0]);
            for(int i = 0; i < letters.Length; i++)
            {
                if (letters[i] == '_') letters[i] = ' ';
            }
            str = new string(letters);
        }
        return str;
    }

    public bool IsValidId(int id)
    {
        if (id < 0 || id >= count)
        {
            Debug.LogError(id.ToString() + " is wrong relic id");
            return false;
        }
        return true;
    }

    // Checks if player has at least one relic of this type
    public bool IsActive(CurseType type)
    {
        return IsActive((int)type);
    }
    public bool IsActive(int id)
    {
        if (!IsValidId(id)) return false;
        return Stats.Instance.relic_count[id] > 0;
    }


    //public void AddRelic(CurseType type, int amount = 1)
    //{
    //    AddRelic((int)type, amount);
    //}
    //public void AddRelic(int id, int amount = 1)
    //{
    //    for (int i = 0; i < amount; i++)
    //    {
    //        if (IsActive(id))
    //        {
    //            Debug.LogWarning("Can't add relic " + id.ToString());
    //            return;
    //        }
    //        Stats.Instance.relic_count[id]++;
    //        RelicEvents.Instance.AddRelic((CurseType)id);
    //    }
    //}

    //public int GetValue(CurseType type)
    //{
    //    return GetValue((int)type);
    //}
    //public int GetValue(int id)
    //{
    //    if (!IsActive(id)) return 0;
    //    return Stats.Instance.relic_value[id];
    //}

    //public void SetValue(CurseType type, int value)
    //{
    //    SetValue((int)type, value);
    //}
    //public void SetValue(int id, int value)
    //{
    //    if (!IsActive(id)) return;
    //    Stats.Instance.relic_value[id] = value;
    //    RelicEvents.Instance.UpdateRelicInfo((CurseType)id);
    //}

    //public int IncreaseValue(CurseType type, int amount)
    //{
    //    return IncreaseValue((int)type, amount);
    //}
    //public int IncreaseValue(int id, int amount)
    //{
    //    SetValue(id, GetValue(id) + amount);
    //    return GetValue(id);
    //}

    public Sprite GetSprite(int id)
    {
        if (!IsValidId(id)) return null;
        return curseData[id].sprite;
    }





    /// <summary>
    /// Returns id of some random relic
    /// </summary>
    public int GenerateRelic()
    {
        return UnityEngine.Random.Range(0, count);
    }
    public int GenerateAvaliableRelic()
    {
        for (int t = 0; t < 50; t++)
        {
            int id = UnityEngine.Random.Range(0, count);
            if (!IsActive(id)) return id;
        }
        Debug.LogWarning("Failed to choose random aviable relic");

        for (int i = 0; i < count; i++)
        {
            if (!IsActive(i)) return i;
        }
        Debug.LogError("Failed to choose any aviable relic");
        return 0;
    }

}