namespace Game.Reading.DefaultPanelReader {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using TMPro;

    public class DecisionsHolder : MonoBehaviour {
        [SerializeField, Required, SceneObjectsOnly]
        private DecisionSelector decisionPreset;
        [SerializeField, Required]
        private StoryVisualReader storyVisualReader;

        private List<DecisionSelector> decisionSpawns = new List<DecisionSelector>();

        public void SpawnDecisions(List<Decision> decisions) {
            KillAllDecisionSpawns();

            foreach (Decision decision in decisions) {
                DecisionSelector decisionSpawn =
                    decisionPreset.gameObject.CreateObjectFromPreset().GetComponent<DecisionSelector>();
                decisionSpawns.Add(decisionSpawn);

                decisionSpawn.GiveText(decision.NameText);

                decisionSpawn.OnSelectDecision.AddListener(delegate {
                    storyVisualReader.HasSelectedDecision(decision);
                    DecisionHasBeenSelected(decisionSpawn);
                });
            }

            DecisionSpawnsSetInteractible(false);
        }

        /// <summary>
        /// Sets interactibility of the decisions buttons
        /// </summary>
        private void DecisionSpawnsSetInteractible(bool isInteractible) {
            foreach (DecisionSelector decision in decisionSpawns) {
                decision.ButtonInteractible = isInteractible;
            }
        }

        /// <summary>
        /// Makes all decision buttons interactible. (They are born non-interactible)
        /// </summary>
        public void MakeDecisionsInteractible(float delay) {
            this.ExecuteWithDelay(delay, delegate { DecisionSpawnsSetInteractible(true); });
        }

        private void KillAllDecisionSpawns() {
            for (int i = decisionSpawns.Count - 1; i >= 0; i--) {
                Destroy(decisionSpawns[i].gameObject);
            }
            decisionSpawns = new List<DecisionSelector>();
        }

        private void DecisionHasBeenSelected(DecisionSelector selectedDecision) {
            selectedDecision.WasSelected();
            foreach (DecisionSelector decision in decisionSpawns) {
                DecisionSpawnsSetInteractible(false);
                if (decision == selectedDecision)
                    continue;
                decision.WasNotSelected();
            }
        }
    }
}
