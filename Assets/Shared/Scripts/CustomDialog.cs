using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

namespace HyperCasual.Runner
{
    public class CustomDialog : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_TitleText;
        [SerializeField] private TextMeshProUGUI m_MessageText;
        [SerializeField] private TMP_InputField m_InputField;
        [SerializeField] private GameObject m_ButtonsContainer;
        [SerializeField] private Button m_PositiveButton;
        [SerializeField] private Button m_NegativeButton;
        [SerializeField] private TextMeshProUGUI m_PositiveButtonText;
        [SerializeField] private TextMeshProUGUI m_NegativeButtonText;

        private UniTaskCompletionSource<(bool, string)> m_DialogTaskCompletionSource;

        /// <summary>
        /// Displays the dialog with a title, message, and an optional input field.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message of the dialog.</param>
        /// <param name="positiveButtonText">The text for the positive button.</param>
        /// <param name="negativeButtonText">The text for the negative button (optional).</param>
        /// <param name="showInputField">Whether to show the input field.</param>
        /// <returns>A tuple containing a bool (true if positive button was clicked) and a string (input field value).</returns>
        public async UniTask<(bool, string)> ShowDialog(string title, string message, string positiveButtonText, string negativeButtonText = null, bool showInputField = false)
        {
            m_TitleText.text = title;
            m_MessageText.text = message;
            m_PositiveButtonText.text = positiveButtonText;

            if (!string.IsNullOrEmpty(negativeButtonText))
            {
                m_NegativeButtonText.text = negativeButtonText;
                m_NegativeButton.gameObject.SetActive(true);
            }
            else
            {
                m_NegativeButton.gameObject.SetActive(false);
            }

            m_InputField.text = null;
            m_InputField.gameObject.SetActive(showInputField);

            m_DialogTaskCompletionSource = new UniTaskCompletionSource<(bool, string)>();

            gameObject.SetActive(true);

            m_PositiveButton.onClick.AddListener(OnPositiveClicked);
            m_NegativeButton.onClick.AddListener(OnNegativeClicked);

            return await m_DialogTaskCompletionSource.Task;
        }

        /// <summary>
        /// Handles the positive button click event.
        /// </summary>
        private void OnPositiveClicked()
        {
            string inputValue = m_InputField.gameObject.activeSelf ? m_InputField.text : string.Empty;
            CloseDialog(true, inputValue);
        }

        /// <summary>
        /// Handles the negative button click event.
        /// </summary>
        private void OnNegativeClicked()
        {
            CloseDialog(false, string.Empty);
        }

        /// <summary>
        /// Closes the dialog and returns the result.
        /// </summary>
        /// <param name="result">True if positive button was clicked, false if negative button was clicked.</param>
        /// <param name="inputValue">The value of the input field, or an empty string if not shown.</param>
        private void CloseDialog(bool result, string inputValue)
        {
            m_PositiveButton.onClick.RemoveListener(OnPositiveClicked);
            m_NegativeButton.onClick.RemoveListener(OnNegativeClicked);

            gameObject.SetActive(false);

            m_DialogTaskCompletionSource.TrySetResult((result, inputValue));
        }
    }
}
