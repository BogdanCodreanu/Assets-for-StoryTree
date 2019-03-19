namespace Game.Writing {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using DG.Tweening;
    using UIAddition;
    using RazzielModules.UIHelper;

    public class WrittenDecisions : MonoBehaviour {
        private StoryLine storyLine;

        [SerializeField, Required, AssetsOnly]
        private WrittenDecision decisionPrefab;

        [SerializeField, Required]
        private WritingPanel writingPanel;
        public WritingPanel WritingPanel { get { return writingPanel; } }

        private List<WrittenDecision> decisions = new List<WrittenDecision>();
        public List<WrittenDecision> Decisions { get { return decisions; } }

        [SerializeField, Required]
        private PanelsQuickAnimations showAnimations, hideAnimations;

        [SerializeField, Required]
        private ButtonsQuickInteractible buttonsInteractibleWhileAnimating;

        [SerializeField, Required]
        private WritingPanelNextQuickPanel nextQuickPanel;

        /// <summary>
        /// If true, it means that the writing panel uses decisions. Otherwise, it means
        /// that it uses quick next panel going.
        /// </summary>
        public bool UsesDecisions { get { return !nextQuickPanel.UsesNextQuickPanel; } }

        /// <summary>
        /// Spawns a decision and ignores the return value.
        /// </summary>
        public void SpawnDecisionViaButton() {
            SpawnDecision(true);
            RegisterActions.Instance.RegisterNewState();
        }

        /// <summary>
        /// Spawns a new decision.
        /// </summary>
        /// <returns>The spawned decision</returns>
        public WrittenDecision SpawnDecision(bool useAnimations) {
            WrittenDecision spawn =
                Instantiate(decisionPrefab.gameObject, transform)
                .GetComponent<WrittenDecision>();
            spawn.transform.SetSiblingIndex(transform.childCount - 2);
            decisions.Add(spawn);
            spawn.InitOnCreation(this, useAnimations);

            nextQuickPanel.ShowOrHideByExistingDecisions(Decisions.Count);
            return spawn;
        }

        public void RemoveDecisionVariable(WrittenDecision decision) {
            decisions.Remove(decision);
            nextQuickPanel.ShowOrHideByExistingDecisions(Decisions.Count);
        }

        public void DeleteDecision(WrittenDecision decision) {
            Destroy(decision.gameObject);
        }

        public List<Decision> DecisionsValuesForSerialization() {
            List<Decision> decisionsValues = new List<Decision>();
            foreach (WrittenDecision decision in Decisions) {
                decisionsValues.Add(decision.GetTheDecision());
            }
            return decisionsValues;
        }

        public void PlayHideAnimation() {
            for (int i = Decisions.Count - 1; i >= 0; i --) {
                Decisions[i].DeleteDecision(true);
            }
            buttonsInteractibleWhileAnimating.SetButtonsInteractible(false);
            hideAnimations.PlayAllAnimations();

            nextQuickPanel.HidePart();
        }

        public void PlayShowAnimation() {
            buttonsInteractibleWhileAnimating.SetButtonsInteractible(true);
            showAnimations.PlayAllAnimations();

            nextQuickPanel.ShowOrHideByExistingDecisions(Decisions.Count);
        }
    }
}