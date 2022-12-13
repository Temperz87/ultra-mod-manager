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
        /// <summary>
        /// Gets the KeyCode associated with the keybind
        /// </summary>
        public KeyCode keyBind { get; internal set; }
        
        /// <summary>
        /// Gets the name associated with the bind
        /// </summary>
        public string bindName { get; internal set; }
        
        /// <summary>
        /// A UnityEvent that fires when the button is pressed
        /// </summary>
        public UnityEvent onPress = new UnityEvent();
        
        /// <summary>
        /// A UnityEvent that fires when the button is pressed and the player is in a playable scene
        /// </summary>
        public UnityEvent onPerformInScene = new UnityEvent();
        
        /// <summary>
        /// Whether or not the keybind is currently being used by a mod
        /// </summary>
        internal bool enabled = false;


        /// <summary>
        /// Gets whether or not the keybind is pressed and the player is in a playable scene
        /// </summary>
        public bool IsPressedInScene
        {
            get
            {
                return IsPressed && !MonoSingleton<OptionsManager>.Instance.paused && !MonoSingleton<NewMovement>.Instance.dead && HudController.Instance != null;
            }
        }

        /// <summary>
        /// Gets whether or not the keybind was pressed in this frame and the player is in a playable scene.
        /// </summary>
        public bool WasPerformedThisFrameInScene
        {
            get
            {
                return WasPerformedThisFrame && !MonoSingleton<OptionsManager>.Instance.paused && !MonoSingleton<NewMovement>.Instance.dead && HudController.Instance != null;
            }
        }

        internal UKKeyBind(InputAction action, string BindName, KeyCode KeyBind) : base(action)
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

        internal void ChangeKeyBind(KeyCode bind)
        {
            ButtonControl buttonControl;
            if (LegacyInput.current.TryGetButton(bind, out buttonControl))
            {
                Action.ChangeBinding(0).WithPath(buttonControl.path);
                this.keyBind = bind;
            }
        }

        internal void CheckEvents()
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
