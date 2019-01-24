using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

using System;
using System.Runtime.InteropServices;
using System.Reflection;

public class ExcelUtility
{
	#region read
	public static T[] FromExcel<T> (string path)
	{
		using (FileStream stream = File.Open (path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
			return Parse<T> (stream, path == ".xls");
		}
	}

	static T[] Parse<T> (FileStream stream, bool xls)
	{
		Debug.Log ("Act------");
		IWorkbook book = null;
		if (xls) {
			book = new HSSFWorkbook (stream);
		} else {
			book = new XSSFWorkbook (stream);
			Debug.Log ("XSS");
		}

		Debug.Log ("NumberOfSheets: " + book.NumberOfSheets);
		//
		// 複数シート
		//for (int i = 0; i < book.NumberOfSheets; ++i) {
		//	ISheet s = book.GetSheetAt (i);
		//}

		// headerInfo
		ISheet sheet = book.GetSheetAt (0);
		IRow titleRow = sheet.GetRow (0);

		System.Reflection.FieldInfo[] headerFields = new FieldInfo[titleRow.Cells.Count];
		for (int i = 0; i < headerFields.Length; i++) {
			string headerProp = titleRow.GetCell (i).StringCellValue;
			headerFields[i] = typeof(T).GetField(headerProp, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
		}

		Debug.Log ("titleRow.LastCellNum: " + titleRow.LastCellNum + " / " + sheet.FirstRowNum +" / " + sheet.LastRowNum);

		List<T> list = new List<T>();
		for (int n = sheet.FirstRowNum+1; n <= sheet.LastRowNum; n++) {
			T entry = (T)Activator.CreateInstance<T> ();

			IRow dataRow = sheet.GetRow (n);

			for (int i = 0; i < dataRow.LastCellNum; i++) {
				ICell cell = dataRow.GetCell (i);
				SetParam (entry, headerFields[i], cell);
			}
			list.Add ((T)entry);
		}
		return list.ToArray();
	}

	static void SetParam<T>(T entry, System.Reflection.FieldInfo fieldInfo, ICell cell)
	{
		string str = cell.ToString ();

		// T のfieldの型に当てはめて値を入れる。
		if (fieldInfo.FieldType == typeof(Int32)) {
			int n;
			if (int.TryParse (str, out n)) {
				fieldInfo.SetValue (entry, n);
			}
		} else if (fieldInfo.FieldType == typeof(Single)) {
			float f;
			if (float.TryParse (str, out f)) {
				fieldInfo.SetValue (entry, f);
			}
		}else{
			fieldInfo.SetValue (entry, str.ToString());
		}
	}
    #endregion

    #region read Array
    public static List<ArrayList> FromExcel(string path)
    {
        using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            return ParseToArray(stream, path == ".xls");
        }
    }

    static List<ArrayList> ParseToArray(FileStream stream, bool xls)
    {
        IWorkbook book = null;
        if (xls)
        {
            book = new HSSFWorkbook(stream);
        }
        else
        {
            book = new XSSFWorkbook(stream);
            Debug.Log("XSS");
        }

        Debug.Log("NumberOfSheets: " + book.NumberOfSheets);
        //
        // 複数シート
        //for (int i = 0; i < book.NumberOfSheets; ++i){
        //    ISheet s = book.GetSheetAt(i);
        //}
        
        ISheet sheet = book.GetSheetAt(0);

        List<ArrayList> list = new List<ArrayList>();
        for (int n = sheet.FirstRowNum; n <= sheet.LastRowNum; n++)
        {
            IRow dataRow = sheet.GetRow(n);
            ArrayList row = new ArrayList();

            for (int i = 0; i < dataRow.LastCellNum; i++)
            {
                ICell cell = dataRow.GetCell(i);
                row.Add(cell);
            }
            list.Add(row);
        }
        return list;
    }
    #endregion

    #region write
    public static IWorkbook ToExcel<T>(T[] data, string sheetName, bool xls = true)
	{
		IWorkbook book = null;
		if (xls) {
			book = new HSSFWorkbook ();
		} else {
			book = new XSSFWorkbook ();
			Debug.Log ("XSS");
		}

		ISheet sheet = book.CreateSheet (sheetName);
		IRow titleRow = sheet.CreateRow (0);

		FieldInfo[] fInfos = typeof(T).GetFields ();
		// header
		for (int i = 0; i < fInfos.Length; i++) {
			var fInfo = fInfos[i];
			titleRow.CreateCell (i).SetCellValue (fInfo.Name);
		}

		for (int n = 0; n < data.Length; n++) {
			T d = data[n];
			IRow dataRaw = sheet.CreateRow (n+1);

			for (int i = 0; i < fInfos.Length; i++) {
				var fInfo = fInfos[i];
				object o = fInfo.GetValue (d);
				if (fInfo.FieldType == typeof(Int32)) {
					dataRaw.CreateCell (i).SetCellValue ((int)o);
				} else if (fInfo.FieldType == typeof(float)) {
					dataRaw.CreateCell (i).SetCellValue ((float)o);
				}else{
					dataRaw.CreateCell (i).SetCellValue ((string)o);
				}
			} 
		}
		return book;
	}

	public static bool ToExcelAndWriteFile<T>(string path, T[] data, string sheetName, bool xls = true)
	{
		IWorkbook book = ToExcel<T> (data, sheetName, xls);
		path += xls ? ".xls" : ".xlsx";
		using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)){
			book.Write(fs);

			return true;
		}
	}
	#endregion
}
