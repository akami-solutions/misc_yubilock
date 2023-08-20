using System;
using System.Management;
using System.Runtime.InteropServices;

namespace YubiLock_Service;

public class yubiLockCode
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool LockWorkStation();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    static bool LockWorkStationWrapper()
    {
        return LockWorkStation();
    }

    public static void yubiLock()
    {
        bool state = false;
        bool alreadyFinished = false;
        while (true)
        {
            ManagementObjectSearcher searcherCap =
            new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%YubiKey%'");
            ManagementObjectCollection collectionCap = searcherCap.Get();
            if (collectionCap.Count > 0)
            {
                Console.WriteLine("YubiKey Detected!");
                if (state && !alreadyFinished)
                {
                    alreadyFinished = true;
                    MessageBox(IntPtr.Zero, "YubiKey Detected security passed!", "Satowa Network Security Systems", 0);
                }
            }
            else
            {
                bool result = LockWorkStationWrapper();
                if (result)
                {
                    Console.WriteLine("System has been locked. Please insert YubiKey to unlock.");
                    alreadyFinished = false;
                    state = false;
                }
                else
                {
                    Console.WriteLine("There was an issue locking this PC");
                }
            }

            string query = "SELECT * FROM Win32_USBControllerDevice";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection collection = searcher.Get();
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            WqlEventQuery eventQuery =
                new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBControllerDevice'");
            watcher.EventArrived += (sender, e) =>
            {
                state = true;
                Console.WriteLine("YubiKey Connection Detected!");
                if (state && !alreadyFinished)
                {
                    alreadyFinished = true;
                    MessageBox(IntPtr.Zero, "YubiKey Detected security passed!", "Satowa Network Security Systems", 0);

                }
            };
            watcher.Query = eventQuery;
            watcher.Start();

            watcher = new ManagementEventWatcher();
            eventQuery =
                new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBControllerDevice'");
            watcher.EventArrived += (sender, e) =>
            {
                state = false;
                alreadyFinished = false;
                Console.WriteLine("YubiKey Disconnection Detected!");

                bool resultEnd = LockWorkStationWrapper();
                if (resultEnd)
                {
                    state = false;
                    alreadyFinished = false;
                    Console.WriteLine("System has been locked. Please insert YubiKey to unlock.");
                }
                else
                {
                    Console.WriteLine("There was an issue locking this PC");
                }
            };

            watcher.Query = eventQuery;
            watcher.Start();

            Console.WriteLine("Waiting for YubiKey Connection...");
            Console.ReadLine();
            System.Threading.Thread.Sleep(1000);
        }
    }
}