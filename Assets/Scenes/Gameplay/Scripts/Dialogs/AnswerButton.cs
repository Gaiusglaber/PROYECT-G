using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

namespace ProyectG.Common.UI.Dialogs
{
    public class AnswerButton : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private TextMeshProUGUI answerText = null;
        #endregion

        #region PRIVATE_FIELDS
        private string feedbackLine = string.Empty;
        private bool buttonPressed = false;
        private Button button = null;
        #endregion

        #region PROPERTIES
        public DialogAnswer AnswerData
        {
            get;set;
        }
        public string Feedback
        {
            get
            {
                return feedbackLine;
            }
            set
            {
                feedbackLine = value;
            }
        }
        public bool ButtonPressed
        {
            get
            {
                return buttonPressed;
            }
            set
            {
                buttonPressed = value;
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void ChangeState()
        {
            ButtonPressed = !ButtonPressed;
        }
        #endregion

        #region EXPOSED_METHODS
        public void Init(UnityAction onGiveFeedback)
        {
            button = GetComponent<Button>();
            
            if(button != null)
            {
                button.onClick.AddListener(ChangeState);
                button.onClick.AddListener(onGiveFeedback);
            }
        }
        public void DeInit()
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }
        public void ResetData()
        {
            answerText.text = string.Empty;
            Feedback = string.Empty;
            ButtonPressed = false;
        }
        public void SetData(DialogAnswer answer)
        {
            AnswerData = answer;
            answerText.text = answer.answer;
            Feedback = answer.feedback;
        }
        #endregion
    }
}