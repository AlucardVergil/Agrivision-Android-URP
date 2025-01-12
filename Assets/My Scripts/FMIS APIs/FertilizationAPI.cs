using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class FertilizationAPI : FMIS_API
{

    public void GetFertilizationData(string parcelID, Action<string> onFertilizationDataReceived)
    {
        // Start the coroutine to fetch the fertilization data
        StartCoroutine(GetFertilizationDataEnumerator(parcelID, onFertilizationDataReceived));
    }

    // Coroutine to fetch data from the management zones API
    IEnumerator GetFertilizationDataEnumerator(string parcelID, Action<string> onFertilizationDataReceived)
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(parcelID))
        {
            Debug.LogError("API key or Parcel ID is missing.");
            yield break;
        }

        string url = $"https://api.cropapp.gr/external/management-zones?parcel_id={parcelID}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("apiKey", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onFertilizationDataReceived?.Invoke(null);
        }
        else if (request.responseCode == 412)  // Handle HTTP 412 error
        {
            Debug.LogError("Error 412: Precondition Failed. Check if the parcel_id or apiKey is correct.");
            onFertilizationDataReceived?.Invoke(null);
        }
        else
        {
            // Get the JSON response
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Fertilization Data: " + jsonResponse);

            onFertilizationDataReceived?.Invoke(jsonResponse);

            //ParseFertilizationData(jsonResponse);
        }
    }


    public Texture2D ParseFertilizationData(string jsonResponse)
    {
        // Parse the JSON response using the FertilizationData class
        FertilizationData fertilizationData = JsonUtility.FromJson<FertilizationData>("{\"managementZones\":" + jsonResponse + "}");

        // Example: Log the data for the most recent season
        if (fertilizationData.managementZones.Length > 0)
        {
            Debug.Log("Season: " + fertilizationData.managementZones[0].season);
            Debug.Log("Image (Base64): " + (string.IsNullOrEmpty(fertilizationData.managementZones[0].image) ? "No image available" : "Image available"));
        }

        // Loop through the seasons and log each one
        for (int i = 0; i < fertilizationData.managementZones.Length; i++)
        {
            Debug.Log($"Season: {fertilizationData.managementZones[i].season}, Image: {(string.IsNullOrEmpty(fertilizationData.managementZones[i].image) ? "No image available" : "Image available")}");
        }


        // Set the image texture
        if (fertilizationData.managementZones.Length > 0 && !string.IsNullOrEmpty(fertilizationData.managementZones[0].image))
        {
            Texture2D parcelImage = Base64ToTexture(fertilizationData.managementZones[0].image);
            //imageUI.texture = parcelImage;  // Set the texture to the RawImage component
            return parcelImage;
        }

        return null;
    }




    //public Texture2D ParseFertilizationData(string jsonResponse)
    //{
    //    // Parse the JSON response using the FertilizationData class
    //    FertilizationData fertilizationData = JsonUtility.FromJson<FertilizationData>("{\"managementZones\":" + jsonResponse + "}");

    //    // Example: Log the data for the most recent season
    //    if (fertilizationData != null)
    //    {
    //        Debug.Log("Season: " + fertilizationData.season);
    //        Debug.Log("Image (Base64): " + (string.IsNullOrEmpty(fertilizationData.image) ? "No image available" : "Image available"));
    //    }

    //    Debug.Log($"Season: {fertilizationData.season}, Image: {(string.IsNullOrEmpty(fertilizationData.image) ? "No image available" : "Image available")}");

    //    // Set the image texture
    //    if (!string.IsNullOrEmpty(fertilizationData.image))
    //    {
    //        Texture2D parcelImage = Base64ToTexture(fertilizationData.image);
    //        //imageUI.texture = parcelImage;  // Set the texture to the RawImage component
    //        return parcelImage;
    //    }

    //    return null;
    //}


    // Function to convert Base64 string to Texture2D
    public Texture2D Base64ToTexture(string base64)
    {
        byte[] imageBytes = System.Convert.FromBase64String(base64);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes); // Automatically resizes the texture based on image size
        return texture;
    }

}
//iVBORw0KGgoAAAANSUhEUgAAAZAAAAFtBAMAAADbnSE4AAAAD1BMVEX\/\/\/\/lUBz1tC4ZnkP\/\/\/8WjqXgAAAH\/klEQVR4Xu1dbZbbOAzzpieYPcG89gL72vvfbW0lTmILlEyalOQU+dE21IcJAiRlJzOdJr4YgSEj8OPPn\/+GdEzrFIFoIxY6f6YjvUIv0mJzAmkRZcU1HsKa\/7p44SIQBevxU190pH9dV1wEEi8W3RU+gpG1EW7A6OIwxmwCGYOHpxe79FjfXq8CE8hQyhLoSOahHK05QyC1CDUdL9GxjF2mcBFIU91ULwZPJjuSqpuMMIFARmDh6cMROi5RuAhkKF3V+sdmfCjPd84QyFDsqOgYuXARyFC6mo72j+ELF4EMJSwTHUlkQ8GYbAlCIIEsfoa07CgGa+8EkrJ9nEdcyZ0zr8DcVW19BkNaq7pa4OSPAHIuz9cQDJAmBLLRY29G6nT8\/ple9SwKTOEDWxNIztCBsEVNqdJxV9Xzz1+59++WfmlCIJiZboxgd57Wna7ub8vqisqByr4EIkagErmQYdGZx8CjD+b6Ki7skCZFf+ZBAgnRj7BptX+IdNRLl3DJGDOBPFK\/1ExiQo93LeZ5RVWvKibt0rBwSS4kO4Gs4WnFSImOvPuVLEKmEAhOaNH6IYwUOkhJRcKYEJQW4iIQyAkZETNYMYCiCMN9zIi2a\/NsHl35mM9wFtqOQBTCmqfeYGStRsxIk8\/iCASS1pGRaYIeWY3C0bHJh4pWn+E6AtGVKDzbM98lRi5XuAgEi0Vr9RSXUIHbHFQIBJTgroz4dkUJSot7Xtf2TiDaGgXnO+a7xMjlCheBQKlojY7SmmuyREqDwkUgoCd2ZcS3J\/4UT8FayRvmw9BajQRiYCBb4pnvIiMtuiKBgETqyohn4SoAafEcAoTWaCKQrASZDMbwg2WdGfH7kKEE5FIVmEBMKYEWAb3bTNItSbI3uC+xeQ1WEQjSicV2A9G1mIqMXKlwEYhFRniNRUhgTbmVXKhwEQjWickKhGIxdc93rw8ZCMQkI7jIqSv2z3cC2daE\/ow4PVCpAGlx4vIpXAQCK5DR6JPvlVbS4sRFIJvCNQAj\/3y9vSwnrbSmBqTBcwgCUUmrASPT9K6try+juqqtpEFXJJB38kZgZNrk+8LPvzZ91UpX+ANUAtkRR0aMx0SwbFu47u8MeVLNeHBpXxOBbGnrz0hegRNHpjJcSnlfHaHdsgpMIMUTPYqhtw3lu+0IWcqU8Oa+PwOvsAw1WP564Jw9BKIQIPM9lx8rsEJApalYXHnAj1hkUhrkO4EAiroyIjQT04mr9MyuJG+nMdjeCUSSl1PUi9ugfHdnpMVBhUDywiUJq83R0bNwlY7zRX37DPoVLgLxYUR4DpHnwAFLIU0a\/L4OVLhs97ydb98\/HIjtGVfpxBV+nMeMEIiY8OGMePbEe2WTGopTmZW38euJBCJHWTPyMYwIzd14X7LIS8j48HzHFZhA+jGCK\/AJRsQn9JrUNc1F+U4gc7Z364m4cB24BRGnCFD6FC7RywMDBGLK8fdFIN8PBL4wReiKpx2tbUAgEim9GAFdUXLxmF0A0uBJcCauYw5Lswikls318f0pWIr1QbvQShp8I5hAMEX9GNkXLuzfYasIpHnhOuwynkgg9bJUn7HJdxxojVUkpe1xXuMynksgde1UZ7wdVHCUNVaRkQZdkUAQUz0ZeXsOgVxT20QsDQuX2mm0gECqZak24dUTUYDVts9nJLwCt2KEQGq58Rpfe6I6H\/AC6SlE\/M8AEMhojKzNHfultsrSapXvapfxAgI5Xp7kmSnfcYDV1hIj4Q9UCATw1ZWRe+E68wn1GyLx3Jggyvp2GUknLgLZCqwrI9Mirk9ghEBA2eosrfQhA3DLYupbgQkk56wzI3Phyn0yWYpA4u8UCSRjrTcjc+Fy6onSN2hXhC7HqtImBLJXV0VcpWD6jLkVrnJ7D79T9CtcBOKjrHkXpxNXhZHwO0UC2ZctMuKWI26Fq9JKrlOBCcRPXE4nrgHy3ecMTCBu2nKS1txaKqREfz+FQPbtvTcjbj1xBlbpJm7pgDdyu70iEBxgg9XtMXDvNCGQ4QqXz\/\/BcodVbosG4auWZKG1GwhEFXlx8s1OQbayKycEkvFROwWLovAaAB6ZTcUzV\/Rx3uw1WEggLvJivgNtsQK7aMvzxFUuwlcqXMVbEwI5qjzPwlW6g49mZCIQUIHJyNE8KM+DoTUaxRNX2QWfUaPPcBmBuHDiWbjgicvFywObEAhIk66MOP3Pzw9YWb4f0ITbFBBas4lAPGgxhx8s3DLi4Z1iD+CP2UQgisDLU29mAvKFrwosXy9shEByQt7u3MPiXtrYj5NVWuE3txgOgWTi6syI54FrhtJJV0ltWWjNBgLB6au2uuX7r+ifPK5AI5B9MvVmxCnfvyvEtxjeh9b0nkA8qTqd7yPQsQSEQNZsGoWRs4XLU+Yn9zIVqueikxf3XE4gKQKeIT27l71wnb2y83oCGUpXiV1bvn87K8NhOwJxCKLvFvp8H1BXS0gIxFcYHrup8t3jglF7EEhUZK37Khj5tl6jyToCaRJm1UUgJ+tHH8\/BsXWVABOIivcWk7NzyvYbTL8Xylr4cfoaBHI6hO4bbPO96\/ewzmEjkHPxC1j9zPf8K5cBVwvckkACg2vcesn3XFedP0a3YCEQS9RC19zy3+ASer2wzQkkLLTGjT+GkWnaHRZ7fjXOSMZjGYGci1\/A6h8vTq6rqyUuBBKgjpNb3rV1bV2lEBDISSX4L1\/y3X\/XDjsSSIeg85J\/XwT+B98EOdDAMXaQAAAAAElFTkSuQmCC"},{"season":2023,"image":"iVBORw0KGgoAAAANSUhEUgAAAZAAAAFtBAMAAADbnSE4AAAAD1BMVEX\/\/\/\/lUBz1tC4ZnkP\/\/\/8WjqXgAAAH5klEQVR4Xu1dbXbcNgxU3BOkJ8hLL9D3ev+7lavd1crCB0EQICVl9pdNkRQHgxlAsuMsCz73isB\/5fPvHSAByNlYBCNgJCMCj7xaPxmbj9wTQEZGu36vvzZCLl4T7wFkj+LSegeQuvYGziB0XLWWAMjArLHcalc\/9l9e7\/kKQCx0D5vDCv3F0bBDRNwIQCKiGLaHRscjvS5jXAASlhMxGwkFZD8cc6PsXQAkO8JN+xvouIZxAUgT77mTa+XjO1m5Z+naHUC6whe9uI2OExsXgESnRud+xvJxfuMCkM5MiF3uouOMxgUgp3p4\/\/2Pm5CyMDbHu3YDkJXIrhiGLv79+HRk12lkAiAvizgHIysdz48\/u0Iz3bkZgOxLjzOIcct2dGhf1nNutky00zfl3FQgRhTvabUuZh4WAOG5ASPd3oXUOldqfTUSsk5XC8okkQAIn1hldAYjLjrW5BJhPC50+0\/zBgByJkbWDOn5KNk1VCY9GNa1ANIsZW3BVzchOivavUOvAch3JkWdhEZd3CyIjgcmqesS7x16AUCoQ0xlJJCQgkyQyYiaCCA0s+YyssRSImVXqD\/xmwEIm1sTGVkW4UTO4YnG5TyxsAxAeAk3jcbqfWJ9BxBWJhMZiTUuCciQl3VsaJ2DANJkUNLkUL0LpWTI61MA4YQ0k5GScoGkTNX7fYAEVhORkSF6BxCq99swUqCJWEa8GqKh9Y8AiNR7NI0HlhLpLUQplk1H8k0GEE5LcxsV7kTOMRHI1Z6wAMSn7+OqOL3LjAzJLQAhljCZkbgeWAEypCiGvZwHkKP\/eL8nye4cACNeBo7rnASQZdMZCXudIj5bPSEeAxj\/fVRNBJA4bki+uwYqjIzouFznJosAJC61YFzH9KpkV2Doha1u48BhzXyFkgHvT49J4vweQIScdww7KSDL1N5xxKMiOZFzAEAcWcQvCbLgCiMDOi4A+S6m+YxEFcVKKbmOcf05QPL17qwch2VVRgCELxvc6CAHztc7gLSVknxGokqJ8tsDr3qZ\/oQVY1wAwjmQc+zHzxhSqtXEeT7zMgBpre\/m0Lon\/iyfv2PyS2vp3eczLwQQyuJcRpYfD07Khx6seUTzrvSaCCAMX3MZKbbwzK4I7zqFTABkl2STGVmWl3PlkmKubR0TAYR6l5xdHYG2L32XxYC6KJtwflF8O3BIgQcQe\/6oM19675e9zMiI9ymbAwPI5l9gRM18+0Vo5FgTZ6fW9oS1o8bXe2lIhtbEDxIAERuuEYwcS8mbmKMGDN+LOMoFu\/34Z343LgDRf8zgj7N9Jc9IGTVkE5mieJf9RM6ZtwHClZIXOJ8RT\/s7gttzIuEGQFgnzq8mMiUuwct\/isCpYvMyACHu+xqQTNgcWvdEIvNt4GJ6BxApuVjbGvG\/GSh69\/34d5ZMAETKrVmMSM9XTxeQTquOz5KJbFy+hn5a63gbIEo37+u4hNTKf35XjEvVgnQRQNyN1rZQlomr45IcOP+3tAGEl8k8RoKNSwRyNeMCkH7b0jouGBdbGNNfDd3GgWXjik2t\/KIodlwAwmoknxHFuPj6XRvlcZTRCJNV9xD1Xjsyfx1A1GhbLo5iJF0mANKokXRGgmui\/Aeo040rtiYCiMWYqnN4UlxdisbILJlcEAjfqMQDSdc7gDC1RHkHMat1RGqJHTA0Ui0gnwlsKWEkYBhSCBnwE2sAoRTNZYQtJfSQlhEAaZC0NpV5VLTEn86Zzcj7H1weADnK4uTiDiAkuWYzwj+9h6fWgNaRLYoAopiXZp4x1xgLdv1GnQJivTTlJ1iO3JJ+52nDByDmxGP1TvzVMlDJLvOJnBMBhJA0mRG2m4dxaazAuMzqZ\/TuKSXq2+yVKvOJnBMB5GBcejM\/gBHWuIi7WgY0rT+uzdC75dxkDoA41U2WMc08ibZxoEIKuXXsAIBQmuYywr2FcNXEAqxiwrGpRHaLqokAQkLrHhim9+yieBsgjN6pJZlHVO9yZ41tIdW7+dh0IoDYgl6ZFSgTvZpcSO8AUkka42Wqd9d7oVX7eqdiPJB3GoBQ\/53LCPPw7u2BCzS1lgw3LgCZzAjpuDoYqWSX15GM647GBSCrkWl6N0a2Y9o3UvoY0Uy444TGpQDSVhaNYe2YBkbASEf6qEv3uZXlWtm91goQQKhIxMerIYzsu\/nO1OKL+yAUJbk+71MA5J1mXLs1jpGP3rsZYdot1TKDL27GBSCbg1HjCg66ul0eI+pt4y8CSLUmxge9suOrlPSLfV8TB\/ruBg9AaHJ9auIMRl41MTS1KtmcdPlpXACyy7AttZJCXt32ofcIRgqmgqV6u7wJAMIaV17A6zsXvUel1hTj3RDeBkh55o1gpE59\/gwA2es9P96GO3xRB2obMdxjyBQAefM2JNymm7Rl0mH2L9MtxkwCkEcEzsTI4tb7qVCU9AWQszGyLD69j\/GiprsASFO4Rkxu1\/v59LHGCUBGpEvbPZr0ftK8WhEDSBvx+bNbGMk\/TccdAKQjeDlLraUk5+6BuwJIYDBjtjJR8ivmXqm7AEhqeB2bgxFH0FKX3IaRegd8Bctaua71WwCSKgl+c5EUfvp5RwHkdNzwJnwZmX\/ieRsgjAlfkI6VGKJ3AJltAAeZzD6O\/\/4A4o9d1sqP3q8q9FdkACQrRdz7PvV+8bx6oAcQdw6kLSx6T9t76MYAMjTcuNkfGoH\/AeQklNLVbQhiAAAAAElFTkSuQmCC"},{"season":2022,"image":"iVBORw0KGgoAAAANSUhEUgAAAZAAAAFtBAMAAADbnSE4AAAAD1BMVEX\/\/\/\/lUBz1tC4ZnkP\/\/\/8WjqXgAAAISElEQVR4Xu1cWZbbOAzU61xgMidIOhfIS+5\/t5ElNc0FJEiQAClN+SdtbkKhUAAk29k2vJ7lgb\/76\/cTIAHIaiyCETCi4YFXXB0vjcMtzwQQS2\/z1\/rmCLl5TXwGEB\/FrfUOILz2DFckdNy1lgCIYdTUXMqrH\/6f97u\/ApAaus3WkEK\/ODIzYsSFAGSEF4edUaLjFV63SVwAMiwmxhyUKSD+8JgLaZ8CINoebjq\/go57JC4AaeJddzFXPkKydG3pOh1Autw3enMbHQsnLgAZHRqd51WWj\/UTF4B0RsLY7SI6VkxcALLUzbuYjmPj2BjvOg1AFmOkj5CFnnEByOWBNRJXLx3LyARAfA90Jf8RmwV0\/Pk8Xr\/CrbNl8gwgchSLkQIgJyHryASMgJERFSM5o\/lp3MWD\/88StQRAPErmMzKCDqJRSeJXfQBAIsVHraM6AcEF2usgka\/c0ESZAAhJzDRGRsk8l4TNZAIgZGAdg0F02TDSTMff6w49j+I14ydhAGnywFMYacRRFVZxaJl8axNAyloPn0KYPKprooSxPpi2zsAAwrFjzci2NbS+nPH+vH3rCCBlfuwZ2Wr1Xl0ND4Qx0U39hmwxgBRjawIjtYmrLbTijxhMOq7Ye\/R7ADn9skzHVVQEMenfJJ5QZLmoaVdN4iJsLQ4BSBMF6WKGlEaln1zFpKwgk\/sAKVeTohqykzEjRl92pKvHOZq1tTgBIKmEW0bGM5K0KTZfdgSQvE5SkUzMwKLE67ClNLdEu3AtXRMB5GBlCiN0TexjZKHEBSCn4NdJXMMZMbl7JxIXgORCy4QRInF1MrLDSWUiLHRN25LEDyCuTQEjTaEUL0ZouUhK\/pgTWsnHJf1ip1pHg99cxjURQLwQS4Rn82AovOwIRoiaaFHfASTJV4WaaMFImLiGhNYKiQtAwkCbn7gGMbLDSrAYPK3zrgkgSQ6bwYj35adxjLyQRQ1k3HuPf\/\/2HoAkoTWFkXdRHMtInLv0E5fr5gGEDC17RtwTrtGMTEtcAJIJrZgR\/X7+KiXajABIfRdwZuDhjCSxpf5oCEByMr\/GkyeP9UEiXbnrnTFKNh039OqNCoAwRJkzsn0wFsmnIyzS4K\/dByAsVcaMbBtrkXRBmIRrQ0S+Tmonuw9ApKTo6f1FmuNFvSYqZmAAkQaXLikuD4vNq9+oKhMAqSfCW8lWhY4FLnGJLGvc1GEnuxVAGsk4l5voXb8objYZGECagowVrXzB+96kySLhYrmd7E4AEXHC+lW+AIz8vxnRLCXv+\/eblxIAEYnE6BmX0LambfIEy+z0n3E1WSRczJgjn34MEOv\/2VHucnZn8ExbGDAN2z5YgzoWeFjUqwmA1BBlyYhmmxIKviHeRUutYkv9yxCPAaLYpkS1RD1x1chWuCaoJQBSrX1Fmdh+PgogvHJsGVFMXCEQ\/S878q4VrgCQ6kwVLdTSe8SI\/s9HAYTRjjkjWokrAXLXxAUg0qy171PSe9AAH286bKzaCiDlxGXPCBJXmZH0JxnIwFVSPxap6D0tJfqUAEhJJjMYUUpcaQZWL4o635sHkPo0lawsBXvPXEKK9mPHHmNLewEkiZnaAZVSEv\/k3aKbB5CSQmYwgo6rzAjRzOu3jiqNCtFx6d+9azQqAFJbN8h1TLxLpsEI6enaQYnLmT1zGMHNe5EWghTtZv45jKjUxJ2umBR1RgCkKJIpjGi0KS+U8T1vbUWQr2N8K50GEDklH1Knc\/usSQGQ1RjRqiUTkjDnWum8eX2XGsrtMwei0zvuMGMk6h+XaCUuAJFXRS7cxfMRKer9vNhQbiOASKNLS+9JQy81sHYfgHAiiW+xaj0rX8daRC74+T14EWvibn7RxAUg8tDhdhJRwQ+FgXW8IzZF0cVZ0jlPGMAPAUin14vbP3gCohWR0h09yUFRo1I0o38SQBKhTGak+SlEAsAbiLGEiUu7KMZXZ96vC6Tl5r2E4pwj3PDmpV\/SxRMa9A4gJytFf46YJOKBGspVkICnf6mdX9G1iN4BxN1naTNSmbiqGPlOxpZrV0YooXBGXeICEO+BcMGbY6aoXBON8UXkWkGfdWUubZnQFw9GAeR0hxEjFYmrmpF9IU2wSX3nExeAOHpMGNmTH0NKCyN0WTwRjUmzpVMAhOCKbFTASCmOiDmE1mqhxaWtuu73RJXVyA8iFIYPMbEFIK4i7n+YMMI8dmxhhG63jFDsoer7LvkbQJxL7BhhEheRZ\/NDaeIanp0KB5YTV95qYgZACn6un9JjpN6GISsBJBVJpJEhfm45JCkf\/oC0lPxosWDQWgBJg+vtkxmMMDXxsyG6HJBBwdJ4TDlxAUijO4csL+r98zMVQ35kP2qISbJDACRlZi4j27fjKe2fDDOpufkRq1vbTOg9Bsi2XR\/L0KTUlpKMm0yHAcSXi6nrcxc7ZZKTfF7dbiZ3sPU4gHxRYu35\/PUuvdPlhEtc\/+TPNZ8BkFdwrcTI1ajkKkohcS2FYo9kl7jIxgtAzMW+X\/BL7+7foPfKZa4ZljLXBBDGQfbTgd4PfuKbrVjzq2Wsy2cAYh883BUTve8D0X3jO7oWjasDI4BwVFvPU4zE0fVVGa1ta7oegDS5y2JxWko8ks78dYSWhS1d1wCQLvdpbC5S8gqzvf1a69Y24wUAyThm2jAYmeb6zIUfwwjdAfuti\/bvpjIebh\/O9FtuGEDafdq9I0tK98nGBwCIscP5y9FJ+DYyfwN8DBCimtyQjoOYRO8AwitSd0UkE92LaZ4OIJrelZ391vtdhX7hBhBZACjuOvV+87h6+QdAFKNEePSud+HOxbYByGKEwJxHeuA\/PYGzSRHEb3sAAAAASUVORK5CYII="},{"season":2021,"image":"iVBORw0KGgoAAAANSUhEUgAAAZAAAAFtBAMAAADbnSE4AAAAD1BMVEX\/\/\/\/lUBz1tC4ZnkP\/\/\/8WjqXgAAAH+klEQVR4Xu1dbXYjKQz0ZE6QPYFf5gL7Jve\/23bT\/ugY0ajUSGC2\/CsDEqhUJYHbTuZy4WuuDHwtr+sMkAhkNBbJCBnxyMCqq\/TyWDxyTQKJzHZ9r48HIW9+Js4BZI\/ireudQOq1F2iR0bGKK3D\/ZlsRSLNUtlkodan8dW2zeuAqOYY0QiCBHOy2+vVPgZBluE9Exl0JxJg4H7dfn5+fB9J6n3onEB+BmFddhFVuWduMee1QRwIJTXdts4WOQsf68717\/Vtbp\/s8gXSn4BnAenysL6Fl7VV1+3mgwF9DIZDXjHT9950OqWX96FcPlQ3auAikq47yzW8NK29ZQr96DOXL9B8hkP4c7CJAzsEfUhutcRHIUIyY6UgiG6hECGQwRgrvB+XrlXA6DlMmBHJjZwxG9Hd2QVT3oREaF4HsCerOyM9nDOo2lYms+8P5OYC0QrHwsy517aYvAslKhIy0USOlNZa0PvaE2E+QG6i\/\/doWgeTCWkZ6MfKDjq8vMTZocAXS4xsqBFKiqQcj+261\/FwKDR4PLxMCqXAUzMiuzCuBodPBZUIgVYIiGdkfH6evVxmyhKTNxby2CoFk2ZcG4hi5EdJeVHdYq7YinkEQiCQkaSyKkctCiZ+uNmQxjYtAJB3JYzGMXC7y7g1Ho8qkYcjyUgRSu5W8zv+WE9luNOp8JxAtZ1GMsHFpGYn7+EofkdEyqgNfWO9ahqLqfRpGljPSG8s0ZRIGJOY0eb0eufxbW7hmu5nemLgw8LqoOdNqx6UJRzwaUsdjNiSQV\/FU\/u19lGxMVoJoMU0gUNG0SHl1DSgiq3HEd4KtsUF+BFIV1M4got7\/XpGIjLYEAlRJCCMBl\/n1m1xGvUBuQGqtpgRCRqzaqfhRWpC03J8Lbd8\/DWhc\/mdi0GPHaYBEnIlBT+sqTafBNIFgfSugcaUPscCoDOb+9U4gKC0NCrqyBOsd46SSzgbTQYz4f\/kp\/cZPQONqkPPKEgSCFUnIoRjyeUnEoUggmLoqxXp+em1bUzSuyYD4f9B7XjuVFTZGCERf8d4d+MaIf70TSKU27tNhjAQ8GGK96ys9WSo1YjeLYsT9DhxWJmxcSrWFMeJeJukN7\/ICi9dgrkyt1YxAcE686317LuR\/B3bvwASCi8udlKjGRSDanhzGSNCxaBA97KLNrdFuOxbhqAwOxgC1bnFA3Ot9gRzzqwzu5zuBwIWi1fspOzgqg8OpALXOhrhgF20sp+zgqAwOpwLUOhvigl20sZyyg6MyOJwKUOtsiAt3+fD\/6zzf3xG\/XEIgWl2tdhGMXC4R2gr5T1gIBBEX3oVwj\/SW9A8Slc0Wjwz0mAbI+ofetpct02ov995FIGouNkN3Rp7aavh3mkWQYPXC5g9tEYiY\/3wQTjHscO9b3geKe5kQSC6fwxF3RvaNy\/e6AqsedNg1LgI5FNV9EkywwfxZ79tPXvdh9zIhEJWinkbujPxoXA96wChV5gbdQy77xkUgGkqg9JqMX+vdixf3MiEQjaB2Nu6MyI1r5an16WhSPuAkNi4CORAckFyjKev9IP3SlHu9T8NIWONypySqcRGIvotFlYk+IqMlgUht9mDMmGe9WxQj7h9aE8iBjMSpaTrwt\/fXCKLORALRN67Cjav1+8SAb9vI6npDIOt3hvJXeyDuZUIg4oFxNAgUrs00F1b7h0IrQFt0gBeBHOlImANyazQVO7AQydmhPjeus1EL\/gSiV5pQ70JGzw65MyKeiWejFvwJRC8t8b7lcCwCERlNhSJZhppfHY3RAW7TAAl7f+V+4xJP9\/ba8r86TgNEPEuWyhGOg5ND7qeJXO8noxbcCUTdhKcpEwIRCuFwSK0RsyHr\/ZCAfJKNS601ud5lxaXRPNu6EXVERkMC0fHwtDImGnA7kFFpyvT2q1O9lzCkcQIBhIKbHua+NIlWSLLHY4M8SrEejhMIlGPQ+OMw94VJU72DgaHmBAIXCppi2L4gn+NhGMbi4H0oHkdcmB0RSOGpYwHBfZj1Dktf7zBN4yo9dayIa8QyqYQsTxOIXvS4paVMLIy4X4IJBKYFVwvoIRf04SgMIjkMeFEhEFAsoDle7zZGBmxcBAJqBTcH1TUoIwtuAsG4waWCepARMoJqRmsPasv0UZz3XSthJRCgSEIYgR9DAACSaRCKRVyHb0DySQLRth+7HVjv2JNTe1i4J4FoygXPq90DZAQ4E+0xmTwJpCotU17POOWnRW2kiiHyJHxCr4WdzxPIGeEofOF6XziqkKLY1cGEQMq0OKRbtWRez5qR0r1LtaWPkSbs3IZAfNhIq1rqPacojVwdw6wvPQ0Q+D2vTEc9Y\/4WcmTgqH+Y9R3AkGXz+jYBFqfLJCBG1RYEcleZKl0hRrLulaPXkBB1myhDls0IRJdkzMpc7yPRsUImkNEYsd64MAGHWMsdqTYaEhq2SS1keR7bI8Yar\/fx6iNlikBiBIPsIpdBYXRQXSW8hZDlYQJBNGK0lVMvjxq3iHGTQ5ZHYyIy7iKHLI8atwhy+5CDzkaDwrFvQyD23Dl5qii5Om3eclkCaZnNFmuRkRZZbLnGNIzUb8Dv0LIStdkp\/jJAIC0rQLlWkRSl\/zBmBDIMFfdA5Cb8NmX+zOc0QIQm\/IZ0JGKyeieQp2D7\/PRSJn2CaLErgbTIYts1nvX+roV+yweBtBVGg9W2en9zXa15IJAGami8xFLvjVfstByBdEo8t\/1fZeA\/NRhscRAD6goAAAAASUVORK5CYII="},{"season":2020,"image":"iVBORw0KGgoAAAANSUhEUgAAAZAAAAFtBAMAAADbnSE4AAAAD1BMVEX\/\/\/\/lUBz1tC4ZnkP\/\/\/8WjqXgAAAHdElEQVR4Xu1ca3rtNgj0TVeQriBfuoF+7f73Vh+fxz2xQBZoQLI7+XWDXgzDgOy4XRb+XCsC\/64\/f18BEoHMxiIZISMREbjl1fYTsXnmngSSGe3js\/54EXLynngNIO8oTq13AjnWXuKMgo6z9hICScyalqPe+sf7P8\/3fEUgLXSnzRGF\/uAozQnEQQSCiCJsjxodt\/Q6TeEiEFhOYDZSGsi7GXNQ9C4EEh1h0\/4NdJyjcBGIiffYyUft4ydZsb507U4gXeFDL7bRMXHhIhB0anTu19g+5i9cBNKZCdjlLjpmLFwEMtXDu5uObSE2x7t2I5DJGOkjZKJ3XATyiMAchauXjmlkQiDvEegq\/ojFEDpum4yWyTWAwFDcNxpHCoHIESAj3VVLDqzbSkY6GbG\/jTviahAlBKISM4IRPB0bvM5sdywnEDWvBjBS96VvNFUmfa7WVxOIXedBMn8RZffIuYJA6uJIZiSajhWOM1GMywikMa+yGEkgJOfFEIG0Z1YOI0sKJRmFi0AsuZXByLKYPFon\/\/Ot\/PxV2SnjDlw5XhwiEOOdwz7doHeVjUe2VbLL7pd5BYGUoh\/LSGvhOsqrDZdYHzbjPIWLQO4sZTDSdOMqFSFaxsqkpXCJbpdGAjG3DWVBnZQy8rplbOE6kInudjkyGIjYTUovWyxjZUIgJUfTMdLUykscN4uKJaMtFiIlkMGM\/KzAHXRs2Vbw+zQofQxp\/tETCeSh\/pGMvFpJLxvV1Mp8wiKQt6aiplYKI8vHmyt9\/9SBpCAhkIK+wYwsS+GR01ABkvOHXqffxTICQd1VitA6DWSEjOwjgGol6kPiPen2x+J\/J5BdVRjOCKonHgDJuHE5C66NEQKxVAXq3ZZdltj65l6GEVTh0t+c3ptiwvtTTOEiEJ8ixFUgSvQ3p1k3LgLZRaD6fJXy8A4qwQdAEi4qBPIzt8YzgmqKR7f5hC9QMYXr\/wMkXu9JjBCIeCMRjUkVOL4tEoitlcQzgmolla8HHv0y\/AkLU7gIRCxBPiNI70fPvPEyIZCdui5z49pw1a70vsy3rEIVLgKxRL06F6X3lZKaUsJ7IvA7GwKpZoxlEJddYwsXMLsIxJJAtbkp3aTmAGqMQMoI6DJBRb26D65wVbpJfFNcgIWLQKoZYxgss91pqd24DP64pzrdLpcRiJuDnwvL0DotZISM7CLw69OZS\/tltdzK6IkEMhsjy\/L5594n5+\/6xTH+5elNMASyJ244IzBKVmSV2gVqGPo2n1cBsvz6RPWSR7IpORbeTQhkr\/bn76MYueUWqpfcsWiS14WKGSEQLbdGMQLs7qNlAtaI+pfF8Ap8GSAsXJrc1UsXps7qu1ymArNwqbmlfRLBwqXrYjfyoQfXNaJcHeOf3wlE4WsYI7ivNh\/ItKvjWb7SfhFEIM31SZ2I1bvKyNkKF4GoGWMYUEqp16ySEn5R8XqsrCMQQxIpU7MKV3huEYioElUj8dcU+I1r3N1RDK3fSCBKNWo3+4MvrlQZCZeJ6I7fSCDtOaTNxLYS9Z3jSpXmAchOILKQ9KYYzchKLJKUCpDwwnUdIMiLSo2RcL0TiKB3MoLpJkJonSYygmEEWYH1e+M6cqaHdwIBJZdT3OWysYwAeyKBgFIL9n+brz1bxT9erdEos91pGdwTrwME1hTrjCRkF+pBkUBQZQun92orybhxOctUsYxAYMmF0rv6Be2TK5jHykYEYpWJEkigufDIaaDeUaQ4CSiWDWeEN649J7xxoUTCG9c+t7SvzF9lIPxlXeGR0zC8cDn9lpYdYMHJQdxJ8shpIxAxwmbjhzP+wrKDbmJ2zbaAQEpKxjKC64krsrreo3tJGVq3ZSwQ2B14w1\/FYpOveTZQ7wRijr68wC0KYWG1dp1I7\/U\/\/RCInEmlFan3am5Ff1pHIFa1RzMCvajUm+KZ9F7t7gRSlijZgtR7\/aYinw+zEohQuIYyslKLJKXSFmFJpG5EIEJ2kRE1XwwDTK3ZUgtatrQbV\/QVZctAZG4RiEHUlalCtrtNQgVOyasNnttrYSGBVDKmfShU7+1u9M8kEEEkRQXuj3P7DlBGbuBekm\/3ATKTQKTUGskItpU80cX\/91ZCOmqh7bETiBDodhNc79\/f7YcjZxKIriJknC176R55Riwng+d63NXXgJ2zbKc75RmxnIyeC9T7F9o3036XAQK6qJiCFzTZI4ZiTZBvpm0LpzwG04lRk7tlEuWYdV8CeeagNXJx8z2qeK35ivPLvDOB3CIwEyP+vzJMhWLNRHfhIhCzjlsX+PTeunviPAJJDHbbUXa9zyb0B04CaSM8c5ZJ75Pm1RYvAslMm5azLIy07DdsDoEMC712cGsr0dZPYyeQaah4OtJEydd0bpcOEUgZk7EWMjI2\/uXpl2Hk+AZ8hpK1EXR03yKQMo\/DLSop4SeDDyAQcED7t5OL8Glk\/jsAlwEiFOET0rERU+idQPoV27fDTiZ9m41cTSAjoy+f\/VvvZxX6AxeByAQPtN71fvK8usWPQAZmkXL0qndl5GRmAjkZYXT3lBH4D6Az8mk2I7pTAAAAAElFTkSuQmCC