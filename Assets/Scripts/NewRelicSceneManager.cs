using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRelicSceneManager : MonoBehaviour
{
    public List<ChooseRelicPanel> options;

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        foreach (ChooseRelicPanel panel in options)
        {
            panel.gameObject.SetActive(true);
        }
        for (int i = 0; i < options.Count; i++)
        {
            // if relic repeats, generate again
            bool b1 = false;
            for (int it = 0; it < 50; it++)
            {
                options[i].Generate();
                bool b2 = false;
                for (int j = 0; j < i; j++)
                {
                    if (options[i].relic_type == options[j].relic_type)
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
                Debug.LogError("Failed to generate new relics");
            }
        }

    }
}
