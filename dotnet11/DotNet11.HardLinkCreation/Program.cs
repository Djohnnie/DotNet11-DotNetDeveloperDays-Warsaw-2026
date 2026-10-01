// .NET 11 adds File.CreateHardLink(string path, string pathToTarget), rounding out the
// filesystem-link APIs alongside the existing File.CreateSymbolicLink and File.ResolveLinkTarget.
// A hard link is a second directory entry pointing at the exact same file data (the same inode
// on Unix, the same MFT record on NTFS) - unlike a symbolic link, there's no "original" file:
// both paths are equally real, and the data is only freed once every hard link to it is deleted.

var workingDirectory = Path.Combine(Path.GetTempPath(), "DotNet11.HardLinkCreation_" + Guid.NewGuid());
Directory.CreateDirectory(workingDirectory);

try
{
    var originalPath = Path.Combine(workingDirectory, "original.txt");
    var hardLinkPath = Path.Combine(workingDirectory, "hardlink.txt");

    File.WriteAllText(originalPath, "Hello from .NET 11!");

    // path, pathToTarget: creates 'hardLinkPath' as a hard link to the existing 'originalPath'.
    File.CreateHardLink(hardLinkPath, originalPath);

    Console.WriteLine($"original.txt : {File.ReadAllText(originalPath)}");
    Console.WriteLine($"hardlink.txt : {File.ReadAllText(hardLinkPath)}");

    // Writing through either path mutates the SAME underlying data.
    File.AppendAllText(hardLinkPath, " (appended via the hard link)");
    Console.WriteLine();
    Console.WriteLine("After appending through hardlink.txt:");
    Console.WriteLine($"original.txt : {File.ReadAllText(originalPath)}");
    Console.WriteLine($"hardlink.txt : {File.ReadAllText(hardLinkPath)}");

    // Deleting one path leaves the data intact as long as another link still references it -
    // the exact opposite of a symbolic link, which would break the moment its target is deleted.
    File.Delete(originalPath);
    Console.WriteLine();
    Console.WriteLine($"original.txt deleted. hardlink.txt still readable: {File.ReadAllText(hardLinkPath)}");
}
finally
{
    Directory.Delete(workingDirectory, recursive: true);
}
