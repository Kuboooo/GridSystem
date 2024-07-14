using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class MainMenuUI : MonoBehaviour {
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        
        private void Awake() {
            playButton.onClick.AddListener(() => { SceneLoader.Load(SceneLoader.Scene.GameScene); });
            // Note: Does not work on editor, only in built game
            quitButton.onClick.AddListener(() => Application.Quit());
        }
    }
}
