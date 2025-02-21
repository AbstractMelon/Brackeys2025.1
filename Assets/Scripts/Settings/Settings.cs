using System;
using System.Collections.Generic;
using UnityEngine;

public enum SettingType
{
    Bool,
    Float,
    Enum
}

public class SettingsManager
{
    private readonly List<Dictionary<string, object>> settings = new List<Dictionary<string, object>>();

    public SettingsManager()
    {
        settings.AddRange(GetSettings());
    }

    public List<Dictionary<string, object>> GetSettings()
    {
        return new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                { "Name", "Resolution" },
                { "Type", SettingType.Enum },
                { "Description", "The resolution of the game." },
                { "DefaultValue", "1920x1080" },
                { "EnumOptions", new List<string> { "1920x1080", "1280x720", "800x600", "640x480" } }
            },
            new Dictionary<string, object>
            {
                { "Name", "FullScreen" },
                { "Type", SettingType.Bool },
                { "Description", "Whether the game is in full screen." },
                { "DefaultValue", false }
            },
            new Dictionary<string, object>
            {
                { "Name", "MusicVolume" },
                { "Type", SettingType.Float },
                { "Description", "The volume of the music." },
                { "DefaultValue", 0.5f },
                { "MinValue", 0f },
                { "MaxValue", 1f }
            },
            new Dictionary<string, object>
            {
                { "Name", "SFXVolume" },
                { "Type", SettingType.Float },
                { "Description", "The volume of the sound effects." },
                { "DefaultValue", 0.5f },
                { "MinValue", 0f },
                { "MaxValue", 1f }
            },
            new Dictionary<string, object>
            {
                { "Name", "Language" },
                { "Type", SettingType.Enum },
                { "Description", "The language of the game." },
                { "DefaultValue", "English" },
                { "EnumOptions", new List<string> { "English", "French", "German", "Italian", "Spanish" } }
            }
        };
    }

    public Dictionary<string, object> GetSetting(string name)
    {
        return settings.Find(s => s["Name"].ToString() == name);
    }

    public void SaveSettings()
    {
        foreach (var setting in settings)
        {
            PlayerPrefs.SetString(setting["Name"].ToString(), setting["DefaultValue"].ToString());
        }
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        foreach (var setting in settings)
        {
            if (PlayerPrefs.HasKey(setting["Name"].ToString()))
            {
                setting["DefaultValue"] = Convert.ChangeType(PlayerPrefs.GetString(setting["Name"].ToString()), setting["Type"] as Type);
            }
        }
    }

    public void ResetSettings()
    {
        foreach (var setting in settings)
        {
            setting["DefaultValue"] = setting["DefaultValue"];
        }
        SaveSettings();
    }
}

