using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace UWBNetworkingPackage
{
    public class RoomSave : EditorWindow
    {

        List<GameObject> RL = new List<GameObject>();
        DirectoryInfo root;
        List<DirectoryInfo> Dir = new List<DirectoryInfo>();
        List<string> DirNames = new List<string>();
        private string RoomFolder = "";
        int selected = 0;
        Stack<fileToLoad> FilesToLoad = new Stack<fileToLoad>();

        private struct fileToLoad
        {
            public string filePath;
            public string name;
        }


        [MenuItem("Room Manager/Room Manager Window")]
        private static void showEditor()
        {
            GetWindow<RoomSave>(false, "Room Manager");
        }

        void OnGUI()
        {
            RL.Clear();
            Dir.Clear();
            DirNames.Clear();
            root = new DirectoryInfo(Application.persistentDataPath);

            foreach (DirectoryInfo d in root.GetDirectories())
            {
                if (d.Name.ToString() != "Unity")
                {
                    Dir.Add(d);
                    DirNames.Add(d.Name);
                }
            }

            GUI.Label(new Rect(10, 5, position.width - 20, 20), "Directory to Save Rooms to: ");
            RoomFolder = EditorGUI.TextField(new Rect(10, 25, position.width - 20, 20),
                    "Room Name: ",
                    RoomFolder);
            

            foreach (GameObject g in GameObject.FindObjectsOfType<GameObject>())
            {
                if (g.tag == "Room")
                {
                    RL.Add(g);
                }
            }

            if (GUI.Button(new Rect(10, 50, position.width - 20, 20), "Save Rooms"))
                SaveRooms(RL, Dir);

            if (Dir.Count > 0)
            {
                GUI.Label(new Rect(10, 85, position.width - 20, 20), "Directory to Load Rooms From: ");

                selected = EditorGUI.Popup(new Rect(10, 110, position.width - 20, 20), selected, DirNames.ToArray());

                if (GUI.Button(new Rect(10, 135, position.width - 20, 20), "Load Selected Room"))
                    LoadRoom(Dir[selected]);

                if (GUI.Button(new Rect(10, 160, position.width - 20, 20), "Unload Selected Room"))
                    UnloadRoom(Dir[selected]);
            }

            if (GUI.Button(new Rect(10, 225, position.width - 20, 20), "Delete All Rooms"))
                deleteAllRooms();

            Repaint();

        }

        public void SaveRooms(List<GameObject> RL, List<DirectoryInfo> Dir)
        {
            bool checkDirectoryExists = false;

            if(RoomFolder == "")
            {
                RoomFolder = "Room";
            }

            foreach(DirectoryInfo d in Dir)
            {
                if (d.Name == RoomFolder)
                {
                    checkDirectoryExists = true;
                }
            }

            if(checkDirectoryExists == false)
            {
                Directory.CreateDirectory(root.FullName + '\\' + RoomFolder);
            }

            foreach (GameObject g in RL)
            {
                byte[] b = TangoDatabase.GetMeshAsBytes(TangoDatabase.GetRoomByName(g.name));

                UnityEngine.Debug.Log(root.FullName + '\\' + RoomFolder + '\\' + g.name);
                File.WriteAllBytes(root.FullName + '\\' + RoomFolder + '\\' + g.name, b);
            }
        }

        public void LoadRoom(DirectoryInfo Dir)
        {
            foreach (FileInfo f in Dir.GetFiles())
            {
                fileToLoad file = new fileToLoad();
                file.filePath = f.FullName;
                file.name = f.Name;
                bool checkIfExists = false;
                foreach(GameObject g in RL)
                {
                    if(g.name == file.name)
                    {
                        checkIfExists = true;
                    }
                }
                if (checkIfExists == false)
                {
                    FilesToLoad.Push(file);
                }
            }
        }

        public void UnloadRoom(DirectoryInfo Dir)
        {
            foreach (FileInfo f in Dir.GetFiles())
            {
                foreach (GameObject g in RL)
                {
                    if (g.name == f.Name)
                    {
                        Destroy(g);
                    }
                }
            }
        }

        private void ReadRoom(string filePath, string name)
        {
            byte[] b = File.ReadAllBytes(filePath);
            TangoDatabase.UpdateMesh(b, name);
        }

        private void deleteAllRooms()
        {
            foreach(GameObject g in RL)
            {
                Destroy(g);
            }
        }

        [ExecuteInEditMode]
        void Update()
        {
            if(FilesToLoad.Count > 0)
            {
                fileToLoad f = FilesToLoad.Pop();
                ReadRoom(f.filePath, f.name);
            }
        }
    }
}
