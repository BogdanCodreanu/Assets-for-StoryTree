namespace Game.UIAddition {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Settings;
    using TMPro;
#pragma warning disable 0649 // disable default value warning

    public class TextLimitAssignAndPreview : MonoBehaviour {
        [InfoBox("Assigns a limit of characters to the selected input field and also displays" +
            " it on another text")]

        public enum TextLimit { ForDecision, ForWritingPanel }

        [SerializeField]
        private TextLimit assignedLimit;

        [SerializeField, Required]
        private MainSettings mainSettings;

        [SerializeField, Required]
        private TMP_InputField targetedInputField;

        [SerializeField, Required]
        private TMP_Text visualDisplayLimit;
        
        private void Awake() {
            AddListener();
        }


        private void Start() {
            UpdateDisplayText(targetedInputField.text);
        }

        private void AddListener() {
            targetedInputField.characterLimit = AssignedTextLimit;
            targetedInputField.onValueChanged.AddListener(UpdateDisplayText);
        }

        private void UpdateDisplayText(string currentText) {
            visualDisplayLimit.text = currentText.Length + " / " + AssignedTextLimit;
        }

        private int AssignedTextLimit {
            get {
                if (assignedLimit == TextLimit.ForDecision)
                    return mainSettings.DecisionWritingTextLimit;
                return mainSettings.PanelWritingTextLimit;
            }
        }
    }
}