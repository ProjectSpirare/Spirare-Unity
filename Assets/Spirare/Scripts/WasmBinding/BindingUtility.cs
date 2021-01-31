using System;
using System.Collections;
using System.Collections.Generic;

namespace Spirare.WasmBinding
{
    internal static class BindingUtility
    {
        public static uint InterpretAsUint(int value)
        {
            try
            {
                var bytes = BitConverter.GetBytes(value);
                return BitConverter.ToUInt32(bytes, 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int InterpretAsInt(uint value)
        {
            try
            {
                var bytes = BitConverter.GetBytes(value);
                return BitConverter.ToInt32(bytes, 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
