using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using ProyectG.Toolbox.Lerpers;

namespace ProyectG.Gameplay.UI{

	public class EnergyHandler : MonoBehaviour
	{
		#region EXPOSED_FIELDS
		[SerializeField] private TMPro.TMP_Text energyTxt = null;
		[SerializeField] private Image fillImage = null;
		[SerializeField] private Image cantEnergyImg = null;
		[SerializeField] private Image maxEnergyImg = null;
		[SerializeField] private float lerperSpeed = 0;
		[SerializeField] private int maxEnergy = 0;
		[SerializeField] private int initialEnergy = 0;
		[SerializeField] private Color energyColor = Color.white;
		[SerializeField] private Color maxEnergyColor = Color.white;
		[SerializeField] private float energyDecreaseInterval = 2.0f;
		[SerializeField] private int decreceEnergy = 150;

		private float timer = 0;

		#endregion

		#region PRIVATE_FIELDS
		[NonSerialized] public int cantEnergy = 0;
		private FloatLerper fillLerper = null;
		private FloatLerper txtLerper = null;
        #endregion

        #region PROPERTIES
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
			cantEnergy = initialEnergy;
			cantEnergyImg.color = energyColor;
			maxEnergyImg.color = maxEnergyColor;
			fillLerper = new FloatLerper(lerperSpeed, AbstractLerper<float>.SMOOTH_TYPE.STEP_SMOOTHER);
			txtLerper = new FloatLerper(lerperSpeed, AbstractLerper<float>.SMOOTH_TYPE.STEP_SMOOTHER);
			StartCoroutine(LerpBar(GetFillAmmount()));
			StartCoroutine(LerpTxtValue(cantEnergy));
        }

		private void Update()
		{
			DecreaseEnergyOverTime();
		}

		#endregion

		#region PUBLIC_METHODS
		public void UpdateEnergy(int valueToUpdate)
        {
			cantEnergy += valueToUpdate > maxEnergy ? maxEnergy : valueToUpdate;
			StartCoroutine(LerpBar(GetFillAmmount()));
			StartCoroutine(LerpTxtValue(cantEnergy));
		}
		#endregion

		#region PRIVATE_METHODS
		private float GetFillAmmount()
        {
			return (float)cantEnergy / (float)maxEnergy;
        }

		public void DecreaseEnergyOverTime()
		{
			if (timer < energyDecreaseInterval)
				timer += Time.deltaTime;
			else
			{
				cantEnergy -= decreceEnergy;
				timer = 0.0f;
				UpdateEnergy(cantEnergy);
			}
		}

		#endregion

		#region PUBLIC_CORROUTINES
		#endregion

		#region PRIVATE_CORROUTINES
		private IEnumerator LerpBar(float valueToChange)
        {
			fillLerper.SetValues(fillImage.fillAmount, valueToChange, true);
			while (fillLerper.On)
            {
				fillLerper.Update();
				fillImage.fillAmount = fillLerper.CurrentValue;
				yield return new WaitForEndOfFrame();
            }
        }
		private IEnumerator LerpTxtValue(int valueToChange)
        {
			txtLerper.SetValues(cantEnergy, valueToChange, true);
            while (txtLerper.On)
            {
				txtLerper.Update();
				energyTxt.text = "<color=#"+ ColorUtility.ToHtmlStringRGB(energyColor) + ">" + txtLerper.CurrentValue.ToString() +
					"<color=#" + ColorUtility.ToHtmlStringRGB(Color.white) + ">" + "/" +
					"<color=#" + ColorUtility.ToHtmlStringRGB(maxEnergyColor) + ">" + maxEnergy.ToString();
				yield return new WaitForEndOfFrame();
            }
        }
		#endregion

	}

}