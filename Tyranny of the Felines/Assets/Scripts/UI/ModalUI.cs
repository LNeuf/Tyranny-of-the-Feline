using UnityEngine;
using UnityEngine.UIElements;

namespace Matryoshka.UI
{
    public class ModalUI : MonoBehaviour
    {
        public static ModalUI Singleton { get; private set; }

        private VisualElement currentModal;

        private const string ModalContainer = "Modal";
        private const string MessageModal = "MessageModal";
        private const string InputModal = "InputModal";

        private const string MessageTitle = "MessageTitle";
        private const string MessageContent = "MessageContent";
        private const string InputTitle = "InputTitle";

        private const string MessageButtonContainer = "MessageButtonContainer";
        private const string InputTextField = "InputTextField";

        private const string MessageConfirmButton = "MessageConfirmButton";
        private const string InputCancelButton = "InputCancelButton";
        private const string InputConfirmButton = "InputConfirmButton";

        private VisualElement modal;
        private VisualElement messageModal;
        private VisualElement inputModal;

        private Label messageTitle;
        private Label messageContent;
        private Label inputTitle;

        private VisualElement messageButtonContainer;
        private TextField inputTextField;

        private Button messageConfirmButton;
        private Button inputCancelButton;
        private Button inputConfirmButton;

        public delegate void OnMessageConfirm();
        public delegate void OnInputCancel();
        public delegate void OnInputConfirm(string input);

        public OnMessageConfirm onMessageConfirm;
        public OnInputCancel onInputCancel;
        public OnInputConfirm onInputConfirm;

        // Start is called before the first frame update
        void Start()
        {
            Singleton = this;
            var root = GetComponent<UIDocument>().rootVisualElement;

            modal = root.Q<VisualElement>(ModalContainer);
            messageModal = root.Q<VisualElement>(MessageModal);
            inputModal = root.Q<VisualElement>(InputModal);
            currentModal = messageModal;

            messageTitle = root.Q<Label>(MessageTitle);
            messageContent = root.Q<Label>(MessageContent);
            inputTitle = root.Q<Label>(InputTitle);

            messageButtonContainer = root.Q<VisualElement>(MessageButtonContainer);
            inputTextField = root.Q<TextField>(InputTextField);

            messageConfirmButton = root.Q<Button>(MessageConfirmButton);
            inputCancelButton = root.Q<Button>(InputCancelButton);
            inputConfirmButton = root.Q<Button>(InputConfirmButton);

            messageConfirmButton.clicked += MessageConfirmButtonPressed;
            inputCancelButton.clicked += InputCancelButtonPressed;
            inputConfirmButton.clicked += InputConfirmButtonPressed;
        }

        private void ShowModal()
        {
            modal.style.display = DisplayStyle.Flex;
        }

        private void HideModal()
        {
            modal.style.display = DisplayStyle.None;
        }

        private void ShowMessageModalButtons()
        {
            messageButtonContainer.style.display = DisplayStyle.Flex;
        }

        private void HideMessageModalButtons()
        {
            messageButtonContainer.style.display = DisplayStyle.None;
        }

        public void ShowMessageModal(string title, string message, OnMessageConfirm? messageConfirm = null)
        {
            SetModalType(messageModal);
            ShowMessageModalButtons();
            var onConfirm = messageConfirm ?? DefaultMessageConfirmCallback;
            messageTitle.text = title;
            messageContent.text = message;
            onMessageConfirm = onConfirm;
            ShowModal();
        }

        public void ShowMessageModalWithoutButtons(string title, string message, OnMessageConfirm? messageConfirm = null)
        {
            SetModalType(messageModal);
            HideMessageModalButtons();
            var onConfirm = messageConfirm ?? DefaultMessageConfirmCallback;
            messageTitle.text = title;
            messageContent.text = message;
            onMessageConfirm = onConfirm;
            ShowModal();
        }

        public void ShowInputModal(string title, OnInputCancel? inputCancel = null, OnInputConfirm? inputConfirm = null)
        {
            SetModalType(inputModal);
            var onCancel = inputCancel ?? DefaultInputCancelCallback;
            var onConfirm = inputConfirm ?? DefaultInputConfirmCallback;
            onInputCancel = onCancel;
            onInputConfirm = onConfirm;
            inputTitle.text = title;
            ShowModal();
        }

        public void CloseModal()
        {
            HideModal();
        }

        private void MessageConfirmButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            HideModal();
        }

        private void InputCancelButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            HideModal();
        }

        private void InputConfirmButtonPressed()
        {
            string text = inputTextField.value;
            SoundManager.Singleton.PlayMenuClick();
            HideModal();
            if (onInputConfirm != null)
            {
                onInputConfirm(text);
            }
        }

        private void DefaultMessageConfirmCallback() 
        {

        }

        private void DefaultInputCancelCallback()
        {

        }

        private void DefaultInputConfirmCallback(string input)
        {

        }

        private void SetModalType(VisualElement newModal)
        {
            currentModal.style.display = DisplayStyle.None;
            newModal.style.display = DisplayStyle.Flex;
            currentModal = newModal;
        }
    }
}
