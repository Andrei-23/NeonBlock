using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Selectable))]
public class ButtonScript : MonoBehaviour, IPointerEnterHandler
{
    private Selectable _selectable;
    
    private void Start()
    {
        _selectable = GetComponent<Selectable>();
        if (GetComponent<Button>() != null)
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }
    }
    private void OnClick()
    {
        AudioManager.Instance.PlaySound(SoundClip.UIclick);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_selectable.interactable)
        {
            AudioManager.Instance.RestartSound(SoundClip.UImouseEnter);
            //EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

}
