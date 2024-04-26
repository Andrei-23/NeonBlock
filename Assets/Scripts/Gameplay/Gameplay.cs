using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
    [HideInInspector] public PieceManager pieceManager;
    [HideInInspector] public GlassManager glassManager;
    [HideInInspector] public DrawGlass drawGlass;
    [HideInInspector] public Controller controller;
    [HideInInspector] public InteractionManager interactionManager;
    [HideInInspector] public LevelStartInfoManager levelStartInfoManager;
    [HideInInspector] public SceneSwitcher sceneSwitcher;

    [HideInInspector] public LevelDataManager.LevelData level;
    [HideInInspector] public bool started; // played checked level info and pressed start button

    private void Awake()
    {
        // This script and all compomemts are in main camera
        pieceManager = gameObject.GetComponent<PieceManager>();
        glassManager = gameObject.GetComponent<GlassManager>();
        drawGlass = gameObject.GetComponent<DrawGlass>();
        controller = gameObject.GetComponent<Controller>();
        interactionManager = gameObject.GetComponent<InteractionManager>();
        levelStartInfoManager = gameObject.GetComponent<LevelStartInfoManager>();

        sceneSwitcher = GetComponent<SceneSwitcher>();
        RelicEvents.Instance.ResetLevelValues();

        if(MapDataSaver.Instance.curLevelDifficulty == LevelDataManager.LevelType.boss)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.boss_music);
        }
        
        level = LevelDataManager.Instance.GetRandomLevel(MapDataSaver.Instance.curLevelDifficulty);
        Stats.Instance.energy_task += level.task_extra;
        Stats.Instance.turn_limit += level.turns_extra;
        level.laser += RelicsManager.Instance.GetCount(RelicsManager.RelicType.ExtraSpace);
        level.laser += RelicsManager.Instance.GetCount(RelicsManager.RelicType.DeathLaser) * 2;

        PlayerStatEventManager.Instance.CurrentLiveState = PlayerLiveState.Alive;
    }
    void Start()
    {

    }

    public void Launch() // start of gameplay
    {
        started = true;
        HintEventManager.Instance.SetVisibility(false);
        PieceStateManager.Instance.SetState(PieceState.Default);
        drawGlass.Launch();
        pieceManager.Launch();
        levelStartInfoManager.Launch();
    }
    public bool CheckDeleteDirectionTop()
    {
        //bool has_protector = FindBlockType(Block.Type.Protector);
        bool relic = RelicsManager.Instance.IsActive(RelicsManager.RelicType.LaserModule);
        return (relic || level.delete_from_top);
    }

    public void LevelComplete()
    {
        if (RelicsManager.Instance.IsActive(RelicsManager.RelicType.OverchargeModule))
        {
            int val = Mathf.Max(Stats.Instance.energy - Stats.Instance.energy_task, 0);
            PlayerStatEventManager.Instance.AddMoney(val);
        }

        StartCoroutine(waiter(0.7f));
    }
    IEnumerator waiter(float delay)
    {

        yield return new WaitForSeconds(delay);

        Stats.Instance.energy_task -= level.task_extra;
        Stats.Instance.turn_limit -= level.turns_extra;
        Stats.Instance.lvl_cnt++;

        if (level.type == LevelDataManager.LevelType.miniboss)
        {
            Stats.Instance.epic_cnt++;
            Stats.Instance.money += 80;
        }

        Stats.Instance.money += 60 + Stats.Instance.event_cnt * 15;
        //Stats.Instance.energy = 0;

        RelicEvents.Instance.ResetLevelValues();

        HintEventManager.Instance.SetVisibility(true);

        switch (MapDataSaver.Instance.curLevelDifficulty){
            case LevelDataManager.LevelType.standart:
                sceneSwitcher.LoadScene(1);
                break;
            case LevelDataManager.LevelType.miniboss:
                sceneSwitcher.LoadScene(5);
                break;
            case LevelDataManager.LevelType.boss:
                sceneSwitcher.OpenWinScene();
                break;
            default:
                sceneSwitcher.OpenMap();
                break;
        }
    }

}
