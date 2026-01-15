using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// 添加标签对话框 - 共享组件
    /// </summary>
    public class AddTagDialog : EditorWindow
    {
        private TextField tagNameField;
        private System.Action<string> onConfirm;

        public static void Show(string title, string message, System.Action<string> onConfirm)
        {
            var window = CreateInstance<AddTagDialog>();
            window.Initialize(title, message, onConfirm);
            window.ShowModal();
        }

        public void Initialize(string title, string message, System.Action<string> onConfirm)
        {
            this.titleContent = new GUIContent(title);
            this.onConfirm = onConfirm;
            this.minSize = new Vector2(400, 120);
            this.maxSize = new Vector2(400, 120);

            var root = rootVisualElement;
            root.style.paddingTop = 10;
            root.style.paddingBottom = 10;
            root.style.paddingLeft = 10;
            root.style.paddingRight = 10;

            var messageLabel = new Label(message);
            messageLabel.style.marginBottom = 10;
            messageLabel.style.whiteSpace = WhiteSpace.Normal;
            root.Add(messageLabel);

            tagNameField = new TextField("Tag Name:");
            tagNameField.style.marginBottom = 10;
            tagNameField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    Confirm();
                    evt.StopPropagation();
                }
                else if (evt.keyCode == KeyCode.Escape)
                {
                    Close();
                    evt.StopPropagation();
                }
            });
            root.Add(tagNameField);

            var buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.Row;
            buttonContainer.style.justifyContent = Justify.FlexEnd;

            var addButton = new Button(Confirm) { text = "Add" };
            addButton.style.width = 80;
            addButton.style.marginRight = 5;
            buttonContainer.Add(addButton);

            var cancelButton = new Button(() => Close()) { text = "Cancel" };
            cancelButton.style.width = 80;
            buttonContainer.Add(cancelButton);

            root.Add(buttonContainer);

            tagNameField.Focus();
        }

        private void Confirm()
        {
            if (!string.IsNullOrEmpty(tagNameField.value))
            {
                onConfirm?.Invoke(tagNameField.value);
            }
            Close();
        }
    }
}

