using UnityEngine.SceneManagement;

namespace UI {
    public static class SceneLoader {

        public enum Scene {
            LoadingScene,
            GameScene,
            MainMenu
        }

        private static Scene targetScene;

        public static void Load(Scene targetScene) {

            SceneLoader.targetScene = targetScene;

            SceneManager.LoadScene(Scene.LoadingScene.ToString());

        }

        public static void LoaderCallback() {
            SceneManager.LoadScene(targetScene.ToString());

        }
    }
}