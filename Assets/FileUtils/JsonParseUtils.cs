using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

using System.IO;
using System.Text;

// https://qiita.com/akira-sasaki/items/71c13374698b821c4d73
public class JsonParseUtils {

	[System.Serializable]
	public class Wrapper<T>
	{
		public T[] items;
	}

	// json でいきなり配列で定義されている場合　items を加えオブジェクト化した string にする。
	public static string ArrayJsonWrapper(string original_json)
	{
		string pattern = @"\s+"; // remove white

		string result = Regex.Replace(original_json, pattern, "");
		//Debug.LogWarning(result);

		//Debug.LogWarning( result.LastIndexOf ("},]") + " / "+ result.Length);
		result = Regex.Replace(result, "},]", "}]");
		//Debug.LogWarning(result);

		string newJson = "{ \"items\": " + result + "}";

		return newJson;
	}

	//
	public static T[] LoadAndFromJson<T>(string path, Encoding encoder= null){
		
		using (FileStream stream = File.Open (path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
            if (encoder == null)
            {
                encoder = Encoding.GetEncoding("Shift_JIS");
            }
			StreamReader sReader = new StreamReader (stream, encoder);
			string loadStr = sReader.ReadToEnd ();

			string newJson = JsonParseUtils.ArrayJsonWrapper (loadStr);
			JsonParseUtils.Wrapper<T> list = JsonUtility.FromJson<JsonParseUtils.Wrapper<T>> (newJson);

			sReader.Close ();
			stream.Close ();

			return list.items;
		}
	}

    public static T[] FromJson<T>(string jsonStr)
    {
        string newJson = JsonParseUtils.ArrayJsonWrapper(jsonStr);
        JsonParseUtils.Wrapper<T> list = JsonUtility.FromJson<JsonParseUtils.Wrapper<T>>(newJson);

        return list.items;
    }
}
