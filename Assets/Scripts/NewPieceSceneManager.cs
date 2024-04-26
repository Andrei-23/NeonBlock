using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewPieceSceneManager : MonoBehaviour
{
    public List<ChoosePieceOption> options;

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        for (int i = 0; i < options.Count; i++)
        {
            // if piece repeats, generate again
            bool b1 = false;
            for (int it = 0; it < 100; it++)
            {
                options[i].Generate();
                bool b2 = false;
                for (int j = 0; j < i; j++)
                {
                    if (options[i].figure_id == options[j].figure_id)
                    {
                        b2 = true;
                        break;
                    }
                }
                if (b2) continue;
                b1 = true;
                break;
            }
            if (!b1)
            {
                Debug.LogError("Failed to generate new pieces");
            }
        }

    }
}
