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

        List<GameObject> RL = new List<GameObject>(); //List of all Room objects
        DirectoryInfo root; //reference to the persistent data path for the application
        List<DirectoryInfo> Dir = new List<DirectoryInfo>(); //list of directories in the persistent data path
        List<string> DirNames = new List<string>(); //list of string names of the directories in the persistent data path
        private string RoomFolder = ""; //text entry box
        int selected = 0; //Room drop down value
        Stack<fileToLoad> FilesToLoad = new Stack<fileToLoad>(); //stack to keep track of room loading

        private struct fileToLoad //struct to keep track of file's path and name for loading
        {
            public string filePath;
            public string name;
        }

        /// <summary>
        /// Toolbar Declaration and window creation
        /// </summary>
        [MenuItem("Room Manager/Room Manager Window")]
        private static void showEditor()
        {
            GetWindow<RoomSave>(false, "Room Manager");
        }

        /// <summary>
        /// Creates the UI for the Room Manager Window
        /// </summary>
        void OnGUI()
        {
            //clear all lists
            RL.Clear();
            Dir.Clear();
            DirNames.Clear();

            //establish root directory
            root = new DirectoryInfo(Application.persistentDataPath);

            //get all directories in the root directory
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                if (d.Name.ToString() != "Unity")
                {
                    Dir.Add(d);
                    DirNames.Add(d.Name);
                }
            }

            //create a label for save section
            GUI.Label(new Rect(10, 5, position.width - 20, 20), "Directory to Save Rooms to: ");
            //create a text entry for Directory creation/saving
            RoomFolder = EditorGUI.TextField(new Rect(10, 25, position.width - 20, 20),
                    "Room Name: ",
                    RoomFolder);
            
            //Get all game objects with the "Room" tag
            foreach (GameObject g in GameObject.FindObjectsOfType<GameObject>())
            {
                if (g.tag == "Room")
                {
                    RL.Add(g);
                }
            }

            //Create a save Rooms button and link it to the function
            if (GUI.Button(new Rect(10, 50, position.width - 20, 20), "Save Rooms"))
                SaveRooms(RL, Dir);

            //If there are directories within the root directory
            if (Dir.Count > 0)
            {
                //Create the load lable
                GUI.Label(new Rect(10, 85, position.width - 20, 20), "Directory to Load Rooms From: ");

                //get the currently selected item in the drop down
                selected = EditorGUI.Popup(new Rect(10, 110, position.width - 20, 20), selected, DirNames.ToArray());

                //create load button
                if (GUI.Button(new Rect(10, 135, position.width - 20, 20), "Load Selected Room"))
                    LoadRoom(Dir[selected]);

                //create unload button
                if (GUI.Button(new Rect(10, 160, position.width - 20, 20), "Unload Selected Room"))
                    UnloadRoom(Dir[selected]);
            }

            //create a delete all button
            if (GUI.Button(new Rect(10, 225, position.width - 20, 20), "Delete All Rooms"))
                deleteAllRooms();

            Repaint();

        }

        /// <summary>
        /// Goes through the list of directories and determines if the save directory already exists
        /// If the directory does not exist, it creates it
        /// Goes through each room game object and writes it to the directory
        /// </summary>
        /// <param name="RL"></param>
        /// <param name="Dir"></param>
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

        /// <summary>
        /// Gets each file within a directory and checks to see if the room gameobject already exists.
        /// if it does not exist it pushes the room to a stack to be read
        /// </summary>
        /// <param name="Dir"></param>
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

        /// <summary>
        /// Goes through each file in a directory and room gameobject and deletes the appropriate rooms
        /// </summary>
        /// <param name="Dir"></param>
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

        /// <summary>
        /// Reads the room game object from the file path and sends it to the TangoDatabase with it's name
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="name"></param>
        private void ReadRoom(string filePath, string name)
        {
            byte[] b = File.ReadAllBytes(filePath);
            TangoDatabase.UpdateMesh(b, name);
        }

        /// <summary>
        /// Deletes all Room gameobjects
        /// </summary>
        private void deleteAllRooms()
        {
            foreach(GameObject g in RL)
            {
                Destroy(g);
            }
        }

        /// <summary>
        /// Goes through each of the files to be loaded and load one per update
        /// </summary>
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
