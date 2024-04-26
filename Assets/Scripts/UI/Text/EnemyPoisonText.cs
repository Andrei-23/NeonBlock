using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyPoisonText : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public GameObject poisonPanel;

    private void Start()
    {
        poisonPanel.SetActive(false); // poison was removed :(
    }

    void Update()
    {
        //countText.text = Stats.Instance.poison.ToString();
        //poisonPanel.SetActive(Stats.Instance.poison > 0);
    }

}
