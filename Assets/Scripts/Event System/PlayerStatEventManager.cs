using System;
using UnityEngine;
using Relic = RelicsManager.RelicType;

public class PlayerStatEventManager
{
    private static PlayerStatEventManager _instance;
    public static PlayerStatEventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerStatEventManager();
            }
            return _instance;
        }
    }
    private PlayerStatEventManager()
    {

    }

    public delegate void MoneyChangeHandler(int amount);
    public event MoneyChangeHandler OnMoneyIncrease;
    public event MoneyChangeHandler OnMoneyDecrease;

    public delegate void HealthChangeHandler(int amount);
    public event HealthChangeHandler OnHealthIncrease;
    public event HealthChangeHandler OnHealthDecrease;

    public delegate void EnergyChangeHandler(int amount);
    public event EnergyChangeHandler OnEnergyIncrease;

    public delegate void ComboChangeHandler();
    public event ComboChangeHandler OnComboChange;

    public delegate void TestModeChangeHandler(bool isActive);
    public event TestModeChangeHandler OnTestModeChange;

    public delegate void TurnCountHandler();
    public event TurnCountHandler OnTurnCountChanged;

    public Action<int> OnPieceAdd;
    public Action OnInventoryChanged;
    //public delegate void InventoryHandler(int id);
    //public event InventoryHandler OnPieceAdd;
    //public event InventoryHandler OnInventoryChanged;

    public PlayerLiveState CurrentLiveState { get; set; }
    public delegate void DeathHandler();
    public event DeathHandler OnDeath;
    
    private Stats stats => Stats.Instance;

    /// <summary>
    /// add/substract amount to money in Stats
    /// </summary>
    public void AddMoney(int amount)
    {
        if (stats.money + amount < 0)
        {
            amount = stats.money;
        }
        if(amount > 0)
        {
            AudioManager.Instance.PlayCoinSound();
        }
        stats.money += amount;
        
        if (amount > 0) OnMoneyIncrease?.Invoke(amount);
        if (amount < 0) OnMoneyDecrease?.Invoke(-amount);
    }

    /// <summary>
    /// Deal [amount] damage, amount > 0;
    /// </summary>
    public void DamagePlayer(int amount)
    {
        if(amount > 0)
        {
            stats.hp -= amount;
            stats.total_damage += amount;

            //int x = RelicsManager.Instance.IsActive(Relic.Geometry) ? 2 : 1;
            RelicsManager.Instance.IncreaseValue(Relic.Triangle, 1);
            
            OnHealthDecrease?.Invoke(amount);
            
            if (stats.hp <= 0)
            {
                KillPlayer();
            }
        }
    }

    /// <summary>
    /// Heal [amount] damage, amount > 0;
    /// </summary>
    public void HealPlayer(int amount)
    {
        if (amount > 0)
        {
            int dh = stats.max_hp - stats.hp;
            if (dh < amount)
            {
                amount = dh;
            }

            //int x = RelicsManager.Instance.IsActive(Relic.Geometry) ? 2 : 1;
            RelicsManager.Instance.IncreaseValue(Relic.Circle, amount);

            stats.hp += amount;
            OnHealthIncrease?.Invoke(amount);
        }
    }

    public void AddHealth(int amount)
    {
        if (amount > 0) HealPlayer(amount);
        if (amount < 0) DamagePlayer(-amount);
    }

    /// <summary>
    /// Call OnDeath() event, game ends.
    /// </summary>
    public void KillPlayer()
    {
        CurrentLiveState = PlayerLiveState.Dead;
        OnDeath?.Invoke();
    }

    //public void PoisonDamageEnemy()
    //{
    //    int amount = Stats.Instance.poison;
    //    if (amount <= 0)
    //    {
    //        return;
    //    }
    //    Stats.Instance.poison--;
    //    Stats.Instance.energy -= amount;
    //    OnEnergyIncrease?.Invoke(amount);
    //}


    public void GainEnergy(float value)
    {
        //Debug.Log(value);
        if(value <= 0)
        {
            return;
        }

        float mult = 1.0f;
        int amount = (int)(Mathf.Ceil(value * mult));
        stats.energy += amount;
        stats.total_energy += amount;
        OnEnergyIncrease?.Invoke(amount);
    }

    public void SetTestMode(bool isActive)
    {
        stats.test_mode = isActive;
        OnTestModeChange?.Invoke(isActive);
    }

    public void MakeTurn()
    {
        stats.turn_cnt += 1;
        stats.total_turns += 1;
        OnTurnCountChanged?.Invoke();
    }
    public void SetTurnValue(int value)
    {
        stats.turn_cnt = value;
        OnTurnCountChanged?.Invoke();
    }

    public void AddCombo(float value)
    {
        stats.combo += value;
        OnComboChange?.Invoke();
    }

    public void ResetCombo()
    {
        SetCombo(0f);
    }
    public void SetCombo(float combo)
    {
        stats.combo = combo;
        OnComboChange?.Invoke();
    }

    public void EnterMapPoint()
    {
        stats.event_cnt++;
        //if (stats.event_cnt == 0) return; // first point
        UpdateEnergyTask();
    }

    public void AddPiece(int id)
    {
        OnPieceAdd?.Invoke(id);
        OnInventoryChanged?.Invoke();
    }

    public void UpdateEnergyTask()
    {
        stats.energy_task = stats.levelTasks[stats.event_cnt];
    }
}
