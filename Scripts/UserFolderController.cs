using Godot;

namespace PlayerSpace;

public partial class UserFolderController : Node
{
    public override void _Ready()
    {
        //CheckForUserFolder();
    }

    private void CheckForUserFolder()
    {
        DirAccess directory = DirAccess.Open("user://");
        if (directory.DirExists("user://Maps"))
        {
            GD.Print("Maps folder exists!");
        }
        else
        {
            GD.Print("Maps folder doesn't exist!  Creating Maps Folder");
            directory.MakeDir("user://Maps");

        }
        string[] files = directory.GetFiles();
        if (files.Length > 0)
        {
            foreach (string file in files)
            {
                GD.Print("Files in User: " + file);
            }
        }
        else
        {
            GD.Print("No files in User directory.");
        }
    }
}