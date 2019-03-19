namespace Game.Settings {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Sirenix.Serialization;

    [CreateAssetMenu(menuName = "Game Settings/Font Settings", fileName = "Font Settings")]
    public class FontSettings : SerializedScriptableObject {
        [OdinSerialize, ListDrawerSettings(AlwaysAddDefaultValue = true, ShowIndexLabels = true,
            NumberOfItemsPerPage = 3)]
        private List<FontPreset> presets = new List<FontPreset>();

        /// <summary>
        /// Currently using font preset
        /// </summary>
        public FontPreset CurrentFontPreset {
            get {
                return presets[currentFontPresetIndex];
            }
        }

        private int currentFontPresetIndex = 0;

        /// <summary>
        /// Sets the current font preset to the given index (used for loading game settings)
        /// </summary>
        public void AssignFontPreset(int fontIndexInList) {
            currentFontPresetIndex = fontIndexInList;
        }
        /// <summary>
        /// Index in the presets list of the current using font preset
        /// </summary>
        public int IndexOfCurrentFontPreset {
            get {
                return currentFontPresetIndex;
            }
        }

        /// <summary>
        /// A preset of how game fonts should behave.
        /// </summary>
        [System.Serializable]
        public class FontPreset {
            [OdinSerialize]
            public string FontName { get; private set; }

            [OdinSerialize, MinValue(1), MaxValue(50)]
            public float ReadingFontSize { get; private set; }

            [OdinSerialize, MinValue(1), MaxValue(50)]
            public float SmallerFontSize { get; private set; }

            public enum FontSize { ReadingSize, SmallerSize }

            public float GetFontSize(FontSize fontSize) {
                switch (fontSize) {
                    case FontSize.ReadingSize:
                        return ReadingFontSize;
                    case FontSize.SmallerSize:
                        return SmallerFontSize;
                }
                return ReadingFontSize;
            }
        }
    }
}
