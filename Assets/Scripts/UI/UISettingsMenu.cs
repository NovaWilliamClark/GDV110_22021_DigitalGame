using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

[RequireComponent(typeof(CanvasGroup))]
public class UISettingsMenu : UIMenu, ICancelHandler
{
    public PlayerInput input;
    private UIMainMenu mainMenu;

    [SerializeField] private Button backButton;
    [SerializeField] private Button applyButton;

    public override void Awake()
    {
        input = new PlayerInput();
        mainMenu = FindObjectOfType<UIMainMenu>();
        if (!mainMenu)
        {
            Debug.LogWarning("Can't find main menu");
        }
    }
    
    public override void Start()
    {
        base.Start();
    }

    public override void OnEnable()
    {
        input.UI.Enable();
        input.UI.Accept.performed += OnApplyInput;
        input.UI.Back.performed += OnBackInput;
        backButton.onClick.AddListener(OnBackButton);
        applyButton.onClick.AddListener(OnApplyButton);
    }



    public override void OnDisable()
    {
        input.UI.Disable();
        input.UI.Accept.performed -= OnApplyInput;
        input.UI.Back.performed -= OnBackInput;
        backButton.onClick.RemoveListener(OnBackButton);
        applyButton.onClick.RemoveListener(OnApplyButton);
    }

    private void CloseMenu()
    {
        OnDisable();
        mainMenu.PreviousMenu();
    }

    private void ApplySettings()
    {
        
    }
    
    private void OnBackInput(InputAction.CallbackContext obj)
    {
        CloseMenu();
    }
    
    private void OnBackButton()
    {
        CloseMenu();
    }
    
    private void OnApplyInput(InputAction.CallbackContext obj)
    {
        ApplySettings();
    }
    
    private void OnApplyButton()
    {
        ApplySettings();
    }


    public void OnCancel(BaseEventData eventData)
    {
        CloseMenu();
    }
}