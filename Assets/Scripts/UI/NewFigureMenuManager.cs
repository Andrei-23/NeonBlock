using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewFigureMenuManager : MonoBehaviour
{
    private SceneSwitcher sceneSwitcher;
    public List<GameObject> options;

    private int n;
    private List<PieceView> figureViews;
    private List<TextMeshProUGUI> descriptions;
    private void Awake()
    {
        sceneSwitcher = Camera.main.GetComponent<SceneSwitcher>();
        
        n = options.Count;
        figureViews = new List<PieceView>();
        descriptions = new List<TextMeshProUGUI>();
        for(int i = 0; i < n; i++)
        {
            figureViews.Add(options[i].GetComponentInChildren<PieceView>());
            descriptions.Add(options[i].GetComponentsInChildren<TextMeshProUGUI>()[1]);
        }
    }

    void Start()
    {
        
    }

    public void GenerateOptions()
    {
        
    }
    void Update()
    {
        
    }
}
