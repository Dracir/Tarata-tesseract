using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

using Magicolo.EditorTools;
using RickEditor.Editor;
using System.Xml;

namespace RickTools.Cards
{
    [System.Serializable]
    public class CardsLoaderWindow : CustomWindowBase
    {

        public string pathChosen;
        public GameObject root;

        [MenuItem("Rick's Tools/Cards")]
        public static void Create()
        {
            CreateWindow<CardsLoaderWindow>("Card Loader", new Vector2(275, 250));
        }

        void OnGUI()
        {
            pathChosen = RickEditorGUI.FilePath(new GUIContent("Card XML"), pathChosen, RickEditorGUI.resourcesFolder, "xml");
            root = RickEditorGUI.GamebjectField("Root",root);
            if (RickEditorGUI.Button(new GUIContent("Load"))) {
                XmlDocument xmldoc = XmlUtils.load(pathChosen);
                string pathFolder = Path.GetDirectoryName(pathChosen);
                CardLoader loader = new CardLoader(root, pathFolder);
                loader.load(xmldoc);
            }
        }
    }
}