using Cortex;
using Rainbow;
using Rainbow.Events;
using Rainbow.Model;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationGameobject : MonoBehaviour
{
    public TMP_Text messageText;
    public Button acceptButton;
    public Button declineButton;
    public Button resultButton;
    public TMP_Text resultText;

    public Invitation currentContactInvitation;
    //public Bubble currentBubbleInvitation;
    public BubbleInvitationEventArgs currentBubbleEventArgs;

    private Contacts rbContacts;
    private Invitations rbInvitations;
    private Bubbles rbBubbles;


    // Start is called before the first frame update
    async void Start()
    {
        await Task.Delay(200); // To make sure it doesn't execute before currentInvitation is assigned

        ConnectionModel model = ConnectionModel.Instance;
        rbContacts = model.Contacts;
        rbInvitations = model.Invitations;
        rbBubbles = model.Bubbles;

        // If currentContactInvitation is not null it means this is an invitation to add as contact, otherwise if bubbleEventArgs is not null it means it's a bubble invitation
        if (currentContactInvitation != null) 
        {
            rbContacts.GetContactFromContactIdFromServer(currentContactInvitation.InvitingUserId, callback =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Contact fromUser = callback.Data;

                    Debug.Log($"currentContactInvitation {currentContactInvitation.InvitingUserId} - From User {fromUser}");
                    Debug.Log("NOTIFICATION Status " + currentContactInvitation.Status + " Type: " + currentContactInvitation.Type);

                    messageText.text = $"{fromUser.FirstName} {fromUser.LastName} invited you on {currentContactInvitation.InvitingDate}";
                });                
            });

            
            

            acceptButton.onClick.AddListener(() =>
            {
                AcceptContactInvitation(currentContactInvitation.Id);
            });

            declineButton.onClick.AddListener(() =>
            {
                DeclineContactInvitation(currentContactInvitation.Id);
            });
        }
        else if (currentBubbleEventArgs != null)
        {
            var fromUser = rbContacts.GetContactFromContactId(currentBubbleEventArgs.UserId);
            messageText.text = $"{fromUser.FirstName} {fromUser.LastName} invited you to join the bubble {currentBubbleEventArgs.BubbleName}";

            acceptButton.onClick.AddListener(() =>
            {
                AcceptBubbleInvitation(currentBubbleEventArgs.BubbleId);
            });

            declineButton.onClick.AddListener(() =>
            {
                DeclineBubbleInvitation(currentBubbleEventArgs.BubbleId);
            });
        }
        
    }


    void AcceptContactInvitation(string invitationId)
    {
        rbInvitations.AcceptReceivedPendingInvitation(invitationId, callback =>
        {
            if (callback.Result.Success)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    acceptButton.gameObject.SetActive(false);
                    declineButton.gameObject.SetActive(false);

                    resultButton.gameObject.SetActive(true);
                    resultText.text = "Accepted";

                    Destroy(gameObject, 5);
                });                
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }


    void DeclineContactInvitation(string invitationId)
    {
        rbInvitations.DeclineReceivedPendingInvitation(invitationId, callback =>
        {
            if (callback.Result.Success)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    acceptButton.gameObject.SetActive(false);
                    declineButton.gameObject.SetActive(false);

                    resultButton.gameObject.SetActive(true);
                    resultText.text = "Declined";

                    Destroy(gameObject, 5);
                });
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }




    void AcceptBubbleInvitation(string bubbleId)
    {
        rbBubbles.AcceptInvitation(bubbleId, callback =>
        {
            if (callback.Result.Success)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    acceptButton.gameObject.SetActive(false);
                    declineButton.gameObject.SetActive(false);

                    resultButton.gameObject.SetActive(true);
                    resultText.text = "Accepted";

                    Destroy(gameObject, 5);
                });
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }


    void DeclineBubbleInvitation(string bubbleId)
    {
        rbBubbles.DeclineInvitation(bubbleId, callback =>
        {
            if (callback.Result.Success)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    acceptButton.gameObject.SetActive(false);
                    declineButton.gameObject.SetActive(false);

                    resultButton.gameObject.SetActive(true);
                    resultText.text = "Declined";

                    Destroy(gameObject, 5);
                });
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }


    private void HandleError(SdkError error)
    {
        if (error.Type == SdkError.SdkErrorType.IncorrectUse)
        {
            Debug.LogError("Error: " + error.IncorrectUseError.ErrorMsg);
        }
        else if (error.Type == SdkError.SdkErrorType.Exception)
        {
            Debug.LogError("Exception: " + error.ExceptionError.Message);
        }
    }
}
