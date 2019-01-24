using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;

public class FileWriter : MonoBehaviour {

    // https://qiita.com/tetsujp84/items/37a44da7d5b9c890fc1d
	public void WriteTextStreamingAssetsPath(string file, string contents, Encoding encoding = null)
	{
        // androidでは Application.streamingAssetsPath への読み込みはエラーとなる。
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, file);
        
		WriteText (path, contents, encoding);
	}

    public void WriteTextPersistentDataPath(string file, string contents, Encoding encoding = null)
    {
        string path = Application.persistentDataPath +"/"+ file;
        Debug.Log("SavePath: "+path);

        WriteText(path, contents, encoding);
    }

    public void WriteTextToPath(string dir, string file, string contents, Encoding encoding = null)
    {
        string path = dir + "/" + file;
        Debug.Log("SavePath: " + path);

        WriteText(path, contents, encoding);
    }

	public void WriteText(string path, string contents, Encoding encoding = null)
	{
        if (encoding == null)
        {
            encoding = Encoding.GetEncoding("Shift_JIS");
        }
        File.WriteAllText (path, contents, encoding);
	}
}
