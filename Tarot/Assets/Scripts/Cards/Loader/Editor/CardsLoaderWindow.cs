using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

using Magicolo.EditorTools;
using RickEditor.Editor;
using System.Xml;
using System;

namespace RickTools.Cards
{
    [System.Serializable]
    public class CardsLoaderWindow : CustomWindowBase
    {

        public string pathChosen;
        public GameObject root;
        public bool makeGameObjectForCardGroups;

        [MenuItem("Rick's Tools/Cards")]
        public static void Create()
        {
            CreateWindow<CardsLoaderWindow>("Card Loader", new Vector2(400, 250));
        }

        void OnGUI()
        {
            showGuiFields();
            showLoadButton();
            
        }

        private void showGuiFields()
        {
            pathChosen = RickEditorGUI.FilePath(new GUIContent("Card XML"), pathChosen, RickEditorGUI.resourcesFolder, "xml");
            root = RickEditorGUI.GamebjectField("Root", root);
            makeGameObjectForCardGroups = RickEditorGUI.CheckBox(new GUIContent("Separate CardGroups","Make new Parent GameObject for each Card Groups"), makeGameObjectForCardGroups);
        }

        private void showLoadButton()
        {
            if (RickEditorGUI.Button(new GUIContent("Load")))
            {
                if (root == null)
                {
                    Debug.LogError("CardLoader : Can't load card without Root Parent selected");
                }
                else
                {
                    load();
                }
            }
        }

        private void load()
        {
            XmlDocument xmldoc = XmlUtils.load(pathChosen);
            string pathFolder = Path.GetDirectoryName(pathChosen);
            CardLoader loader = new CardLoader(root, pathFolder,makeGameObjectForCardGroups);
            loader.load(xmldoc);
        }
    }
}