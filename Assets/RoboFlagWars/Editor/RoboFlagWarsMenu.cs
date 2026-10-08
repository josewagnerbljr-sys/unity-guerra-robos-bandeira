using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RoboFlagWars.EditorTools
{
    public static class RoboFlagWarsMenu
    {
        [MenuItem("Robo Flag Wars/1 - Criar cena do jogo")]
        public static void CreateScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();

            System.IO.Directory.CreateDirectory("Assets/Scenes");
            string path = "Assets/Scenes/RoboFlagWars.unity";
            EditorSceneManager.SaveScene(scene, path);

            var list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (!list.Exists(s => s.path == path))
            {
                list.Insert(0, new EditorBuildSettingsScene(path, true));
                EditorBuildSettings.scenes = list.ToArray();
            }
            EditorUtility.DisplayDialog("Robo Flag Wars", "Cena criada em " + path + " e adicionada ao Build Settings.\nAperte Play!", "Bora!");
        }

        [MenuItem("Robo Flag Wars/2 - Configurar projeto para celular")]
        public static void ConfigureMobile()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            EditorUtility.DisplayDialog("Robo Flag Wars",
                "Orientação paisagem e Color Space Linear configurados.\n\nLembre-se: Project Settings > Player > Other Settings > Active Input Handling = Both (para testar com teclado no PC).",
                "OK");
        }
    }
}
