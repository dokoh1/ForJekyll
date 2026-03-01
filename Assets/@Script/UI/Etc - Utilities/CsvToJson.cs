using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Newtonsoft.Json;

public class CsvToJson
{
    public static bool ConvertCsv(string csvFile, out string result)
    {
        result = null;

        if (string.IsNullOrEmpty(csvFile))
        {
            Debug.LogError("CSV 파일 없음.");
            return false;
        }

        // CSV 데이터를 줄 단위로 읽고 모든 줄바꿈 문자 제거
        string[] csvLines = csvFile.Replace("\r", "").Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // 첫 번째 줄은 헤더로 사용
        string[] headers = SplitCsvLine(csvLines[0]);

        // 모든 데이터를 저장할 리스트
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

        // 각 줄을 읽어 Dictionary로 변환
        for (int i = 1; i < csvLines.Length; i++)
        {
            string[] values = SplitCsvLine(csvLines[i]);
            Dictionary<string, object> row = new Dictionary<string, object>();

            for (int j = 0; j < headers.Length; j++)
            {
                // "Index" 열은 int로 변환
                if (headers[j].Trim() == "Index" && int.TryParse(values[j].Trim(), out int intValue))
                {
                    row[headers[j]] = intValue;
                }
                else
                {
                    row[headers[j]] = values[j].Trim();
                }
            }
            rows.Add(row);
        }

        // JSON 변환
        result = JsonConvert.SerializeObject(rows, Formatting.Indented);

        return true;
    }

    // 큰 따옴표로 감싼 쉼표를 무시하면서 CSV 줄을 분할하는 메서드
    public static string[] SplitCsvLine(string csvLine)
    {
        var matches = Regex.Matches(csvLine, @"(?<=^|,)(?:""([^""]*)""|([^,]*))");
        List<string> values = new List<string>();

        foreach (Match match in matches)
        {
            // 큰 따옴표로 감싼 경우와 아닌 경우를 나누어 추가
            values.Add(match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value);
        }

        return values.ToArray();
    }
}
