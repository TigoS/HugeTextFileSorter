namespace MultiSorterLib
{
    public sealed class SystemMemoryHelper
    {
        // P/Invoke definitions for Windows
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MEMORYSTATUSEX>();
                dwMemoryLoad = 0;
                ullTotalPhys = 0;
                ullAvailPhys = 0;
                ullTotalPageFile = 0;
                ullAvailPageFile = 0;
                ullTotalVirtual = 0;
                ullAvailVirtual = 0;
                ullAvailExtendedVirtual = 0;
            }
        }

        /// <summary>
        /// Retrieves information about the system's current memory status, including physical and virtual memory
        /// statistics.
        /// </summary>
        /// <remarks>This method is a P/Invoke wrapper for the Windows API function GlobalMemoryStatusEx.
        /// It provides detailed memory information for the system. If the method returns false, the contents of lpBuffer are undefined. Callers should check the return value before using the data in lpBuffer.</remarks>
        /// <param name="lpBuffer">A reference to a MEMORYSTATUSEX structure that receives the memory status information. The structure must be
        /// initialized with its dwLength member set to the size of MEMORYSTATUSEX before calling this method.</param>
        /// <returns>true if the function succeeds; otherwise, false.</returns>
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        /// <summary>
        /// Gets the available physical memory on the system in bytes.
        /// </summary>
        /// <remarks>
        /// On Windows, uses GlobalMemoryStatusEx via P/Invoke to retrieve available physical memory.
        /// On non-Windows platforms, falls back to GC-managed memory information which approximates available memory for allocations.
        /// </remarks>
        public static ulong GetAvailablePhysicalMemoryBytes()
        {
            if (OperatingSystem.IsWindows())
            {
                MEMORYSTATUSEX status = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(ref status))
                {
                    return status.ullAvailPhys;
                }

                return 0;
            }

            // Fallback for non-Windows: not true physical memory, but a reasonable approximation
            return (ulong)GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        }
    }
}
