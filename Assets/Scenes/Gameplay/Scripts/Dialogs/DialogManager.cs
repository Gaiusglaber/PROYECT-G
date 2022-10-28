using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace ProyectG.Common.UI.Dialogs
{
    /// <summary>
    /// The dialog handler takes care of showing the information of the conversationSo to the scene.
    /// </summary>
    public class DialogManager : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private TMPro.TMP_Text lineTxt = null;
        [SerializeField] private TMPro.TMP_Text actorTxt = null;
        [SerializeField] private RectTransform actorImageTransform = null;
        [SerializeField] private float wordSpeed;
        [SerializeField] private float fadeSpeed = 0;
        [SerializeField] private float wordSpeedMult = 0;
        [SerializeField] private Image FadeImage = null;
        [SerializeField] private AnswerButton prefabButtonUI = null;
        [SerializeField] private Animator parentPanel = null;
        [SerializeField] private Animator dialogPanel = null;
        [SerializeField] private Button skipButton = null;
        [SerializeField] private bool goodAnswer = false;

        [Header("ANIMATIONS AND DETAILS DIALOGS")]
        [Space(15) ,Tooltip("The size of the text Mesh Pro is the max size that will reach the lerper")] 
        [SerializeField] private float speedAnimationWord = 0;

        [Header("OTHER SETTINGS")]
        [Space(15)]
        [SerializeField] private GameObject carotNextLine = null;
        [SerializeField] private bool aviableSkipDialogs = false;
        #endregion

        #region PRIVATE_FIELDS
        private DialogConversationSO[] Conversation = null;
        private float initialWordSpeed;
        private Dictionary<string, GameObject> actors = new Dictionary<string, GameObject>();
        private bool conversationAwake = false;
        [NonSerialized] public bool canInteract = true;
        private int lineIndex = 0;
        private DialogConversationSO actualDialog = null;

        private string[] textHtml = null;
        
        private DialogPlayer[] dialogPlayers = null;
        private int dialogsExecuted = 0;
        private int maxDialogLinesToExecute = 0;
        private int maxAnswersAmount = 0;

        private List<AnswerButton> answerButtons = new List<AnswerButton>();

        private bool buttonAnswerPress = false;
        #endregion

        #region ACTIONS
        private UnityAction<int> onCorrectAnswer = null;
        private Action onDialogLineEnd = null;
        private Action onDialogStart = null;
        private Action openPanelUI = null;
        public Action OnDialogEnd = null;
        public Action<string> OnPlayDialog = null;
        #endregion

        #region PROPERTIES
        public UnityAction<int> OnCorrectAnswer
        {
            get
            {
                return onCorrectAnswer;
            }
            set
            {
                onCorrectAnswer = value;
            }
        }
        public Action OnDialogLineEnd
        {
            get
            {
                return onDialogLineEnd;
            }
            set
            {
                onDialogLineEnd = value;
            }
        }

        public float WordSpeed
        {
            get
            {
                return wordSpeed;
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init()
        {
            initialWordSpeed = wordSpeed;

            lineTxt.text = string.Empty;


            if (!aviableSkipDialogs)
            {
                skipButton.gameObject.SetActive(false);
            }
            else
            {
                skipButton.gameObject.SetActive(true);
            }

            OnDialogEnd += ClearActorsAtEnd;

            carotNextLine.gameObject.SetActive(false);
        }

        public bool PreLastLineExecuted()
        {
            return (dialogsExecuted == maxDialogLinesToExecute - Conversation[Conversation.Length-1].lines.Length) ? true : false;
        }
        public bool AllDialogsExecuted()
        {
            return (dialogsExecuted >= maxDialogLinesToExecute) ? true : false;
        }
        public void SetConversations(DialogConversationSO[] conversations)
        {
            if (conversations == null)
                return;

            Conversation = conversations;

            CheckMaxDialogLinesToExecute();
            
            if(parentPanel != null)
            {
                CheckMaxAnswersAmount();
                CreateAnswersButtons();
                parentPanel.gameObject.SetActive(false);
            }
        }
        public void InitAllDialogPlayers(Action uiAction, Action onDialogStart, Action onDialogEnd)
        {
            Debug.Log("ENTRO A ENCONTRAR DIALOGOS PLAYERS");

            if (dialogPlayers == null)
            {
                dialogPlayers = FindObjectsOfType<DialogPlayer>();
            }

            if (dialogPlayers.Length < 0)
            {
                return;
            }

            openPanelUI = uiAction;
            this.onDialogStart = onDialogStart;
            onDialogLineEnd = onDialogEnd;

            for (int i = 0; i < dialogPlayers.Length; i++)
            {
                if (dialogPlayers[i] != null)
                {
                    dialogPlayers[i].SetAction(PlayAfterUITransition);//, uiAction);
                }
            }
        }

        public void DeinitAllDialogPlayers()
        {
            if (dialogPlayers.Length < 0)
            {
                return;
            }

            for (int i = 0; i < dialogPlayers.Length; i++)
            {
                if (dialogPlayers[i] != null)
                {
                    dialogPlayers[i].ClearAction(PlayAfterUITransition);//, uiAction);
                }
            }

            for (int i = 0; i < answerButtons.Count; i++)
            {
                if(answerButtons[i] != null)
                {
                    answerButtons[i].DeInit();
                }
            }
        }
        /// <summary>
        /// Turns off all the actors and then displays only the active one and his name and starts playing his line
        /// </summary>
        public void LoadDialogue(string ID)
        {
            if (!conversationAwake)
            {
                LoadDictionary(ID);
                conversationAwake = true;
            }
            if(!dialogPanel.GetBool("IsOpen"))
            {
                return;
            }
            if (!canInteract)
            {
                wordSpeed = wordSpeedMult;
                return;
            }
            canInteract = false;
            wordSpeed = initialWordSpeed;            
            if (!IsLineIndexInRange())
            {
                lineIndex = 0;
                canInteract = true;
                lineTxt.text = string.Empty;

                OnDialogEnd?.Invoke();                
                return;
            }
            foreach (var actor in actors)
            {
                actor.Value.SetActive(false);
            }
            actualDialog = DialogPerId(ID);
            if (lineIndex < actualDialog.lines.Length)
            {
                actorTxt.text = "<uppercase>"+actualDialog.lines[lineIndex].actorId+"</uppercase>";
                actors[actualDialog.lines[lineIndex].actorId].SetActive(true);
                StartCoroutine(PlayLine(actualDialog.lines[lineIndex].line, false));
                OnPlayDialog?.Invoke(actualDialog.lines[lineIndex].line);
            }
        }
        public void PlayNextLineActualDialog()
        {
            Debug.Log("APRETO PARA REPRODUCIR LINEA");
            if (dialogPanel.GetBool("IsOpen"))
            {
                LoadDialogue(actualDialog.id);
            }
        }

        public void SkipDialog()
        {
            lineIndex = actualDialog.lines.Length;
            LoadDialogue(actualDialog.id);
            onDialogLineEnd?.Invoke();
        }

        public void PlayAfterUITransition(string IDConversation)
        {
            IEnumerator PlayWithCheck()
            {
                openPanelUI?.Invoke();

                LoadDialogue(IDConversation);

                yield break;
            }
            StartCoroutine(PlayWithCheck());
        }

        public void EndOfDialog()
        {
            carotNextLine.gameObject.SetActive(false);

            OnDialogLineEnd?.Invoke();
        }
        #endregion

        #region PROTECTED_METHODS
        protected void ClearActorsAtEnd()
        {
            IEnumerator WaitToClearActors()
            {
                yield return new WaitForSeconds(2f);
                
                foreach (var actor in actors)
                {
                    actor.Value.SetActive(false);
                }

                yield break;
            }

            StartCoroutine(WaitToClearActors());
        }
        protected void CheckMaxDialogLinesToExecute()
        {
            if(Conversation != null)
            {
                for (int i = 0; i < Conversation.Length; i++)
                {
                    for (int j = 0; j < Conversation[i].lines.Length; j++)
                    {
                        maxDialogLinesToExecute++;
                    }
                }
            }
        }

        protected void CheckMaxAnswersAmount()
        {
            if (Conversation != null)
            {
                for (int i = 0; i < Conversation.Length; i++)
                {
                    for (int j = 0; j < Conversation[i].lines.Length; j++)
                    {
                        if (maxAnswersAmount < Conversation[i].lines[j].answers.Length)
                        {
                            maxAnswersAmount = Conversation[i].lines[j].answers.Length;
                        }
                    }
                }
            }
        }
        protected void CreateAnswersButtons()
        {
            if (prefabButtonUI == null)
                return;

            for (int i = 0; i < maxAnswersAmount; i++)
            {
                AnswerButton button = Instantiate(prefabButtonUI, parentPanel.gameObject.transform);
                if (button != null)
                {
                    button.Init(SomeButtonPress);
                    answerButtons.Add(button);
                    button.gameObject.SetActive(false);
                }
            }
        }
        protected IEnumerator ShowAnswerOptions()
        {
            if (!IsLineIndexInRange() || actualDialog.lines[lineIndex].answers.Length < 1)
            {
                buttonAnswerPress = false;
                yield break;
            }

            buttonAnswerPress = false;
            int activeButtons = actualDialog.lines[lineIndex].answers.Length;

            for (int i = 0; i < activeButtons; i++)
            {
                if(answerButtons[i] != null)
                {
                    answerButtons[i].SetData(actualDialog.lines[lineIndex].answers[i]);
                }
            }

            parentPanel.gameObject.SetActive(true);
            parentPanel.SetBool("IsOpen",true);
            ChangeStateAnswerButtons(answerButtons, activeButtons, true);

            while (!buttonAnswerPress)
            {
                if(!parentPanel.gameObject.activeSelf)
                {
                    parentPanel.gameObject.SetActive(true);
                }
                yield return null;
            }

            parentPanel.SetBool("IsOpen",false);
            ChangeStateAnswerButtons(answerButtons, activeButtons, false);

            AnswerButton bttnPress = null;

            for (int i = 0; i < answerButtons.Count; i++)
            {
                if(answerButtons[i] != null)
                {
                    if(answerButtons[i].ButtonPressed)
                    {
                        bttnPress = answerButtons[i];

                        if(bttnPress.AnswerData.isCorrect)
                        {
                            OnCorrectAnswer?.Invoke(bttnPress.AnswerData.scoreValue);

                        }
                        else
                        {
                        }

                        break;
                    }
                }
            }

            yield return StartCoroutine(PlayLine(bttnPress.Feedback, true));
            
            yield return new WaitForSeconds(1.5f);

            ResetDataButtons(answerButtons, activeButtons);

            if (!bttnPress.AnswerData.isCorrect)
            {
                buttonAnswerPress = true;
                goodAnswer = false;
            }
            else
            {
                buttonAnswerPress = false;
                goodAnswer = true;
            }
        }
        protected void SomeButtonPress()
        {
            buttonAnswerPress = true;
        }
        protected void ResetDataButtons(List<AnswerButton> buttons, int amount)
        {
            if (buttons == null || amount > buttons.Count)
                return;

            for (int i = 0; i < amount; i++)
            {
                if (buttons[i] != null)
                {
                    buttons[i].ResetData();
                }
            }
        }
        protected void ChangeStateAnswerButtons(List<AnswerButton> buttons,int amount,bool isActive)
        {
            if (buttons == null || amount > buttons.Count)
                return;

            for (int i = 0; i < amount; i++)
            {
                if(buttons[i] != null)
                {
                    buttons[i].gameObject.SetActive(isActive);
                }
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private bool IsLineIndexInRange()
        {
            return lineIndex < actualDialog.lines.Length;
        }
        /// <summary>
        /// Loads the actor GO and ID to a dictionary that displays the information on runtime
        /// </summary>
        private void LoadDictionary(string ID)
        {
            actualDialog = DialogPerId(ID);
            foreach (var actor in actualDialog.actors)
            {
                actors.Add(actor.id, actor.GO);
                actors[actor.id] = Instantiate(actors[actor.id], actorImageTransform, false);
                actors[actor.id].SetActive(false);
            }
        }
        /// <summary>
        /// Search a DialogConversationSO by his ID
        /// </summary>
        private DialogConversationSO DialogPerId(string id)
        {
            for (int i = 0; i < Conversation.Length; i++)
            {
                if (id == Conversation[i].id)
                {
                    return Conversation[i];
                }
            }
            Debug.LogError("Couldnt find conversation ID");
            return null;
        }
        #endregion

        #region PRIVATE_COROUTINES
        /// <summary>
        /// Takes the line string into a array and plays each word with a second delay
        /// </summary>
        /// 
        private IEnumerator PlayLine(string line, bool singleAnswer)
        {
            IEnumerator AnimateWord(int indexWord, float duration)
            {
                float timeLerping = 0;

                while (timeLerping < duration)
                {
                    timeLerping += Time.deltaTime;

                    float sizeLetter = Mathf.Clamp(lineTxt.fontSize * timeLerping / duration, 0, lineTxt.fontSize);

                    if(indexWord < textHtml.Length)
                    {
                        textHtml[indexWord] = "<size=" + sizeLetter.ToString() + ">" + line[indexWord] + "</size>";
                    }

                    lineTxt.text = "";

                    for (int i = 0; i < textHtml.Length; i++)
                    {
                        lineTxt.text += textHtml[i];
                    }

                    yield return null;
                }
            }

            onDialogStart?.Invoke();

            lineTxt.text = string.Empty;
            textHtml = new string[line.Length];

            for (int i = 0; i < line.Length; i++)
            {
                yield return new WaitForSeconds(wordSpeed);

                StartCoroutine(AnimateWord(i, speedAnimationWord));

            }

            if (!singleAnswer)
            {
                yield return StartCoroutine(ShowAnswerOptions());

                if (!goodAnswer && buttonAnswerPress && actualDialog.lines[lineIndex].loopOnBadAnswerws)
                {
                    StartCoroutine(PlayLine(line, singleAnswer));
                    yield break;
                }

                if(IsLineIndexInRange())
                {
                    lineIndex++;
                }

                if (dialogsExecuted < maxDialogLinesToExecute)
                {
                    dialogsExecuted++;
                }
                canInteract = true;
                carotNextLine.gameObject.SetActive(true);
            }
        }
        #endregion
    }
}
