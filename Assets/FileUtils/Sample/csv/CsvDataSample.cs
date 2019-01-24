using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CsvItem
{
    public float id;
    public string label0;
    public string label1;
    public string label2;
}

public class CsvDataSample : MonoBehaviour
{

    [SerializeField] FileLoader fileLoader = null;
    [SerializeField] FileWriter fileWriter = null;

    [SerializeField] CsvItem[] items = null;

    // Use this for initialization
    void Awake()
    {
        // event ---
        //fileLoader.eventWWWFileLoadComplete += (WWW www) => {
        //	string txt = www.text;

        //	Encoding encoder = Encoding.GetEncoding("Shift_JIS");
        //	string str = encoder.GetString(www.bytes);

        //	CsvItem[] list = CsvUtility.FromCsv<CsvItem>(str);
        //	Debug.Log (">>> "+list.Length);
        //	items = list;
        //};
        //fileLoader.LoadWWW (Application.streamingAssetsPath, "excelTest.csv");

        // callback ---
        fileLoader.LoadWWW(Application.streamingAssetsPath, "excelTest.csv", (WWW www) =>
        {
            // callback
            string txt = www.text;

            Encoding encoder = Encoding.GetEncoding("Shift_JIS");
            string str = encoder.GetString(www.bytes);

            CsvItem[] list = CsvUtility.FromCsv<CsvItem>(str);
            items = list;
        });

        // stream ---
        //Encoding encoder = Encoding.GetEncoding("Shift_JIS");
        //string loadStr = fileLoader.LoadStream(Application.streamingAssetsPath, "excelTest.csv", encoder);
        //CsvItem[] list = CsvUtility.FromCsv<CsvItem>(loadStr);

    }

    void SaveToAsset()
    {
        #if UNITY_EDITOR
        Debug.Log("SaveToAsset");

        ScCsvData scData = ScriptableObject.CreateInstance<ScCsvData>();
        scData.items = items;
        string scPath = "Assets/csvData.asset";
        AssetDatabase.CreateAsset(scData, scPath);
        #endif
    }

    void SaveToFile()
    {
        Debug.Log("SaveToFile");

        string csvStr = CsvUtility.ToCSV(items);
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Save/excelTestSave.csv");
        fileWriter.WriteText(path, csvStr);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SaveToFile();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveToAsset();
        }
    }
}
