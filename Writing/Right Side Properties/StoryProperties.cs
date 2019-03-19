namespace Game.Writing.Properties {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using DG.Tweening;
    using TMPro;
#pragma warning disable 0649 // disable default value warning

    public class StoryProperties : PropertyPanelContent<Story> {
        [SerializeField, Required]
        private TMP_InputField titleInputField, authorField;
        
        protected override void AssignUIListenersToChangeStory() {
            titleInputField.onEndEdit.AddListener(delegate { UpdateTheStory(); });
            authorField.onEndEdit.AddListener(delegate { UpdateTheStory(); });
        }
        
        public override void AssignInitialValues() {
            titleInputField.text = ModifyingPart.Name;
            authorField.text = ModifyingPart.AuthorName;
        }

        protected override void UpdateTheStory() {
            ModifyingPart.Name = titleInputField.text;
            ModifyingPart.AuthorName = authorField.text;
        }
    }
}