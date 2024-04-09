using UnityEngine;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI
{
    public class DishDescriptionScreen : MonoBehaviour
    {
        // Element keys
        private const string ScreenRootKey = "RootVisualElement";
        private const string ScrollViewKey = "ScrollView";
        private const string CollapseButtonKey = "CollapseButton";
        private const string PreviousButtonKey = "PreviousButton";
        private const string NextButtonKey = "NextButton";

        // Style keys
        private const string MinimizedStyle = "screen-minimized";

        public UIDocument document;

        // Elements
        private VisualElement _screenRoot;
        private ScrollView _scrollView;
        private Button _switchCollapsedStateButton;
        private Button _previousButton;
        private Button _nextButton;

        // Logic fields
        private bool _isCollapsed;

        private void Awake()
        {
            VisualElement root = document.rootVisualElement;

            _screenRoot = root.Q(ScreenRootKey);
            _scrollView = root.Q<ScrollView>(ScrollViewKey);
            _switchCollapsedStateButton = root.Q<Button>(CollapseButtonKey);
            _previousButton = root.Q<Button>(PreviousButtonKey);
            _nextButton = root.Q<Button>(NextButtonKey);
        }

        private void Start()
        {
            SetCollapsedState(true);
        }

        private void OnEnable()
        {
            _switchCollapsedStateButton.RegisterCallback<ClickEvent>(OnMinimizeButtonClicked);
            _previousButton.RegisterCallback<ClickEvent>(OnPreviousButtonClicked);
            _nextButton.RegisterCallback<ClickEvent>(OnNextButtonClicked);
        }

        private void OnDisable()
        {
            _switchCollapsedStateButton.UnregisterCallback<ClickEvent>(OnMinimizeButtonClicked);
            _previousButton.RegisterCallback<ClickEvent>(OnPreviousButtonClicked);
            _nextButton.RegisterCallback<ClickEvent>(OnNextButtonClicked);
        }

        private void SetCollapsedState(bool state)
        {
            _isCollapsed = state;
            if (state)
            {
                _screenRoot.AddToClassList(MinimizedStyle);
            }
            else
            {
                _screenRoot.RemoveFromClassList(MinimizedStyle);
            }
            _scrollView.SetEnabled(!state);
        }

        private void OnMinimizeButtonClicked(ClickEvent evt)
        {
            SetCollapsedState(!_isCollapsed);
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
