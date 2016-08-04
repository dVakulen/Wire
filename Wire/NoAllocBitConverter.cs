﻿using System.Runtime.CompilerServices;
using System.Text;

namespace Wire
{
    /// <summary>
    /// Provides methods not allocating the byte buffer but using <see cref="SerializerSession.GetBuffer"/> to lease a buffer.
    /// </summary>
    public static class NoAllocBitConverter
    {
        public static byte[] GetBytes(char value, SerializerSession session)
        {
            return GetBytes((short) value, session);
        }

        public static unsafe byte[] GetBytes(short value, SerializerSession session)
        {
            const int length = 2;

            var bytes = session.GetBuffer(length);
            fixed (byte* b = bytes)
                *((short*) b) = value;
            return bytes;
        }

        public static unsafe byte[] GetBytes(int value, SerializerSession session)
        {
            const int length = 4;
            var bytes = session.GetBuffer(length);
            fixed (byte* b = bytes)
                *((int*) b) = value;
            return bytes;
        }

        public static unsafe byte[] GetBytes(long value, SerializerSession session)
        {
            const int length = 8;
            var bytes = session.GetBuffer(length);
            fixed (byte* b = bytes)
                *((long*) b) = value;
            return bytes;
        }

        public static byte[] GetBytes(ushort value, SerializerSession session)
        {
            return GetBytes((short) value, session);
        }

        public static byte[] GetBytes(uint value, SerializerSession session)
        {
            return GetBytes((int) value, session);
        }

        public static byte[] GetBytes(ulong value, SerializerSession session)
        {
            return GetBytes((long) value, session);
        }

        public static unsafe byte[] GetBytes(float value, SerializerSession session)
        {
            return GetBytes(*(int*) &value, session);
        }

        public static unsafe byte[] GetBytes(double value, SerializerSession session)
        {
            return GetBytes(*(long*) &value, session);
        }

        internal static readonly UTF8Encoding Utf8 = (UTF8Encoding) Encoding.UTF8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe byte[] GetBytes(this string str,SerializerSession session,out int byteCount)
        {
            byteCount = Utf8.GetByteCount(str);
            if (byteCount < 254) //short string
            {
                byte[] bytes = session.GetBuffer(byteCount + 1);
                Utf8.GetBytes(str, 0, byteCount, bytes, 1);
                bytes[0] = (byte) (byteCount+1);
                byteCount += 1;
                return bytes;
            }
            else //long string
            {
                byte[] bytes = session.GetBuffer(byteCount + 1 + 4);
                Utf8.GetBytes(str, 0, byteCount, bytes, 1+4);
                bytes[0] = 255;
    
                fixed (byte* b = bytes)
                    *((int*)b+1) = byteCount;

                byteCount += 1+4;

                return bytes;
            }
        }
    }
}