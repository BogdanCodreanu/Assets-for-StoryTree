namespace Game {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Sirenix.Serialization;
#pragma warning disable 0649 // disable default value warning

    [CreateAssetMenu(menuName = "Game/Main Settings")]
    public class MainSettings : SerializedScriptableObject {

        /// <summary>
        /// The text limit that a decision input field should have
        /// </summary>
        [Title("Writing Text Limits")]
        [OdinSerialize, MinValue(0)]
        public int DecisionWritingTextLimit { get; private set; }

        /// <summary>
        /// The text limit that a writing panel input field should have
        /// </summary>
        [OdinSerialize, MinValue(0)]
        public int PanelWritingTextLimit { get; private set; }
    }
}
