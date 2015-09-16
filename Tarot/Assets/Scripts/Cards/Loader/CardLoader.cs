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

        public CardLoader(GameObject root, string folderPath) {
            this.root = root.transform;
            this.folderPath = folderPath;
        }

        public void load(XmlDocument doc) {
            XmlElement rootDoc = doc.DocumentElement;
            foreach (XmlNode cardGroup in rootDoc.ChildNodes)
            {
                string groupName = cardGroup.Attributes.GetNamedItem("groupName").InnerText;
                GameObject cardGroupGO = GameObjectExtend.createGameObject(groupName, root);
                loadGroup(cardGroupGO.transform, cardGroup);
            }
                
        }

        private void loadGroup(Transform parent, XmlNode cardGroup)
        {
            foreach (XmlNode card in cardGroup.ChildNodes)
            {
                string name = card.SelectSingleNode("name").InnerText;
                string descripton = card.SelectSingleNode("description").InnerText;
                Sprite front = findSpriteForm(card.SelectSingleNode("textureFront"));
                Sprite back = findSpriteForm(card.SelectSingleNode("textureBack"));
                createCard(parent, name, descripton, front, back);
            }
        }

        private void createCard(Transform parent, string name, string descripton, Sprite front, Sprite back)
        {
            GameObject cardRoot = GameObjectExtend.createGameObject(name, parent);
            Card card = cardRoot.AddComponent<Card>();
            card.description = descripton;
            card.cardName = name;
            card.front = createCardTextureGame("Front", cardRoot.transform, front);
            card.front = createCardTextureGame("Back", cardRoot.transform, back);
        }

        private GameObject createCardTextureGame(string name, Transform parent, Sprite sprite)
        {
            GameObject facade = GameObjectExtend.createGameObject(name, parent);
            SpriteRenderer sr = facade.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            return facade;
        }

        private Sprite findSpriteForm(XmlNode textureNode) {
            string fileName = textureNode.Attributes.GetNamedItem("textureFile").InnerText;
            int index = Int32.Parse(textureNode.Attributes.GetNamedItem("index").InnerText);
            return ResourcesUtils.loadSprite(folderPath, fileName, index);
        }
    }
}
