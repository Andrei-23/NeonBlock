using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Localization.Settings;

public class BlockDataManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public Sprite DefaultSprite; // Set for blocks that dont have sprite
    //public Sprite[] sprites;
    public List<KeyWord> KeyWords;
    public List<KeyWord> KeyWordsRus;
    public static BlockDataManager Instance;

    [System.Serializable]
    public class BlockStruct
    {
        public Sprite sprite;
        public int prior;
        public bool isMultiColor; // if false, only white color
        public string blockName; // block name
        public string description; // info about block for desctiption UI
        public string blockNameRUS;
        public string descriptionRUS;
    }

    [System.Serializable]
    public class KeyWord
    {
        public string key;
        public string changedKey;
    }

    [SerializeField] public EnumMap<Block.Type, BlockStruct> TypeDataMap;

    //[SerializeField] private List<BlockStruct> blockDataListUnsorted;
    //public List<BlockStruct> blockDataList;

    [HideInInspector] public List<Sprite> sprites;
    [HideInInspector] public List<int> prior;
    [HideInInspector] public List<bool> isMultiColor;

    private List<string> blockName;
    private List<string> description;

    private List<string> blockNameRUS;
    private List<string> descriptionRUS;

    public static int color_count = 9;
    public List<Color> colors;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            //Debug.Log("Instance already exists");
            Destroy(gameObject);
        }

        int n = System.Enum.GetNames(typeof(Block.Type)).Length;
        sprites = new List<Sprite>();
        prior = new List<int>();
        isMultiColor = new List<bool>();
        blockName = new List<string>();
        description = new List<string>();
        blockNameRUS = new List<string>();
        descriptionRUS = new List<string>();

        for (int i = 0; i < n; ++i)
        {
            sprites.Add(DefaultSprite);
            prior.Add(0);
            isMultiColor.Add(false);
            blockName.Add("");
            description.Add("");
            blockNameRUS.Add("");
            descriptionRUS.Add("");
        }

        foreach(Block.Type type in Enum.GetValues(typeof(Block.Type)))
        {
            int id = (int)type;
            BlockStruct bs = TypeDataMap[type];
            sprites[id] = bs.sprite;
            prior[id] = bs.prior;
            isMultiColor[id] = bs.isMultiColor;
            blockName[id] = bs.blockName;
            blockNameRUS[id] = bs.blockNameRUS;
            description[id] = changeKeyWords(bs.description, 0);
            descriptionRUS[id] = changeKeyWords(bs.descriptionRUS, 1);
        }

    }

    private string changeKeyWords(string text, int lang)
    {
        string res = text;
        if(lang == 0)
        { // eng
            foreach (KeyWord kw in KeyWords)
            {
                res = res.Replace(kw.key, kw.changedKey);
            }
        }
        else
        { // rus
            foreach (KeyWord kw in KeyWordsRus)
            {
                res = res.Replace(kw.key, kw.changedKey);
            }
        }
        return res;
    }

    void OnValidate()
    {
        TypeDataMap.TryRevise();
    }

    void Reset()
    {
        TypeDataMap.TryRevise();
    }
    public string GetBlockName(Block.Type type, int lang)
    {
        char[] name = ((lang == 0) ? blockName[(int)type] : blockNameRUS[(int)type]).ToCharArray();
        if (name.Length == 0)
        {
            name = type.ToString().ToCharArray();
        }
        if (name.Length > 0)
        {
            name[0] = char.ToUpper(name[0]);
        }
        else
        {
            Debug.LogWarning("Empty block name");
        }
        return new string(name);
    }
    public string GetBlockName(Block.Type type)
    {
        int lang = LanguageSettings.GetLanguage();
        return GetBlockName(type, lang);
    }

    public string GetBlockDescription(Block.Type type, int lang)
    {
        char[] text = ((lang == 0) ? description[(int)type] : descriptionRUS[(int)type]).ToCharArray();
        
        if (text.Length > 0)
        {
            text[0] = char.ToUpper(text[0]);
        }
        else
        {
            Debug.LogWarning("Empty description");
        }
        return new string(text);
    }

    public string GetBlockDescription(Block.Type type)
    {
        int lang = LanguageSettings.GetLanguage();
        return GetBlockDescription(type, lang);
    }

    public void SetBlock(GameObject go, Block block)
    {
        int id = (int)block.type;
        if(id < 0 || id >= sprites.Count)
        {
            Debug.LogError("Sprite id out of bounds");
            id = 0;
        }
        go.GetComponent<Image>().sprite = sprites[id];
        if (isMultiColor[id])
        {
            go.GetComponent<Image>().color = colors[block.color];
        }
        else
        {
            go.GetComponent<Image>().color = Color.white;
        }
        go.GetComponent<BlockHintColliderScript>().block = block;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
