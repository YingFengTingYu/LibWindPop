using LibWindPop.Utils;
using System;

namespace LibWindPop.PopCap.Packs.Rsb.Map
{
    internal static unsafe class CompiledMapEncoder
    {
        public static void Sort(CompiledMapEncodePair* pair_pointer, uint pair_count, nuint pool_pointer_number)
        {
            new Span<CompiledMapEncodePair>(pair_pointer, (int)pair_count).Sort((CompiledMapEncodePair pair1, CompiledMapEncodePair pair2) =>
            {
                int result = String_H.StrCmp((byte*)(pool_pointer_number + pair1.KeyOffset), (byte*)(pool_pointer_number + pair2.KeyOffset));
                if (result == 0)
                {
                    return (*(uint*)(pool_pointer_number + pair1.ValueOffset) < *(uint*)(pool_pointer_number + pair2.ValueOffset)) ? -1 : 1;
                }
                return result;
            });
        }

        public static void ComputeRepeatLength(CompiledMapEncodePair* pair_pointer, uint pair_count, nuint pool_pointer_number)
        {
            if (pair_count > 1u)
            {
                pair_pointer[0].repeat_len = 0;
                for (int i = 1; i < pair_count; i++)
                {
                    byte* last_str = (byte*)(pool_pointer_number + pair_pointer[i - 1].KeyOffset);
                    byte* now_str = (byte*)(pool_pointer_number + pair_pointer[i].KeyOffset);
                    uint repeat_len = 0;
                    while (*last_str == *now_str)
                    {
                        if (*last_str == 0)
                        {
                            repeat_len = uint.MaxValue;
                            break;
                        }
                        last_str++;
                        now_str++;
                        repeat_len++;
                    }
                    pair_pointer[i].repeat_len = repeat_len;
                }
            }
            else if (pair_count == 1u)
            {
                pair_pointer[0].repeat_len = 0;
            }
        }

        public static uint PeekSize(CompiledMapEncodePair* pair_pointer, uint pair_count)
        {
            uint size = 0u;
            for (int i = 0; i < pair_count; i++)
            {
                // Do not need to encode repeat string
                if (pair_pointer[i].repeat_len != uint.MaxValue)
                {
                    size += 4 * (pair_pointer[i].KeySize - pair_pointer[i].repeat_len) + pair_pointer[i].ValueSize;
                }
            }
            return size;
        }

        public static void Encode(CompiledMapEncodePair* pair_pointer, uint pair_count, nuint pool_pointer_number, nuint out_pointer_number)
        {
            if (pair_count == 0u)
            {
                return;
            }
            uint str_max_len = 0u;
            for (int i = 0; i < pair_count; i++)
            {
                str_max_len = Math.Max(str_max_len, pair_pointer[i].KeySize);
            }
            uint* offset_array = stackalloc uint[(int)str_max_len];
            uint current_offset = 0u;
            for (uint i = 0u; i < pair_count; i++)
            {
                if (pair_pointer[i].repeat_len == uint.MaxValue)
                {
                    continue;
                }
                // Write offset
                if (i != 0u)
                {
                    *(uint*)(out_pointer_number + offset_array[pair_pointer[i].repeat_len]) |= (current_offset >> 2) << 8;
                }
                // Write repeat part
                byte* key_pointer = (byte*)(pool_pointer_number + pair_pointer[i].KeyOffset + pair_pointer[i].repeat_len);
                for (uint j = pair_pointer[i].repeat_len; j < pair_pointer[i].KeySize; j++)
                {
                    offset_array[j] = current_offset;
                    *(uint*)(out_pointer_number + current_offset) = *key_pointer++;
                    current_offset += 4;
                }
                // Write extra data
                byte* value_pointer = (byte*)(pool_pointer_number + pair_pointer[i].ValueOffset);
                for (uint j = 0u; j < pair_pointer[i].ValueSize; j++)
                {
                    *(byte*)(out_pointer_number + current_offset) = *value_pointer++;
                    current_offset++;
                }
            }
        }
    }
}
