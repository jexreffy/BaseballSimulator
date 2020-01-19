using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


[Serializable]
public class SaveState {

    public List<int> highScores;
    
    private int _version;
    
    private const int    CURRENT_VERSION = 1;
    private const string SAVE_FILE_NAME  = "/save.bin";

    private SaveState() {
        highScores = new List<int>();
    }
    
    public void Save() {
        var formatter = new BinaryFormatter();
        var file      = File.Open(Application.persistentDataPath + SAVE_FILE_NAME, FileMode.OpenOrCreate);
        formatter.Serialize(file, this);
        file.Close();
    }
    
    public static SaveState Load() {
        if (DoesSaveFileExist) {
            var formatter = new BinaryFormatter();
            var file      = File.Open(Application.persistentDataPath + SAVE_FILE_NAME, FileMode.Open);
            var saveFile  = (SaveState)formatter.Deserialize(file);
            file.Close();
            saveFile.ReconcileExistingSaveFile();
            return saveFile;
        } else {
            return Create();
        }
    }
    private static SaveState Create() {
        var newSave = new SaveState();
        newSave._version = CURRENT_VERSION;
        newSave.Save();
        return newSave;
    }
    public static bool DoesSaveFileExist { get { return File.Exists(Application.persistentDataPath + SAVE_FILE_NAME);  } }
    private void ReconcileExistingSaveFile() {
        var shouldResave = false;
        if (_version < CURRENT_VERSION) {
            shouldResave = true;
            ReconcileFileVersion();
        }
        if (shouldResave) {
            _version = CURRENT_VERSION;
            Save();
        }
    }
    private void ReconcileFileVersion() { }
}
