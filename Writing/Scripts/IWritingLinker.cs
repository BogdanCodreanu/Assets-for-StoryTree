namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IWritingLinker {

        /// <summary>
        /// Tha panel that links to this. This can be null if no panel exists
        /// </summary>
        WritingPanel PanelLinkedTo { get; }

        /// <summary>
        /// The writing panel that will contain this linker
        /// </summary>
        WritingPanel ContainingPanel { get; }

        /// <summary>
        /// Should the line renderer be displayed
        /// </summary>
        bool ShouldDisplayLineRend { get; }

        Vector3 DisplayLineRendFromPoint { get; }
        Vector3 DisplayLineRendToPoint { get; }


        /// <summary>
        /// Usually called by UI button. Starts the link.
        /// </summary>
        void StartLinking();

        /// <summary>
        /// When linking has been completed. And we need to reset variables.
        /// If you want to link a linker to a panel use panel.LinkWith(linker) !!
        /// </summary>
        void LinkWith(WritingPanel panel);

        /// <summary>
        /// Unlinks from the current link.
        /// </summary>
        void Unlink();

        /// <summary>
        /// Knowing if the link with the new linked panel is different from the previous link.
        /// </summary>
        /// <param name="newLinkedPanel">The new linked panel</param>
        /// <returns>True if it is different. False if it's the same link as before</returns>
        bool IsDifferentLink(WritingPanel newLinkedPanel);
    }
}