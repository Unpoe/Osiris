using System.IO;
using UnityEngine;
using UnityEditor;

namespace Osiris
{
    public class ActorPrefabWindow : EditorWindow
    {
        private ActorId actorId;

        // Prefab variables
        private string prefabSavePath = "1 Art/Prefabs/Actors";
        private GameObject characterModel = null;

        // Content variables
        private bool createContentAssets = true;
        private string contentSavePath = "Assets/4 Content/Actor";

        [MenuItem("Tools/Osiris/Actor Prefab Window")]
        private static void CreateWindow() {
            GetWindow<ActorPrefabWindow>("Actor Prefab Window");
        }

        private void OnGUI() {
            actorId = (ActorId)EditorGUILayout.EnumPopup("Actor ID", actorId);

            prefabSavePath = EditorGUILayout.TextField("Prefab Path", prefabSavePath);
            characterModel = (GameObject)EditorGUILayout.ObjectField("Character Model", characterModel, typeof(GameObject), false);

            createContentAssets = EditorGUILayout.Toggle("Create Content Assets", createContentAssets);
            if (createContentAssets) {
                contentSavePath = EditorGUILayout.TextField("Content Path", contentSavePath);
            }

            if (GUILayout.Button("Create")) {
                CreatePrefab();
            }
        }

        private void CreatePrefab() {
            if(characterModel == null) {
                Debug.LogError("[ActorPrefabWindow] Character model not assigned. No asset has been created.");
                return;
            }

            // Root object
            GameObject rootObject = new GameObject($"PF_Actor_{actorId.ToString()}");

            // Empty transform called visuals pivot
            GameObject visualsPivot = new GameObject("visualsPivot");
            visualsPivot.transform.SetParent(rootObject.transform);

            // Character model
            GameObject model = Instantiate(characterModel, visualsPivot.transform);
            Animator animator = model.GetComponent<Animator>();
            if (animator == null) {
                Debug.LogError(
                    "[ActorPrefabWindow] Character model does not have an animator. No asset has been created." +
                    "\nTip: go to the Rig tab in the model and check that it is a Humanoid with an avatar created from the model."
                );
                DestroyImmediate(rootObject);
                return;
            }
            animator.applyRootMotion = false;

            // Actor component
            Actor actorComponent = rootObject.AddComponent<Actor>();
            actorComponent.SetEditorDependencies(animator);

            bool success;
            string prefabName = $"PF_Actor_{actorId.ToString()}.prefab";
            string prefabPath = Path.Combine(Application.dataPath, prefabSavePath, prefabName);
            prefabPath = prefabPath.Replace('/', '\\'); // I never know which one is right. Using this to make sure it works

            // TODO: what happens if the prefab already exists?

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(rootObject, prefabPath, out success);

            if (success) {
                Debug.Log("[ActorPrefabWindow] New default actor prefab has been created succesfully.");

                if (!createContentAssets) { // Only focus it if we are not creating other assets
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = prefab;
                }
            } else {
                Debug.LogError("[ActorPrefabWindow] A problem ocurred while creating a new actor prefab. No asset has been created.");
            }

            DestroyImmediate(rootObject);

            // Content files
            if (createContentAssets) {
                // Actor definition
                ActorDefinition actorDef = ScriptableObject.CreateInstance<ActorDefinition>();
                string definitionName = $"ActorDefinition_{actorId.ToString()}.asset";
                string definitionPath = Path.Combine(contentSavePath, definitionName);
                definitionPath = definitionPath.Replace('/', '\\'); // I never know which one is right. Using this to make sure it works
                AssetDatabase.CreateAsset(actorDef, definitionPath);
                AssetDatabase.SaveAssets();

                // Animation configuration
                ActorAnimationConfig animConfig = ScriptableObject.CreateInstance<ActorAnimationConfig>();
                string animConfigName = $"ActorAnimationConfig_{actorId.ToString()}.asset";
                string animConfigPath = Path.Combine(contentSavePath, animConfigName);
                animConfigPath = animConfigPath.Replace('/', '\\'); // I never know which one is right. Using this to make sure it works
                AssetDatabase.CreateAsset(animConfig, animConfigPath);
                AssetDatabase.SaveAssets();

                // Set definition dependencies
                actorDef.SetEditorDependencies(actorId, actorComponent, animConfig);

                // Set definition to the table
                string[] assetGUIDs = AssetDatabase.FindAssets("t:ActorTable");
                if (assetGUIDs.Length > 0) {
                    string actorTablePath = AssetDatabase.GUIDToAssetPath(assetGUIDs[0]);
                    ActorTable actorTable = AssetDatabase.LoadAssetAtPath<ActorTable>(actorTablePath);
                    actorTable.AddDefinition(actorDef);
                } else {
                    Debug.LogError("[ActorPrefabWindow] No ActorTable asset has been found. The new definition will not be added to the table.");
                }
            }
        }
    }
}