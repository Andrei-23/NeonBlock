using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicsList : MonoBehaviour
{
    public GameObject gridObj;
    public GameObject relicPrefab;

    private GridLayoutGroup grid;
    private List<GameObject> objects;

    private void Awake()
    {
        grid = gridObj.GetComponent<GridLayoutGroup>();
        objects = new List<GameObject>();
        RelicEvents.onRelicListUpdate += UpdateList;
    }
    private void OnDestroy()
    {
        RelicEvents.onRelicListUpdate -= UpdateList;
    }
    void Start()
    {
        UpdateList();
    }

    public void UpdateList()
    {
        foreach(GameObject obj in objects)
        {
            Destroy(obj);
        }

        objects = new List<GameObject> ();
        for(int i = 0; i < RelicsManager.Instance.count; i++)
        {
            if (Stats.Instance.relic_count[i] > 0)
            {
                GameObject go = Instantiate(relicPrefab, gridObj.transform);
                RelicView rv = go.GetComponentInChildren<RelicView>();
                objects.Add(go);
                rv.SetView(i);
            }
        }
    }
}
