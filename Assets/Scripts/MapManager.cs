using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MapManager : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject ScrollObject;
    public GameObject ContentObject;

    public MapBGMovement mbgm;

    //public Sprite defaultSprite;
    //public Sprite startSprite;
    //public Sprite levelSprite;
    //public Sprite dangerSprite;
    //public Sprite bossSprite;
    //public Sprite shopSprite;
    //public Sprite unknownSprite;

    public GameObject defaultIcon;
    public GameObject startIcon;
    public GameObject shopIcon;
    public GameObject unknownIcon;
    public GameObject levelIcon;
    public GameObject epicIcon;
    public GameObject bossIcon;

    public Gradient highlightGradient;
    public Gradient defaultGradient;

    public UnityEngine.Color CurrentColor;
    public UnityEngine.Color NextColor;

    private List<MapPoint> mapPoints;
    void Start()
    {
        MapDataSaver mds = MapDataSaver.Instance;
        ContentObject.transform.SetParent(ScrollObject.transform, false);

        mapPoints = new List<MapPoint>();
        foreach(MapPoint mp in ContentObject.GetComponentsInChildren<MapPoint>())
        {
            mapPoints.Add(mp);
        }

        int id = 0;
        if (mds.saved)
        {
            foreach (MapPoint point in mapPoints)
            {
                point.Setup(id, mds.levelTypes[id]);
                id++;
            }
        }
        else
        {
            mds.levelTypes = new List<MapPoint.Type>();
            foreach (MapPoint point in mapPoints)
            {
                point.Setup(id++);
                mds.levelTypes.Add(point.type);
            }
            mds.saved = true;
        }

        foreach(MapPoint point in mapPoints)
        {
            if (point.is_cur_lvl)
            {
                float f = (point.transform.localPosition.x - Screen.width / 2f) / (scrollRect.content.rect.width - Screen.width);
                scrollRect.horizontalNormalizedPosition = Mathf.Max(0, Mathf.Min(1, f));
                break;
            }
        }
        mbgm.FollowScroll();
    }

    public GameObject GetIconPrefab(MapPoint.Type type)
    {
        switch (type)
        {
            case MapPoint.Type.start: return startIcon;
            case MapPoint.Type.level: return levelIcon;
            case MapPoint.Type.danger_level: return epicIcon;
            case MapPoint.Type.boss: return bossIcon;
            case MapPoint.Type.shop: return shopIcon;

            case MapPoint.Type.unknown_piece: return unknownIcon;
            case MapPoint.Type.unknown_delete: return unknownIcon;
            case MapPoint.Type.unknown_relic: return unknownIcon;
            case MapPoint.Type.unknown_level: return unknownIcon;

            default: Debug.LogWarning("unexpected type");  return defaultIcon;
        }
    }
}
