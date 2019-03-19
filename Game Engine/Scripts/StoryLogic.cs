namespace Game {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Xml.Serialization;
    using System.Runtime.Serialization;
    using Writing;

    /// <summary>
    /// A story, a storyLine or a decision
    /// </summary>
    public interface IStoryLogicPart { }

    [System.Serializable, XmlRoot("Story")]
    public class StoryLine : IStoryLogicPart {
        public GraphPosition GraphPosition { get; set; }

        /// <summary>
        /// Index of this panel the list of all story lines
        /// </summary>
        public int StoryLineIndex { get; set; }

        /// <summary>
        /// Text of the story line.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The type of this node.
        /// </summary>
        public WritingPanel.Type NodeType { get; set; }

        /// <summary>
        /// If this node is of type end node.
        /// </summary>
        public bool IsNodeFinal { get { return NodeType == WritingPanel.Type.EndNode; } }
        
        /// <summary>
        /// If false, this node will go straight to another node
        /// </summary>
        public bool UsesDecisions { get; set; }

        /// <summary>
        /// When we don't use decisions, the next story panel index is this. (from quick going)
        /// </summary>
        public int NextQuickStoryLineIndex { get; set; }

        /// <summary>
        /// Decisions
        /// </summary>
        [XmlArray("Decision"), XmlArrayItem(typeof(Decision))]
        public List<Decision> Decisions { get; set; }

        /// <summary>
        /// Gets the decision at the given decision index. Can return null if this line does not
        /// use decisions.
        /// </summary>
        /// <param name="decisionIndex">The index of the seeked decision.</param>
        /// <returns>Null if it doesn't use decisions</returns>
        public Decision GetDecisionAtIndex(int decisionIndex) {
            if (!UsesDecisions)
                return null;
            return Decisions[decisionIndex];
        }

        /// <summary>
        /// Get the index of a decision. Can also return -1 if this decision does not exist or is null.
        /// </summary>
        public int IndexOfDecision(Decision decision) {
            if (decision == null)
                return -1;
            return Decisions.IndexOf(decision);
        }
    }

    [System.Serializable, XmlRoot("StoryLine")]
    public class Decision : IStoryLogicPart {
        public GraphPosition GraphPosition { get; set; }

        /// <summary>
        /// Decision name and text
        /// </summary>
        public string NameText { get; set; }

        /// <summary>
        /// Next story line index in the list of all possible story lines
        /// </summary>
        public int NextStoryLineIndex { get; set; }
    }

    [System.Serializable]
    public class GraphPosition {
        public Vector2 localPosition { get; set; }
        public Vector2 sizeDelta { get; set; }

        public GraphPosition() { } // empty constructor for serialization

        public GraphPosition(RectTransform rectTransform) {
            localPosition = rectTransform.localPosition;
            sizeDelta = rectTransform.sizeDelta;
        }
        public GraphPosition(RectTransform rectTransform, Vector2 customSizeDelta) {
            localPosition = rectTransform.localPosition;
            sizeDelta = customSizeDelta;
        }

        public void ApplyPositioning(RectTransform rectTransform) {
            rectTransform.localPosition = localPosition;
            rectTransform.sizeDelta = sizeDelta;
        }
    }
}
