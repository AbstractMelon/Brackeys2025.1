using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenuPanel;
    [SerializeField] private GameObject settingsMenuTemplate;
    [SerializeField] private TMP_Dropdown dropdownPrefab;
    [SerializeField] private TMP_InputField inputFieldPrefab;
    [SerializeField] private Slider sliderPrefab;
    [SerializeField] private Toggle togglePrefab;

    private List<SettingsMenuItem> settingsMenuItems = new List<SettingsMenuItem>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateSettingsMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateSettingsMenu()
    {
        settingsMenuPanel.SetActive(true);

        foreach (var setting in SettingsManager.GetSettings())
        {
            var menuItem = CreateMenuItem(setting);
            settingsMenuItems.Add(menuItem);
        }
    }

    private SettingsMenuItem CreateMenuItem(Settings setting)
    {
        var menuItem = new SettingsMenuItem();

        switch (setting.Type)
        {
            case SettingsType.Bool:
                menuItem.SetToggle(CreateToggle(setting));
                break;
            case SettingsType.Int:
                menuItem.SetSlider(CreateSlider(setting));
                break;
            case SettingsType.Float:
                menuItem.SetSlider(CreateSlider(setting));
                break;
            case SettingsType.String:
                menuItem.SetInputField(CreateInputField(setting));
                break;
            case SettingsType.Enum:
                menuItem.SetDropdown(CreateDropdown(setting));
                break;
        }

        return menuItem;
    }

    private Toggle CreateToggle(Settings setting)
    {
        var toggle = Instantiate(togglePrefab, settingsMenuTemplate.transform);
        toggle.isOn = (bool)setting.GetValue();
        toggle.onValueChanged.AddListener(value => setting.SetValue(value));
        return toggle;
    }

    private TMP_Dropdown CreateDropdown(Settings setting)
    {
        var dropdown = Instantiate(dropdownPrefab, settingsMenuTemplate.transform);
        dropdown.ClearOptions();
        foreach (var option in setting.GetEnumOptions())
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(option));
        }
        dropdown.value = (int)setting.GetValue();
        dropdown.onValueChanged.AddListener(value => setting.SetValue(value));
        return dropdown;
    }

    private Slider CreateSlider(Settings setting)
    {
        var slider = Instantiate(sliderPrefab, settingsMenuTemplate.transform);
        slider.minValue = setting.MinValue is float ? (float)setting.MinValue : 0f;
        slider.maxValue = setting.MaxValue is float ? (float)setting.MaxValue : 1f;
        slider.value = (float)setting.GetValue();
        slider.onValueChanged.AddListener(value => setting.SetValue(value));
        return slider;
    }

    private TMP_InputField CreateInputField(Settings setting)
    {
        var inputField = Instantiate(inputFieldPrefab, settingsMenuTemplate.transform);
        inputField.text = (string)setting.GetValue();
        inputField.onValueChanged.AddListener(value => setting.SetValue(value));
        return inputField;
    }
}

public class SettingsMenuItem
{
    public Toggle Toggle { get; private set; }
    public TMP_Dropdown Dropdown { get; private set; }
    public Slider Slider { get; private set; }
    public TMP_InputField InputField { get; private set; }

    public void SetToggle(Toggle toggle)
    {
        Toggle = toggle;
    }

    public void SetDropdown(TMP_Dropdown dropdown)
    {
        Dropdown = dropdown;
    }

    public void SetSlider(Slider slider)
    {
        Slider = slider;
    }

    public void SetInputField(TMP_InputField inputField)
    {
        InputField = inputField;
    }
}

public enum SettingsType
{
    Bool,
    Int,
    Float,
    String,
    Enum
}

public class Settings
{
    public string Name { get; set; }
    public SettingsType Type { get; set; }
    public string Description { get; set; }
    public object DefaultValue { get; set; }
    public object MinValue { get; set; }
    public object MaxValue { get; set; }
    public List<string> EnumOptions { get; set; }

    public object GetValue()
    {
        return PlayerPrefs.GetString(Name, DefaultValue.ToString());
    }

    public void SetValue(object value)
    {
        PlayerPrefs.SetString(Name, value.ToString());
    }

    public List<string> GetEnumOptions()
    {
        return EnumOptions;
    }
}

public static class SettingsManager
{
    public static List<Settings> GetSettings()
    {
        return new List<Settings>
        {
            new Settings
            {
                Name = "Resolution",
                Type = SettingsType.Enum,
                Description = "The resolution of the game.",
                DefaultValue = "1920x1080",
                EnumOptions = new List<string> { "1920x1080", "1280x720", "800x600", "640x480" }
            },
            new Settings
            {
                Name = "FullScreen",
                Type = SettingsType.Bool,
                Description = "Whether the game is in full screen.",
                DefaultValue = false
            },
            new Settings
            {
                Name = "MusicVolume",
                Type = SettingsType.Float,
                Description = "The volume of the music.",
                DefaultValue = 0.5f,
                MinValue = 0f,
                MaxValue = 1f
            },
            new Settings
            {
                Name = "SFXVolume",
                Type = SettingsType.Float,
                Description = "The volume of the sound effects.",
                DefaultValue = 0.5f,
                MinValue = 0f,
                MaxValue = 1f
            },
            new Settings
            {
                Name = "Language",
                Type = SettingsType.Enum,
                Description = "The language of the game.",
                DefaultValue = "English",
                EnumOptions = new List<string> { "English", "French", "German", "Italian", "Spanish" }
            }
        };
    }
}

