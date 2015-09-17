using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Linq;
using System;

namespace RickTools.Cards
{
    public class CardLoader
    {
        public Transform root;
        public string folderPath;
        private bool makeGameObjectForCardGroups;

        public CardLoader(GameObject root, string folderPath, bool makeGameObjectForCardGroups)
        {
            this.root = root.transform;
            this.folderPath = folderPath;
            this.makeGameObjectForCardGroups = makeGameObjectForCardGroups;
        }


        public void load(XmlDocument doc) {
            XmlElement rootDoc = doc.DocumentElement;
            foreach (XmlNode cardGroup in rootDoc.ChildNodes)
            {
                if (makeGameObjectForCardGroups)
                {
                    string groupName = cardGroup.Attributes.GetNamedItem("groupName").InnerText;
                    GameObject cardGroupGO = GameObjectExtend.createGameObject(groupName, root);
                    loadGroup(cardGroupGO.transform, cardGroup);
                }
                else
                {
                    loadGroup(root, cardGroup);
                }
                
            }
                
        }

        private void loadGroup(Transform parent, XmlNode cardGroup)
        {
            string cardGroupName = cardGroup.Attributes.GetNamedItem("groupName").InnerText;
            foreach (XmlNode card in cardGroup.ChildNodes)
            {
                string name = card.SelectSingleNode("name").InnerText;
                string descripton = card.SelectSingleNode("description").InnerText;
                Sprite front = findSpriteForm(card.SelectSingleNode("textureFront"));
                Sprite back = findSpriteForm(card.SelectSingleNode("textureBack"));
                createCard(parent, name, descripton, cardGroupName, front, back);
            }
        }

        private void createCard(Transform parent, string name, string descripton, string cardGroupName, Sprite front, Sprite back)
        {
            GameObjectBuilder builder = new GameObjectBuilder(name, parent);
            Transform cardRoot = builder.GameObject.transform;
            Card card = builder.GameObject.AddComponent<Card>();
            card.description = descripton;
            card.cardName = name;
            card.cardGroupName = cardGroupName;
            card.front = createCardTextureGame("Front", cardRoot, front);
            card.back = createCardTextureGame("Back", cardRoot, back);
            card.back.SetActive(false);
        }

        private GameObject createCardTextureGame(string name, Transform parent, Sprite sprite)
        {
            GameObjectBuilder builder = new GameObjectBuilder(name, parent);
            builder.addSprite(sprite, 1);            
            return builder.GameObject;
        }

        private Sprite findSpriteForm(XmlNode textureNode) {
            string fileName = textureNode.Attributes.GetNamedItem("textureFile").InnerText;
            int index = Int32.Parse(textureNode.Attributes.GetNamedItem("index").InnerText);
            return ResourcesUtils.loadSprite(folderPath, fileName, index);
        }
    }
}
