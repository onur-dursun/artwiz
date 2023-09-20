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
string parentMenu = "ArtWiz";

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

    // Get an array of custom commands from the "commands" folder
    string[] customCommands = Directory.GetFiles(folder, "*.bat");

    for (int i = 0; i < customCommands.Length; i++)
    {
        customCommands[i] = Path.GetFileNameWithoutExtension(customCommands[i]);
    }

    // Concatenate the custom commands separated by semicolons
    string subcommandsValue = string.Join(";", customCommands);

    // Add the subcommands under the parent menu
    foreach (string batchFilePath in Directory.GetFiles(folder, "*.bat"))
    {
        string commandName = Path.GetFileNameWithoutExtension(batchFilePath);

        // Check if the file extension belongs to a video or image type
        Array.ForEach(extensions, ext =>
        {
            AddContextMenuParent(parentMenu, ext);
            AddContextMenu(commandName, Path.Combine(GetScriptFolder(), batchFilePath), ext);
        });
    }
}

void UnloadImageCommands()
{
    UnloadContextMenuItems(imageExtensions, imageCommandsFolder);
}

void UnloadVideoCommands()
{
    UnloadContextMenuItems(videoExtensions, videoCommandsFolder);
}

void UnloadContextMenuItems(string[] extensions, string folder)
{
    Console.WriteLine("Unloading: " + folder);

    // Get an array of custom commands from the "commands" folder
    string[] customCommands = Directory.GetFiles(folder, "*.bat");

    for (int i = 0; i < customCommands.Length; i++)
    {
        customCommands[i] = Path.GetFileNameWithoutExtension(customCommands[i]);
    }

    // Concatenate the custom commands separated by semicolons
    string subcommandsValue = string.Join(";", customCommands);

    // Add the subcommands under the parent menu
    foreach (string batchFilePath in Directory.GetFiles(folder, "*.bat"))
    {
        string commandName = Path.GetFileNameWithoutExtension(batchFilePath);

        // Check if the file extension belongs to a video or image type
        Array.ForEach(extensions, ext =>
        {
            RemoveContextMenu(commandName, ext);
            RemoveContextMenuParent(parentMenu, ext);
        });
    }
}
void AddContextMenuParent(string parentMenu, string fileExtension)
{
    string registryKeyPath = $"SystemFileAssociations\\{fileExtension}\\shell\\{parentMenu}";
    using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(registryKeyPath))
    {
        if (key != null)
        {
            key.SetValue("", "");
            key.SetValue("SubCommands", "");
        }
    }
}

void AddContextMenu(string commandName, string command, string fileExtension)
{
    string registryKeyPath = $"SystemFileAssociations\\{fileExtension}\\shell\\{parentMenu}\\shell\\{commandName}";
    using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(registryKeyPath))
    {
        if (key != null)
        {
            key.SetValue("", commandName);
            key.CreateSubKey("command").SetValue("", command + " \"%1\"");
        }
    }
}

void RemoveContextMenuParent(string commandName, string fileExtension)
{
    string registryKeyPath = $"SystemFileAssociations\\{fileExtension}\\shell\\{parentMenu}";
    Registry.ClassesRoot.DeleteSubKeyTree(registryKeyPath, false);
}
void RemoveContextMenu(string commandName, string fileExtension)
{
    string registryKeyPath = $"SystemFileAssociations\\{fileExtension}\\shell\\{parentMenu}\\shell\\{commandName}";
    Registry.ClassesRoot.DeleteSubKeyTree(registryKeyPath, false);
}
