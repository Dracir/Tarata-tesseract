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

        public CardLoader(GameObject root) {
            this.root = root.transform;
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
            card.name = name;
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
            string file = textureNode.Attributes.GetNamedItem("textureFile").InnerText;
            string index = textureNode.Attributes.GetNamedItem("index").InnerText;
            string fileName = file + "_" + index;
            return Resources.Load<Sprite>(fileName);
        }
    }
}
