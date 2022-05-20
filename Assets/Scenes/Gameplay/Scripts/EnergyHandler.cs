using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

using ProyectG.Toolbox.Lerpers;

namespace ProyectG.Gameplay.UI
{

	public class EnergyHandler : MonoBehaviour
	{
		#region EXPOSED_FIELDS
		[SerializeField] private int lowEneryThreshhold = 0;
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
		[SerializeField] private Light2D[] factoryLights = null;
		[SerializeField] public Player.Controller.PlayerController playerController = null;

		private float timer = 0;

		#endregion

		#region PRIVATE_FIELDS
		[NonSerialized] public int cantEnergy = 0;
		private float processDecreaseInterval = 0;
		private int processDecreaseCost = 0;
		private FloatLerper fillLerper = null;
		private FloatLerper txtLerper = null;
		private bool ToggleLowEnergy = false;
		#endregion

		#region PROPERTIES
		static public Action<bool> Withoutenergy;
		#endregion

		#region ACTIONS
		public Action<float> OnUpdateEnergy = null;
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
			OnUpdateEnergy = ChangeLightValue;
        }

		private void Update()
		{
			DecreaseEnergyOverTime();
			if(cantEnergy <= 0)
			{
				Withoutenergy?.Invoke(true);
			}
		}

		#endregion

		#region PUBLIC_METHODS
		public void UpdateEnergy(int valueToUpdate)
        {
			cantEnergy = valueToUpdate > maxEnergy ? maxEnergy : valueToUpdate;
			cantEnergy = valueToUpdate < 0 ? 0 : valueToUpdate;
			if (cantEnergy< lowEneryThreshhold&&!ToggleLowEnergy)
            {
				OnUpdateEnergy?.Invoke(0.1f);
				playerController.ChangeSpeed(true);
				ToggleLowEnergy = true;
            }
            else if (cantEnergy > lowEneryThreshhold&& ToggleLowEnergy)
            {
				ToggleLowEnergy = false;
				playerController.ChangeSpeed(false);
				OnUpdateEnergy?.Invoke(1f);
			}
			StartCoroutine(LerpBar(GetFillAmmount()));
			StartCoroutine(LerpTxtValue(cantEnergy));
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

		public void ConsumeEnergyByProcess()
        {
			if (timer < processDecreaseInterval)
				timer += Time.deltaTime;
			else
			{
				cantEnergy -= processDecreaseCost;
				timer = 0.0f;
				UpdateEnergy(cantEnergy);
			}
		}

		public void SetCostOfProcessDecrement(int decreaseCost, float timePerDecrease)
        {
			processDecreaseCost = decreaseCost;
			processDecreaseInterval = timePerDecrease;
		}
		#endregion

		#region PRIVATE_METHODS
		private float GetFillAmmount()
        {
			return (float)cantEnergy / (float)maxEnergy;
        }
		private void ChangeLightValue(float value)
        {
			foreach(var light in factoryLights)
            {
				StartCoroutine(LerpLight(light, value));
            }
        }
		#endregion

		#region PUBLIC_CORROUTINES
		#endregion

		#region PRIVATE_CORROUTINES
		private IEnumerator LerpLight(Light2D light,float valueToLerp)
        {
			FloatLerper lerper = new FloatLerper(2, AbstractLerper<float>.SMOOTH_TYPE.STEP_SMOOTHER);
			lerper.SetValues(light.intensity, valueToLerp, true);
            while (lerper.On)
            {
				lerper.Update();
				light.intensity = lerper.CurrentValue;
				yield return new WaitForEndOfFrame();
            }
        }
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