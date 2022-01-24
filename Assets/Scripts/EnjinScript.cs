using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enjin.SDK;
using Enjin.SDK.Graphql;
using Enjin.SDK.Models;
using Enjin.SDK.Models.v2;
using Enjin.SDK.ProjectSchema;
using Enjin.SDK.PlayerSchema;
using Enjin.SDK.Shared;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using System.Net;
using System;


public class EnjinScript : MonoBehaviour
{
    [SerializeField] private RawImage displayNFT;
    static EnjinUser currentUser;
    static ProjectClient client;
    public static EnjinToken currentCharacterToken;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void createEnjinUser(string in_name)
    {
        //Creating a new player
        CreatePlayer newPlayer = new CreatePlayer()
            .Name(in_name);

        // Using a authenticated ProjectClient
        GraphqlResponse<AccessToken> newRes = client.CreatePlayer(newPlayer).Result;

        AccessToken createdAT = newRes.Result;

        if (newRes.Errors != null)
        {
            Debug.Log("Error in creation");
        }

        getPlayer(in_name);
    }

    public static void AuthUser(string in_name, string in_email, string in_password)
    {
        EnjinOauth req = new EnjinOauth()
            .Name(in_name);
        //.Email(in_name)
        //.Password(in_password);

        GraphqlResponse<EnjinUser> res = client.AuthUser(req).Result;

        EnjinUser lv_user = res.Result;

        client.Auth(lv_user.AccessTokens[0].Token);
        Debug.Log(lv_user.AccessTokens[0].Token);

        // Checks if the client was authenticated.
        if (client.IsAuthenticated)
        {
            Debug.Log("Client is now authenticated");
        }
        else
        {
            Debug.Log("Client was not authenticated");
        }
    }

    public static void AuthApp()
    {
        try
        {
            client = new ProjectClient(EnjinHosts.KOVAN);
            // Creates the request to authenticate the client.
            // Replace the appropriate strings with the project's UUID and secret.
            AuthProject req = new AuthProject()
                .Id(5800)
                .Secret("UuMrNJVbdMdlO0EGNoDiJkOUXkghgQ2O12PEFQE9");


            // Sends the request to the platform and gets the response.
            GraphqlResponse<AccessToken> res = client.AuthProject(req).Result;

            // Checks if the request was successful.
            if (!res.IsSuccess)
            {
                Debug.Log("AuthProject request failed");
                client.Dispose();
                return;
            }

            // Authenticates the client with the access token in the response.
            client.Auth(res.Result.Token);

            // Checks if the client was authenticated.
            if (client.IsAuthenticated)
            {
                Debug.Log("Client is now authenticated");
            }
            else
            {
                Debug.Log("Client was not authenticated");
            }
        } catch (Exception e)
        {

            File.WriteAllText("error.data", e.Message);
        }

    }

    public static int getPlayer(string in_name)
    {
        GetPlayer getPlayer = new GetPlayer()
            .Name(in_name);

        // Using a authenticated ProjectClient
        GraphqlResponse<EnjinUser> newerRes = client.GetPlayer(getPlayer).Result;

        currentUser = newerRes.Result;

        if (currentUser == null) return -1;
        if (currentUser.Identities[0].LinkingCodeQr != null) return -2;
        return 0;
        //        Debug.Log($"UserName: {currentUser.Name}, ID: {currentUser.Identities[0].Id} EthAddress: {currentUser.Identities[0].Wallet.EthAddress}");
    }

    public void getQRLinkingCode(RawImage in_QRCode)
    {
        Texture lv_img = null;
        using (WebClient wc = new WebClient())
        {
//            var json = wc.DownloadString(currentUser.Identities[0].LinkingCodeQr);
 //           Dictionary<string, object> lv_img_url = MiniJsonExtensions.dictionaryFromJson(json.Replace("\\/", "/"));
            //foreach (KeyValuePair<string, object> it_dict in lv_img_url)
            //{
            //    Debug.Log($"Key: {it_dict.Key} , Value: {it_dict.Value}");
            //}
            StartCoroutine(DownloadImage(currentUser.Identities[0].LinkingCodeQr, (getImg) =>
            {
                in_QRCode.texture =  getImg;
            }));
            //                Debug.Log($"Asset Name: {it_asset.Token.Name}, ID: {it_asset.Token.Id}, Index: {it_asset.Index} ");

        }

    }

    public static void createNewCharacter()
    {
        CreateNewCharacter new_request = new CreateNewCharacter()
            .IdentityID(currentUser.Identities[0].Id)
            .Name(currentUser.Name);


        GraphqlResponse<EnjinTransaction> newRes = client.CreateNewCharacter(new_request).Result;
    }

    public static void setMetadataURI()
    {
        string lv_link = @"https://jplanh.tk/Json/";
        string lv_extension = ".json";
        string lv_file = lv_link + NetworkMain.Username + lv_extension;
        SetUri new_request = new SetUri()
            .IdentityId(int.Parse(currentUser.Identities[0].Id))
            .Token_id(currentCharacterToken.Id)
            .Item_Uri(lv_file);

        GraphqlResponse<EnjinTransaction> newRes = client.SetUri(new_request).Result;

    }

    public static void mintCharacterAssetToUser()
    {
        MintAsset new_requst = new MintAsset()
            .IdentityID(currentUser.Identities[0].Id)
            .Token_id(currentCharacterToken.Id)
            .Recipients(new List<string>() {currentUser.Identities[0].Wallet.EthAddress});

        GraphqlResponse<EnjinTransaction> result = client.MintAsset(new_requst).Result;
    }

    public static List<EnjinToken> findAssetByName()
    {
        FindAssetToken findAssets = new FindAssetToken()
            //.Name(NetworkMain.Username);
            .Name("Tenoshi");

        GraphqlResponse<List<EnjinToken>> newerRes = client.GetAssets(findAssets).Result;

        return newerRes.Result;
    }
    public static List<EnjinBalance> getUserAssets()
    {
        Debug.Log("Getting all assets");
        GetBalances getBalance = new GetBalances()
            .EthAddress(currentUser.Identities[0].Wallet.EthAddress)
            .AppIds(new List<int>() { 5800 });


        GraphqlResponse<List<EnjinBalance>> newerRes = client.GetBalances(getBalance).Result;

        if (newerRes.Errors != null)
        {
//            Debug.Log(getBalance.)
        }

        return newerRes.Result;
    }

    IEnumerator DownloadImage(string MediaUrl, Action<Texture> callback)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
        {
            callback(((DownloadHandlerTexture)request.downloadHandler).texture);

        }
    }
}
