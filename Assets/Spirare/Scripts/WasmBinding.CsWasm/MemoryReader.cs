using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasm.Interpret;

namespace Spirare
{
    public class MemoryReader
    {
        private LinearMemory memory;

        public MemoryReader(LinearMemory memory)
        {
            this.memory = memory;
        }

        /*
        internal LinearMemory Memory
        {
            get
            {
                return moduleInstance.Memories[0];
            }
        }
        */

        public bool TryRead(uint pointer, out int value)
        {
            var memory32 = memory.Int32;
            try
            {
                value = memory32[pointer];
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                value = 0;
                return false;
            }
        }

        public bool TryWrite(uint pointer, int value)
        {
            try
            {
                var memory32 = memory.Int32;
                memory32[pointer] = value;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return false;
            }
        }

        public bool TryWrite(uint pointer, byte[] value)
        {
            var memory8 = memory.Int8;
            try
            {
                for (var i = 0; i < value.Length; i++)
                {
                    memory8[(uint)(pointer + i)] = (sbyte)value[i];
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return false;
            }
        }
    }
}
