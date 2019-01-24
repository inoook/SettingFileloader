using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class FileLoader : MonoBehaviour {

    #region WWW
    public delegate void WWWFileLoadCompleteHandler (WWW www);
    public event WWWFileLoadCompleteHandler eventWWWFileLoadComplete;

    public delegate void FileLoadCompleteHandler(string str);
    public event FileLoadCompleteHandler eventFileLoadComplete;

    public System.Action<WWW> wwwFileLoadCompleteAct;


    public void LoadWWW (string dir, string file) {
        string path = "file://" + System.IO.Path.Combine(dir, file);
        StartCoroutine(StartLoadWithPath(path));
    }

    public void LoadWWW(string dir, string file, System.Action<WWW> loadCompleteAct){
        string path = "file://" + System.IO.Path.Combine(dir, file);
        wwwFileLoadCompleteAct = loadCompleteAct;
        StartCoroutine(StartLoadWithPath(path));
    }

    public void LoadWWW (string filePath) {
        StartCoroutine(StartLoadWithPath(filePath));
    }

    IEnumerator StartLoadWithPath (string fPath) {
        string path = fPath;

        Debug.Log(path);
        WWW www = new WWW(path);

        yield return www;
        if(wwwFileLoadCompleteAct != null)
        {
            wwwFileLoadCompleteAct(www);
            wwwFileLoadCompleteAct = null;
        }

        if (!string.IsNullOrEmpty(www.error)) {
            Debug.LogError(www.error);
        } else {
            if (eventWWWFileLoadComplete != null) {
                eventWWWFileLoadComplete(www);
            }
            if(eventFileLoadComplete != null){
                eventFileLoadComplete(www.text);
            }
        }
    }
    #endregion

    #region Stream
    public string LoadStream(string dir, string file, Encoding encoder = null)
    {
        string path = System.IO.Path.Combine(dir, file);
        return LoadStream(path, encoder);
    }

    public string LoadStream(string filePath, Encoding encoder = null)
    {
        if (encoder == null)
        {
            encoder = Encoding.GetEncoding("Shift_JIS");//  Shift_JIS or utf-8
        }

        System.IO.StreamReader reader = new System.IO.StreamReader(filePath, encoder);
        string str = reader.ReadToEnd();
        reader.Close();

        return str;
    }
    #endregion
}
