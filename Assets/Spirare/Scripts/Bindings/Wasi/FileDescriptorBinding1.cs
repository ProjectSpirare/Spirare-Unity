using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Spirare
{
    public class FileDescriptorBinding1
    {
        private int Success
        {
            get => 0;
        }
        private int Invalid
        {
            get => 1;
        }
        private int ErrorResult
        {
            get => 1;
        }


        public int Read(ArgumentParser parser, MemoryReader memoryReader)
        {
            throw new NotImplementedException();
        }

        public int Write(ArgumentParser parser, MemoryReader memoryReader)
        {
            if (!parser.TryReadInt(out int fd))
            {
                return ErrorResult;
            }
            if (!parser.TryReadVectoredBuffer(out byte[] buffer))
            {
                return ErrorResult;
            }
            if (!parser.TryReadUInt(out uint nwritten))
            {
                return ErrorResult;
            }

            try
            {
                Debug.Log(fd);
                var text = Encoding.UTF8.GetString(buffer);

                if (fd == 1)
                {
                    Debug.Log(text);
                }
                else if (fd == 2)
                {
                    Debug.LogError(text);
                }

                if (!memoryReader.TryWrite(nwritten, buffer.Length))
                {
                    return ErrorResult;
                }
                return Success;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return ErrorResult;
            }
        }

        public int Close(ArgumentParser parser, MemoryReader memoryReader)
        {
            Debug.Log("Close");
            if (!parser.TryReadInt(out var fd))
            {
                return Invalid;
            }
            //return ReturnValue.FromObject(1);
            return Success;
        }
    }
}
