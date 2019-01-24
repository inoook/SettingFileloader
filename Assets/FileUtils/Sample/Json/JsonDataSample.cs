using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
	// 読み込みJsonに合わせて設定する必要あり。
	public float limit;
	public string[] path;

	public override string ToString()
	{
		return "limit: "+limit + " / "+path.Length.ToString();
	}
}

public class SaveData
{
	public int sampleInt = 1;
	public string sampleStr = "sample";
	public int[] sampleArray = new int[]{ 1, 2, 3 };
}


public class JsonDataSample : MonoBehaviour {

	[SerializeField] FileLoader fileLoader;
	[SerializeField] FileWriter fileWriter;
	[SerializeField] public Item[] items;
	
	[SerializeField] string saveFile = "saveData.json";

	// Use this for initialization
	void Awake () {
        // Load ----------
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "menu.json");

        // direct ---
        //items = JsonParseUtils.LoadAndFromJson<Item> (path);

        // stream ---
        System.Text.Encoding encoder = System.Text.Encoding.UTF8;
        string str = fileLoader.LoadStream(path, encoder);
        items = JsonParseUtils.FromJson<Item>(str);

        // Save ----------
        SaveToFile();
    }

    void SaveToFile()
	{
        Debug.Log("SaveToFile");

        SaveData data = new SaveData ();
		string jsonStr = JsonUtility.ToJson (data);
        jsonStr = JsonFormatUtils.ToReadable(jsonStr);

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Save/"+saveFile);
		fileWriter.WriteText (path, jsonStr);
	}
}
