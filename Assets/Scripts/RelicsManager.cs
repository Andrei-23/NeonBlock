using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using static BlockDataManager;

public class RelicsManager : MonoBehaviour
{
    public static RelicsManager Instance;

    public enum RelicType
    {
        Null = 0, // empty (used in hint code)
        LED, // +5% energy
        Battery, // +1 hp each line clear
        Discount, // - costs in shop
        Telescope, // +1 piece in queue
        Trashcan, // remove hold piece after 5 turns
        BatterySlot, // + enregy when hold piece, can't hold if already holding
        Resistor, // - overflow damage
        Transistor, // + clear energy 
        
        LaserModule, // lines removed from top on overflow
        ExtraSpace, // laser 1 block higher

        Drone, // - speed
        ExtraWeight, // + speed, + energy

        Detonator, // + explotion damage
        ShockModule, // ++ explotion damage, - line damage
        
        NetworkUpgrade, // + energy for each block in glass
        LaserRoulette, // + energy for blocks under laser

        RecyclingProtocol, // +++ energy, - energy for each block
        CleanContract, // +$ if matrix(glass) is clear

        ComboUpgrade, // + energy for combo
        ParallelModule, // + energy for clearing multiple lines
        OverchargeModule, // extra $ for extra energy

        Timer, // + turns before laser move
        DeathLaser // +laser height, +turns, death on overflow
    } 

    [System.Serializable]
    public class RelicData
    {
        //public RelicType type;
        public Sprite sprite;
        public int max_count = -1; // limit of amount, -1 means infinite
        public string name = string.Empty;
        public string description = string.Empty;
        public bool show_value = false;

        //public RelicData() { 
        //    count = 0;
        //    max_count = 1;
        //    name = string.Empty;
        //    description = string.Empty;
        //}
    }

    [SerializeField] public EnumMap<RelicType, RelicData> relicDataMap;
    //public List<RelicData> relicDataInput;
    [HideInInspector] public List<RelicData> relicData;
    [HideInInspector] public int count; // total amount of relic types


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        count = Enum.GetNames(typeof(RelicType)).Length;
        relicData = new List<RelicData>();
        for(int i = 0; i < count; i++)
        {
            relicData.Add(new RelicData());
        }

        foreach (RelicType type in Enum.GetValues(typeof(RelicType)))
        {
            int id = (int)type;
            relicData[id] = relicDataMap[type];
            if (relicData[id].name == string.Empty)
            {
                relicData[id].name = EnumToString(type);
            }
        }

    }

    void OnValidate()
    {
        relicDataMap.TryRevise();
    }

    void Reset()
    {
        relicDataMap.TryRevise();
    }

    private string EnumToString(RelicType type)
    {
        string str = Enum.GetName(type.GetType(), type);
        if(str != string.Empty)
        {
            char[] letters = str.ToCharArray();
            letters[0] = char.ToUpper(letters[0]);
            for (int i = 0; i < letters.Length; i++)
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

    public bool IsAvaliable(RelicType type)
    {
        return IsAvaliable((int)type);
    }
    public bool IsAvaliable(int id)
    {
        if (!IsValidId(id)) return false;
        return (relicData[id].max_count < 0) || (Stats.Instance.relic_count[id] < relicData[id].max_count);
    }

    // Checks if player has at least one relic of this type
    public bool IsActive(RelicType type)
    {
        return IsActive((int)type);
    }
    public bool IsActive(int id)
    {
        if (!IsValidId(id)) return false;
        return Stats.Instance.relic_count[id] > 0;
    }

    /// <summary>
    /// Choose random a
    /// </summary>
    /// <returns></returns>
    //public int ChooseRandomRelic()
    //{
    //    for(int t = 0; t < 30; t++)
    //    {
    //        int id = UnityEngine.Random.Range(0, relicData.Count);
    //        if (IsAvaliable(id))
    //        {
    //            return id;
    //        }
    //    }
    //    Debug.LogWarning("Can't choose random relic");

    //    for(int i = 0; i < count; i++)
    //    {
    //        if(IsAvaliable(i)) return i;
    //    }
    //    Debug.LogError("Can't choose ANY relic?!");
    //    return 0;
    //}

    public void AddRelic(RelicType type, int amount = 1)
    {
        AddRelic((int)type, amount);
    }
    public void AddRelic(int id, int amount = 1)
    {
        for(int i = 0; i < amount; i++)
        {
            if (!IsAvaliable(id))
            {
                Debug.LogWarning("Can't add relic " + id.ToString());
                return;
            }
            Stats.Instance.relic_count[id]++;
            RelicEvents.Instance.AddRelic((RelicType)id);
        }
    }

    public int GetCount(RelicType type)
    {
        return GetCount((int)type);
    }
    public int GetCount(int id)
    {
        if (!IsValidId(id)) return 0;
        return Stats.Instance.relic_count[id];
    }

    public int GetValue(RelicType type)
    {
        return GetValue((int)type);
    }
    public int GetValue(int id)
    {
        if (!IsActive(id)) return 0;
        return Stats.Instance.relic_value[id];
    }

    public void SetValue(RelicType type, int value)
    {
        SetValue((int)type, value);
    }
    public void SetValue(int id, int value)
    {
        if (!IsActive(id)) return;
        Stats.Instance.relic_value[id] = value;
        RelicEvents.Instance.UpdateRelicInfo((RelicType)id);
    }

    public int IncreaseValue(RelicType type, int amount)
    {
        return IncreaseValue((int)type, amount);
    }
    public int IncreaseValue(int id, int amount)
    {
        SetValue(id, GetValue(id) + amount);
        return GetValue(id);
    }

    public Sprite GetSprite(int id) {
        if (!IsValidId(id)) return null;
        return relicData[id].sprite;
    }
    public string GetName(RelicType type)
    {
        return relicData[(int)type].name;
    }
    public string GetDescription(RelicType type)
    {
        return relicData[(int)type].description;
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
        for(int t = 0; t < 50; t++)
        {
            int id = UnityEngine.Random.Range(0, count);
            if (IsAvaliable(id)) return id;
        }
        Debug.LogWarning("Failed to choose random aviable relic");

        for(int i = 0; i < count; i++)
        {
            if (IsAvaliable(i)) return i;
        }
        Debug.LogError("Failed to choose any aviable relic");
        return 0;
    }
}
