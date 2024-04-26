using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controls all player data (inventory, hp, money...).
/// Only this class must access save files.
/// </summary>
public class Stats
{
    private static Stats _instance;
    public static Stats Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Stats();
            }
            return _instance;
        }
    }

    public int piece_count { get; private set; }
    public int relic_total { get; private set; }
    public List<int> pieces; // inventory
    public List<int> relic_count;
    public List<int> relic_value; // used as counter for some relics

    //public float speed; // time before auto-fall in seconds

    public int money;
    public int hp;
    public int max_hp;

    public int turn_cnt; // amount of avaliable piece resets before laser drop
    public int turn_limit; // if limit was reached, lower the laser, reset cnt

    public int lvl_cnt; // count of complete levels (any)
    public int epic_cnt; // count of complete minibosses
    public int event_cnt; // count of all map points

    public bool test_mode; // if true, show additional functions (heal, rerolls...)

    public int energy;
    public int energy_task;
    //public int poison;

    public int cur_lvl_id; // id of point on map

    // Total stats for results
    public int total_energy;
    //public int total_pixels;
    public int total_damage;
    public int total_turns;
    public DateTime start_time;

    //private bool createSave = false;
    
    Stats()
    {
        pieces = new List<int>(new int[PieceData.Instance.count]);
        relic_count = new List<int>(new int[RelicsManager.Instance.count]);
        relic_value = new List<int>(new int[RelicsManager.Instance.count]);
        Load();
    }

    //public void ResetInitialData()
    //{
    //    turn_cnt = 0;
    //    turn_limit = 10;
    //    start_time = DateTime.UtcNow;
    //}

    public void Reset()
    {
        Debug.Log("Data reset.");

        for (int i = 0; i < PieceData.Instance.count; i++)
        {
            pieces[i] = (i < 7) ? 1 : 0;
        }
        for (int i = 0; i < RelicsManager.Instance.count; i++)
        {
            relic_count[i] = relic_value[i] = 0;
        }
        piece_count = 7;
        relic_total = 0;

        money = 0;
        hp = max_hp = 100;

        turn_cnt = 0;
        turn_limit = 10;

        lvl_cnt = 0;
        epic_cnt = 0;
        event_cnt = -1;

        test_mode = false;

        energy_task = 30;
        energy = 0;

        cur_lvl_id = -1;

        total_energy = 0;
        total_damage = 0;
        total_turns = 0;

        start_time = DateTime.UtcNow;

        //test_mode = false;

        MapDataSaver.Instance.Reset();

        Save();
    }

    public void Load()
    {
        Reset();
    }

    /*
    public void Load()
    {
        if (!createSave)
        {
            Reset();
            return;
        }

        ResetInitialData();

        for (int i = 0; i < PieceData.Instance.count; i++)
        {
            pieces[i] = PlayerPrefs.GetInt("piece" + i.ToString(), 0);
            piece_count += pieces[i];
        }
        for (int i = 0; i < RelicsManager.Instance.count; i++)
        {
            relic_count[i] = PlayerPrefs.GetInt("relic_count" + i.ToString(), 0);
            relic_value[i] = PlayerPrefs.GetInt("relic_value" + i.ToString(), 0);
            relic_total += relic_count[i];
        }

        money = PlayerPrefs.GetInt("money", 0);
        lvl_cnt = PlayerPrefs.GetInt("lvl_cnt", 0);
        epic_cnt = PlayerPrefs.GetInt("epic_cnt", 0);

        hp = PlayerPrefs.GetInt("hp", 100);
        max_hp = PlayerPrefs.GetInt("max_hp", 100);
        energy = PlayerPrefs.GetInt("energy", 30);
        energy_task = PlayerPrefs.GetInt("energy_task", 30);

        speed = PlayerPrefs.GetFloat("speed", 0.8f);

        cur_lvl_id = PlayerPrefs.GetInt("cur_lvl_id", -1);

        test_mode = PlayerPrefs.GetInt("test_mode", 0) == 1;
    }
    */

    public void Save()
    {
        // no saves :)
    }

    /*
    public void Save()
    {
        if(!createSave)
        {
            return;
        }

        Debug.Log("Data saved.");
        for (int i = 0; i < PieceData.Instance.count; i++)
        {
            PlayerPrefs.SetInt("piece" + i.ToString(), pieces[i]);
        }
        for (int i = 0; i < RelicsManager.Instance.count; i++)
        {
            PlayerPrefs.SetInt("relic_count" + i.ToString(), relic_count[i]);
            PlayerPrefs.SetInt("relic_value" + i.ToString(), relic_value[i]);
        }
        PlayerPrefs.SetInt("money", money);
        PlayerPrefs.SetInt("lvl_cnt", lvl_cnt);
        PlayerPrefs.SetInt("epic_cnt", epic_cnt);

        PlayerPrefs.SetInt("hp", hp);
        PlayerPrefs.SetInt("max_hp", max_hp);
        PlayerPrefs.SetInt("energy", energy);
        PlayerPrefs.SetInt("energy_task", energy_task);

        PlayerPrefs.SetFloat("speed", speed);

        PlayerPrefs.SetInt("cur_lvl_id", cur_lvl_id);

        PlayerPrefs.SetInt("test_mode", test_mode ? 1 : 0);
    }
    */


    /// <summary>
    /// Adds/removes pieces in your inventory and updates PlayerPrefs
    /// </summary>
    /// <param name="id">Id of piece</param>
    /// <param name="amount">How much you need to add, use negative numbers to remove pieces</param>
    public void AddPiece(int id, int amount = 1)
    {
        if (amount == 0)
        {
            Debug.LogWarning("Trying to add 0 pieces");
        }
        if (id < 0 || id >= PieceData.Instance.count)
        {
            Debug.LogError("Incorrect id");
            return;
        }
        if(pieces[id] + amount < 0)
        {
            piece_count -= pieces[id];
            pieces[id] = 0;
            Debug.LogWarning("Not enough pieces to remove, amount set to 0");
            return;
        }
        if(piece_count + amount == 0)
        {
            Debug.LogWarning("Can't delete last piece");
            piece_count = 1;
            pieces[id] = 1;
            return;
        }
        pieces[id] += amount;
        piece_count += amount;
    }

    /// <summary>
    /// Randomly chooses piece from player's inventory. 
    /// </summary>
    /// <returns>ID of piece</returns>
    public int GetRandomPiece()
    {
        if(piece_count <= 0)
        {
            Debug.LogError("Empty inventory");
            return 0;
        }
        int x = UnityEngine.Random.Range(0, piece_count);
        for (int i = 0; i < PieceData.Instance.count; i++)
        {
            if (pieces[i] > x)
            {
                return i;
            }
            x -= pieces[i];
        }
        Debug.LogError("Runtime error");
        return 0;
    }

    /// <summary>
    /// Returns piece on [id] position in inventory
    /// </summary>
    /// <returns>Id of piece in pieceData</returns>
    public int GetPieceByPos(int id)
    {
        for(int i = 0; i < pieces.Count; i++)
        {
            id -= pieces[i];
            if(id < 0)
            {
                return i;
            }
        }
        Debug.LogWarning("Cant find this piece");
        return 0;
    }
}
