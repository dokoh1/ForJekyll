using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu]
public class GetGoogleSheet : ScriptableObject
{
    private TimeLiner timeLiner => TimeLiner.Instance;
    
    private readonly string address =
        "https://docs.google.com/spreadsheets/d/1UT7fbEvVaJS-rFCN8MN6QG1DlMxwCFBARwvleH2mbXE/export?format=csv&gid=";

    private readonly string mainCsv = "1030325950";
    private readonly string interactionCsv = "64221609";
    private readonly string favorCsv = "1138938785";

    private readonly string VoiceCsv = "579036207";

    public string CsvString => timelines[mainCsv];
    public string InteractionString => timelines[interactionCsv];
    public string FavorString => timelines[favorCsv];
    public string VoiceString => timelines[VoiceCsv];

    private Dictionary<string, string> timelines;

    private bool init => timelines != null;

    public async void Init()
    {
        if (!init)
        {
            timelines = new Dictionary<string, string> { { mainCsv, null }, {interactionCsv, null}, {favorCsv, null}, {VoiceCsv, null} };

            await LoadSheet();
            
            if (!String.IsNullOrEmpty(VoiceString)) { }
                //DataManager.Instance.LoadVoiceData();
        }
    }

    private async Awaitable LoadSheet()
    {
        List<Task> downloadTask = new List<Task>();
        
        downloadTask.Add(DownloadSheet(mainCsv));
        downloadTask.Add(DownloadSheet(interactionCsv));
        downloadTask.Add(DownloadSheet(favorCsv));
        downloadTask.Add(DownloadSheet(VoiceCsv));

        await Task.WhenAll(downloadTask);
    }
    
    private async Task DownloadSheet(string gid)
    {
        UnityWebRequest www = UnityWebRequest.Get(address + gid);

        var request = www.SendWebRequest();
        await request;

        if (www.responseCode != 200)
        {
            Debug.LogError($"Web 통신 실패. 에러코드 : {www.responseCode}");
            return;
        }
        
        while (!request.isDone)
        {
            Debug.Log(request.progress);
            await Awaitable.NextFrameAsync();
        }
        
        Debug.Log($"Web 통신 성공. Csv 다운로드 완료");
        
        if (timelines.ContainsKey(gid))
            timelines[gid] = www.downloadHandler.text;
    }

    public async Awaitable<AudioClip> LoadVoice(string gid)
    {
        UnityWebRequest www =
            UnityWebRequestMultimedia.GetAudioClip($"https://drive.google.com/uc?export=download&id={gid}",

                UnityEngine.AudioType.WAV);
        
        var request = www.SendWebRequest();
        await request;

        if (www.responseCode != 200)
        {
            Debug.LogError($"Web 통신 실패. 에러코드 : {www.responseCode}");
            return null;
        }
        
        while (!request.isDone)
        {
            Debug.Log(request.progress);
            await Awaitable.NextFrameAsync();
        }

        return DownloadHandlerAudioClip.GetContent(www);
    }
}
