using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates wall of random blocks, used to create cool BG on itchio page :)
/// </summary>
public static class RandomBlockWallGenerator
{

    private static List<Block.Type> extra_types = new List<Block.Type> {
        Block.Type.Coin,
        Block.Type.ExtraEnergy,
        Block.Type.Heal,
        Block.Type.Ghost,
        Block.Type.Sand,
        Block.Type.Bomb,
        Block.Type.Bush,
        Block.Type.Stone,
        Block.Type.Chained,
    };
    private static float default_prob = 0.75f;
    private static int color_cnt = 5;

    public static Piece GenerateWall(int w = 10, int h = 10)
    {
        List<List<Block>> shape = new List<List<Block>> ();
        for(int i = 0; i < h; i++)
        {
            shape.Add(new List<Block> { });
            for (int j = 0; j < w; j++)
            {
                Block b = ChooseRandomBlock();
                shape[i].Add(b);
            }
        }
        return new Piece(shape);
    }

    static private Block ChooseRandomBlock()
    {
        float x = Random.Range(0f, 1f);
        if(x <= default_prob)
        {
            int color = Random.Range(0, color_cnt);
            return new Block(Block.Type.Default, color);
        }
        int i = Random.Range(0, extra_types.Count);
        return new Block(extra_types[i]);
    }
}
