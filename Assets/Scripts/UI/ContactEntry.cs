using System;
using System.Collections;
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
        private Bubble currentBubble; // Add this to store the current bubble context

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

        // Add property to set the current bubble context
        public Bubble CurrentBubble
        {
            get { return currentBubble; }
            set { currentBubble = value; }
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

        // Add references for the member management UI
        [SerializeField]
        private GameObject memberManagementButton;
        [SerializeField]
        private GameObject memberManagementMenu;
        [SerializeField]
        private Button makeModeratorButton;
        [SerializeField]
        private Button makeUserButton;
        [SerializeField]
        private Button removeMemberButton;

        /// <summary>
        /// Gets/Sets whether the current entry is selected or not
        /// </summary>
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                Background.color = _selected ? darkBgColor : normalBgColor;
            }
        }

        // Vagelis
        GameObject contactGameobject;
        GameObject rainbowGameobject;
        private ConfirmationDialog confirmationDialog; // Reference to the ConfirmationDialog
        private BubbleManager bubbleManager;

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

            // Initialize member management UI references
            if (memberManagementButton != null)
            {
                memberManagementButton.GetComponent<Button>().onClick.AddListener(OnMemberManagementButtonClick);
            }

            if (makeModeratorButton != null)
            {
                makeModeratorButton.onClick.AddListener(() => UpdateMemberRole(Bubble.MemberPrivilege.Moderator));
            }

            if (makeUserButton != null)
            {
                makeUserButton.onClick.AddListener(() => UpdateMemberRole(Bubble.MemberPrivilege.User));
            }

            if (removeMemberButton != null)
            {
                removeMemberButton.onClick.AddListener(OnRemoveMemberClick);
            }
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
            bubbleManager = rainbowGameobject.GetComponent<BubbleManager>();
            currentBubble = bubbleManager.currentSelectedBubble;
            // Show/hide member management button based on whether this is a bubble member entry
            if (memberManagementButton != null)
            {
                memberManagementButton.SetActive(currentBubble != null);
            }

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
                memberManagementButton.SetActive(false); // hide menuButton when contact entry is on contacts list and not in bubble

                removeContactButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    string confirmationMessage = $"Are you sure you want to remove {Util.GetContactDisplayName(contact)} from your contacts?";
                    confirmationDialog.Show(confirmationMessage, () => rainbowGameobject.GetComponent<ConversationsManager>().RemoveContact(contact.Id));
                });
            }
        }

        private void OnMemberManagementButtonClick()
        {
            if (memberManagementMenu != null)
            {
                memberManagementMenu.SetActive(!memberManagementMenu.activeSelf);
            }
        }

        private void UpdateMemberRole(string newPrivilege)
        {
            Debug.Log("UpdateMemberRole called " + currentBubble + " " + contact);
            if (currentBubble != null && contact != null)
            {
                Debug.Log("UpdateMemberRole called 2");
                Debug.Log($"Attempting to change {contact.DisplayName}'s role to {newPrivilege} in bubble {currentBubble.Name}");
                
                // Get current privilege before change
                string currentPrivilege = currentBubble.UsersById[contact.Id].Privilege;
                Debug.Log($"Current privilege: {currentPrivilege}");

                bubbleManager.UpdateMemberRole(currentBubble, contact, newPrivilege);
                
                // Add a small delay to check the new privilege after the update
                StartCoroutine(CheckPrivilegeAfterUpdate(currentPrivilege, newPrivilege));
                
                if (memberManagementMenu != null)
                {
                    memberManagementMenu.SetActive(false);
                }
            }
            else
            {             
                Debug.LogWarning("Cannot update member role: currentBubble or contact is null");
            }
        }

        private IEnumerator CheckPrivilegeAfterUpdate(string oldPrivilege, string expectedNewPrivilege)
        {
            // Wait for a short time to allow the update to complete
            yield return new WaitForSeconds(0.5f);

            if (currentBubble != null && contact != null && currentBubble.UsersById.ContainsKey(contact.Id))
            {
                string actualNewPrivilege = currentBubble.UsersById[contact.Id].Privilege;
                Debug.Log($"Privilege change result for {contact.DisplayName}:");
                Debug.Log($"Old privilege: {oldPrivilege}");
                Debug.Log($"Expected new privilege: {expectedNewPrivilege}");
                Debug.Log($"Actual new privilege: {actualNewPrivilege}");
                
                if (actualNewPrivilege == expectedNewPrivilege)
                {
                    Debug.Log("Privilege change successful!");
                }
                else
                {
                    Debug.LogWarning("Privilege change may have failed - actual privilege doesn't match expected");
                }
            }
            else
            {
                Debug.LogWarning("Could not verify privilege change - member or bubble data not available");
            }
        }

        private void OnRemoveMemberClick()
        {
            if (currentBubble != null && contact != null)
            {
                string confirmationMessage = $"Are you sure you want to remove {Util.GetContactDisplayName(contact)} from this bubble?";
                confirmationDialog.Show(confirmationMessage, () => 
                {
                    bubbleManager.RemoveMemberFromBubble(currentBubble, contact);
                    if (memberManagementMenu != null)
                    {
                        memberManagementMenu.SetActive(false);
                    }
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