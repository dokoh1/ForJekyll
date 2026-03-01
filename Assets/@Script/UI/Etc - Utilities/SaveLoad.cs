using System.ComponentModel.Design;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class SaveLoad
{
    
    public static void LoadDialogue(string csv, out JArray arr)
    {
        string json;
        
        if (CsvToJson.ConvertCsv(csv, out json))
        {
            arr = JArray.Parse(json);
        }
        else
        {
            Debug.LogError("csv 파일이 존재하지 않습니다.");
            throw CheckoutException.Canceled;
        }
    }
}
