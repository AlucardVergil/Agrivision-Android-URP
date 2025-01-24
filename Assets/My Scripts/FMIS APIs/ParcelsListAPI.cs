using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Newtonsoft.Json;


public class ParcelsListAPI : FMIS_API
{
    private string jsonResponse;

    public GameObject parcelListScrollViewContent;
    public GameObject parcelItemPrefab;

    public string selectedParcelId;
    public ARFieldVisualizer arFieldVisualizer;

    public string email;



    public void GetParcelsListData(Action<string> onParcelsDataReceived)
    {
        if (parcelListScrollViewContent.transform.childCount == 0) // Only execute this once if content has no children and so it never instantiated the parcels in the list
            StartCoroutine(GetParcelsListDataEnumerator(onParcelsDataReceived));
        Debug.Log("EMAIL= " + email);
    }



    // Coroutine to fetch data from the parcels list API
    IEnumerator GetParcelsListDataEnumerator(Action<string> onParcelsDataReceived)
    {
        // Create the request URL by appending the parcel ID
        string url = $"https://api.cropapp.gr/external/parcels";

        // Create the UnityWebRequest with GET method
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Set the API key header
        request.SetRequestHeader("apiKey", apiKey);
        request.SetRequestHeader("X-USER-EMAIL", email);

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Check for network or HTTP errors
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onParcelsDataReceived?.Invoke(null); // Notify the caller that an error occurred
        }
        else
        {
            // Successful request, log the response
            jsonResponse = request.downloadHandler.text;
            Debug.Log("Parcels List Data: " + jsonResponse);

            // Pass the result back through the callback
            onParcelsDataReceived?.Invoke(jsonResponse);


            // Parse the JSON response into C# objects
            ParseParcelsListData(jsonResponse);
        }
    }

    // Method to parse the JSON data and log some information
    void ParseParcelsListData(string jsonResponse)
    {
        // Parse the JSON response into the ParcelsListData structure
        //ParcelsData parcelsData = JsonUtility.FromJson<ParcelsData>(("{\"parcels\":" + jsonResponse + "}"));
        ParcelsData parcelsData = JsonConvert.DeserializeObject<ParcelsData>(("{\"parcels\":" + jsonResponse + "}")); // I used this instead of JsonUtility.FromJson bcz it didn't support complex types like nested array for coordinates



        // Log some information about the forecast
        if (parcelsData.parcels.Length > 0)
        {
            for (int i = 0; i < parcelsData.parcels.Length; i++)
            {
                var parcelItem = Instantiate(parcelItemPrefab, parcelListScrollViewContent.transform);
                parcelItem.GetComponent<ParcelsListItem>().parcelId = parcelsData.parcels[i].id.ToString();                
                parcelItem.GetComponent<ParcelsListItem>().parcelData = parcelsData.parcels[i];
                parcelItem.GetComponent<ParcelsListItem>().InstantiateParcelItem();

                arFieldVisualizer.parcels = parcelsData.parcels;

                Debug.Log("PARCELS size= " + parcelsData.parcels[0].id);
                Debug.Log("PARCELS name= " + parcelsData.parcels[0].name);
                Debug.Log("PARCELS size= " + parcelsData.parcels[0].size);
                Debug.Log("PARCELS crop_type= " + parcelsData.parcels[0].crop_type);
                Debug.Log("PARCELS sowing_date= " + parcelsData.parcels[0].sowing_date);
                Debug.Log("PARCELS harvesting_date= " + parcelsData.parcels[0].harvesting_date);
                Debug.Log("PARCELS crop_type_support_diseases= " + parcelsData.parcels[0].crop_type_support_diseases);

                Debug.Log("PARCELS shape.type= " + parcelsData.parcels[0].shape.type);

                Debug.Log("Parcel test" + i + " " + parcelsData.parcels[i].shape.coordinates[0][0][0]);
                //Debug.Log("2 Parcel test" + i + " " + parcelsData.parcels[i].shape.coordinates[0].coordinates[0]);


                var test = parcelsData.parcels[i].shape.coordinates[0];
                Debug.Log("Parcel" + i + " " + parcelsData.parcels[i].shape.coordinates[0]);
                Debug.Log("Parcel length" + i + " " + parcelsData.parcels[i].shape.coordinates.Length);




                Debug.Log("parcel Type of coordinates: " + parcelsData.parcels[i].shape.coordinates.GetType());
                Debug.Log("parcel Value: " + JsonUtility.ToJson(parcelsData.parcels[i].shape.coordinates));


            }
        }
    }

}




//Parcels List Data: [{"id":10476,"name":"\u039c16- Control (\u0398\u0395\u03a3\u0393\u0397)","size":16.49,"crop_type":"alfalfa","sowing_date":"2024-02-01","harvesting_date":"2024-12-15","shape":{"type":"Polygon","coordinates":[[[22.618798, 39.500943],[22.619136, 39.500472],[22.622011, 39.501634],[22.621667, 39.502091],[22.618798, 39.500943]]]},"bbox":{ "type":"Polygon","coordinates":[[[22.617798166, 39.499472006],[22.617798166, 39.503090972],[22.623010401, 39.503090972],[22.623010401, 39.499472006],[22.617798166, 39.499472006]]]},"crop_type_support_diseases":true},{ "id":179447,"name":"\u0392\u03b1\u03bc\u03b2\u03b1\u03ba\u03b9 \u03c6\u03b1\u03c1\u03c3\u03b1","size":24.66,"crop_type":"cotton","sowing_date":"2024-04-22","harvesting_date":"2024-10-31","shape":{ "type":"Polygon","coordinates":[[[22.393377, 39.356485],[22.393741, 39.357132],[22.39636, 39.355613],[22.395814, 39.354798],[22.393377, 39.356485]]]},"bbox":{ "type":"Polygon","coordinates":[[[22.392377869, 39.353798058],[22.392377869, 39.358129662],[22.397359735, 39.358129662],[22.397359735, 39.353798058],[22.392377869, 39.353798058]]]},"crop_type_support_diseases":true},{ "id":179437,"name":"\u03bc\u03b7\u03b4\u03b9\u03ba\u03b7 \u03c6\u03b1\u03c1\u03c3\u03b1","size":7.89,"crop_type":"alfalfa","sowing_date":"2024-02-01","harvesting_date":"2024-12-15","shape":{ "type":"Polygon","coordinates":[[[22.463959, 39.338559],[22.465947, 39.338602],[22.465914, 39.338109],[22.463948, 39.338219],[22.463959, 39.338559]]]},"bbox":{ "type":"Polygon","coordinates":[[[22.462948523, 39.337110562],[22.462948523, 39.339601766],[22.466944767, 39.339601766],[22.466944767, 39.337110562],[22.462948523, 39.337110562]]]},"crop_type_support_diseases":true},{ "id":5836,"name":"Spanaki Demo","size":6.01,"crop_type":"spinach","sowing_date":"2024-05-01","harvesting_date":"2024-06-30","shape":{ "type":"Polygon","coordinates":[[[21.692269, 39.636541],[21.693132, 39.635601],[21.693572, 39.635877],[21.692695, 39.636791],[21.692269, 39.636541]]]},"bbox":{ "type":"Polygon","coordinates":[[[21.691270232, 39.634601604],[21.691270232, 39.63779011],[21.694571705, 39.63779011],[21.694571705, 39.634601604],[21.691270232, 39.634601604]]]},"crop_type_support_diseases":true},{ "id":194091,"name":"Test","size":5.98,"crop_type":"spinach","sowing_date":"2024-09-01","harvesting_date":"2024-10-31","shape":{ "type":"Polygon","coordinates":[[[22.583941, 40.827614],[22.584042, 40.826532],[22.584544, 40.826555],[22.584617, 40.827638],[22.583941, 40.827614]]]},"bbox":{ "type":"Polygon","coordinates":[[[22.582945328, 40.825533048],[22.582945328, 40.82863737],[22.585614736, 40.82863737],[22.585614736, 40.825533048],[22.582945328, 40.825533048]]]},"crop_type_support_diseases":true},{ "id":90328,"name":"Test Weather","size":11.68,"crop_type":"wheat","sowing_date":"2023-11-01","harvesting_date":"2024-06-30","shape":{ "type":"Polygon","coordinates":[[[22.587869, 40.798825],[22.587545, 40.797279],[22.588347, 40.797189],[22.58864, 40.798746],[22.587869, 40.798825]]]},"bbox":{ "type":"Polygon","coordinates":[[[22.586545001, 40.796191835],[22.586545001, 40.799819792],[22.589639783, 40.799819792],[22.589639783, 40.796191835],[22.586545001, 40.796191835]]]},"crop_type_support_diseases":true},{ "id":10199,"name":"\u03a4\u03a1\u0391\u0399\u0391\u039d\u039f\u03a3 AGRIBIT","size":12.11,"crop_type":"peach","sowing_date":"2025-01-01","harvesting_date":"2025-12-31","shape":{ "type":"Polygon","coordinates":[[[22.391444, 40.767253],[22.392205, 40.767253],[22.392219, 40.765593],[22.39143, 40.76558],[22.391444, 40.767253]]]},"bbox":{ "type":"Polygon","coordinates":[[[22.390430035, 40.764580136],[22.390430035, 40.768253],[22.393218964, 40.768253],[22.393218964, 40.764580136],[22.390430035, 40.764580136]]]},"crop_type_support_diseases":true},{ "id":179458,"name":"\u03b2\u03b1\u03bc\u03b2\u03b1\u03ba\u03b9 \u03b8\u03b5\u03c3\u03b3\u03b7","size":21.19,"crop_type":"cotton","sowing_date":"2024-04-20","harvesting_date":"2024-10-15","shape":{ "type":"Polygon","coordinates":[[[22.694584, 39.442751],[22.695409, 39.443776],[22.696413, 39.444136],[22.696951, 39.443804],[22.695696, 39.442253],[22.694584, 39.442751]]]},"bbox":{ "type":"Polygon","coordinates":[[[22.693586349, 39.441254073],[22.693586349, 39.445135894],[22.697948229, 39.445135894],[22.697948229, 39.441254073],[22.693586349, 39.441254073]]]},"crop_type_support_diseases":true},{ "id":10864,"name":"stevia","size":1.32,"crop_type":"rice","sowing_date":"2024-04-20","harvesting_date":"2024-09-30","shape":{ "type":"Polygon","coordinates":[[[22.352131, 38.931054],[22.352046, 38.930472],[22.352289, 38.93042],[22.352338, 38.931035],[22.352131, 38.931054]]]},"bbox":{ "type":"Polygon","coordinates":[[[22.351046929, 38.929420235],[22.351046929, 38.932049814],[22.353334841, 38.932049814],[22.353334841, 38.929420235],[22.351046929, 38.929420235]]]},"crop_type_support_diseases":false},{ "id":10475,"name":"M16 -DIANA (\u0398\u0395\u03a3\u0393\u0397)","size":16.69,"crop_type":"alfalfa","sowing_date":"2023-02-01","harvesting_date":"2023-12-13","shape":{ "type":"Polygon","coordinates":[[[22.619488, 39.500002],[22.622384, 39.501114],[22.622015, 39.501602],[22.619166, 39.50046],[22.619488, 39.500002]]]},"bbox":{ "type":"Polygon","coordinates":[[[22.618166072, 39.49900207],[22.618166072, 39.502601964],[22.623383543, 39.502601964],[22.623383543, 39.49900207],[22.618166072, 39.49900207]]]},"crop_type_support_diseases":true},{ "id":10478,"name":"\u039c16- Control (\u0398\u0395\u03a3\u0393\u0397)","size":21.76,"crop_type":"cotton","sowing_date":"2023-04-21","harvesting_date":"2023-12-13","shape":{ "type":"Polygon","coordinates":[[[22.706509, 39.423821],[22.708184, 39.422213],[22.708893, 39.422688],[22.707332, 39.424501],[22.706726, 39.424077],[22.706509, 39.423821]]]},"bbox":{ "type":"Polygon","coordinates":[[[22.705510317, 39.421213045],[22.705510317, 39.425500806],[22.709892165, 39.425500806],[22.709892165, 39.421213045],[22.705510317, 39.421213045]]]},"crop_type_support_diseases":true},{ "id":157212,"name":"Machi","size":6.05,"crop_type":"cotton","sowing_date":"2024-05-03","harvesting_date":"2024-12-04","shape":{ "type":"Polygon","coordinates":[[[23.874578, 40.983003],[23.873888, 40.982629],[23.874484, 40.982081],[23.875262, 40.982335],[23.874578, 40.983003]]]},"bbox":{ "type":"Polygon","coordinates":[[[23.872890228, 40.981086378],[23.872890228, 40.984000316],[23.876261718, 40.984000316],[23.876261718, 40.981086378],[23.872890228, 40.981086378]]]},"crop_type_support_diseases":true},{ "id":10477,"name":"\u039c16-DIANA (\u0398\u0395\u03a3\u0393\u0397)","size":21.04,"crop_type":"cotton","sowing_date":"2023-04-21","harvesting_date":"2023-10-31","shape":{ "type":"Polygon","coordinates":[[[22.705881, 39.422961],[22.707333, 39.421578],[22.708157, 39.422177],[22.706491, 39.423802],[22.705881, 39.422961]]]},"bbox":{ "type":"Polygon","coordinates":[[[22.704881067, 39.420578545],[22.704881067, 39.424801948],[22.7091567, 39.424801948],[22.7091567, 39.420578545],[22.704881067, 39.420578545]]]},"crop_type_support_diseases":true},{ "id":11256,"name":"Test","size":28.73,"crop_type":"orange","sowing_date":"2025-01-01","harvesting_date":"2025-12-31","shape":{ "type":"Polygon","coordinates":[[[23.916462, 38.663856],[23.915628, 38.66248],[23.916823, 38.661658],[23.9183, 38.662687],[23.916462, 38.663856]]]},"bbox":{ "type":"Polygon","coordinates":[[[23.914628254, 38.660658004],[23.914628254, 38.66485553],[23.919299779, 38.66485553],[23.919299779, 38.660658004],[23.914628254, 38.660658004]]]},"crop_type_support_diseases":true}]