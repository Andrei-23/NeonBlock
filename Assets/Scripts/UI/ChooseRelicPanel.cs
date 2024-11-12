using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRelicPanel : MonoBehaviour
{
    public RelicView rv;
    //public GameObject edgePanel;
    private SceneSwitcher sceneSwitcher;

    [HideInInspector] public int relic_type;

    private void Awake()
    {
        sceneSwitcher = Camera.main.GetComponent<SceneSwitcher>();
    }

    public void Generate()
    {
        relic_type = RelicsManager.Instance.GenerateAvaliableRelic();
        rv.SetView((RelicsManager.RelicType)relic_type);
    }
    public void Choose()
    {
        RelicsManager.Instance.AddRelic(relic_type);
        AudioManager.Instance.PlaySound(SoundClip.selectRelic);
        SceneSwitcher.OpenMap();
    }

}
