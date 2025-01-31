using System;
using System.Collections.Generic;
using UnityEngine;
using Rainbow;
using Rainbow.Model;
using Rainbow.Events;
using Cortex;
using UnityEditor;
using TMPro;
using System.IO;


public class FileManager : MonoBehaviour
{
    private FileStorage fileStorage;
    private InstantMessaging instantMessaging;
    private Contact myContact;
    private string fileDescriptorId; // To track file upload progress

    [Header("Displayed Paths For Contacts")]
    public TMP_Text uploadFilePath;
    public TMP_Text downloadFilePath;

    [Header("Displayed Paths For Bubbles")]
    public TMP_Text uploadFilePathBubble;
    public TMP_Text downloadFilePathBubble;

    [HideInInspector] public string selectedFilePath;

    ConnectionModel model;


    public void InitializeFileManager() // Probably will need to assign the variables in the other function bcz they are called too early and not assigned (TO CHECK)
    {
        if (model != null) return;

        model = ConnectionModel.Instance;
        
        instantMessaging = model.InstantMessaging;
        fileStorage = model.FileStorage;
        myContact = model.CurrentUser;

        // Subscribe to file upload progress updates
        fileStorage.FileUploadUpdated += FileUploadUpdatedHandler;
        fileStorage.FileDownloadUpdated += FileDownloadUpdatedHandler;
        fileStorage.FileStorageUpdated += FileStorageUpdatedHandler;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (fileStorage != null)
        {
            fileStorage.FileUploadUpdated -= FileUploadUpdatedHandler;
            fileStorage.FileDownloadUpdated -= FileDownloadUpdatedHandler;
            fileStorage.FileStorageUpdated -= FileStorageUpdatedHandler;
        }
    }



    public void OpenUploadFileDialog()
    {
        // Open file panel
        selectedFilePath = EditorUtility.OpenFilePanel("Choose a File", "", "*");

        // Check if a file was selected
        if (!string.IsNullOrEmpty(selectedFilePath))
        {
            Debug.Log($"File selected: {selectedFilePath}");

            if (uploadFilePath.isActiveAndEnabled)
                uploadFilePath.text = "File Selected: " + selectedFilePath;
            else
                uploadFilePathBubble.text = "File Selected: " + selectedFilePath;            
        }
    }



    // Get all received file descriptors
    public void GetAllReceivedFiles()
    {
        fileStorage.GetAllFilesDescriptorsReceived(callback =>
        {
            if (callback.Result.Success)
            {
                List<FileDescriptor> fileDescriptors = callback.Data;
                foreach (var file in fileDescriptors)
                {
                    Debug.Log($"File Received - ID: {file.Id}, Name: {file.FileName}, Size: {file.Size} bytes");
                }
            }
            else
            {
                HandleError(callback.Result);
            }
        });
    }

    // Share a file with a conversation
    public void ShareFileWithConversation(Conversation conversation, string message = "")
    {
        Debug.Log($"FILE UPLOAD: {selectedFilePath} -> {conversation}");
        instantMessaging.SendMessageWithFileToConversation(conversation, message, selectedFilePath, null, UrgencyType.Std, null,
        callbackFileDescriptor =>
        {
            if (callbackFileDescriptor.Result.Success)
            {
                var fileDescriptor = callbackFileDescriptor.Data;
                fileDescriptorId = fileDescriptor.Id;
                Debug.Log($"FileDescriptor created. Upload started. ID: {fileDescriptorId}");
                                
                GetComponent<FileManager>().StreamSharedFile(fileDescriptorId, onTextureReceived =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        GetComponent<ConversationsManager>().CreateChatMessage(message, true, myContact.Id, onTextureReceived);
                    });
                });
            }
            else
            {
                HandleError(callbackFileDescriptor.Result);
            }
        },
        callbackMessage =>
        {
            if (callbackMessage.Result.Success)
            {
                Debug.Log("File and message successfully sent to the conversation.");
            }
            else
            {
                HandleError(callbackMessage.Result);
            }
        });

        selectedFilePath = "";
        uploadFilePath.text = "";
        uploadFilePathBubble.text = "";
    }




    #region List of files (received, sent or both) by conversation

    public void GetFilesReceived(string conversationId)
    {
        fileStorage.GetFilesDescriptorReceivedInConversationId(conversationId, callback =>
        {
            if (callback.Result.Success)
            {
                List<FileDescriptor> files = callback.Data;
                // Process the received files
            }
            else
            {
                Debug.LogError("Error retrieving received files: " + callback.Result);
            }
        });
    }

    public void GetFilesSent(string conversationId)
    {
        fileStorage.GetFilesDescriptorSentInConversationId(conversationId, callback =>
        {
            if (callback.Result.Success)
            {
                List<FileDescriptor> files = callback.Data;
                // Process the sent files
            }
            else
            {
                Debug.LogError("Error retrieving sent files: " + callback.Result);
            }
        });
    }

    public void GetAllFiles(string conversationId)
    {
        fileStorage.GetFilesDescriptorInConversationId(conversationId, callback =>
        {
            if (callback.Result.Success)
            {
                List<FileDescriptor> files = callback.Data;
                // Process all files                
            }
            else
            {
                Debug.LogError("Error retrieving all files: " + callback.Result);
            }
        });
    }


    #endregion



    #region How to share/upload a file without to send an IM message and Download file



    public void UploadFileOnFilesPanel()
    {
        //UploadFileWithoutMessage(downloadFilePath);
    }



    public void UploadFileWithoutMessage(string filePath, Conversation conversation)
    {
        string fileDescriptorId = null;        

        fileStorage.UploadFile(filePath, conversation.PeerId, conversation.Type,
            callbackFileDescriptor =>
            {
                if (callbackFileDescriptor.Result.Success)
                {
                    FileDescriptor fileDescriptor = callbackFileDescriptor.Data;
                    fileDescriptorId = fileDescriptor.Id;
                    Debug.Log($"File descriptor created: {fileDescriptorId}");
                }
                else
                {
                    Debug.LogError("Error creating file descriptor: " + callbackFileDescriptor.Result);
                }
            },
            callbackResult =>
            {
                if (callbackResult.Result.Success)
                {
                    Debug.Log("File uploaded successfully.");
                }
                else
                {
                    Debug.LogError("Error uploading file: " + callbackResult.Result);
                }
            });
    }



    public void DownloadSharedFile(string fileDescriptorId, string destinationFolder, string destinationFileName)
    {
        fileStorage.DownloadFile(fileDescriptorId, destinationFolder, destinationFileName, callback =>
        {
            if (callback.Result.Success)
            {
                Debug.Log("File download started successfully.");
            }
            else
            {
                Debug.LogError("Error starting file download: " + callback.Result);
            }
        });
    }



    public void StreamSharedFile(string fileDescriptorId, Action<Texture> onTextureReceived)
    {
        fileStorage.GetFileDescriptor(fileDescriptorId, fileDescriptorResult =>
        {
            if (fileDescriptorResult.Result.Success && fileDescriptorResult.Data != null)
            {
                Debug.Log("File descriptor retrieved successfully!");

                // Create a memory stream instead of saving to disk, in order to display it in chat
                MemoryStream memoryStream = new MemoryStream();

                fileStorage.DownloadFile(fileDescriptorId, memoryStream, callback =>
                {
                    if (callback.Result.Success)
                    {
                        Debug.Log("File download started successfully.");
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {                        
                            Texture texture = LoadImageToChat(memoryStream);

                            onTextureReceived?.Invoke(texture);
                        });
                    }
                    else
                    {
                        Debug.LogError("Error starting file download: " + callback.Result);
                    }
                });
            }
            else
            {
                Debug.LogError("Failed to retrieve file descriptor!");
            }
        });


        
    }


    private Texture LoadImageToChat(MemoryStream memoryStream)
    {
        byte[] imageData = memoryStream.ToArray();
        Texture2D texture = new Texture2D(2, 2);

        if (texture.LoadImage(imageData))
        {
            Debug.Log($"TEXTURE DIMENSIONS= {texture.dimension} Height: {texture.height} Width: {texture.width}");
            //Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return texture; 
        }
        else
        {
            Debug.LogError("Failed to load image from stream.");
            return null;
        }
    }


    #endregion







    // Utility method to handle errors
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


    // Follow file upload progress
    private void FileUploadUpdatedHandler(object sender, FileUploadEventArgs evt)
    {
        if (evt.FileDescriptor.Id == fileDescriptorId)
        {
            if (evt.InProgress)
            {
                Debug.Log($"Uploading: {evt.SizeUploaded} bytes uploaded so far.");
            }
            else if (evt.Completed)
            {
                Debug.Log("File upload completed successfully.");
            }
            else
            {
                Debug.LogWarning("File upload stopped or failed.");
            }
        }
    }


    // Follow file download progress
    private void FileDownloadUpdatedHandler(object sender, FileDownloadEventArgs evt)
    {
        if (evt.FileId == fileDescriptorId)
        {
            if (evt.InProgress)
            {
                Debug.Log($"Downloading: {evt.SizeDownloaded}/{evt.FileSize} bytes");
            }
            else if (evt.Completed)
            {
                Debug.Log("File download completed successfully.");
            }
        }
    }



    // Event for whenever a file is added, deleted or updated in file storage
    private void FileStorageUpdatedHandler(object sender, FileStorageEventArgs evt)
    {
        Debug.Log($"ACTION: {evt.Action} -> ID: {evt.FileId} => {sender}");
        

    }
}
