package com.yourcompany.filepicker;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.provider.DocumentsContract;
import android.webkit.MimeTypeMap;

import com.unity3d.player.UnityPlayer;

public class FilePickerPlugin {
    private static final int REQUEST_CODE_FOLDER = 42;
    private static final int REQUEST_CODE_FILE = 43;
    
    private static Activity unityActivity;
    
    public static void Initialize(Activity activity) {
        unityActivity = activity;
    }

    public static void OpenFolderPicker() {
        Intent intent = new Intent(Intent.ACTION_OPEN_DOCUMENT_TREE);
        unityActivity.startActivityForResult(intent, REQUEST_CODE_FOLDER);
    }

    public static void OpenFilePicker() {
        Intent intent = new Intent(Intent.ACTION_GET_CONTENT);
        intent.setType("*/*"); // Accept all file types
        intent.addCategory(Intent.CATEGORY_OPENABLE);
        unityActivity.startActivityForResult(intent, REQUEST_CODE_FILE);
    }

    public static void OnActivityResult(int requestCode, int resultCode, Intent data) {
        if (resultCode == Activity.RESULT_OK) {
            Uri uri = data.getData();
            if (uri != null) {
                String path = uri.toString(); // Get the file path (you may need to process this further)
                
                if (requestCode == REQUEST_CODE_FOLDER) {
                    UnityPlayer.UnitySendMessage("Rainbow", "OnFolderSelected", path);
                } else if (requestCode == REQUEST_CODE_FILE) {
                    UnityPlayer.UnitySendMessage("Rainbow", "OnFileSelected", path);
                }
            }
        }
    }
}
