using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using System.Reflection;

// https://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/
public class CsvUtility
{
//	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string SPLIT_RE = @"\s*,\s*(?=(?:[^""]*""[^""]*"")*[^""]*$)";// http://www.atmarkit.co.jp/ait/articles/1702/15/news024.html
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	//static char[] TRIM_CHARS = { '\"' };

	public static T[] FromCsv<T>(string csvText)
	{
		csvText = csvText.Trim(new char[] {'\r', '\n'});
		var t_lines = Regex.Split (csvText, LINE_SPLIT_RE);

		List<string> lineList = new List<string> ();
		for (int i = 0; i < t_lines.Length; i++) {
			string line = t_lines[i];
			int count = CountString (line, "\"");
			//Debug.Log (line + " / " + ((count % 2) == 1));
			if ((count % 2) == 1) {
				// " が奇数の時
				bool end = false;
				while(!end){
					i++;
					string _line = t_lines[i];
					count = CountString (_line, "\"");
					line += "\n"+_line;
					end = (count % 2) == 1;
				}
				lineList.Add (line);
			}else{
				// " が偶数の時
				lineList.Add (line);
			}
		}

		var lines = lineList.ToArray ();

		if(lines.Length <= 0) return null;
		
		List<T> list = new List<T>();

		// ヘッダの名前を元に、T のメンバーに値を入れていく。
		var header = Regex.Split(lines[0], SPLIT_RE);
		for(var i = 1; i < lines.Length; i++) {

			//Debug.LogWarning (lines[i]);
			string[] values = Regex.Split(lines[i], SPLIT_RE);
			if(values.Length == 0 ||values[0] == "") continue;

			// http://pgnote.net/?p=854
			T entry = (T)Activator.CreateInstance<T> ();
			for(var j = 0; j < header.Length && j < values.Length; j++ ) {
				string value = values[j];
				//Debug.LogWarning ("---" + value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS));
//				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

				string headerProp = header [j];
				// プロパティ情報の取得
				System.Reflection.FieldInfo field = typeof(T).GetField(headerProp, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                if (!field.FieldType.IsArray)
                {
                    if (field.FieldType == typeof(Int32))
                    {
                        int n;
                        if (int.TryParse(value, out n))
                        {
                            field.SetValue(entry, n);
                        }
                    }
                    else if (field.FieldType == typeof(Single))
                    {
                        float f;
                        if (float.TryParse(value, out f))
                        {
                            field.SetValue(entry, f);
                        }
                    }
                    else if (field.FieldType == typeof(String))
                    {
                        field.SetValue(entry, value.ToString());
                    }else{
                        Debug.LogWarning("Error not support type.");
                    }
                }
                else {
                    Debug.LogWarning("Error: not support Array type." + field.FieldType);
                }
			}
			list.Add ((T)entry);
		}
		return list.ToArray();
	}



	/// <summary>
	/// 指定された文字列内にある文字列が幾つあるか数える
	/// </summary>
	/// <param name="strInput">strFindが幾つあるか数える文字列</param>
	/// <param name="strFind">数える文字列</param>
	/// <returns>strInput内にstrFindが幾つあったか</returns>
	public static int CountString(string strInput, string strFind)
	{
		int foundCount = 0;
		int sPos = strInput.IndexOf(strFind);
		while (sPos > -1)
		{
			foundCount++;
			sPos = strInput.IndexOf(strFind, sPos + 1);
		}

		return foundCount;
	}

	// 未使用 ----

	// http://dobon.net/vb/dotnet/file/readcsvfile.html
	/// <summary>
	/// CSVをArrayListに変換
	/// </summary>
	/// <param name="csvText">CSVの内容が入ったString</param>
	/// <returns>変換結果のArrayList</returns>
	public static System.Collections.ArrayList CsvToArrayList2(string csvText)
	{
		//前後の改行を削除しておく
		csvText = csvText.Trim(new char[] {'\r', '\n'});

		System.Collections.ArrayList csvRecords =
			new System.Collections.ArrayList();
		System.Collections.ArrayList csvFields =
			new System.Collections.ArrayList();

		int csvTextLength = csvText.Length;
		int startPos = 0, endPos = 0;
		string field = "";

		while (true)
		{
			//空白を飛ばす
			while (startPos < csvTextLength &&
				(csvText[startPos] == ' ' || csvText[startPos] == '\t'))
			{
				startPos++;
			}

			//データの最後の位置を取得
			if (startPos < csvTextLength && csvText[startPos] == '"')
			{
				//"で囲まれているとき
				//最後の"を探す
				endPos = startPos;
				while (true)
				{
					endPos = csvText.IndexOf('"', endPos + 1);
					if (endPos < 0)
					{
						throw new ApplicationException("\"が不正");
					}
					//"が2つ続かない時は終了
					if (endPos + 1 == csvTextLength || csvText[endPos + 1] != '"')
					{
						break;
					}
					//"が2つ続く
					endPos++;
				}

				//一つのフィールドを取り出す
				field = csvText.Substring(startPos, endPos - startPos + 1);
				//""を"にする
				field = field.Substring(1, field.Length - 2).Replace("\"\"", "\"");

				endPos++;
				//空白を飛ばす
				while (endPos < csvTextLength &&
					csvText[endPos] != ',' && csvText[endPos] != '\r')
				{
					endPos++;
				}
			}
			else
			{
				//"で囲まれていない
				//カンマか改行の位置
				endPos = startPos;
				while (endPos < csvTextLength &&
					csvText[endPos] != ',' && csvText[endPos] != '\r')
				{
					endPos++;
				}

				//一つのフィールドを取り出す
				field = csvText.Substring(startPos, endPos - startPos);
				//後の空白を削除
				field = field.TrimEnd();
			}

			//フィールドの追加
			csvFields.Add(field);

			//行の終了か調べる
			if (endPos >= csvTextLength || csvText[endPos] == '\r')
			{
				//行の終了
				//レコードの追加
				csvFields.TrimToSize();
//				Debug.LogWarning ("csvFields: "+csvFields);
				csvRecords.Add(csvFields);
				csvFields = new System.Collections.ArrayList(
					csvFields.Count);

				if (endPos >= csvTextLength)
				{
					//終了
					break;
				}
			}

			//次のデータの開始位置
			startPos = endPos + 1;
		}

		csvRecords.TrimToSize();
		return csvRecords;
	}

	//
	public static string ToCSV<T>(T[] data)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder ();
		
		FieldInfo[] fInfos = typeof(T).GetFields ();
        // header
        for (int i = 0; i < fInfos.Length; i++) {
			var fInfo = fInfos[i];
			sb.Append (fInfo.Name);
            if(i < fInfos.Length - 1){
                sb.Append(",");
            }
        }
		sb.AppendLine ();
		//
		for (int n = 0; n < data.Length; n++) {
			var d = data [n];

			string[] strs = new string[fInfos.Length];
			for (int i = 0; i < fInfos.Length; i++) {
				var fInfo = fInfos[i];
				string cellStr = fInfo.GetValue (d).ToString ();
				strs [i] = CorrectFormat(cellStr);
			}
			string lineStr = string.Join (",", strs);
			sb.AppendLine (lineStr);
		}

		return sb.ToString ();
	}

	public static string CorrectFormat(string str)
	{
        //if (str.IndexOf(",") > -1 || str.IndexOf("\n") > -1 || str.IndexOf("\r") > -1){
        str = str.Replace("\n", "\r");

        return str;
	}
}

