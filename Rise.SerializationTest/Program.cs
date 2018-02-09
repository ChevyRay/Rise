using System;
using Rise.Serialization;
namespace Rise.SerializationTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            var tree = new DataTree(typeof(AppData));
            var app = (AppData)tree.Root;

            app.Project.Path.Value = "First!";
            app.Project.Path.Record();
            app.Project.Path.Value = "Second!";
            app.Project.Path.Record();
            app.Project.Path.Value = "Third!";

            Console.WriteLine(app.Project.Path.Value);
            tree.Undo();
            Console.WriteLine(app.Project.Path.Value);
            tree.Undo();
            Console.WriteLine(app.Project.Path.Value);
            tree.Redo();
            Console.WriteLine(app.Project.Path.Value);
            tree.Redo();
            Console.WriteLine(app.Project.Path.Value);
            tree.Undo();
            Console.WriteLine(app.Project.Path.Value);
            tree.Undo();
            Console.WriteLine(app.Project.Path.Value);

            //Prints out:
            //  Third!
            //  Second!
            //  First!
            //  Second!
            //  Third!
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
