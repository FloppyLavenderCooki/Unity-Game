using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Object = UnityEngine.Object;

namespace UI
{
    public class EmailUI : MonoBehaviour
    {
        public int emailNum;
        private VisualElement _root;
        private VisualElement _cursor;
        private Label _content;
        private VisualElement _attachment;
        private Label _attachmentName;
        private InputAction _escapeAction;
        
        private void Start()
        {
            _escapeAction = InputSystem.actions.FindAction("Escape");
            
            var uiDocument = GetComponent<UIDocument>();
            if (!uiDocument) return;
            
            _root = uiDocument.rootVisualElement;
            _cursor = _root.Q<VisualElement>("cursor");
            _cursor.pickingMode = PickingMode.Ignore;
            
            _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            
            _content = _root.Q<Label>("content");
            Object emailContent = Resources.Load($"Emails/email{emailNum}");
            string currentTime = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToString("hh:mm:ss tt").ToUpper()}";
            _content.text = emailContent.ToString().Replace("[current time]", currentTime);
            
            _attachment = _root.Q<VisualElement>("attachment");
            _attachmentName = _root.Q<Label>("attachment-name");

            switch (emailNum)
            {
                case 1:
                    _attachment.style.display = DisplayStyle.Flex;
                    _attachmentName.text = "Books.txt";
                    break;
                default:
                    _attachment.style.display = DisplayStyle.None;
                    _attachmentName.text = "";
                    break;
            }
        }

        private void Update()
        {
            if (_escapeAction.WasPressedThisFrame()) LoadingUI.LoadScene("Scenes/Main Menu");
        }
        
        private void OnPointerMove(PointerMoveEvent e)
        {
            _root.CapturePointer(e.pointerId);
            
            Vector2 pos = e.localPosition;
            if (float.IsNaN(pos.x) || float.IsNaN(pos.y))
            {
                Cursor.visible = true;
                _cursor.style.visibility = Visibility.Hidden;
            }
            else
            {
                Cursor.visible = false;
                _cursor.style.visibility = Visibility.Visible;
                _cursor.style.translate = new Translate(pos.x, pos.y, 0);
            }
        }
    }
}
