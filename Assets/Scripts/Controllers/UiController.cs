using Assets.Scripts.Services;
using UnityEngine;

public class UiController : MonoBehaviour
{
    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private RectTransform _defaultPanel;

    private UiService _panelService;

    private void Awake()
    {
        _panelService = new UiService(_canvas, _defaultPanel);
    }

    public void OpenPanel(RectTransform uiComponent)
    {
        _panelService.OpenPanel(uiComponent);
    }

    public void GoBack()
    {
        _panelService.GoBack();
    }
}
