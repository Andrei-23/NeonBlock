using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BlockDataManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public Sprite DefaultSprite; // Set for blocks that dont have sprite
    //public Sprite[] sprites;

    public static BlockDataManager Instance;

    [System.Serializable]
    public class BlockStruct
    {
        public Sprite sprite;
        public int prior;
        public bool isMultiColor; // if false, only white color
        public string blockName; // block name
        public string description; // info about block for desctiption UI
    }

    [SerializeField] public EnumMap<Block.Type, BlockStruct> TypeDataMap;

    //[SerializeField] private List<BlockStruct> blockDataListUnsorted;
    //public List<BlockStruct> blockDataList;

    [HideInInspector] public List<Sprite> sprites;
    [HideInInspector] public List<int> prior;
    [HideInInspector] public List<bool> isMultiColor;
    [HideInInspector] public List<string> blockName;
    [HideInInspector] public List<string> description;

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
            Debug.Log("Instance already exists");
            Destroy(gameObject);
        }

        int n = System.Enum.GetNames(typeof(Block.Type)).Length;
        sprites = new List<Sprite>();
        prior = new List<int>();
        isMultiColor = new List<bool>();
        blockName = new List<string>();
        description = new List<string>();

        for (int i = 0; i < n; ++i)
        {
            sprites.Add(DefaultSprite);
            prior.Add(0);
            isMultiColor.Add(false);
            blockName.Add("");
            description.Add("");
        }

        foreach(Block.Type type in Enum.GetValues(typeof(Block.Type)))
        {
            int id = (int)type;
            BlockStruct bs = TypeDataMap[type];
            sprites[id] = bs.sprite;
            prior[id] = bs.prior;
            isMultiColor[id] = bs.isMultiColor;
            blockName[id] = bs.blockName;
            description[id] = bs.description;
        }

    }

    void OnValidate()
    {
        TypeDataMap.TryRevise();
    }

    void Reset()
    {
        TypeDataMap.TryRevise();
    }
    public string GetBlockName(Block.Type type)
    {
        char[] name = blockName[(int)type].ToCharArray();
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
