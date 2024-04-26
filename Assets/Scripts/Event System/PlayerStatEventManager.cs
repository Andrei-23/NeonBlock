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

    public delegate void TestModeChangeHandler(bool isActive);
    public event TestModeChangeHandler OnTestModeChange;

    public delegate void TurnCountHandler();
    public event TurnCountHandler OnTurnCountChanged;
    public PlayerLiveState CurrentLiveState { get; set; }
    public delegate void DeathHandler();
    public event DeathHandler OnDeath;

    /// <summary>
    /// add/substract amount to money in Stats
    /// </summary>
    public void AddMoney(int amount)
    {
        if (Stats.Instance.money + amount < 0)
        {
            amount = Stats.Instance.money;
        }
        if(amount > 0)
        {
            AudioManager.Instance.PlayCoinSound();
        }
        Stats.Instance.money += amount;
        
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
            Stats.Instance.hp -= amount;
            Stats.Instance.total_damage += amount;

            OnHealthDecrease?.Invoke(amount);
            
            if (Stats.Instance.hp <= 0)
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
            int dh = Stats.Instance.max_hp - Stats.Instance.hp;
            if (dh < amount)
            {
                amount = dh;
            }
            Stats.Instance.hp += amount;
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
        Debug.Log(value);
        if(value <= 0)
        {
            return;
        }

        float mult = 1.0f;
        if (RelicsManager.Instance.IsActive(Relic.LED))
        {
            mult *= 1.0f + 0.05f * RelicsManager.Instance.GetCount(Relic.LED);
        }
        if (RelicsManager.Instance.IsActive(Relic.BatterySlot))
        {
            mult *= 1.5f;
        }
        mult *= Mathf.Pow(1.15f, RelicsManager.Instance.GetCount(Relic.ExtraWeight));

        int amount = (int)(value * mult);
        Stats.Instance.energy += amount;
        Stats.Instance.total_energy += amount;
        OnEnergyIncrease?.Invoke(amount);
    }

    public void SetTestMode(bool isActive)
    {
        Stats.Instance.test_mode = isActive;
        OnTestModeChange?.Invoke(isActive);
    }

    public void MakeTurn()
    {
        Stats.Instance.turn_cnt += 1;
        Stats.Instance.total_turns += 1;
        OnTurnCountChanged?.Invoke();
    }
    public void SetTurnValue(int value)
    {
        Stats.Instance.turn_cnt = value;
        OnTurnCountChanged?.Invoke();
    }

    public void EnterMapPoint()
    {
        Stats.Instance.event_cnt++;
        if (Stats.Instance.event_cnt == 0) return; // first point

        Stats.Instance.energy_task += 10;
    }
}
