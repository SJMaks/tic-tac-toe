using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class UiService
    {
        private readonly Canvas _canvas;
        private Stack<RectTransform> _panelHistory = new Stack<RectTransform>();

        public UiService(Canvas canvas, RectTransform defaultPanel)
        {
            _canvas = canvas;

            if (defaultPanel.parent == _canvas.transform)
            {
                OpenPanel(defaultPanel);
                _panelHistory.Push(defaultPanel);
            }
            else
            {
                Debug.LogWarning("Панель по умолчанию не назначена или не является дочерним объектом Canvas.");
            }
        }

        public void OpenPanel(RectTransform uiComponent)
        {
            if (uiComponent.parent != _canvas.transform)
            {
                Debug.LogWarning($"Компонент {uiComponent.name} не является дочерним компонентом канваса");
                return;
            }

            CloseAll();
            uiComponent.gameObject.SetActive(true);
            _panelHistory.Push(uiComponent);
        }

        public void GoBack()
        {
            if (_panelHistory.Count > 1)
            {
                var currentPanel = _panelHistory.Pop();
                currentPanel.gameObject.SetActive(false);

                var previousPanel = _panelHistory.Peek();
                previousPanel.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Нет предыдущей панели для возврата.");
            }
        }

        private void CloseAll()
        {
            foreach (Transform child in _canvas.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}