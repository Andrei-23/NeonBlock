using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MapPoint : MonoBehaviour
{
    public enum Type
    {
        none = 0,
        start,
        boss,

        level,
        danger_level,
        shop,

        random, // level/danger_level/shop/unknown
        unknown, // question mark, one of types below
        unknown_piece,
        unknown_relic,
        unknown_delete,
        unknown_level,
    }

    public GameObject lineRendererPrefab;
    public List<Type> availableTypes;
    public List<GameObject> nextPoints;
    [HideInInspector] public Type type = Type.none;
    [HideInInspector] public int id = 0;

    private Camera cam;
    private SceneSwitcher sceneSwitcher;
    private MapManager mapManager;
    [HideInInspector] public bool is_cur_lvl = false;

    public static Type chosen_type = Type.none;

    private void Awake()
    {
        cam = Camera.main;
        mapManager = cam.GetComponent<MapManager>();
        sceneSwitcher = cam.GetComponent<SceneSwitcher>();
    }
    void Start()
    {

    }

    private void OnDrawGizmos()
    {
        foreach(var go in nextPoints)
        {
            Gizmos.DrawLine(transform.position, go.transform.position);
        }
    }
    public void Setup(int id_, Type type_)
    {
        type = type_;
        id = id_;
        Setup();
    }
    public void Setup(int id_)
    {
        id = id_;
        if(availableTypes.Count == 0)
        {
            type = Type.level;
        }
        type = availableTypes[UnityEngine.Random.Range(0, availableTypes.Count)];
        Setup();
    }
    public void Setup()
    {
        if (type == Type.random) type = ChooseRandomType();
        if (type == Type.unknown) type = ChooseUnknownType();

        GameObject icon = Instantiate(mapManager.GetIconPrefab(type), transform);
        icon.transform.localPosition = new Vector2(0, 0);

        is_cur_lvl = IsCurrent();
        DrawLines();

        if (is_cur_lvl)
        {
            GetComponent<Image>().color = mapManager.CurrentColor;
        }
    }
    private Type ChooseUnknownType()
    {
        float r = UnityEngine.Random.Range(0f, 1f);
        if (r < 0.3f) return Type.unknown_piece; r -= 0.3f;
        if (r < 0.25f) return Type.unknown_delete; r -= 0.25f;
        if (r < 0.2f) return Type.unknown_relic; //r -= 0.2f;
        return Type.unknown_level;
    }
    private Type ChooseRandomType()
    {
        float r = UnityEngine.Random.Range(0f, 1f);
        if (r < 0.1f) return Type.danger_level; r -= 0.1f;
        if (r < 0.15f) return Type.shop; r -= 0.15f;
        if (r < 0.25f) return Type.unknown; // r -= 0.3f;
        return Type.level;
    }

    bool IsCurrent()
    {
        int cur_id = Stats.Instance.cur_lvl_id;
        if (cur_id == -1 && type == Type.start) return true;
        return cur_id == id;
    }

    void DrawLines()
    {
        foreach (var point in nextPoints)
        {
            GameObject go = Instantiate(lineRendererPrefab);
            MapPointLine mpl = go.GetComponent<MapPointLine>();
            mpl.from = gameObject;
            mpl.to = point;
            mpl.SetGradient(is_cur_lvl);

            if (is_cur_lvl)
            {
                point.GetComponent<Button>().interactable = true;
                point.GetComponent<Image>().color = mapManager.NextColor;
            }
        }
    }
    public void Choose()
    {
        Stats.Instance.cur_lvl_id = id;
        PlayerStatEventManager.Instance.EnterMapPoint();

        chosen_type = type;
        switch(type)
        {
            case Type.shop: sceneSwitcher.LoadScene(2); break;
            case Type.unknown_piece: sceneSwitcher.LoadScene(1); break;
            case Type.unknown_delete: sceneSwitcher.LoadScene(8); break;
            case Type.unknown_relic: sceneSwitcher.LoadScene(5); break;

            case Type.level:
                MapDataSaver.Instance.curLevelDifficulty = LevelDataManager.LevelType.standart;
                sceneSwitcher.OpenLevel(); 
                break;
            case Type.unknown_level:
                MapDataSaver.Instance.curLevelDifficulty = LevelDataManager.LevelType.standart;
                sceneSwitcher.OpenLevel();
                break;
            case Type.danger_level:
                MapDataSaver.Instance.curLevelDifficulty = LevelDataManager.LevelType.miniboss;
                sceneSwitcher.OpenLevel();
                break;
            case Type.boss:
                MapDataSaver.Instance.curLevelDifficulty = LevelDataManager.LevelType.boss;
                sceneSwitcher.OpenLevel();
                break;
            default: Debug.LogError("Unexpected event type"); break;
        }
    }

    void Update()
    {
        
    }
}
