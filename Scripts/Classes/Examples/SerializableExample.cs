﻿using System;
using System.IO;
using CippSharp.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CippSharp.Experimental.Examples
{
    [ExecuteInEditMode]
    public class SerializableExample : MonoBehaviour
    {
        /// <summary>
        /// A nicer contextual name usable for logs.
        /// </summary>
        public static readonly string LogName = string.Format("[{0}]: ", typeof(SerializableExample).Name);

        [Header("Data:")]
        public BinaryHolder binaryHolder = null;
        
        [Header("References:")]
        [SerializeField] public SceneAsset sceneAsset = null;

        [Header(Constants.Commands + ":")]
        public bool saveScene = false;
        public bool loadScene = false;
        
        private void Awake()
        {
            
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (saveScene)
            {
                saveScene = false;
                TrySaveScene();
            }

            if (loadScene)
            {
                loadScene = false;
                TryLoadScene();
            }
        }

        private void TrySaveScene()
        {
            if (binaryHolder == null)
            {
                return;
            }

            if (binaryHolder.Serialize(gameObject.scene))
            {
                Debug.Log(LogName+"Scene saved.", this);
            }
        }

        private void TryLoadScene()
        {
            Scene scene = binaryHolder.Deserialize<Scene>();
            try
            {
                string path = AssetDatabase.GetAssetPath(sceneAsset);
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(path);
                string newName = fileNameNoExtension + " Copy";
                path = path.Replace(fileNameNoExtension, newName);
                Debug.Log(path);
                EditorSceneManager.SaveScene(scene, path, true);
            }
            catch (Exception e)
            {
               Debug.LogError("Fail to load for: "+e.Message, this);
            }
//            if (SceneManager.SetActiveScene(scene))
//            {
                Debug.Log(LogName+"Scene loaded.", this);
//            }
        }
    }
}
