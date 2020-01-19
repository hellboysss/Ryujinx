﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ARMeilleure.Decoders
{
    abstract class BaseOpCode32Simd : OpCode32, IOpCode32Simd
    {
        public int Vd { get; protected set; }
        public int Vm { get; protected set; }
        public int Size { get; protected set; }

        // Helpers to index doublewords within quad words. Essentially, looping over the vector starts at quadword Q and index Fx or Ix within it,
        // depending on instruction type.
        // 
        // Qx: The quadword register that the target vector is contained in.
        // Ix: The starting index of the target vector within the quadword, with size treated as integer.
        // Fx: The starting index of the target vector within the quadword, with size treated as floating point. (16 or 32)
        public int Qd => GetQuadwordIndex(Vd);
        public int Id => GetQuadwordSubindex(Vd) << (3 - Size);
        public int Fd => GetQuadwordSubindex(Vd) << (1 - (Size & 1)); // When the top bit is truncated, 1 SHOULD be fp16, but switch does not support it so we always assume 64.

        public int Qm => GetQuadwordIndex(Vm);
        public int Im => GetQuadwordSubindex(Vm) << (3 - Size);
        public int Fm => GetQuadwordSubindex(Vm) << (1 - (Size & 1));

        protected int GetQuadwordIndex(int index)
        {
            switch (RegisterSize)
            {
                case RegisterSize.Simd128:
                    return index >> 1;
                case RegisterSize.Simd64:
                    return index >> 1;
            }

            throw new InvalidOperationException();
        }

        protected int GetQuadwordSubindex(int index)
        {
            switch (RegisterSize)
            {
                case RegisterSize.Simd128:
                    return 0;
                case RegisterSize.Simd64:
                    return index & 1;
            }

            throw new InvalidOperationException();
        }

        public BaseOpCode32Simd(InstDescriptor inst, ulong address, int opCode) : base(inst, address, opCode)
        {
        }
    }
}
