﻿using System;
using Rise.Serialization;
namespace Rise.SerializationTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            var tree = new DataTree(typeof(AppData));
            var app = (AppData)tree.Root;

            Console.WriteLine(app.Project.Scene.Entities.PathToNode);
        }
    }

    public class AppData : DataNode
    {
        public SettingsData Settings;
        public ProjectData Project;
    }

    public class SettingsData : DataNode
    {
        public DataList<Data<string>> PreviousProjects;
    }

    public class ProjectData : DataNode
    {
        public Data<string> Path;
        public SceneData Scene;
    }

    public class SceneData : DataNode
    {
        public Data<string> Path;
        public DataList<EntityData> Entities;
    }

    public class EntityData : DataNode
    {
        public Data<int> ID;
        public Data<Vector2> Position;
        public Data<Vector2> Scale;
        public Data<float> Rotation;
    }
}