namespace Game {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using RazzielModules.UIHelper;
#pragma warning disable 0649 // disable default value warning

    [CreateAssetMenu(menuName = "Game/Scene Controller", fileName = "Scene controller")]
    public class SceneController : ScriptableObject {
        [SerializeField]
        private string editTreeSceneName, playStorySceneName, mainMenuSceneName;

        public void ChangeToEditTree() {
            SceneManager.LoadScene(editTreeSceneName);
        }
        public void ChangeToPlayStory() {
            SceneManager.LoadScene(playStorySceneName);
        }

        public void ChangeToMainMenu() {
            SceneManager.LoadScene(mainMenuSceneName);
        }

        public void QuitApplication() {
            Application.Quit();
        }

        /// <summary>
        /// Change to main menu scene but also show an popup screen to confirm exit if there were
        /// changes not saved.
        /// </summary>
        public void ChangeToMainMenuButCheckIfWritingSaved() {
            if (!Writing.RegisterActions.Instance.CurrentState.IsThisStateSaved) {
                UIHelper.Instance.DisplayConfirmActionPanel("Are you sure you want to exit without saving?",
                    ChangeToMainMenu);
            } else {
                ChangeToMainMenu();
            }
        }
    }
}
