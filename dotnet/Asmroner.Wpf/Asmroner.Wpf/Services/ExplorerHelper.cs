using System.IO;
using System.Runtime.InteropServices;

namespace Asmroner.Wpf.Services;

/// <summary>
/// 使用 Windows Shell API 打开文件夹并选中指定文件。
/// 若目标目录的资源管理器窗口已打开，则复用该窗口而非重复打开新窗口。
/// </summary>
public static class ExplorerHelper
{
    /// <summary>
    /// 在资源管理器中打开并选中指定文件。若目录窗口已打开则复用。
    /// </summary>
    public static void SelectInExplorer(string filePath)
    {
        var directoryPidl = IntPtr.Zero;
        var filePidl = IntPtr.Zero;

        try
        {
            var directory = Path.GetDirectoryName(filePath)
                ?? throw new ArgumentException("无法获取文件所在目录。", nameof(filePath));

            // 将目录路径解析为 PIDL
            var hr = SHParseDisplayName(directory, IntPtr.Zero, out directoryPidl, 0, out _);
            Marshal.ThrowExceptionForHR(hr);

            // 将文件路径解析为 PIDL
            hr = SHParseDisplayName(filePath, IntPtr.Zero, out filePidl, 0, out _);
            Marshal.ThrowExceptionForHR(hr);

            // 打开文件夹并选中目标文件（复用已打开的同目录窗口）
            hr = SHOpenFolderAndSelectItems(directoryPidl, 1, [filePidl], 0);
            Marshal.ThrowExceptionForHR(hr);
        }
        finally
        {
            if (directoryPidl != IntPtr.Zero) ILFree(directoryPidl);
            if (filePidl != IntPtr.Zero) ILFree(filePidl);
        }
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHParseDisplayName(
        string pszName,
        IntPtr pbc,
        out IntPtr ppidl,
        uint sfgaoIn,
        out uint psfgaoOut);

    [DllImport("shell32.dll")]
    private static extern int SHOpenFolderAndSelectItems(
        IntPtr pidlFolder,
        uint cidl,
        IntPtr[] apidl,
        uint dwFlags);

    [DllImport("shell32.dll")]
    private static extern void ILFree(IntPtr pidl);
}
