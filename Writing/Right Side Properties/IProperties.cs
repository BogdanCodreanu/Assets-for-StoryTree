namespace Game.Writing.Properties {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using DG.Tweening;
    using TMPro;

    /// <summary>
    /// A property content object. That comes and modifies properties
    /// </summary>
    public interface IPropertyContentForPanel<T> where T : IStoryLogicPart {
        void AssignInitialValues();
        void Initialize(IPropertySpawner<T> spawner);

    }

    /// <summary>
    /// An object that spawns properties
    /// </summary>
    public interface IPropertySpawner<T> where T : IStoryLogicPart {
        T StoryLogicPart { get; }
        void ShowProperties();
    }

    public abstract class PropertyPanelContent<T> : MonoBehaviour, IPropertyContentForPanel<T>
        where T : IStoryLogicPart {

        /// <summary>
        /// Object who holds the stroy part and who spawned this object.
        /// </summary>
        private IPropertySpawner<T> ContainerOfModifierPart { get; set; }
        /// <summary>
        /// The part that is modified by the panel
        /// </summary>
        protected T ModifyingPart { get { return ContainerOfModifierPart.StoryLogicPart; } }
        
        /// <summary>
        /// Assign to every UI element listeners to call UpdateTheStory() when
        /// an element changed it's value.
        /// </summary>
        protected abstract void AssignUIListenersToChangeStory();

        public void Initialize(IPropertySpawner<T> spawner) {
            ContainerOfModifierPart = spawner;
            AssignUIListenersToChangeStory();
        }

        /// <summary>
        /// Assigns all UI elements to match the given story logic part data.
        /// </summary>
        public abstract void AssignInitialValues();

        /// <summary>
        /// Updates the ModifyingPart variables to the UI variables
        /// </summary>
        protected abstract void UpdateTheStory();
    }


}