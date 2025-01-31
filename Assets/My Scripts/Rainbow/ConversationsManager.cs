using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rainbow;
using Rainbow.Model;
using Rainbow.Events;
using System;
using TMPro;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Xml;
using Cortex;
using Unity.VisualScripting;
using System.Diagnostics.Contracts;
using Unity.XR.CoreUtils;
using Rainbow.WebRTC.Unity;
using System.Threading;

public class ConversationsManager : MonoBehaviour
{
    private Rainbow.Application rbApplication;
    private InstantMessaging instantMessaging;
    private Conversations rbConversations;
    private Contacts rbContacts;
    private Invitations rbInvitations;

    //Contact prefab and target parent
    public GameObject contactPrefab;
    public Transform contactScrollViewContent;

    //Conversation Content Area
    public TMP_Text conversationContentArea;
    public Transform conversationScrollViewContent;


    public TMP_InputField messageInputField;
    public Conversation currentSelectedConversation;

    private bool initializationPerformedFlag = false;
    private byte[] avatarData;
    private Image[] avatarImage;
    private Contact[] tempContacts;
    private byte[][] tempAvatarData;
    private int tempCount = 0;
    private bool doOnce = true;

    private bool doOnceRefreshTextArea = false;

    public TMP_Text isTypingTextArea;
    private bool doOnceRefreshIsTypingTextArea = false;

    public GameObject chatMessagePrefab;
    public GameObject chatMessagePrefabMyself;
    private Image currentChatMessageAvatar;

    private bool[] alreadyFetchedMessagesForThisContactOnce;
    private GameObject[] parentForAllMessagesOfEachContact;

    public GameObject conversationScrollView;

    [Header("For Search Contact Panel")]
    public GameObject searchedContactsScrollviewContent;
    public TMP_InputField searchContactInputField;
    public List<Contact> contactsToInvite; 
    public GameObject searchContactsPanel;

    // Bcz the search panel is the same for both add to contacts and add to bubbles, when you click the open search panel button, this bool becomes true if it was opened from
    // bubbles or false if opened from contacts, so that when i hit the send invitation button it knows whether to send invite for contacts or for bubbles
    private bool isSearchPanelOpenedFromBubbles; 

    public GameObject selectedContactToAddContent;

    private ConnectionModel model;

    [Header("Notifications Center")]    
    public GameObject notificationsPanel;
    public GameObject notificationsContent;
    public Button notificationsButton;
    public TMP_Text notificationsCountText;
    public GameObject notificationPrefab;


    #region New Way To Display Contact Entry
    private readonly ResetCancellationToken cancelLoad = new();
    [HideInInspector] public RainbowAvatarLoader AvatarLoader;
    #endregion

    [Header("Contact List Panel")]
    public GameObject contactListPanel;


    [HideInInspector] public bool wasDisconnectedFromSearchContactByName = false;



    public void InitializeConversationsAndContacts() // Probably will need to assign the variables in the other function bcz they are called too early and not assigned (TO CHECK)
    {
        if (model != null) return;

        model = ConnectionModel.Instance;

        instantMessaging = model.InstantMessaging;
        rbConversations = model.Conversations;
        rbContacts = model.Contacts;
        rbInvitations = model.Invitations;

        
        /*
        rbApplication = RainbowManager.Instance.GetRainbowApplication();

        instantMessaging = rbApplication.GetInstantMessaging();
        rbConversations = rbApplication.GetConversations();
        rbContacts = rbApplication.GetContacts();
        */

        // Attach event listeners for message receipt and typing status
        instantMessaging.MessageReceived += MyApp_MessageReceived;
        instantMessaging.ReceiptReceived += MyApp_ReceiptReceived;
        instantMessaging.UserTypingChanged += MyApp_UserTypingChanged;

        rbContacts.PeerInfoChanged += MyApp_PeerInfoChanged;
        rbContacts.PeerAvatarChanged += MyApp_PeerAvatarChanged;
        rbContacts.PeerAvatarDeleted += MyApp_PeerAvatarDeleted;

        rbContacts.RosterPeerAdded += MyApp_RosterPeerAdded;
        rbContacts.RosterPeerRemoved += MyApp_RosterPeerRemoved;

        rbInvitations.InvitationReceived += MyApp_RosterInvitationReceived;
        rbInvitations.InvitationSent += MyApp_RosterInvitationSent;
        rbInvitations.InvitationAccepted += MyApp_RosterInvitationAccepted;
        rbInvitations.InvitationDeclined += MyApp_RosterInvitationDeclined;
        rbInvitations.InvitationCancelled += MyApp_RosterInvitationCancelled;
        rbInvitations.InvitationDeleted += MyApp_RosterInvitationDeleted;


        FetchAllConversations();
        //FetchAllContactsInRoster();



        List<string> emails = new List<string>();
        emails.Add("@gmail.com");

        // Display searched contacts results when you stop tryping and click outside the input field
        searchContactInputField.onEndEdit.AddListener((string nameToSearch) =>
        {
            if (nameToSearch != "" && nameToSearch.Length > 1)
            {
                if (nameToSearch.Contains("@")) // If input is email
                {
                    emails[0] = nameToSearch;
                    SearchContactByEmails(emails);
                }
                else
                {
                    SearchContactByName(nameToSearch);
                }
                    
            }                
        });

        contactsToInvite = new List<Contact>();



        rbInvitations.GetReceivedInvitations(callback =>
        {
            if (callback.Result.Success)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    List<Invitation> notificationsList = callback.Data;
                    notificationsCountText.text = notificationsList.Count.ToString();

                    Debug.Log($"Notification Count= {notificationsList.Count}");

                    foreach (Invitation invitation in notificationsList)
                    {
                        var notification = Instantiate(notificationPrefab, notificationsContent.transform);
                        notification.GetComponent<NotificationGameobject>().currentContactInvitation = invitation;
                    }
                });
            }
            else
            {
                HandleError(callback.Result);
            }

        });
    }


    public void EnableDisableNotificationsPanel()
    {
        if (notificationsPanel.activeSelf)
        {
            notificationsPanel.SetActive(false);
        }
        else
        {
            notificationsPanel.SetActive(true);
            notificationsCountText.text = "0";
        }
    }



    private async void Update()
    {
        // Force refresh conversation text area. When the text was assigned it didn't refresh so i used this to force refresh it
        if (doOnceRefreshTextArea)
        {
            doOnceRefreshTextArea = false;

            await Task.Delay(200); // Placed delay to give it a little time to first update the conversation text and then refresh the conversation area object

            StartCoroutine(RefreshConversationTextArea());
        }


        if (doOnceRefreshIsTypingTextArea)
        {
            doOnceRefreshIsTypingTextArea = false;

            await Task.Delay(50);

            StartCoroutine(RefreshIsTypingTextArea());
        }




        // This is to inform when initialization is performed and execute the function from Update() because when I run the coroutine from
        // the event handler RbApplication_InitializationPerformed or the callback method (not sure which), it didn't execute due to threading issues bcz it
        // wasn't executed in the main thread and coroutines need to execute only in the main thread.

        // When i called the FetchCurrentProfile() from the event handler it didn't execute the coroutine or the rest of the code. When i called the entire
        //FetchCurrentProfile() from the Update() it didn't execute the coroutine but executed the rest of the code in FetchCurrentProfile() and when i called
        //the FetchCurrentProfile() from the event handler but only called the coroutine from the Update() it worked correctly.
        if (initializationPerformedFlag)
        {
            if (doOnce)
            {
                doOnce = false;
                for (int i = 0; i < tempContacts.Length; i++)
                {
                    Debug.Log("GetContactAvatar = " + i);
                    GetContactAvatar(tempContacts[i].Id);

                    await Task.Delay(500); // Wait for 500ms
                }
            }




            if (tempCount >= tempContacts.Length)
            {
                for (int i = 0; i < tempContacts.Length; i++)
                {
                    Debug.Log("HandleAvatarData = " + i);

                    if (tempAvatarData[i] != null) // If avatar image data exists
                        HandleAvatarData(tempAvatarData[i], avatarImage[i]);

                    await Task.Delay(500); // Wait for 500ms
                }

                initializationPerformedFlag = false;
                tempCount = 0;
                doOnce = true;
            }



        }
    }



    // When the text was assigned it didn't refresh so i used this to force refresh it
    IEnumerator RefreshConversationTextArea()
    {
        //conversationContentArea.gameObject.SetActive(false);

        yield return new WaitForEndOfFrame();

        //conversationContentArea.gameObject.SetActive(true);

        //UpdateTextContentHeight();
    }



    // When the text was assigned it didn't refresh so i used this to force refresh it
    IEnumerator RefreshIsTypingTextArea()
    {
        isTypingTextArea.gameObject.SetActive(false);
        GetComponent<BubbleManager>().isTypingTextArea.gameObject.SetActive(false);

        yield return new WaitForEndOfFrame();

        isTypingTextArea.gameObject.SetActive(true);
        GetComponent<BubbleManager>().isTypingTextArea.gameObject.SetActive(true);
    }




    void UpdateTextContentHeight()
    {
        RectTransform contentRect = conversationScrollViewContent.GetComponent<RectTransform>();

        // Get the preferred height of the text (based on the current content)
        float newHeight = conversationContentArea.preferredHeight;

        // Update the content's RectTransform to match the new height of the text
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, newHeight);
    }




    void UpdateContentHeight()
    {
        RectTransform contentRect = contactScrollViewContent.GetComponent<RectTransform>();

        // Calculate the new height based on the number of children and their heights
        float totalHeight = 0f;

        totalHeight = 150 * contactScrollViewContent.childCount; // Accumulate the height of all contact children

        //foreach (RectTransform child in contactScrollViewContent)
        //{
        //    totalHeight += 150; // child.sizeDelta.y; // Accumulate the height of all contact children
        //}

        // Update the content's RectTransform to the new height
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
    }





    //// Event handler for message received
    //private void MyApp_MessageReceived(object sender, MessageEventArgs evt)
    //{
    //    Debug.Log($"Message received in conversation {evt.ConversationId}");
    //    // Handle received message
    //}

    // Event handler for receipt received
    private void MyApp_ReceiptReceived(object sender, ReceiptReceivedEventArgs evt)
    {
        Debug.Log($"Receipt received for message {evt.MessageId}: {evt.ReceiptType}");
    }

    //// Event handler for typing status
    //private void MyApp_UserTypingChanged(object sender, UserTypingEventArgs evt)
    //{
    //    Debug.Log($"{evt.ContactJid} is typing: {evt.IsTyping}");
    //}




    // Fetch all conversations
    public void FetchAllConversations()
    {
        //Conversations conversations = RainbowManager.Instance.GetRainbowApplication().GetConversations();

        rbConversations.GetAllConversations(callback =>
        {
            if (callback.Result.Success)
            {
                List<Conversation> list = callback.Data;  // List of conversations

                foreach (Conversation conversation in list)
                {
                    if (conversation.Type == Conversation.ConversationType.User)
                    {
                        Debug.Log("Peer-to-peer conversation.");
                    }
                    else if (conversation.Type == Conversation.ConversationType.Room)
                    {
                        Debug.Log("Room conversation.");
                    }
                }

            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }




    public Conversation OpenConversationWithContact(Contact contact)
    {
        //Conversations conversations = RainbowManager.Instance.GetRainbowApplication().GetConversations();
       
        return rbConversations.GetOrCreateConversationFromUserId(contact.Id);
    }


    public Conversation OpenConversationWithBubble(Bubble bubble)
    {
        //Conversations conversations = RainbowManager.Instance.GetRainbowApplication().GetConversations();

        return rbConversations.GetOrCreateConversationFromBubbleId(bubble.Id);
    }



    public void RemoveConversation(Conversation conversation)
    {
        //Conversations conversations = RainbowManager.Instance.GetRainbowApplication().GetConversations();

        rbConversations.RemoveFromConversations(conversation, callback =>
        {
            if (callback.Result.Success)
            {
                Debug.Log("Conversation removed.");
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }






    void CreateChatMessage(string messageText, bool isOwnMessage, string contactID, Texture texture = null)
    {
        Contact contact = rbContacts.GetContactFromContactId(contactID);

        // First check if contact can be found in roster for optimization, else look in server. No need to look for contact if it's my own message bcz there is no avatar image shown
        if (contact != null || isOwnMessage)
        {
            CreateChatEntry(messageText, isOwnMessage, contact, texture);
        }  
        else
        {
            rbContacts.GetContactFromContactIdFromServer(contactID, callback =>
            {
                CreateChatEntry(messageText, isOwnMessage, callback.Data, texture); // If contact is null it skips the avatar image creation in CreateChatEntry
            });
        }
    }

    private void CreateChatEntry(string messageText, bool isOwnMessage, Contact contact, Texture texture)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameObject newMessage;

            Transform currentlySelectedConversationScrollViewContent = conversationScrollViewContent;

            if (currentSelectedConversation.Type == Conversation.ConversationType.User)
            {
                Debug.Log("Peer-to-peer conversation.");
                currentlySelectedConversationScrollViewContent = conversationScrollViewContent;
            }
            else if (currentSelectedConversation.Type == Conversation.ConversationType.Room)
            {
                Debug.Log("Room conversation.");
                currentlySelectedConversationScrollViewContent = GetComponent<BubbleManager>().bubbleConversationScrollViewContent;
            }


            // Instantiate the chat message prefab
            if (!isOwnMessage)
            {
                //newMessage = Instantiate(chatMessagePrefab, conversationScrollViewContent.Find(contactID));
                //newMessage = Instantiate(chatMessagePrefab, currentlySelectedConversationScrollViewContent);

                #region New Way To Display Contact Entry
                cancelLoad.Reset();
                var token = cancelLoad.Token;

                newMessage = Instantiate(chatMessagePrefab, currentlySelectedConversationScrollViewContent);

                if (contact != null)
                {
                    ChatPrefabAvatar entry = newMessage.GetComponent<ChatPrefabAvatar>();
                    entry.Contact = contact;

                    LoadChatAvatar(entry, contact, token);
                }
                #endregion

                // Get references to the Image and TextMeshProUGUI components in the prefab and Assign the profile photo (avatar) and the message text
                //Image profileImage = newMessage.GetComponentInChildren<Image>();
                //profileImage.sprite = currentChatMessageAvatar.sprite;
            }
            else
            {
                //newMessage = Instantiate(chatMessagePrefabMyself, conversationScrollViewContent.Find(contactID));
                newMessage = Instantiate(chatMessagePrefabMyself, currentlySelectedConversationScrollViewContent);
            }

            RectTransform rectTransform = newMessage.GetComponent<RectTransform>();

            // Change only the width while keeping the height the same
            Vector2 newSize = rectTransform.sizeDelta;
            newSize.x = 780; // Set the desired width
            rectTransform.sizeDelta = newSize;

            TMP_Text messageTextComponent = newMessage.GetNamedChild("Message").GetComponent<TMP_Text>();
            messageTextComponent.text = messageText;

            Debug.Log("TEXTURE= " + texture);
            if (texture != null)
            {
                RawImage imageGameobject = newMessage.GetComponent<ChatPrefabAvatar>().imageGameobject;

                if (imageGameobject == null)
                {
                    imageGameobject = newMessage.GetNamedChild("RawImage").GetComponent<RawImage>();
                }

                imageGameobject.gameObject.SetActive(true);
                imageGameobject.texture = texture;

                float aspectRatio = (float)texture.width / (float)texture.height;

                // RectTransform to maintain aspect ratio
                RectTransform imageRectTransform = newMessage.GetComponent<ChatPrefabAvatar>().imageGameobject.GetComponent<RectTransform>();
                if (imageRectTransform != null)
                {
                    float parentWidth = imageRectTransform.parent.GetComponent<RectTransform>().rect.width;
                    imageRectTransform.sizeDelta = new Vector2(parentWidth, parentWidth / aspectRatio);
                }

            }

            // Get the Horizontal Layout Group component from the chat message prefab
            HorizontalLayoutGroup layoutGroup = newMessage.GetComponentInChildren<HorizontalLayoutGroup>();

            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childControlWidth = false;

            // Check if the message is sent by the user
            if (isOwnMessage)
            {
                // Align the message to the right (for messages sent by the user)
                layoutGroup.childAlignment = TextAnchor.MiddleRight;

                // Optionally, you can add padding on the right to control the spacing
                layoutGroup.padding.right = 0;  // Adjust this value as needed                
            }
            else
            {
                // Align the message to the left (for messages received from others)
                layoutGroup.childAlignment = TextAnchor.MiddleLeft;

                // Optionally, add padding on the left
                layoutGroup.padding.left = 20;  // Adjust this value as needed
            }


            // Force the Canvas to update before scrolling
            Canvas.ForceUpdateCanvases();

            // Set verticalNormalizedPosition to 0 to scroll to the bottom when new message is added
            currentlySelectedConversationScrollViewContent.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0f;

        });
    }




    public void FetchLastMessagesReceivedInConversation(Conversation conversation, int numOfMessages = 200)
    {
        instantMessaging.GetMessagesFromConversation(conversation, numOfMessages, callback =>
        {
            if (callback.Result.Success)
            {
                List<Message> messagesList = callback.Data; // List of messages just retrieved
                int nb = messagesList.Count; // Number of messages - if nb < numOfMessages there is no more message

                Debug.Log("listCount = " + messagesList.Count);

                if (messagesList.Count <= 0)
                {
                    messagesList = FetchAllMessagesInConversationFromCache(conversation);
                    Debug.Log("listCount from cache = " + messagesList.Count);
                }

                // Process retreived messages
                string texts = "";
                Contact myContact = rbContacts.GetCurrentContact();

                Debug.Log("messagesList.Count " + messagesList.Count);

                for (int i = messagesList.Count - 1; i >= 0; i--)
                {
                    Debug.Log($"messagesList Count= {i}");
                    Debug.Log($"messagesList Count2= {messagesList[i].Content}");
                    //Debug.Log("content " + i + " = " + messagesList[i].Content);

                    // Align my own messages to the right and all the other to the left
                    //if (myContact.Jid_im == messagesList[i].FromJid)
                    //    texts += $"<align=right>{messagesList[i].Content}</align>\n\n";
                    //else
                    //    texts += $"<align=left>{messagesList[i].Content}</align>\n\n";

                    if (messagesList[i].Content != null) // check if content is null. These entries are bcz it includes call notifications
                    {
                        Debug.Log("Test4");
                        //if (myContact.Jid_im == messagesList[i].FromJid)
                        //    CreateChatMessage(messagesList[i].Content, true, conversation.PeerId);
                        //else
                        //    CreateChatMessage(messagesList[i].Content, false, conversation.PeerId);


                        FileAttachment fileAttachment = messagesList[i].FileAttachment;
                        Debug.Log("fileAttachment new " + fileAttachment);

                        if (myContact.Jid_im == messagesList[i].FromJid)
                        {
                            if (fileAttachment == null)
                                CreateChatMessage(messagesList[i].Content, true, rbContacts.GetContactIdFromContactJid(messagesList[i].FromJid)); // NOTE: PeerId or FromJid?
                            else
                            {
                                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                                {
                                    Debug.Log($"senderName 2 {messagesList[i].FromJid}");
                                    GetComponent<FileManager>().StreamSharedFile(fileAttachment.Id, onTextureReceived =>
                                    {
                                        CreateChatMessage(messagesList[i].Content, true, rbContacts.GetContactIdFromContactJid(messagesList[i].FromJid), onTextureReceived);
                                    });
                                });
                            }
                        }
                        else
                        {
                            if (fileAttachment == null)
                                CreateChatMessage(messagesList[i].Content, false, rbContacts.GetContactIdFromContactJid(messagesList[i].FromJid)); // NOTE: PeerId or FromJid?
                            else
                            {
                                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                                {
                                    Debug.Log($"senderName 2 {messagesList[i].FromJid}");
                                    GetComponent<FileManager>().StreamSharedFile(fileAttachment.Id, onTextureReceived =>
                                    {
                                        CreateChatMessage(messagesList[i].Content, false, rbContacts.GetContactIdFromContactJid(messagesList[i].FromJid), onTextureReceived);
                                    });
                                });
                            }
                        }
                            
                    }
                    
                }

                doOnceRefreshTextArea = true; // Placed the bool above the conversationContentArea.text bcz when i placed it below the bool assignment wouldn't execute

                //conversationContentArea.text = texts;
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }








    // Method to get cached messages from a conversation
    public List<Message> FetchAllMessagesInConversationFromCache(Conversation conversation)
    {
        //InstantMessaging instantMessaging = RainbowManager.Instance.GetRainbowApplication().GetInstantMessaging();

        return instantMessaging.GetAllMessagesFromConversationFromCache(conversation);
    }




    public void SendMessageOrSendFileWithMessageToConversation()
    {
        if (string.IsNullOrEmpty(GetComponent<FileManager>().selectedFilePath))
        {
            SendMessageToConversation();
        }
        else
        {
            Debug.Log("ShareFileWithConversation");
            GetComponent<FileManager>().ShareFileWithConversation(currentSelectedConversation, messageInputField.text);
        }

        messageInputField.text = "";
    }


    public void SendMessageToConversation()
    {
        //InstantMessaging instantMessaging = RainbowManager.Instance.GetRainbowApplication().GetInstantMessaging();

        instantMessaging.SendMessageToConversation(currentSelectedConversation, messageInputField.text, null, UrgencyType.Std, null, callback =>
        {
            if (callback.Result.Success)
            {
                Debug.Log("Message sent.");

                // Refresh the messages in display
                //FetchLastMessagesReceivedInConversation(currentSelectedConversation);

                //conversationContentArea.text += $"<align=right>{messageInputField.text}</align>\n\n"; ;
                //doOnceRefreshTextArea = true; // Here it works even if it's below. The problem is the retrieval of texts not the sending

                CreateChatMessage(messageInputField.text, true, currentSelectedConversation.PeerId);

                messageInputField.text = "";
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }


    public void SendMessageOrSendFileWithMessageToBubble()
    {
        if (string.IsNullOrEmpty(GetComponent<FileManager>().selectedFilePath))
        {
            SendMessageToBubbleConversation();
        }
        else
        {
            Debug.Log("ShareFileWithConversation");
            GetComponent<FileManager>().ShareFileWithConversation(currentSelectedConversation, GetComponent<BubbleManager>().messageInputField.text);
        }

        GetComponent<BubbleManager>().messageInputField.text = "";
    }

    public void SendMessageToBubbleConversation()
    {
        //InstantMessaging instantMessaging = RainbowManager.Instance.GetRainbowApplication().GetInstantMessaging();

        instantMessaging.SendMessageToConversation(currentSelectedConversation, GetComponent<BubbleManager>().messageInputField.text, null, UrgencyType.Std, null, callback =>
        {
            if (callback.Result.Success)
            {
                Debug.Log("Message sent.");

                // Refresh the messages in display
                //FetchLastMessagesReceivedInConversation(currentSelectedConversation);

                //conversationContentArea.text += $"<align=right>{messageInputField.text}</align>\n\n"; ;
                //doOnceRefreshTextArea = true; // Here it works even if it's below. The problem is the retrieval of texts not the sending

                CreateChatMessage(GetComponent<BubbleManager>().messageInputField.text, true, currentSelectedConversation.PeerId);

                GetComponent<BubbleManager>().messageInputField.text = "";
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }


    // Method to mark a message as read
    public void MarkMessageAsRead(string conversationId, string messageId)
    {
        //InstantMessaging instantMessaging = RainbowManager.Instance.GetRainbowApplication().GetInstantMessaging();

        instantMessaging.MarkMessageAsRead(conversationId, messageId, callback =>
        {
            if (callback.Result.Success)
            {
                Debug.Log("Message marked as read");
            }
            else
            {
                Debug.LogError("Error marking message as read: " + callback.Result.ExceptionError?.Message);
            }
        });
    }



    // Method to send "is typing" status
    public void SendIsTyping(Conversation conversation, bool isTyping)
    {
        //InstantMessaging instantMessaging = RainbowManager.Instance.GetRainbowApplication().GetInstantMessaging();

        instantMessaging.SendIsTypingInConversation(conversation, isTyping, callback =>
        {
            if (callback.Result.Success)
            {
                Debug.Log("Typing status sent");
            }
            else
            {
                Debug.LogError("Error sending typing status: " + callback.Result.ExceptionError?.Message);
            }
        });
    }



    private void MyApp_UserTypingChanged(object sender, UserTypingEventArgs evt)
    {
        // Extract relevant data from the event
        string conversationId = evt.ConversationId;  // ID of the conversation
        string contactJid = evt.ContactJid;          // Jid of the contact who is typing
        bool isTyping = evt.IsTyping;                // Is the contact typing or not?

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            TMP_Text tempIsTypingTextArea = isTypingTextArea;

            if (currentSelectedConversation == null) return; //this is to fix bug when there is an incoming call but this is not set yet

            if (currentSelectedConversation.Type == Conversation.ConversationType.User)
            {
                Debug.Log("Peer-to-peer conversation.");
                tempIsTypingTextArea = isTypingTextArea;
            }
            else if (currentSelectedConversation.Type == Conversation.ConversationType.Room)
            {
                Debug.Log("Room conversation.");
                tempIsTypingTextArea = GetComponent<BubbleManager>().isTypingTextArea;
            }

            // In order to check if the contactJid is mine so that it won't show the is typing for my own messages in bubbles (it only happens in bubbles)
            Contact myContact = rbContacts.GetCurrentContact();

            // Display is typing only if you have the conversation open
            if (currentSelectedConversation.Id == conversationId && myContact.Jid_im != contactJid)
            {
                // Handle 'is typing' status update
                if (isTyping)
                {
                    Debug.Log($"User with Jid {contactJid} is typing in conversation {conversationId}.");

                    doOnceRefreshIsTypingTextArea = true;
                    tempIsTypingTextArea.text = $"{rbContacts.GetContactFromContactJid(contactJid).DisplayName} is typing...";
                }
                else
                {
                    Debug.Log($"User with Jid {contactJid} stopped typing in conversation {conversationId}.");

                    doOnceRefreshIsTypingTextArea = true;
                    tempIsTypingTextArea.text = "";
                }
            }
        });

    }



    private void MyApp_MessageReceived(object sender, MessageEventArgs evt)
    {
        // Extract relevant data from the event
        string conversationId = evt.ConversationId;  // ID of the conversation
        Message message = evt.Message;               // The received message object
        bool isCarbonCopy = evt.CarbonCopy;          // Is this a carbon copy (message sent by the current user from another device)?

        // Process the message
        string senderName = message.FromJid;       // Get sender's name
        string messageContent = message.Content;     // Get message content
        FileAttachment fileAttachment = message.FileAttachment;


        if (isCarbonCopy)
        {
            Debug.Log($"[Carbon Copy] Message from {senderName}: {messageContent}");
        }
        else
        {
            Debug.Log($"New message from {senderName}: {messageContent}");

            //doOnceRefreshTextArea = true; // Placed the bool above the conversationContentArea.text bcz when i placed it below the bool assignment wouldn't execute

            //conversationContentArea.text += $"<align=left>{messageContent}</align>\n\n";

            if (fileAttachment == null)
                CreateChatMessage(messageContent, false, rbContacts.GetContactIdFromContactJid(senderName));
            else
            {
                Debug.Log($"FILE ATTACHMENT {fileAttachment.Id}");
                Debug.Log($"senderName {senderName}");
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GetComponent<FileManager>().StreamSharedFile(fileAttachment.Id, onTextureReceived =>
                    {
                        CreateChatMessage(messageContent, false, rbContacts.GetContactIdFromContactJid(senderName), onTextureReceived);
                    });
                });
            }               

        }
    }
    


    #region Contacts Avatar Image


    // Get the avatar of a contact
    public void GetContactAvatar(string contactId, int size = 80)
    {
        rbContacts.GetAvatarFromContactId(contactId, size, callback =>
        {
            if (callback.Result.Success)
            {
                byte[] avatarData = callback.Data;

                tempAvatarData[tempCount] = avatarData;
                tempCount++;

                //StartCoroutine(HandleAvatarData(avatarData, avatarImageSlot));
                //HandleAvatarData(tempAvatarData, avatarImageSlot);

                Debug.Log("Avatar retrieved successfully.");
                // Handle avatar image usage in Unity as needed (e.g., texture for UI)
            }
            else if (callback.Result.ResponseContentType == "text/html")
            {
                Debug.Log("Avatar retrieved is NOT AN IMAGE!");
                tempCount++;
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }




    // Coroutine to process avatar data and apply it to a SpriteRenderer
    private void HandleAvatarData(byte[] avatarData, Image avatarImageSlot)
    {
        Debug.Log("avatarData = " + avatarData);

        // Convert byte[] to Texture2D on the main thread
        Texture2D avatarTexture = GetImageFromBytes(avatarData);

        if (avatarTexture != null)
        {
            Debug.Log("Avatar fetched and converted successfully.");

            // Convert Texture2D to Sprite
            Sprite avatarSprite = ConvertTextureToSprite(avatarTexture);

            if (avatarImageSlot != null)
            {
                Debug.Log("HANDLE AVATAR IMAGE");
                avatarImageSlot.sprite = avatarSprite; // Set the sprite
            }
        }
        else
        {
            Debug.LogError("Failed to convert avatar image.");
        }

        //yield return null; // Yielding to let Unity update the main thread
    }




    public Texture2D GetImageFromBytes(byte[] data)
    {
        Texture2D texture = null;

        try
        {
            // Create a new Texture2D instance
            texture = new Texture2D(5, 5); // Size will be overwritten by LoadImage
            if (texture.LoadImage(data)) // Automatically resizes the texture dimensions
            {
                Debug.Log("Image loaded successfully");
            }
            else
            {
                Debug.LogError("Image loading failed");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[GetImageFromBytes] Exception: {e.Message}");
        }

        return texture;
    }




    public Sprite ConvertTextureToSprite(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogError("Texture is null, cannot convert to Sprite.");
            return null;
        }

        // Create a new sprite using the Texture2D
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }



    #endregion

    //// Convert byte array to Image (for avatar handling)
    //public Image GetImageFromBytes(ref byte[] data)
    //{
    //    Image result = null;
    //    try
    //    {
    //        using (var ms = new MemoryStream(data))
    //            result = Image.FromStream(ms);
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogWarning($"Exception during image conversion: {e.Message}");
    //    }
    //    return result;
    //}





    // Handle contact updates (PeerInfoChanged event)
    private void MyApp_PeerInfoChanged(object sender, PeerEventArgs evt)
    {
        string jid = evt.Peer.Jid;
        Debug.Log($"Contact info updated for JID: {jid}");
    }

    // Handle contact avatar changes (PeerAvatarChanged event)
    private void MyApp_PeerAvatarChanged(object sender, PeerEventArgs evt)
    {
        string jid = evt.Peer.Jid;
        Debug.Log($"Avatar updated for JID: {jid}");
    }

    // Handle contact avatar deletion (PeerAvatarDeleted event)
    private void MyApp_PeerAvatarDeleted(object sender, PeerEventArgs evt)
    {
        string jid = evt.Peer.Jid;
        Debug.Log($"Avatar deleted for JID: {jid}");
    }




    





    private void HandleError(SdkError error)
    {
        // A pb occurs
        if (error.Type == SdkError.SdkErrorType.IncorrectUse)
        {
            // Bad parameters used
            Debug.LogError($"Incorrect use error: {error.IncorrectUseError.ErrorMsg}");
        }
        else
        {
            // Exception occurs
            Debug.LogError($"Exception: {error.ExceptionError}");
        }
    }





    public void DisplayConversationsWithContact(Contact contact)
    {

        Conversation conversationWithContact = OpenConversationWithContact(contact);
        //conversationContentArea.text = conversationWithContact.LastMessageText;

        currentSelectedConversation = conversationWithContact;

        // Save current contact avatar image for use in chat message prefab profile image
        //currentChatMessageAvatar = avatarImage[contactGameobject.transform.GetSiblingIndex()];

        // Use a lambda to pass the delegate to the onValueChanged event listener
        // The lambda checks if the input field has any text (using !string.IsNullOrEmpty(value)),
        // and if it does, SendIsTyping sends true to indicate typing. If the field is empty, it sends false.
        messageInputField.onValueChanged.AddListener((string value) =>
        {
            SendIsTyping(conversationWithContact, !string.IsNullOrEmpty(value));
        });

        FetchLastMessagesReceivedInConversation(conversationWithContact);
        //if (alreadyFetchedMessagesForThisContactOnce[contactGameobject.transform.GetSiblingIndex()])
        //{
        //    // Disable all message prefabs except the current contact's messages (the one that is open now)
        //    for (int j = 0; j < parentForAllMessagesOfEachContact.Length; j++)
        //    {
        //        parentForAllMessagesOfEachContact[j].SetActive(false);
        //    }
        //    conversationScrollViewContent.Find(contactList[contactGameobject.transform.GetSiblingIndex()].Id).gameObject.SetActive(true);
        //}
        //else
        //{
        //    // Disable all message prefabs except the current contact's messages (the one that is open now)
        //    for (int j = 0; j < parentForAllMessagesOfEachContact.Length; j++)
        //    {
        //        parentForAllMessagesOfEachContact[j].SetActive(false);
        //    }
        //    conversationScrollViewContent.Find(contactList[contactGameobject.transform.GetSiblingIndex()].Id).gameObject.SetActive(true);

        //    alreadyFetchedMessagesForThisContactOnce[contactGameobject.transform.GetSiblingIndex()] = true;
        //    FetchLastMessagesReceivedInConversation(conversationWithContact);
        //}
    }


    public void ResetConversationContentGameobject()
    {
        if (conversationScrollViewContent.transform.childCount > 0)
        {
            foreach (Transform child in conversationScrollViewContent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        if (GetComponent<BubbleManager>().bubbleConversationScrollViewContent.transform.childCount > 0)
        {
            foreach (Transform child in GetComponent<BubbleManager>().bubbleConversationScrollViewContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }



    #region Contacts Methods





    // Search for contacts by display name
    public void SearchContactByName(string nameToSearch, int maxNbResult = 20)
    {
        foreach (Transform child in searchedContactsScrollviewContent.transform)
        {
            Destroy(child.gameObject);
        }

        //await Task.Delay(1000);

        wasDisconnectedFromSearchContactByName = true;

        rbContacts.SearchContactsByDisplayName(nameToSearch, maxNbResult, callback =>
        {
            if (callback.Result.Success)
            {
                var searchResult = callback.Data;

                Debug.Log("Searched Contacts= " + searchResult.ContactsList.Count);
                Debug.Log("Searched Contacts2= " + searchResult.ContactsList[0].Id.ToString());
                Debug.Log("Searched Contacts3= " + searchResult.ContactsList[0].DisplayName.ToString());
                Debug.Log("Searched Contacts4= " + searchResult.ContactsList[0].FirstName.ToString());
                Debug.Log("Searched Contacts5= " + searchResult.ContactsList[0].LastName.ToString());
                Debug.Log("Searched Contacts6= " + searchResult.ContactsList[0].NickName.ToString());
                

                foreach (Contact contact in searchResult.ContactsList)
                {
                    //var temp = GetContactById(contact.Id);
                    //Debug.Log($"Contact found: {temp.DisplayName}");

                    if (contact != null && contact.Id != model.CurrentUser.Id)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => {
                            //var foundContact = Instantiate(contactPrefab, searchedContactsScrollviewContent.transform);
                            //foundContact.GetComponent<ContactGameobject>().currentGameobjectContact = contact;
                            //foundContact.GetNamedChild("DisplayNameText").GetComponent<TMP_Text>().text = contact.FirstName + " " + contact.LastName;
                            Debug.Log("TESTING1");
                            #region New Way To Display Contact Entry
                            cancelLoad.Reset();
                            var token = cancelLoad.Token;
                            Debug.Log("TESTING2");
                            Presence p = rbContacts.GetAggregatedPresenceFromContact(contact);
                            GameObject item = Instantiate(contactPrefab, searchedContactsScrollviewContent.transform);
                            Debug.Log("TESTING3 " + item);
                            item.GetComponent<ContactGameobject>().currentGameobjectContact = contact;
                            ContactEntry entry = item.GetComponent<ContactEntry>();
                            entry.Contact = contact;
                            if (p == null)
                            {
                                entry.SetPresenceLevel(PresenceLevel.Offline);
                            }
                            else
                            {
                                entry.SetPresenceLevel(p.PresenceLevel);

                            }

                            LoadAvatar(entry, contact, token);
                            #endregion

                        });
                    }
                }

                foreach (Phonebook phonebook in searchResult.PhonebooksList)
                {
                    Debug.Log("Phonebook contact found.");
                }

                foreach (O365AdContact contact in searchResult.O365AdContactsList)
                {
                    Debug.Log("Office 365 contact found.");
                }

                foreach (DirectoryContact contact in searchResult.DirectoryContactsList)
                {
                    Debug.Log("Enterprise directory contact found.");
                }
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }


    #region New Way To Display Contact Entry
    private async void LoadAvatar(ContactEntry entry, Contact contact, CancellationToken cancellationToken)
    {
        try
        {
            Texture2D tex = await AvatarLoader.RequestAvatar(contact, 64, cancellationToken: cancellationToken);

            if (tex != null)
            {
                UnityExecutor.Execute(() =>
                {
                    if (entry != null)
                    {
                        entry.ContactInitialsAvatar.AvatarImage = tex;
                    }
                });
            }

        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
        {
            // canceled -> nothing to do
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }



    private async void LoadChatAvatar(ChatPrefabAvatar entry, Contact contact, CancellationToken cancellationToken)
    {
        try
        {
            Texture2D tex = await AvatarLoader.RequestAvatar(contact, 64, cancellationToken: cancellationToken);

            if (tex != null)
            {
                UnityExecutor.Execute(() =>
                {
                    if (entry != null)
                    {
                        entry.ContactInitialsAvatar.AvatarImage = tex;
                    }
                });
            }

        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
        {
            // canceled -> nothing to do
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }


    void Awake()
    {
        if (AvatarLoader == null)
        {
            AvatarLoader = FindFirstObjectByType<RainbowAvatarLoader>();
        }
    }
    #endregion


    // Search for contacts by display name
    public async void SearchContactByEmails(List<string> emailsToSearch, int maxNbResult = 20)
    {
        foreach (Transform child in searchedContactsScrollviewContent.transform)
        {
            Destroy(child.gameObject);
        }

        await Task.Delay(1000);

        rbContacts.SearchContactByEmails(emailsToSearch, callback =>
        {
            if (callback.Result.Success)
            {
                var searchResult = callback.Data;

                Debug.Log("Searched Contacts by emails= " + searchResult.ContactsList.Count);
                Debug.Log("Searched Contacts2 by emails= " + searchResult.ContactsList[0].Id.ToString());
                Debug.Log("Searched Contacts3 by emails= " + searchResult.ContactsList[0].DisplayName.ToString());
                Debug.Log("Searched Contacts4 by emails= " + searchResult.ContactsList[0].FirstName.ToString());
                Debug.Log("Searched Contacts5 by emails= " + searchResult.ContactsList[0].LastName.ToString());
                Debug.Log("Searched Contacts6 by emails= " + searchResult.ContactsList[0].NickName.ToString());

                foreach (Contact contact in searchResult.ContactsList)
                {
                    //var contact2 = GetContactById(contact.Id);
                    //Debug.Log($"Contact found: {contact2.DisplayName}");
                                        
                    if (contact != null && contact.Id != model.CurrentUser.Id)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => {                            
                            //var foundContact = Instantiate(contactPrefab, searchedContactsScrollviewContent.transform);
                            //foundContact.GetComponent<ContactGameobject>().currentGameobjectContact = contact;
                            //foundContact.GetNamedChild("DisplayNameText").GetComponent<TMP_Text>().text = contact.FirstName + " " + contact.LastName;

                            #region New Way To Display Contact Entry
                            cancelLoad.Reset();
                            var token = cancelLoad.Token;

                            Presence p = rbContacts.GetAggregatedPresenceFromContact(contact);
                            GameObject item = Instantiate(contactPrefab, searchedContactsScrollviewContent.transform);
                            item.GetComponent<ContactGameobject>().currentGameobjectContact = contact;
                            ContactEntry entry = item.GetComponent<ContactEntry>();
                            entry.Contact = contact;
                            if (p == null)
                            {
                                entry.SetPresenceLevel(PresenceLevel.Offline);
                            }
                            else
                            {
                                entry.SetPresenceLevel(p.PresenceLevel);

                            }

                            LoadAvatar(entry, contact, token);
                            #endregion
                        });
                    }
                }

                foreach (Phonebook phonebook in searchResult.PhonebooksList)
                {
                    Debug.Log("Phonebook contact found.");
                }

                foreach (O365AdContact contact in searchResult.O365AdContactsList)
                {
                    Debug.Log("Office 365 contact found.");
                }

                foreach (DirectoryContact contact in searchResult.DirectoryContactsList)
                {
                    Debug.Log("Enterprise directory contact found.");
                }
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }







    // Retrieve a contact using its Contact ID
    public Contact GetContactById(string contactId)
    {
        return rbContacts.GetContactFromContactId(contactId);
    }

    // Retrieve a contact from the server (useful if it's not in the local roster)
    public void GetContactFromServer(string contactId)
    {
        rbContacts.GetContactFromContactIdFromServer(contactId, callback =>
        {
            if (callback.Result.Success)
            {
                Contact contact = callback.Data;
                Debug.Log($"Contact found: {contact.DisplayName}");
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }



    public void FetchAllContactsInRoster()
    {
        //Contacts contacts = RainbowManager.Instance.GetRainbowApplication().GetContacts();

        rbContacts.GetAllContactsInRoster(callback =>
        {
            if (callback.Result.Success)
            {
                List<Contact> contactList = callback.Data;  // List of contacts


                avatarImage = new Image[contactList.Count];
                tempContacts = new Contact[contactList.Count];

                tempAvatarData = new byte[contactList.Count][];



                // Set the bool array to false which will be used to check if the chatMessage gameobjects were already fetched and instantiated once in scene so that I will
                // make them children of one gameobject in order to disable and enable the messages of each contact's conversation when I open the chat of the corresponding contact.
                // This was done so that i won't have to instantiate and destroy all messages every time I switch between contacts and also bcz once the api fetched the messages of each 
                // contact once, the message list becomes 0 and then i can't fetch them again, so i basically save them in scene and the 2nd time i open the conversation of a contact the 
                // messages are already fetched from the 1st time and i just enable and disable them in scene.
                alreadyFetchedMessagesForThisContactOnce = new bool[contactList.Count];
                parentForAllMessagesOfEachContact = new GameObject[contactList.Count];

                for (int i = 0; i < alreadyFetchedMessagesForThisContactOnce.Length; i++)
                {
                    alreadyFetchedMessagesForThisContactOnce[i] = false;

                    parentForAllMessagesOfEachContact[i] = new GameObject(contactList[i].Id);
                    //parentForAllMessagesOfEachContact[i].transform.parent = conversationScrollViewContent;
                    parentForAllMessagesOfEachContact[i].transform.SetParent(conversationScrollViewContent, false);


                    parentForAllMessagesOfEachContact[i].AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    VerticalLayoutGroup layout = parentForAllMessagesOfEachContact[i].AddComponent<VerticalLayoutGroup>();

                    RectTransform parentRectTransform = parentForAllMessagesOfEachContact[i].GetComponent<RectTransform>();
                    //parentRectTransform.sizeDelta = new Vector2(parentRectTransform.rect.width, parentRectTransform.sizeDelta.y);
                    parentRectTransform.sizeDelta = new Vector2(670, parentRectTransform.sizeDelta.y);

                    layout.spacing = 20;
                    layout.padding = new RectOffset(10, 10, 10, 10);
                    layout.childControlHeight = false;
                    layout.childForceExpandHeight = false;
                    layout.childForceExpandWidth = true;
                    layout.childControlWidth = true;

                    layout.GetComponent<RectTransform>().sizeDelta = new Vector2(938, layout.GetComponent<RectTransform>().sizeDelta.y);




                }




                for (int i = 0; i < contactList.Count; i++)
                //foreach (Contact contact in contactList)
                {
                    Debug.Log($"Contact Name: {contactList[i].DisplayName}, ID: {contactList[i].Id}");

                    GameObject contactGameobject = Instantiate(contactPrefab, contactScrollViewContent);

                    contactGameobject.GetComponentInChildren<TMP_Text>().text = contactList[i].DisplayName;

                    contactGameobject.GetComponent<Button>().onClick.AddListener(() => {

                        //GetComponent<MenuManager>().OpenCloseChatPanels(1);

                        // Used siblingIndex instead of i because in addlistener it used the last assigned value of i in everything
                        Conversation conversationWithContact = OpenConversationWithContact(contactList[contactGameobject.transform.GetSiblingIndex()]);
                        //conversationContentArea.text = conversationWithContact.LastMessageText;

                        currentSelectedConversation = conversationWithContact;

                        // Save current contact avatar image for use in chat message prefab profile image
                        currentChatMessageAvatar = avatarImage[contactGameobject.transform.GetSiblingIndex()];

                        // Use a lambda to pass the delegate to the onValueChanged event listener
                        // The lambda checks if the input field has any text (using !string.IsNullOrEmpty(value)),
                        // and if it does, SendIsTyping sends true to indicate typing. If the field is empty, it sends false.
                        messageInputField.onValueChanged.AddListener((string value) =>
                        {
                            SendIsTyping(conversationWithContact, !string.IsNullOrEmpty(value));
                        });

                        Debug.Log($"Contact i: {i} => {contactGameobject.transform.GetSiblingIndex()}");

                        if (alreadyFetchedMessagesForThisContactOnce[contactGameobject.transform.GetSiblingIndex()])
                        {
                            // Disable all message prefabs except the current contact's messages (the one that is open now)
                            for (int j = 0; j < parentForAllMessagesOfEachContact.Length; j++)
                            {
                                parentForAllMessagesOfEachContact[j].SetActive(false);
                            }
                            conversationScrollViewContent.Find(contactList[contactGameobject.transform.GetSiblingIndex()].Id).gameObject.SetActive(true);
                        }
                        else
                        {
                            // Disable all message prefabs except the current contact's messages (the one that is open now)
                            for (int j = 0; j < parentForAllMessagesOfEachContact.Length; j++)
                            {
                                parentForAllMessagesOfEachContact[j].SetActive(false);
                            }
                            conversationScrollViewContent.Find(contactList[contactGameobject.transform.GetSiblingIndex()].Id).gameObject.SetActive(true);

                            alreadyFetchedMessagesForThisContactOnce[contactGameobject.transform.GetSiblingIndex()] = true;
                            FetchLastMessagesReceivedInConversation(conversationWithContact);
                        }
                        //GetComponent<MenuManager>().OpenCloseChatPanels(1);
                    });







                    // Display the contact's avatar image
                    avatarImage[i] = contactGameobject.GetComponent<Image>();

                    tempContacts[i] = contactList[i];

                    // Hide 1st contact because it's the current user himself
                    if (i == 0)
                        contactGameobject.SetActive(false);
                }

                initializationPerformedFlag = true;

                UpdateContentHeight();
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }


    public void OpenSearchPanelAndSetBubblesOrContacts(bool isBubbles)
    {
        isSearchPanelOpenedFromBubbles = isBubbles;
        foreach (Transform child in searchedContactsScrollviewContent.transform)
        {
            Destroy(child.gameObject);
        }
        searchContactInputField.text = "";
        searchContactsPanel.SetActive(true);
    }


    public void CloseSearchPanelAndClearList()
    {
        contactsToInvite.Clear();
        foreach (Transform child in searchedContactsScrollviewContent.transform)
        {
            Destroy(child.gameObject);
        }
        searchContactInputField.text = "";
        searchContactsPanel.SetActive(false);
        
    }


    // Method to send invitations to add to contacts or to add to bubbles based on the bool isSearchPanelOpenedFromBubbles
    public void SendInvitationsToAddToContactsOrAddInBubbles()
    {
        if (contactsToInvite.Count == 0) return;
                
        // Invite contacts that i selected and added to the list, from the contact search field 
        foreach (Contact contact in contactsToInvite)
        {
            // If search panel opened from bubbles then add to bubble otherwise add to contacts
            if (isSearchPanelOpenedFromBubbles)
            {
                GetComponent<BubbleManager>().AddMemberToBubble(GetComponent<BubbleManager>().currentSelectedBubble, contact);
            }
            else
            {
                // Don't send invitation to contacts that are already added in my roster
                if (!contact.InRoster)
                    AddContact(contact.Id);
                else
                    Debug.Log($"Contact with id= {contact.Id} is already in roster");
            }                
        }

        // Clear the list after I hit send invitation button
        contactsToInvite.Clear();
    }



    public void AddContact(string contactId)
    {
        rbInvitations.SendInvitationByContactId(contactId, callback =>
        {
            if (callback.Result.Success)
            {
                Invitation invitation = callback.Data;  // Invitation state
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }




    public void RemoveContact(string contactId)
    {
        rbContacts.RemoveFromContactId(contactId, callback =>
        {
            if (callback.Result.Success)
            {
                // Contact has been removed  from the roster
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }



   
                                        
    private void MyApp_RosterPeerAdded(object sender, PeerEventArgs evt) // TO CHECK this: JidEventArgs evt instead of PeerEventArgs
    {
        string jid = evt.Peer.Jid; // JID of the contact added
    }

    private void MyApp_RosterPeerRemoved(object sender, PeerEventArgs evt)
    {
        string jid = evt.Peer.Jid; // JID of the contact removed
    }



    private void MyApp_RosterInvitationReceived(object sender, InvitationEventArgs evt) 
    {
        Debug.Log("MyApp_RosterInvitationReceived");

        string invitaionId = evt.InvitationId; // ID of the invitation received        

        rbInvitations.GetInvitationById(invitaionId, callback =>
        {
            if (callback.Result.Success)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Invitation invitation = callback.Data;

                    if (int.TryParse(notificationsCountText.text, out int notificationsCount))
                    {
                        Debug.Log("Converted to int: " + notificationsCount);
                        notificationsCount++;
                        notificationsCountText.text = notificationsCount.ToString();

                        var notification = Instantiate(notificationPrefab, notificationsContent.transform);
                        notification.GetComponent<NotificationGameobject>().currentContactInvitation = invitation;
                    }
                    else
                    {
                        Debug.LogError("Invalid number format: " + notificationsCountText.text);
                    }
                });
            }
            else
            {
                HandleError(callback.Result);
            }
            
        });       
    }


    private void MyApp_RosterInvitationSent(object sender, InvitationEventArgs evt) 
    {
        string invitaionId = evt.InvitationId; // ID of the invitation received
        Debug.Log("MyApp_RosterInvitationSent");
    }


    private void MyApp_RosterInvitationAccepted(object sender, InvitationEventArgs evt) 
    {
        string invitaionId = evt.InvitationId; // ID of the invitation received
        Debug.Log("MyApp_RosterInvitationAccepted");

        contactListPanel.GetComponent<ContactList>().UpdateList();
    }


    private void MyApp_RosterInvitationDeclined(object sender, InvitationEventArgs evt) 
    {
        string invitaionId = evt.InvitationId; // ID of the invitation received
        Debug.Log("MyApp_RosterInvitationDeclined");
    }


    private void MyApp_RosterInvitationCancelled(object sender, InvitationEventArgs evt) 
    {
        string invitaionId = evt.InvitationId; // ID of the invitation received
        Debug.Log("MyApp_RosterInvitationCancelled");
    }


    private void MyApp_RosterInvitationDeleted(object sender, InvitationEventArgs evt) 
    {
        string invitaionId = evt.InvitationId; // ID of the invitation received
        Debug.Log("MyApp_RosterInvitationDeleted");
    }


    #endregion

}
