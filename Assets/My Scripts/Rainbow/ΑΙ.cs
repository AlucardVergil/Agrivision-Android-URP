using System.Collections;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AI : MonoBehaviour
{
    private string tempFilePath;
    private string jobId;
    private const string API_BASE_URL = "https://dev-summary-cortex2.openrainbow.io";
    private const string USERNAME = "cortex2";
    private const string PASSWORD = "Fx!wPyCL-T5V3U2";


    #region Open Close AI panel & Save Chat history

    [Header("Main AI panel to show or hide")]
    public GameObject meetingSummarizationPanel;
    [Header("Text that will display the response of meeting summarization")]
    public TMP_Text meetingSummarizationText;
    private StringBuilder conversationHistory = new StringBuilder();

    public void AddMessage(string user, string message)
    {
        conversationHistory.AppendLine($"{user}: {message}");
    }


    public void SaveConversationToFile()
    {
        // Create a temp file
        tempFilePath = Path.Combine(Application.temporaryCachePath, "conversation.txt");
        File.WriteAllText(tempFilePath, conversationHistory.ToString());

        Debug.Log($"Conversation saved to: {tempFilePath}");
    }


    public void ClearConversationHistory()
    {
        conversationHistory.Clear();
    }



    public void OpenCloseMeetingSummarizationPanel()
    {
        if (meetingSummarizationPanel.activeSelf)
        {
            meetingSummarizationPanel.SetActive(false);
        }
        else
        {
            meetingSummarizationPanel.SetActive(true);
            StartAI();
        }
    }
    #endregion


    public void StartAI()
    {
        StartCoroutine(CreateTempFileAndSendRequest());
    }

    private IEnumerator CreateTempFileAndSendRequest()
    {
        // Create a temp file
        //tempFilePath = Path.Combine(Application.temporaryCachePath, "conversation.txt");
        //File.WriteAllText(tempFilePath, "user1: Hi\nuser2: Hello");
        SaveConversationToFile();

        // ToDo
        // Get the last days conversasion...

        // Create form data
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", File.ReadAllBytes(tempFilePath), "conversation.txt", "text/plain");
        form.AddField("flavor", "llama");
        form.AddField("temperature", "0");
        form.AddField("top_p", "0");

        // Send POST request
        using (UnityWebRequest request = UnityWebRequest.Post($"{API_BASE_URL}/services/summarize/en", form))
        {
            request.SetRequestHeader("accept", "application/json");
            //request.SetRequestHeader("Content-Type", "multipart/form-data");
            request.SetRequestHeader("Authorization", GetBasicAuthHeader());//GetBasicAuthHeader());
            yield return request.SendWebRequest();

            try
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("File sent successfully: " + request.downloadHandler.text);
                    jobId = JsonUtility.FromJson<JobResponse>(request.downloadHandler.text).jobId;

                    // Start polling for results
                    StartCoroutine(GetSummaryResult(jobId));
                }
                else
                {
                    Debug.LogError("Error sending file: " + request.error);
                }
            }
            finally
            {
                DeleteTempFile();
            }
        }
    }

    private IEnumerator GetSummaryResult(string jobId)
    {
        string url = $"{API_BASE_URL}/results/{jobId}";

        // Keep polling until the status is "complete"
        while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("accept", "application/json");
                request.SetRequestHeader("Authorization", GetBasicAuthHeader());
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    SummaryResponse response = JsonUtility.FromJson<SummaryResponse>(request.downloadHandler.text);

                    if (response.status == "complete")
                    {
                        Debug.Log("Summarization Complete: " + response.summarization);

                        // ToDo
                        // Add the response.summarization to a view or in the chat...
                        meetingSummarizationText.alignment = TextAlignmentOptions.TopLeft;
                        meetingSummarizationText.text = response.summarization;

                        break;
                    }
                    else
                    {
                        Debug.Log("Waiting for summarization... Retrying in 5 seconds.");
                        meetingSummarizationText.alignment = TextAlignmentOptions.Center;
                        meetingSummarizationText.text = "\n\n\n\n\n\nWaiting for summarization... \n\nRetrying in 5 seconds.";
                        yield return new WaitForSeconds(5);
                    }
                }
                else
                {
                    Debug.LogError("Error fetching result: " + request.error);
                    break;
                }
            }
        }
    }

    private void DeleteTempFile()
    {
        if (File.Exists(tempFilePath))
        {
            File.Delete(tempFilePath);
            Debug.Log("Temporary file deleted: " + tempFilePath);
        }
    }

    private string GetBasicAuthHeader()
    {
        string credentials = $"{USERNAME}:{PASSWORD}";
        string encodedCredentials = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
        return $"Basic {encodedCredentials}";
    }

    [System.Serializable]
    private class JobResponse
    {
        public string message;
        public string jobId;
    }

    [System.Serializable]
    private class SummaryResponse
    {
        public string status;
        public string message;
        public string summarization;
    }
}