using System;
using Rise.Serialization;
namespace Rise.SerializationTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            
        }
    }

    public class AppData
    {
        public SettingsData Settings;
        public ProjectData Project;
    }

    public class SettingsData
    {
        public string[] PreviousProjects;
    }

    public class ProjectData
    {
        public string Path;
        public SceneData Scene;
    }

    public class SceneData
    {
        public string Path;
        public EntityData[] Entities;
    }

    public class EntityData
    {
        public int ID;
        public Vector2 Position;
        public Vector2 Scale;
        public float Rotation;
    }
}
