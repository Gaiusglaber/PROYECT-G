using UnityEngine;

namespace ProyectG.Toolbox.Utils.Abbreviation
{
    [CreateAssetMenu(fileName = "AbbreviatorTier_", menuName = "Toolbox/AbbreviatorTier", order = 0)]
    public class AbbreviatorTier : ScriptableObject
    {
        #region EXPOSED_FIELDS
        [SerializeField] private char id = 'K';
        [SerializeField] private int range = 1000;
        #endregion

        #region PROPERTIES
        public char Id { get => id; private set => id = value; }
        public int Range { get => range; private set => range = value; }
        #endregion
    }
}
