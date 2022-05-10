using System;
using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.Data;

namespace ProyectG.Gameplay.Objects{

	[CreateAssetMenu(fileName = "WorldItemSo", menuName = "ScriptableObjects/Data/World", order = 1)]
	public class WorldItemSO : ScriptableObject
	{
		#region EXPOSED_FIELDS
		public string id = string.Empty;
		public new string name = string.Empty;
		public Sprite sprite = null;
		public ItemModel itemModel = null;
		#endregion
	}
}