using NewBlood;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace UMM
{
    public class UKKeyBind : InputActionState
    {
        public KeyCode keyBind { get; internal set; }
        public string bindName { get; internal set; }

        public UnityEvent onPress = new UnityEvent();
        public UnityEvent onPerformInScene = new UnityEvent();
        
        internal bool enabled = false;

        public bool IsPressedInScene
        {
            get
            {
                return IsPressed && !MonoSingleton<OptionsManager>.Instance.paused && !MonoSingleton<NewMovement>.Instance.dead && HudController.Instance != null;
            }
        }

        public bool WasPerformedThisFrameInScene
        {
            get
            {
                return WasPerformedThisFrame && !MonoSingleton<OptionsManager>.Instance.paused && !MonoSingleton<NewMovement>.Instance.dead && HudController.Instance != null;
            }
        }

        public UKKeyBind(InputAction action, string BindName, KeyCode KeyBind) : base(action)
        {
            this.bindName = BindName;
            this.keyBind = KeyBind;
            if (LegacyInput.current.TryGetButton(KeyBind, out ButtonControl buttonControl))
            {
                this.Action.AddBinding(buttonControl.path, null, null, null);
                this.Action.Enable();
            }
            else
                throw new ArgumentException("Couldn't find button control for keybind " + BindName + " of bind " + KeyBind);
        }

        public void ChangeKeyBind(KeyCode bind)
        {
            ButtonControl buttonControl;
            if (LegacyInput.current.TryGetButton(bind, out buttonControl))
            {
                Action.ChangeBinding(0).WithPath(buttonControl.path);
                this.keyBind = bind;
            }
        }

        public void CheckEvents()
        {
            if (WasPerformedThisFrameInScene)
            {
                onPerformInScene.Invoke();
                onPress.Invoke();
            }
            else if (WasPerformedThisFrame)
                onPress.Invoke();
        }
    }
}
