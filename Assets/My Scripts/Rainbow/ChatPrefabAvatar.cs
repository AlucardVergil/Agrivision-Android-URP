using System;
using System.Threading;
using Cortex.ColorExtensionMethods;
using Rainbow;
using Rainbow.Model;
using Rainbow.WebRTC.Unity;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cortex
{
    /// <summary>
    /// UI element to display a contact in a list or something similar
    /// </summary>
    public class ChatPrefabAvatar : MonoBehaviour
    {
        private Contact contact;

        public RawImage imageGameobject;

        /// <summary>
        /// Gets/Sets the contact associated with this entry
        /// </summary>
        public Contact Contact
        {
            get
            {
                return contact;
            }

            set
            {
                contact = value;
                ContactInitialsAvatar.Contact = contact;
            }
        }

        /// <summary>
        /// Combined avatar and initials of the contact
        /// </summary>
        public ContactInitialsAvatar ContactInitialsAvatar
        {
            get => m_contactInitialsAvatar;
            private set => m_contactInitialsAvatar = value;
        }

        // Backing field for property ContactInitialsAvatar
        [SerializeField]
        private ContactInitialsAvatar m_contactInitialsAvatar;
        
        private Image Background;
        private Color normalBgColor;
        private Color darkBgColor;
        private bool _selected;

        /// <summary>
        /// Gets/Sets whether the current entry is selected or not
        /// </summary>
        public bool Selected
        {
            get => _selected; set
            {
                _selected = value;

                if (_selected)
                {
                    Background.color = darkBgColor;
                }
                else
                {
                    Background.color = normalBgColor;
                }
            }
        }



        // Vagelis
        GameObject contactGameobject;
        GameObject rainbowGameobject;



        void Awake()
        {
            //if (ContactInitialsAvatar == null)
            //{
            //    ContactInitialsAvatar = GameObjectUtils.FindGameObjectByName(transform, "ContactInitialsAvatar", true).GetComponent<ContactInitialsAvatar>();
            //}

            //Background = GetComponent<Image>();
            //normalBgColor = Background.color;
            //darkBgColor = Background.color.Darken();
        }


        // Vagelis
        private void Start()
        {
            contactGameobject = GameObject.FindGameObjectWithTag("Contacts");

            rainbowGameobject = GameObject.Find("Rainbow");
        }
        
    }
} // end namespace Cortex