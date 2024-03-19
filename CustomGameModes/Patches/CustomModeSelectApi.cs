using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace CustomGameModes.Patches
{
    internal class CustomModeButtonData
    {
        public string Name { get; set; }
        public string ButtonText { get; set; }
        public string Description { get; set; }
        public Color32 Color { get; set; }
        public UnityAction ClickEvent { get; set; }
    }

    public class CustomModeSelectApi
    {
        internal static List<CustomModeButtonData> CustomModeButtons { get; set; } = new List<CustomModeButtonData>();

        public static void AddButton(string name, string buttonText, string description, Color32 color, UnityAction onClick)
        {
            CustomModeButtonData buttonData = new CustomModeButtonData()
            {
                Name = name,
                ButtonText = buttonText,
                Description = description,
                Color = color,
                ClickEvent = onClick,
            };

            if (CustomModeButtons == null)
            {
                CustomModeButtons = new List<CustomModeButtonData>();
            }
            CustomModeButtons.Add(buttonData);
        }

    }
}
