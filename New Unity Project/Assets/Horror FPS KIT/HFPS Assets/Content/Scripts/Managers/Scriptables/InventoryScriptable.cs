using System;
using System.Collections.Generic;
using UnityEngine;

public enum typeEnum { Normal, Heal, ItemPart, Weapon, Bullets, Light }

public class InventoryScriptable : ScriptableObject {

	public List<ItemMapper> ItemDatabase = new List<ItemMapper> ();

	[Serializable]
	public class ItemMapper {

		public string Title;
        [ReadOnly] public int ID;
        [Multiline] public string Description;
        public typeEnum itemType;
        public Sprite itemSprite;
        public GameObject dropObject;
        public GameObject packDropObject;

        [Serializable]
        public class Booleans
        {
            public bool isStackable;
            public bool isUsable;
            public bool isCombinable;
            public bool isDroppable;
            public bool CombineGetItem;
            public bool CombineNoRemove;
            public bool CombineGetSwItem;
            public bool UseItemSwitcher;
        }
        public Booleans itemToggles = new Booleans();

        [Serializable]
        public class Sounds
        {
            public AudioClip useSound;
            public AudioClip combineSound;
            [Range(0,1f)]
            public float soundVolume = 1f;
        }
        public Sounds itemSounds = new Sounds();

        [Serializable]
        public class Settings
        {
            public int maxItemCount;
            public int useSwitcherID = -1;
            public int healAmount;
        }

		public Settings itemSettings = new Settings();

        [Serializable]
        public class CombineSettings
        {
            public int combineWithID;
            public int resultCombineID;
            public int combineSwitcherID;
        }

        public CombineSettings[] combineSettings;
    }

    public void Add(ItemMapper m)
    {
        ItemDatabase.Add(m);
        m.ID = ItemDatabase.IndexOf(m);
    }

    public void Remove(ItemMapper m)
    {
        ItemDatabase.Remove(m);
        m.ID = -1;
        Reseed();
    }

    public void RemoveAt(int index)
    {
        ItemMapper m = ItemDatabase[index];
        ItemDatabase.Remove(m);
    }

    public void RemoveAtReseed(int index)
    {
        ItemMapper m = ItemDatabase[index];
        ItemDatabase.Remove(m);
        Reseed();
    }

    private void Reseed()
    {
        foreach (ItemMapper x in ItemDatabase)
        {
            x.ID = ItemDatabase.IndexOf(x);
        }
    }
}
