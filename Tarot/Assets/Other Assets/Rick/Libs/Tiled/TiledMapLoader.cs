using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.Linq;
using System.Linq;
using System.IO;

namespace Rick.TiledMapLoader{
	public abstract class TiledMapLoader : TiledLoader{

		public const ulong FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
		public const ulong FLIPPED_VERTICALLY_FLAG   = 0x40000000;
		public const ulong FLIPPED_DIAGONALLY_FLAG   = 0x20000000;
		
		public const ulong ROTATION_90_FLAG 	= 0x20000000 + 0x80000000;
		public const ulong ROTATION_180_FLAG	= 0x40000000 + 0x80000000;
		public const ulong ROTATION_270_FLAG	= 0x20000000 + 0x40000000;

		public bool callAddEmptyTiles = true;
		
		protected int mapWidth;
		protected int mapHeight;
		protected string fileName;
		protected Color32 backgroundColor;
		
		protected Dictionary<Int32,Dictionary<String,String>> tilesetTiles = new Dictionary<int, Dictionary<string, string>>();
		
		XDocument document;
		XElement mapElement;
		
		
		public void loadFromFile(string fileName){
			this.fileName = fileName;
			string text = readAllText(fileName);
			
			clearEverything();
	        loadLevel(text);
		}

		string readAllText(string fileName) {
			string[] lines = File.ReadAllLines(fileName);
			string outString = "";
			foreach (var line in lines) {
				outString += line + "\n";
			}
			return outString;
		}
		
		
		void clearEverything(){
			tilesetTiles.Clear();
			backgroundColor = new Color32(0,0,0,0);
		}
		void loadLevel(string levelFileContent){
			document = XDocument.Parse(levelFileContent);
			mapElement = document.Element("map");
			loadMapAttributes();
	        loadMapProperties();
	        loadTilesets();
	        loadLevelsLayers();
	        loadLevelsObjectGroup();
	        afterAll();
		}
		
		
		protected abstract void afterAll();
	
		
		void loadMapAttributes(){
			mapWidth  = parseInt(mapElement.Attribute("width").Value);
			mapHeight = parseInt(mapElement.Attribute("height").Value);
			if(mapElement.Attribute("backgroundcolor") != null){
				string colorInHex = mapElement.Attribute("backgroundcolor").Value.Substring(1);
				backgroundColor = ColorUtils.HexToColor(colorInHex);
			}
			afterMapAttributesLoaded();
		}
		protected abstract void afterMapAttributesLoaded();
	
		void loadTilesets(){
			var tilesets = mapElement.Elements("tileset");
			foreach (var tileset in tilesets) {
				if(tileset.Attribute("source") != null){
					loadExternalTileset(tileset);
				}else{
					loadInternalTileset(tileset);
				}
			}
		}
	
		void loadExternalTileset(XElement tileset) {
			int firstGridId = parseInt(tileset.Attribute("firstgid").Value);
			string source = tileset.Attribute("source").Value;
			source = source.Substring(0,source.Length - 4);
			addExternalTileset(firstGridId, source);
		}
		
		protected virtual void addExternalTileset(int firstGridId,string source){}
		
		
		void loadInternalTileset(XElement tileset) {
			int firstGridId = parseInt(tileset.Attribute("firstgid").Value);
			string source = tileset.Element("image").Attribute("source").Value;
			string name = tileset.Attribute("name").Value;
			addInternalTileset(firstGridId,name,source);
			
			/*foreach (var tile in tileset.Elements("tile")) {
				int id = parseInt(tile.Attribute("id").Value);
				
				Dictionary<string, string> dictionnary = makePropertiesDictionary(tile.Element("properties"));
				tilesetTiles.Add(id, dictionnary);
			}*/
		}
		
		protected virtual void addInternalTileset(int firstGridId,string name, string source){}
		
		
		void loadLevelsObjectGroup(){
			 var objGroups = document.Elements().Descendants().Where(e => e.Name == "objectgroup");
			 foreach (var objGroup in objGroups) {
			 	string name = objGroup.Attribute("name").Value;
			 	foreach (var obj in objGroup.Descendants().Where(e => e.Name == "object")) {
			 		loadObject(obj,name);
			 	}	
			 }
		}
		
		void loadObject(XElement obj, string objectGroupName){
			int x = Int32.Parse(obj.Attribute("x").Value);
			int y = Int32.Parse(obj.Attribute("y").Value);
			var propertiesElemens = obj.Descendants().First(e => e.Name == "properties").Descendants();
			Dictionary<string, string> properties = new Dictionary<string, string>();
			
			foreach (var property in propertiesElemens) {
				string name = property.Attribute("name").Value;
				string value = property.Attribute("value").Value;
				properties.Add(name,value);
			}
			
			addObject(objectGroupName,x,y,properties);
		}
		
		protected abstract void addObject(string objectGroupName, int x, int y, Dictionary<string, string> properties);
		
		#region loadMapLayerRegion
		
		void loadLevelsLayers(){
			var layers = mapElement.Elements("layer");
			foreach (var layer in layers) {
				loadLayer(layer);
			}
		}
	
		void loadLayer(XElement layer){
			int width = Int32.Parse(layer.Attribute("width").Value);
			int height = Int32.Parse(layer.Attribute("height").Value);
			
			createNewLayer(layer, width, height);
			loadLayerTiles(layer, height);
		}
	
		void createNewLayer(XElement layer, int width, int height){
			Dictionary<string, string> properties = new Dictionary<string, string>();
			
			if(layer.Elements("properties").Any() ){
				var propertiesElements = layer.Element("properties").Descendants();
				foreach (var property in propertiesElements) {
					string pname = property.Attribute("name").Value;
					string value = property.Attribute("value").Value;
					properties.Add(pname, value);
				}
			}
			
			string name = layer.Attribute("name").Value;
			addLayer(name, width, height, properties);
		}
		
		protected abstract void addLayer(string layerName, int width, int height, Dictionary<string, string> properties);
	
		void loadLayerTiles(XElement layer, int height){
			XElement data = layer.Element("data");
			if(data.Attribute("encoding").Value == "csv"){
				string tilesCSV = data.Value;
				string[] tilesLines = tilesCSV.Split(new string[] { "\n\r", "\r\n", "\n", "\r" }, StringSplitOptions.None);
				int y = height;
				for (int i = 1; i <= height; i++) {
					y--;
					loadLayerLine( y, tilesLines[i]);
				}
			}else{
				outputError("The layer \"" + layer.Attribute("name").Value + "\" is not encoded in CSV, please change \"Map/Map Properties/Tile Layer Format\" to \"CSV\"");
			}
			
		}
		
		void loadLayerLine(int y, string tileLine){
			string[] tiles = tileLine.Split(new char[] { ',' }, StringSplitOptions.None);
			int x = 0;
			foreach (string tileId in tiles) {
				if(!string.IsNullOrEmpty(tileId)){
					ulong longId = parseULong(tileId);
					
					ulong flags = extractFlags(longId);
					int id = (int)(longId - flags);
					if(callAddEmptyTiles && id == 0){
						addEmptyTiles(x,y);
					}else{
						addTile(x,y,id,flags);
					}
					
				}
				x++;
			}
		}

		ulong extractFlags(ulong longId) {
			ulong flags = 0;
			flags += returnFlagIfHasFlag(longId, FLIPPED_HORIZONTALLY_FLAG);
			flags += returnFlagIfHasFlag(longId, FLIPPED_VERTICALLY_FLAG);
			flags += returnFlagIfHasFlag(longId, FLIPPED_DIAGONALLY_FLAG);
			
			return flags;
		}
		
		ulong returnFlagIfHasFlag(ulong source, ulong flag){
			if((source & flag ) == flag){
				return flag;
			}
			return 0;
		}
		
		
		protected abstract void addTile(int x, int y, int id, ulong flags);
	
		protected virtual void addEmptyTiles(int x, int y) {
			
		}
		#endregion
		
		
		
		
		void loadMapProperties(){
			Dictionary<string, string> dictionnary = makePropertiesDictionary(mapElement.Element("properties"));
			if(dictionnary.Count > 0){
				loadMapProperty(dictionnary);
			}
		}
		
		protected abstract void loadMapProperty(Dictionary<string, string> properties);
		
	}

}