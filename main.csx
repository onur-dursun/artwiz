using System;
using System.IO;
using Microsoft.Win32;
using System.Runtime.CompilerServices;

string imageCommandsFolder = "commands\\image";
string videoCommandsFolder = "commands\\video";
string[] videoExtensions = { ".mp4", ".avi", ".mkv" };
string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
public static string GetScriptPath([CallerFilePath] string path = null) => path;
public static string GetScriptFolder([CallerFilePath] string path = null) => Path.GetDirectoryName(path);
        string menuName = "ArtWiz Master";

Main();

void Main()
{
    string action;
    if (Args.Count < 1)
    {
        Console.WriteLine("Usage: ContextMenuManager.csx load|unload");
        action = "load";
    }
    else
    {
        action = Args[0].ToLower();
    }

    try
    {
        switch (action)
        {
            case "load":
                LoadImageCommands();
                LoadVideoCommands();
                break;
            case "load-i":
                LoadImageCommands();
                break;
            case "load-v":
                LoadVideoCommands();
                break;

            case "unload":
                UnloadImageCommands();
                UnloadVideoCommands();
                break;
            case "unload-i":
                UnloadImageCommands();
                break;
            case "unload-v":
                UnloadVideoCommands();

                break;
            default:
                Console.WriteLine("Invalid action.");

                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }

    Console.WriteLine("Completed.");

}

void LoadImageCommands()
{
    LoadContextMenuItems(imageExtensions, imageCommandsFolder);
}

void LoadVideoCommands()
{
    LoadContextMenuItems(videoExtensions, videoCommandsFolder);
}

void LoadContextMenuItems(string[] extensions, string folder)
{
    Console.WriteLine("Loading: " + folder);

    foreach (string batchFilePath in Directory.GetFiles(folder, "*.bat"))
    {
        string menuName = Path.GetFileNameWithoutExtension(batchFilePath);
        string fileExtension = Path.GetExtension(batchFilePath).ToLower();

        // Check if the file extension belongs to a video or image type
        Array.ForEach(extensions, ext =>
        {
            AddContextMenu(menuName, Path.Combine(GetScriptFolder(), batchFilePath), ext);
        });
    }
}

void UnloadImageCommands()
{
    string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    UnloadContextMenuItems(imageExtensions, imageCommandsFolder);
}

void UnloadVideoCommands()
{
    string[] videoExtensions = { ".mp4", ".avi", ".mkv" };
    UnloadContextMenuItems(videoExtensions, videoCommandsFolder);
}

void UnloadContextMenuItems(string[] extensions, string folder)
{
    Console.WriteLine("Unloading: " + folder);

    foreach (string batchFilePath in Directory.GetFiles(folder, "*.bat"))
    {
        //string menuName = Path.GetFileNameWithoutExtension(batchFilePath);
        string fileExtension = Path.GetExtension(batchFilePath).ToLower();

        Array.ForEach(extensions, ext =>
        {
            RemoveContextMenu(menuName, ext);
        });
    }
}

void AddContextMenu(string menuName, string command, string fileExtension)
{
    string registryKeyPath = $"SystemFileAssociations\\{fileExtension}\\shell\\{menuName}";
    using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(registryKeyPath))
    {
        if (key != null)
        {
            key.SetValue("", menuName);
            key.CreateSubKey("command").SetValue("", "\"" + command + "\"" + "%1");
        }
    }
}

void RemoveContextMenu(string menuName, string fileExtension)
{
    string registryKeyPath = $"SystemFileAssociations\\{fileExtension}\\shell\\{menuName}";
    Registry.ClassesRoot.DeleteSubKeyTree(registryKeyPath, false);
}
