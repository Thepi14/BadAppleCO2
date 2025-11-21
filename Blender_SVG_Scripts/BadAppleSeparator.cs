using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class BadAppleSeparator : MonoBehaviour
{
    public string path = "C:\\Users\\YourUser\\YourFolderLol";
    public int div = 5;
    public List<string> files = new List<string>();

    public void Start()
    {
        files = Directory.GetFiles(path).ToList();
        int index = 1;
        foreach (string file in files)
        {
            if (index % 5 == 0)
            {
                print(index + " is a frame for the new video.");
            }
            else
            {
                File.Delete(file);
            }
            index++;
        }
    }
}
