using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cortex.ColorExtensionMethods;
using Rainbow;
using Rainbow.Model;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

namespace Cortex
{
    /// <summary>
    /// UI element to display a contact in a list or something similar
    /// </summary>
    public class ContactEntry : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// Fired when this entry is clicked
        /// </summary>
        public event Action<ContactEntry> OnClick;

        private Contact contact;

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
                displayName.text = Util.GetContactDisplayName(contact);
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

        [SerializeField]
        private TMP_Text displayName;

        [SerializeField]
        private Image status;

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
        private ConfirmationDialog confirmationDialog; // Reference to the ConfirmationDialog



        void Awake()
        {
            if (displayName == null)
            {
                displayName = GameObjectUtils.FindGameObjectByName(transform, "DisplayName", true).GetComponent<TMP_Text>();
            }

            if (status == null)
            {
                status = GameObjectUtils.FindGameObjectByName(transform, "Status", true).GetComponent<Image>();
            }

            if (ContactInitialsAvatar == null)
            {
                ContactInitialsAvatar = GameObjectUtils.FindGameObjectByName(transform, "ContactInitialsAvatar", true).GetComponent<ContactInitialsAvatar>();
            }

            Background = GetComponent<Image>();
            normalBgColor = Background.color;
            darkBgColor = Background.color.Darken();
        }

        /// <summary>
        /// Sets the presence level of the contact
        /// </summary>
        /// <param name="presenceLevel">The presence level. This must be a value from Rainbow.Model.PresenceLevel</param>
        public void SetPresenceLevel(string presenceLevel)
        {
            if (presenceLevel == PresenceLevel.Online)
            {
                status.color = Color.green;
            }
            else if (presenceLevel == PresenceLevel.Offline)
            {
                status.color = Color.gray;
            }
            else if (presenceLevel == PresenceLevel.Away)
            {
                status.color = Color.yellow;
            }
            else if (presenceLevel == PresenceLevel.Busy)
            {
                status.color = Color.red;
            }
        }



        // Vagelis
        private void Start()
        {
            contactGameobject = GameObject.FindGameObjectWithTag("Contacts");

            rainbowGameobject = GameObject.Find("Rainbow");
            confirmationDialog = rainbowGameobject.GetComponent<ConfirmationDialog>();

            //// Check if this component exists, which means it is a contact entry in the contacts list and so it has a button to remove contact
            //if (GetComponent<ContactGameobject>() == null)
            //{
            //    gameObject.GetNamedChild("RemoveContactButton").GetComponent<Button>().onClick.AddListener(() =>
            //    {
            //        string confirmationMessage = $"Are you sure you want to remove {Util.GetContactDisplayName(contact)} from your contacts?";
            //        confirmationDialog.Show(confirmationMessage, () => rainbowGameobject.GetComponent<ConversationsManager>().RemoveContact(contact.Id));
            //    });
            //}


            //Check if this button exists, which means it is a contact entry in the contacts list and so it has a button to remove contact
            var removeContactButton = gameObject.GetNamedChild("RemoveContactButton");
            if (removeContactButton != null)
            {
                removeContactButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    string confirmationMessage = $"Are you sure you want to remove {Util.GetContactDisplayName(contact)} from your contacts?";
                    confirmationDialog.Show(confirmationMessage, () => rainbowGameobject.GetComponent<ConversationsManager>().RemoveContact(contact.Id));
                });
            }

        }



        public void OnPointerClick(PointerEventData eventData)
        {
            // Check if this component exists, bcz if it doesn't that means this is the prefab for search contacts panel. It's for new New Way To Display Contact Entry region in ConversationsManager
            if (GetComponent<ContactGameobject>() == null) 
            {
                OnClick?.Invoke(this);
                eventData.Use();

                // Vagelis
                contactGameobject.SetActive(false);
            }
        }
    }
} // end namespace Cortex