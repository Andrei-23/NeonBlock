using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<Block>>;


public class GlassManager : MonoBehaviour
{

    // size of glass
    public static int w = 10;
    public static int h = 14;

    public Matrix glass;

    private Gameplay gameplay;

    public static Matrix CloneMatrix(Matrix other)
    {
        Matrix m = new Matrix(other.Count);
        int i = 0;
        foreach(var line in other)
        {
            m.Add(new List<Block>());
            foreach(var block in line)
            {
                m[i].Add(block.Clone());
            }
            i++;
        }
        return m;
    }
    private void Awake()
    {
        gameplay = GetComponent<Gameplay>();
        glass = CloneMatrix(gameplay.level.glass);
    }

}
