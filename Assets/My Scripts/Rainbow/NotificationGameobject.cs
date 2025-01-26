using Cortex;
using Rainbow;
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

    public Invitation currentInvitation;

    private Contacts rbContacts;
    private Invitations rbInvitations;


    // Start is called before the first frame update
    async void Start()
    {
        await Task.Delay(200); // To make sure it doesn't execute before currentInvitation is assigned

        ConnectionModel model = ConnectionModel.Instance;
        rbContacts = model.Contacts;
        rbInvitations = model.Invitations;

        var fromUser = rbContacts.GetContactFromContactId(currentInvitation.InvitingUserId);
        messageText.text = $"{fromUser.FirstName} {fromUser.LastName} invited you on {currentInvitation.InvitingDate}";

        Debug.Log("NOTIFICATION Status " + currentInvitation.Status + " Type: " + currentInvitation.Type);

        acceptButton.onClick.AddListener(() =>
        {
            AcceptInvitation(currentInvitation.Id);
        });

        declineButton.onClick.AddListener(() =>
        {
            DeclineInvitation(currentInvitation.Id);
        });
    }


    void AcceptInvitation(string invitationId)
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


    void DeclineInvitation(string invitationId)
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
