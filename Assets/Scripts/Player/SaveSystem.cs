using System;
using System.Collections.Generic;
using System.IO;
using Books;
using UnityEngine;

namespace Player
{
    public class SaveSystem : MonoBehaviour
    {
        private string _path;

        private void Awake()
        {
            _path = Application.persistentDataPath;
        }

        public void SaveBooks(List<BookAttributes> books)
        {
            string json = JsonUtility.ToJson(books);
            SaveData(json, "normalBookData.grp2", true);
        }

        public void SaveData(string dataToSave, string fileName = "gameData.grp2", bool overwrite = false)
        {
            string fullPath = Path.Combine(_path, fileName);

            try
            {
                FileMode fileMode = overwrite ? FileMode.Create : FileMode.Append;
                using FileStream stream = new FileStream(fullPath, fileMode, FileAccess.Write);
                using StreamWriter writer = new StreamWriter(stream);
                writer.Write(dataToSave);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving data: {e.Message}");
            }
        }

        public void DeleteFile(string fileName)
        {
            string fullPath = Path.Combine(_path, fileName);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
    }
}