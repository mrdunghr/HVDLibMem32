﻿using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace HVDLibMem32
{
    public class HVDLibMem
    {
        #region imports

        [DllImport("Kernel32.dll")]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int nSize,
            IntPtr lpNumberOfBytesRead
            );


        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            int size,
            IntPtr lpNumberOfBytesWritten
            );

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile
            );

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        #endregion

        #region globals


        private Process proc;



        #endregion
        public HVDLibMem(string procName)
        {
            proc = SetProcess(procName);
        }

        public Process GetProcess()
        {
            return proc;
        }
        public Process SetProcess(string procName)
        {
            proc = Process.GetProcessesByName(procName)[0];
            if (proc == null)
                throw new InvalidOperationException("Không tìm thấy quy trình, bạn có đang sử dụng đúng phiên bản bit và không có lỗi chính tả không?");
            return proc;
        }
        public IntPtr GetModuleBase(string moduleName) // cái này không hiểu hỏi chat gpt nó cho ra kết quả :D
        {
            if (string.IsNullOrEmpty(moduleName))
            {
                throw new InvalidOperationException("moduleName là null hoặc trống.");
            }

            if (proc == null)
            {
                throw new InvalidOperationException("quá trình không hợp lệ, hãy kiểm tra init của bạn.");

            }

            try
            {
                if (moduleName.Contains(".exe"))
                {
                    if (proc.MainModule != null)
                        return proc.MainModule.BaseAddress;
                }

                foreach (ProcessModule module in proc.Modules)
                {
                    if (module.ModuleName == moduleName)
                    {
                        return module.BaseAddress;
                    }
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Không tìm thấy mô-đun được chỉ định, chuỗi tìm kiếm có thể có lỗi chính tả.");
            }

            return IntPtr.Zero;
        }


        public IntPtr ReadPointer(IntPtr addy)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(proc.Handle, addy, buffer, buffer.Length, IntPtr.Zero);
            return (IntPtr)BitConverter.ToInt32(buffer);
        }

        public IntPtr ReadPointer(IntPtr addy, int offset)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(proc.Handle, addy + offset, buffer, buffer.Length, IntPtr.Zero);
            return (IntPtr)BitConverter.ToInt32(buffer);
        }

        public IntPtr ReadPointer(IntPtr addy, int[] offsets)
        {
            byte[] buffer = new byte[4];

            foreach (var offset in offsets)
            {
                ReadProcessMemory(proc.Handle, addy + offset, buffer, buffer.Length, IntPtr.Zero);
            }

            return (IntPtr)BitConverter.ToInt32(buffer);
        }

        #region ReadPointer overloads

        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2)
        {
            return ReadPointer(addy, new int[] { offset1, offset2 });
        }

        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3)
        {
            return ReadPointer(addy, new int[] { offset1, offset2, offset3 });
        }

        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3, int offset4)
        {
            return ReadPointer(addy, new int[] { offset1, offset2, offset3, offset4 });
        }

        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3, int offset4, int offset5)
        {
            return ReadPointer(addy, new int[] { offset1, offset2, offset3, offset4, offset5 });
        }

        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3, int offset4, int offset5, int offset6)
        {
            return ReadPointer(addy, new int[] { offset1, offset2, offset3, offset4, offset5, offset6 });
        }

        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3, int offset4, int offset5, int offset6, int offset7)
        {
            return ReadPointer(addy, new int[] { offset1, offset2, offset3, offset4, offset5, offset6, offset7 });
        }

        #endregion

        #region READ
        public byte[] ReadBytes(IntPtr addy, int bytes)
        {
            byte[] buffer = new byte[bytes];
            ReadProcessMemory(proc.Handle, addy, buffer, buffer.Length, IntPtr.Zero);
            return buffer;
        }

        public byte[] ReadBytes(IntPtr addy, int offset, int bytes)
        {
            byte[] buffer = new byte[bytes];
            ReadProcessMemory(proc.Handle, addy + offset, buffer, buffer.Length, IntPtr.Zero);
            return buffer;
        }

        public int ReadInt(IntPtr address)
        {
            return BitConverter.ToInt32(ReadBytes(address, 4));
        }

        public int ReadInt(IntPtr address, int offset)
        {
            return BitConverter.ToInt32(ReadBytes(address + offset, 4));
        }

        public float ReadFloat(IntPtr address)
        {
            return BitConverter.ToSingle(ReadBytes(address, 4));
        }

        public float ReadFloat(IntPtr address, int offset)
        {
            return BitConverter.ToSingle(ReadBytes(address + offset, 4));
        }

        public Vector3 ReadVec(IntPtr address)
        {
            var bytes = ReadBytes(address, 12);
            return new Vector3
            {
                X = BitConverter.ToSingle(bytes, 0),
                Y = BitConverter.ToSingle(bytes, 4),
                Z = BitConverter.ToSingle(bytes, 8)
            };
        }

        public Vector3 ReadVec(IntPtr address, int offset)
        {
            var bytes = ReadBytes(address + offset, 12);
            return new Vector3
            {
                X = BitConverter.ToSingle(bytes, 0),
                Y = BitConverter.ToSingle(bytes, 4),
                Z = BitConverter.ToSingle(bytes, 8)
            };
        }

        public short ReadShort(IntPtr address)
        {
            return BitConverter.ToInt16(ReadBytes(address, 2));
        }

        public short ReadShort(IntPtr address, int offset)
        {
            return BitConverter.ToInt16(ReadBytes(address + offset, 2));
        }

        public ushort ReadUShort(IntPtr address)
        {
            return BitConverter.ToUInt16(ReadBytes(address, 2));
        }

        public ushort ReadUShort(IntPtr address, int offset)
        {
            return BitConverter.ToUInt16(ReadBytes(address + offset, 2));
        }

        public uint ReadUInt(IntPtr address)
        {
            return BitConverter.ToUInt32(ReadBytes(address, 4));
        }

        public uint ReadUInt(IntPtr address, int offset)
        {
            return BitConverter.ToUInt32(ReadBytes(address + offset, 4));
        }

        public float[] ReadMatrix(IntPtr address)
        {
            var bytes = ReadBytes(address, 4 * 16);
            var matrix = new float[bytes.Length];

            matrix[0] = BitConverter.ToSingle(bytes, 0 * 4);
            matrix[1] = BitConverter.ToSingle(bytes, 1 * 4);
            matrix[2] = BitConverter.ToSingle(bytes, 2 * 4);
            matrix[3] = BitConverter.ToSingle(bytes, 3 * 4);

            matrix[4] = BitConverter.ToSingle(bytes, 4 * 4);
            matrix[5] = BitConverter.ToSingle(bytes, 5 * 4);
            matrix[6] = BitConverter.ToSingle(bytes, 6 * 4);
            matrix[7] = BitConverter.ToSingle(bytes, 7 * 4);

            matrix[8] = BitConverter.ToSingle(bytes, 8 * 4);
            matrix[9] = BitConverter.ToSingle(bytes, 9 * 4);
            matrix[10] = BitConverter.ToSingle(bytes, 10 * 4);
            matrix[11] = BitConverter.ToSingle(bytes, 11 * 4);

            matrix[12] = BitConverter.ToSingle(bytes, 12 * 4);
            matrix[13] = BitConverter.ToSingle(bytes, 13 * 4);
            matrix[14] = BitConverter.ToSingle(bytes, 14 * 4);
            matrix[15] = BitConverter.ToSingle(bytes, 15 * 4);

            return matrix;

        }

        #endregion

        #region WRITE


        public bool WriteBytes(IntPtr address, byte[] newbytes)
        {
            return WriteProcessMemory(proc.Handle, address, newbytes, newbytes.Length, IntPtr.Zero);
        }

        public bool WriteBytes(IntPtr address, int offset, byte[] newbytes)
        {
            return WriteProcessMemory(proc.Handle, address + offset, newbytes, newbytes.Length, IntPtr.Zero);
        }

        public bool WriteInt(IntPtr address, int value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteInt(IntPtr address, int offset, int value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteShort(IntPtr address, short value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteShort(IntPtr address, int offset, short value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteUShort(IntPtr address, ushort value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteUShort(IntPtr address, int offset, ushort value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteUInt(IntPtr address, uint value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value));
        }

        public bool WriteUInt(IntPtr address, int offset, uint value)
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        public bool WriteVec(IntPtr address, Vector3 value)
        {
            byte[] bytes = new byte[12];
            byte[] xBytes = BitConverter.GetBytes(value.X);
            byte[] yBytes = BitConverter.GetBytes(value.Y);
            byte[] zBytes = BitConverter.GetBytes(value.Z);
            xBytes.CopyTo(bytes, 0);
            yBytes.CopyTo(bytes, 4);
            zBytes.CopyTo(bytes, 8);
            return WriteBytes(address, bytes);
        }

        public bool WriteVec(IntPtr address, int offset, Vector3 value)
        {
            byte[] bytes = new byte[12];
            byte[] xBytes = BitConverter.GetBytes(value.X);
            byte[] yBytes = BitConverter.GetBytes(value.Y);
            byte[] zBytes = BitConverter.GetBytes(value.Z);
            xBytes.CopyTo(bytes, 0);
            yBytes.CopyTo(bytes, 4);
            zBytes.CopyTo(bytes, 8);
            return WriteBytes(address + offset, bytes);
        }

        public bool Nop(IntPtr address, int length)
        {
            byte[] nopArray = new byte[length];
            for (int i = 0; i < length; i++)
            {
                nopArray[i] = 0x90;
            }
            return WriteBytes(address, nopArray);
        }

        #endregion

        #region SIG SCANNING

        public IntPtr ScanForBytes32(string moduleName, string needle)
        {

            ProcessModule module = proc.Modules.OfType<ProcessModule>().FirstOrDefault(x => x.ModuleName == moduleName);
            if (module == null)
                throw new InvalidOperationException("module was not found. Check your module name.");

            byte[] needleBytes = needle.Split(' ').Select(b => Convert.ToByte(b, 16)).ToArray();
            byte[] haystackBytes;

            if (module.FileName == null)
                throw new InvalidOperationException("module filename was null.");

            using (var stream = new FileStream(module.FileName, FileMode.Open, FileAccess.Read))
            {
                haystackBytes = new byte[stream.Length];
                stream.Read(haystackBytes, 0, (int)stream.Length);
            }
            return (IntPtr)ScanForBytes32(haystackBytes, needleBytes);
        }


        public int ScanForBytes32(byte[] haystack, byte[] needle)
        {
            for (int i = 0; i < haystack.Length - needle.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < needle.Length; j++)
                {
                    if (haystack[i + j] != needle[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion
    }
}