using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class BadTexturer : MonoBehaviour
{
    public string 
        path = "C:\\Users\\YourUser\\YourFolderLol", 
        newPath = "C:\\Users\\YourUser\\YourFolderLol";
    private List<string> files = new List<string>();
    private int frameW = 1440, frameH = 1080, width = 5, height = 2, pxW, pxH;
    int grandIndex = 0;
    public bool tick = false;

    public void OnValidate()
    {
        if (!tick)
            return;
        files = Directory.GetFiles(path).ToList();

        foreach (string file in files)
        {
            var finalName = file.Remove(0, (path + "\\").Length).Replace(" (", "").Replace(")", "");
            var num = int.Parse(finalName.Replace(".png", ""));

            if (num < 1000)
            {
                for (var i = 4 - num.ToString().Length; i > 0; i--)
                {
                    finalName = 0 + finalName;
                }
                File.Copy(file, path + "\\" + finalName, false);
                File.Delete(file);
            }
            else
            {
                File.Copy(file, path + "\\" + finalName, false);
                File.Delete(file);
            }
        }
    }

    public async void Start()
    {
        pxW = frameW * width;
        pxH = frameH * height;
        files = Directory.GetFiles(path).ToList();

        List<Texture2D> textures = new List<Texture2D>();
        foreach (string file in files)
        {
            Texture2D frame = new Texture2D(frameW, frameH);
            if (File.Exists(file))
            {
                var bytes = File.ReadAllBytes(file);
                frame.LoadImage(bytes, false);
                frame.Apply();
            }
            textures.Add(frame);

            if (textures.Count == width * height)
            {
                CreateNewImage(textures);
                Resources.UnloadUnusedAssets();
                GC.Collect();

                await Task.Delay(1);
                textures.Clear();
            }
        }

        if (textures.Count > 0)
        {
            CreateNewImage(textures);
            Resources.UnloadUnusedAssets();
            GC.Collect();

            await Task.Delay(1);
            textures.Clear();
        }
    }

    void CreateNewImage(List<Texture2D> textures)
    {
        print("now on frame " + grandIndex * (width * height));
        var badTexture = new Texture2D(pxW, pxH);
        int index = 0;
        int row = 0;
        foreach (var texture in textures)
        {
            row = index <= (width - 1) ? 1 : 0;

            for (int x = 0; x < frameW; x++)
            {
                for (int y = 0; y < frameH; y++)
                {
                    badTexture.SetPixel(x + (frameW * index), y + (frameH * row), texture.GetPixel(x, y));
                }
            }
            index++;
            badTexture.Apply();
        }

        var newBytes = badTexture.EncodeToJPG();
        var newFile = newPath + "\\" + grandIndex + "_new_tex.jpg";

        using (FileStream file = new FileStream(newFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            //StreamWriter writer = new StreamWriter(file);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(newBytes);

            //File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //File.WriteAllBytes(path, bytes);
            writer.Close();
            file.Close();
            //file.Dispose();
        }

        Resources.UnloadUnusedAssets();
        GC.Collect();
        grandIndex++;
    }
}
