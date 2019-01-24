
// https://qiita.com/sh_akira/items/9829de2d76f8f36361aa

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class JsonFormatUtils {

	//2017/04/17 StringBuilder版
	public static string ToReadable(string json)
	{
		if (string.IsNullOrEmpty(json)) return json;
		int i = 0;
		int indent = 0;
		int quoteCount = 0;
		int position = -1;
		var sb = new StringBuilder();
		int lastindex = 0;
		while (true)
		{
			if (i > 0 && json[i] == '"' && json[i - 1] != '\\') quoteCount++;

			if (quoteCount % 2 == 0) //is not value(quoted)
			{
				if (json[i] == '{' || json[i] == '[')
				{
					indent++;
					position = 1;
				}
				else if (json[i] == '}' || json[i] == ']')
				{
					indent--;
					position = 0;
				}
				else if (json.Length > i && json[i] == ',' && json[i + 1] == '"')
				{
					position = 1;
				}
				if (position >= 0)
				{
					sb.AppendLine(json.Substring(lastindex, i + position - lastindex));
					sb.Append(new string(' ', indent * 4));
					lastindex = i + position;
					position = -1;
				}
			}

			i++;
			if (json.Length <= i)
			{
				sb.Append(json.Substring(lastindex));
				break;
			}

		}
		return sb.ToString();
	}
}
