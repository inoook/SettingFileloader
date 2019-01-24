using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ExcelItem
{
	public float id;
	public string label0;
	public string label1;
	public string label2;
}

public class ExcelDataSample : MonoBehaviour {

	[SerializeField] ExcelItem[] items;

	// Use this for initialization
	void Awake () {
		// read
		string path = System.IO.Path.Combine(Application.streamingAssetsPath, "excelTest.xlsx");
        items = ExcelUtility.FromExcel<ExcelItem> (path);
    }

    void SaveToAsset()
    {
        #if UNITY_EDITOR
        Debug.Log("SaveToAsset");

        ScExcelData scData = ScriptableObject.CreateInstance<ScExcelData>();
        scData.items = items;
        string scPath = "Assets/excelData.asset";
        AssetDatabase.CreateAsset(scData, scPath);
        #endif
    }

    void SaveToFile()
    {
        Debug.Log("SaveToFile");

        string pathSave = System.IO.Path.Combine(Application.streamingAssetsPath, "Save/excelTestSave");
        ExcelUtility.ToExcelAndWriteFile<ExcelItem>(pathSave, items, "testName", true);
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
            SaveToFile();
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            SaveToAsset();
        }
    }
}
