﻿using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using UnityEngine;

namespace CSUR_UI.UI
{
    public class OptionsKeymappingLane : UICustomControl
    {
        private static readonly string kKeyBindingTemplate = "KeyBindingTemplate";

        public static readonly SavedInputKey m_2L1PL = new SavedInputKey("2L1PL",
            "CSUR_UI_SETTING", SavedInputKey.Encode(KeyCode.None, false, false, false), true);
        public static readonly SavedInputKey m_1L0PL = new SavedInputKey("1L0PL",
            "CSUR_UI_SETTING", SavedInputKey.Encode(KeyCode.None, false, false, false), true);
        public static readonly SavedInputKey m_C0PR = new SavedInputKey("C0PR",
            "CSUR_UI_SETTING", SavedInputKey.Encode(KeyCode.None, false, false, false), true);
        public static readonly SavedInputKey m_1R1PR = new SavedInputKey("1R1PR",
            "CSUR_UI_SETTING", SavedInputKey.Encode(KeyCode.None, false, false, false), true);
        public static readonly SavedInputKey m_2R2PR = new SavedInputKey("2R2PR",
            "CSUR_UI_SETTING", SavedInputKey.Encode(KeyCode.None, false, false, false), true);
        public static readonly SavedInputKey m_3R3PR = new SavedInputKey("3R3PR",
            "CSUR_UI_SETTING", SavedInputKey.Encode(KeyCode.None, false, false, false), true);
        public static readonly SavedInputKey m_4R4PR = new SavedInputKey("4R4PR",
            "CSUR_UI_SETTING", SavedInputKey.Encode(KeyCode.None, false, false, false), true);
        public static readonly SavedInputKey m_5R5PR = new SavedInputKey("5R5PR",
            "CSUR_UI_SETTING", SavedInputKey.Encode(KeyCode.None, false, false, false), true);
        public static readonly SavedInputKey m_6R6PR = new SavedInputKey("6R6PR",
            "CSUR_UI_SETTING", SavedInputKey.Encode(KeyCode.None, false, false, false), true);
        public static readonly SavedInputKey m_7R = new SavedInputKey("7R",
            "CSUR_UI_SETTING", SavedInputKey.Encode(KeyCode.None, false, false, false), true);

        private int count;

        private SavedInputKey m_EditingBinding;

        private string m_EditingBindingCategory;

        private void Awake()
        {
            AddKeymapping("2L/1PL", m_2L1PL);
            AddKeymapping("1L/0PL", m_1L0PL);
            AddKeymapping("C/0P", m_C0PR);
            AddKeymapping("1R/1PR", m_1R1PR);
            AddKeymapping("2R/2PR", m_2R2PR);
            AddKeymapping("3R/3PR", m_3R3PR);
            AddKeymapping("4R/4PR", m_4R4PR);
            AddKeymapping("5R/5PR", m_5R5PR);
            AddKeymapping("6R/6PR", m_6R6PR);
            AddKeymapping("7R", m_7R);
        }

        private void AddKeymapping(string label, SavedInputKey savedInputKey)
        {
            var uIPanel =
                component.AttachUIComponent(UITemplateManager.GetAsGameObject(kKeyBindingTemplate)) as UIPanel;
            if (count++ % 2 == 1) uIPanel.backgroundSprite = null;

            var uILabel = uIPanel.Find<UILabel>("Name");
            var uIButton = uIPanel.Find<UIButton>("Binding");
            uIButton.eventKeyDown += OnBindingKeyDown;
            uIButton.eventMouseDown += OnBindingMouseDown;

            uILabel.text = label;
            uIButton.text = savedInputKey.ToLocalizedString("KEYNAME");
            uIButton.objectUserData = savedInputKey;
        }

        private void OnEnable()
        {
            LocaleManager.eventLocaleChanged += OnLocaleChanged;
        }

        private void OnDisable()
        {
            LocaleManager.eventLocaleChanged -= OnLocaleChanged;
        }

        private void OnLocaleChanged()
        {
            RefreshBindableInputs();
        }

        private bool IsModifierKey(KeyCode code)
        {
            return code == KeyCode.LeftControl || code == KeyCode.RightControl || code == KeyCode.LeftShift ||
                   code == KeyCode.RightShift || code == KeyCode.LeftAlt || code == KeyCode.RightAlt;
        }

        private bool IsControlDown()
        {
            return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        }

        private bool IsShiftDown()
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }

        private bool IsAltDown()
        {
            return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        }

        private bool IsUnbindableMouseButton(UIMouseButton code)
        {
            return code == UIMouseButton.Left || code == UIMouseButton.Right;
        }

        private KeyCode ButtonToKeycode(UIMouseButton button)
        {
            if (button == UIMouseButton.Left) return KeyCode.Mouse0;
            if (button == UIMouseButton.Right) return KeyCode.Mouse1;
            if (button == UIMouseButton.Middle) return KeyCode.Mouse2;
            if (button == UIMouseButton.Special0) return KeyCode.Mouse3;
            if (button == UIMouseButton.Special1) return KeyCode.Mouse4;
            if (button == UIMouseButton.Special2) return KeyCode.Mouse5;
            if (button == UIMouseButton.Special3) return KeyCode.Mouse6;
            return KeyCode.None;
        }

        private void OnBindingKeyDown(UIComponent comp, UIKeyEventParameter p)
        {
            if (m_EditingBinding != null && !IsModifierKey(p.keycode))
            {
                p.Use();
                UIView.PopModal();
                var keycode = p.keycode;
                var inputKey = p.keycode == KeyCode.Escape
                    ? m_EditingBinding.value
                    : SavedInputKey.Encode(keycode, p.control, p.shift, p.alt);
                if (p.keycode == KeyCode.Backspace) inputKey = SavedInputKey.Empty;
                m_EditingBinding.value = inputKey;
                var uITextComponent = p.source as UITextComponent;
                uITextComponent.text = m_EditingBinding.ToLocalizedString("KEYNAME");
                m_EditingBinding = null;
                m_EditingBindingCategory = string.Empty;
            }
        }

        private void OnBindingMouseDown(UIComponent comp, UIMouseEventParameter p)
        {
            if (m_EditingBinding == null)
            {
                p.Use();
                m_EditingBinding = (SavedInputKey)p.source.objectUserData;
                m_EditingBindingCategory = p.source.stringUserData;
                var uIButton = p.source as UIButton;
                uIButton.buttonsMask = UIMouseButton.Left | UIMouseButton.Right | UIMouseButton.Middle |
                                       UIMouseButton.Special0 | UIMouseButton.Special1 | UIMouseButton.Special2 |
                                       UIMouseButton.Special3;
                uIButton.text = "Press any key";
                p.source.Focus();
                UIView.PushModal(p.source);
            }
            else if (!IsUnbindableMouseButton(p.buttons))
            {
                p.Use();
                UIView.PopModal();
                var inputKey = SavedInputKey.Encode(ButtonToKeycode(p.buttons), IsControlDown(), IsShiftDown(),
                    IsAltDown());

                m_EditingBinding.value = inputKey;
                var uIButton2 = p.source as UIButton;
                uIButton2.text = m_EditingBinding.ToLocalizedString("KEYNAME");
                uIButton2.buttonsMask = UIMouseButton.Left;
                m_EditingBinding = null;
                m_EditingBindingCategory = string.Empty;
            }
        }

        private void RefreshBindableInputs()
        {
            foreach (var current in component.GetComponentsInChildren<UIComponent>())
            {
                var uITextComponent = current.Find<UITextComponent>("Binding");
                if (uITextComponent != null)
                {
                    var savedInputKey = uITextComponent.objectUserData as SavedInputKey;
                    if (savedInputKey != null) uITextComponent.text = savedInputKey.ToLocalizedString("KEYNAME");
                }

                var uILabel = current.Find<UILabel>("Name");
                if (uILabel != null) uILabel.text = Locale.Get("KEYMAPPING", uILabel.stringUserData);
            }
        }

        internal InputKey GetDefaultEntry(string entryName)
        {
            var field = typeof(DefaultSettings).GetField(entryName, BindingFlags.Static | BindingFlags.Public);
            if (field == null) return 0;
            var value = field.GetValue(null);
            if (value is InputKey key) return key;
            return 0;
        }

        private void RefreshKeyMapping()
        {
            foreach (var current in component.GetComponentsInChildren<UIComponent>())
            {
                var uITextComponent = current.Find<UITextComponent>("Binding");
                var savedInputKey = (SavedInputKey)uITextComponent.objectUserData;
                if (m_EditingBinding != savedInputKey)
                    uITextComponent.text = savedInputKey.ToLocalizedString("KEYNAME");
            }
        }
    }
}