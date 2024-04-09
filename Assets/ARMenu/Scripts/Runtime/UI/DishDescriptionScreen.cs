using UnityEngine;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI
{
    public class DishDescriptionScreen : MonoBehaviour
    {
        public UIDocument document;

        public VisualTreeAsset minimizedDishScreen;
        public VisualTreeAsset descriptionDishScreen;

        private Button _minimizeButton;
        private Button _previousButton;
        private Button _nextButton;

        private void Awake()
        {
            _minimizeButton = document.rootVisualElement.Q<Button>("MinimizeButton");
            _previousButton = document.rootVisualElement.Q<Button>("PreviousButton");
            _nextButton = document.rootVisualElement.Q<Button>("NextButton");
        }

        private void OnEnable()
        {
            _minimizeButton.RegisterCallback<ClickEvent>(OnMinimizeButtonClicked);
            _previousButton.RegisterCallback<ClickEvent>(OnPreviousButtonClicked);
            _nextButton.RegisterCallback<ClickEvent>(OnNextButtonClicked);
        }

        private void OnDisable()
        {
            _minimizeButton.UnregisterCallback<ClickEvent>(OnMinimizeButtonClicked);
            _previousButton.RegisterCallback<ClickEvent>(OnPreviousButtonClicked);
            _nextButton.RegisterCallback<ClickEvent>(OnNextButtonClicked);
        }

        private void OnMinimizeButtonClicked(ClickEvent evt)
        {
            Debug.Log("Minimized");
        }

        private void OnPreviousButtonClicked(ClickEvent evt)
        {
            Debug.Log("Previous");
        }

        private void OnNextButtonClicked(ClickEvent evt)
        {
            Debug.Log("Next");
        }
    }
}
