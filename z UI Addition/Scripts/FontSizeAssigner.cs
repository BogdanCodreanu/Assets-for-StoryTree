namespace Game.UIAddition {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Settings;
    using TMPro;

    public class FontSizeAssigner : MonoBehaviour {
        [InfoBox("Assings a font size to the current TMP text object, acording to the game settings")]
        [SerializeField, Required]
        private FontSettings fontSettings;
        [SerializeField]
        private FontSettings.FontPreset.FontSize fontSize;
        
        private void Awake() {
            TMP_Text tmpText = GetComponent<TMP_Text>();
            tmpText.fontSize = fontSettings.CurrentFontPreset.GetFontSize(fontSize);
        }
    }
}