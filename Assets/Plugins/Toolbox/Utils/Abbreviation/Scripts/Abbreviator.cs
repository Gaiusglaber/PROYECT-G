using UnityEngine;

using System.Globalization;

namespace Pathfinders.Toolbox.Utils.Abbreviation
{
    public class Abbreviator : MonoBehaviour
    {
        [SerializeField] private AbbreviatorTier[] abbreviations = null;

        public string AbbreviateNumber(float number)
        {
            for (int i = abbreviations.Length - 1; i >= 0; i--)
            {
                if (Mathf.Abs(number) >= abbreviations[i].Range)
                {
                    float roundedNumber = number / abbreviations[i].Range;
                    return roundedNumber.ToString("F1", CultureInfo.GetCultureInfo("en-US")) + abbreviations[i].Id;
                }
            }

            return ((int)number).ToString();
        }
    }
}