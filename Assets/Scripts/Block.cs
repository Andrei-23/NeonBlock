using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : ICloneable
{
    public int w = 1;
    public List<List<Block>> shape; // always square
    public PieceData.Rarity rarity = PieceData.Rarity.Common; // common - legendary

    public Piece(List<List<Block>> shape_, PieceData.Rarity rarity = PieceData.Rarity.Common)
    {
        shape = new List<List<Block>>(shape_);
        w = shape.Count;
        this.rarity = rarity;
    }
    public Piece()
    {
        w = 1;
        shape = new List<List<Block>>
        {
            new List<Block>()
        };
        shape[0].Add(new Block());
    }
    public Piece(Piece other) // clone piece
    {
        w = other.w;
        shape = new List<List<Block>>(other.shape);
        rarity = other.rarity;
    }
    public Piece Clone()
    {
        return (Piece)MemberwiseClone();
    }
    object ICloneable.Clone()
    {
        return Clone();
    }
    public void SetRarity(PieceData.Rarity rarity)
    {
        this.rarity = rarity;
    }
    public List<List<Block>> GetShape(int rot = 0)
    {
        List<List<Block>> res = new List<List<Block>>();
        for (int i = 0; i < w; i++)
        {
            res.Add(new List<Block>(w));
            for (int j = 0; j < w; j++)
            {
                int x, y;
                switch (rot)
                {
                    case 0: x = j; y = i; break;
                    case 1: x = w - 1 - i; y = j; break;
                    case 2: x = w - 1 - j; y = w - 1 - i; break;
                    case 3: x = i; y = w - 1 - j; break;
                    default: x = j; y = i; break;
                }
                res[i].Add(shape[y][x].Clone());
            }
        }
        return res;
    }

    public int CountBlocks(Block.Type type)
    {
        int cnt = 0;
        for(int y = 0; y < w; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (shape[y][x].type == type)
                {
                    cnt++;
                }
            }
        }
        return cnt;
    }
    public bool HasBlock(Block.Type type)
    {
        return CountBlocks(type) > 0;
    }
}

public class Block : ICloneable
{
    public enum Type
    {
        Default,
        Empty,
        LightEmpty,
        Bedrock, // can't destroy/move, permament

        Coin,
        ExtraEnergy, // deal extra damage on clear
        Poison, // (REMOVED) deal 1 poison
        Heal, // heal..
        TempEnergy, // extra dmg if cleared this move, otherwise become default
        TempHeal, // (REMOVED) heal if cleared this move, otherwise become default

        //Strenght, // extra dmg each clear, decreases
        //Shield, // reduce dmg player receive, decreases
        //Regen, // heal every move, decreases

        Ghost, // transparent but counts when clear lines
        Sand, // falls on update
        Bomb, // explodes on clear
        Grass, // (REMOVED) spreads on place horizontaly
        Bush, // spreads on place in 4 directons

        Downloader, // fill empty blocks horizontaly, if cleared this move, dmg
        //Shocker, // ghost -> default
        Mirror, // places on opposite side of glass

        //bad
        Stone, // cant clear
        Metal, // cant destroy
        Fire, // does damage to player
        Cursed, // does huge damage to player
        TempFire, // (REMOVED) does huge damage to player if cleared this move


        // only activate
        Grenade, // explode when placed
        Laser, // delete column when placed

        // These will become empty
        Cleared, // line clear
        Exploded, // explotion
        Removed, // damage line
        RemovedSpeical, // damage line, extra damage

        Chained, // can't rotate piece, block has no effect :|
        Protector, // Damage significantly reduced if this block is in glass
        Cleaner, // Changes adjacent blocks to default on put

        Lock, // only destroyed by key
        Key, // destroy locks

        // acitvate once for block after each placed piece
        Miner, // Gives 1$ each turn
        SolarPanel, // Gives 1 energy each turn
        
        TempBlock, // breaks at the end of the turn

        VirusBlock, // Spreads to adjacent area on update. If area connected to the top, virus gets deleted.

        SaveBlock, // Get 5 energy if this piece hold

    } // locked properties, no items
    
    /*
    Build ideas:
    
    Fast pieces
    Slow pieces
    Less blocks
    More blocks
    Money
    Explosive
    Combos (multiple turns with clears)
    Multiple lines (tetris)
    React combos (e.g. sand)
     */
    public Type type = Type.Default;
    
    public int color = 0;
    public int prior = 0;
    public bool locked = false; // cant rotate piece
    
    //public Item item = Item.Empty;
    //public bool fill = false; // false - no block

    //public bool ghost = false; // can put blocks on it
    //public bool sand = false; // fall down
    //public bool fire = false; // does damage
    //public bool virus = false; // does damage, spreads

    public Block()
    {
        Reset();
    }

    public Block Clone()
    {
        return (Block)MemberwiseClone();
    }
    object ICloneable.Clone()
    {
        return Clone();
    }
    public void Reset()
    {
        //fill = true;

        //ghost = false;
        //sand = false;
        //fire = false;
        //virus = false;
        locked = false;

        type = Type.Default;
        //item = Item.Empty;
    }
    public void Clear()
    {
        Reset();
        //fill = false;
        type = Type.Empty;
    }
    public void Set(Type type)
    {
        this.type = type;
    }
    public bool IsSolid()
    {
        return type != Type.Empty && type != Type.Ghost;
    }
    public bool IsEmpty()
    {
        return type == Type.Empty;
    }

    

    //public Block(bool filled, int newcolor = 0) : this(){
    //    fill = filled;
    //    color = newcolor;
    //}

    public Block(Type type, int color = 0) : this()
    {
        this.color = color;
        this.type = type;
        //this.locked = locked;
    }
    public Block(char c, int color = 0) : this()
    {
        this.color = color;
        //this.locked = locked;
        switch (c)
        {
            case '#':
                break;
            case ' ':
                type = Type.Empty;
                break;
            case '.':
                type = Type.Empty;
                break;

            // Bad blocks
            case 'S':
                type = Type.Stone;
                break;
            case 'M':
                type = Type.Metal;
                break;
            case 'B':
                type = Type.Bedrock;
                break;
            case 'F':
                type = Type.Chained;
                break;

            //other
            case 's':
                type = Type.Sand;
                break;
            case '-':
                type = Type.Ghost;
                break;
            case '$':
                type = Type.Coin;
                break;
            case 'b':
                type = Type.Bomb;
                break;
            case '+':
                type = Type.Bush;
                break;

            default:
                Debug.LogWarning("Unknown symbol");
                //fill = false;
                break;
        }
    }

}