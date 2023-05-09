using LibWindPop.Utils;
using LibWindPop.Utils.Extension;
using System;
using System.Text;

namespace LibWindPop.PopCap.Packs.Rsb.Map
{
    /// <summary>
    /// rsb中用于快速查找字符串对应信息的哈希表，利用字符串排序过的特性，可以实现高效率的查找
    /// </summary>
    public unsafe struct CompiledMap
    {
        /// <summary>
        /// 哈希表指针
        /// </summary>
        private void* pointer;
        /// <summary>
        /// 哈希表中32位数据的个数
        /// </summary>
        private uint count;

        private const int string_buffer_len = 0x800;

        private static readonly Encoding encoding = EncodingType.iso_8859_1.GetEncoding();

        /// <summary>
        /// 初始化哈希表
        /// </summary>
        /// <param name="compiled_map_pointer">哈希表指针</param>
        /// <param name="compiled_map_length">哈希表长度</param>
        public void Init(nuint compiled_map_pointer, uint compiled_map_length)
        {
            pointer = (void*)compiled_map_pointer; // 哈希表指针
            count = compiled_map_length >> 2; // 哈希表长度
        }

        /// <summary>
        /// 检测哈希表是否已经初始化
        /// </summary>
        /// <returns>是否已经初始化</returns>
        public bool Initialized()
        {
            return pointer != null; // 如果指针是空就是没初始化过，否则就是初始化过
        }

        public void* Find(ReadOnlySpan<char> input_string)
        {
            byte* buffer = stackalloc byte[input_string.Length + 1];
            encoding.GetBytes(input_string, new Span<byte>(buffer, input_string.Length));
            buffer[input_string.Length] = 0;
            return Find(buffer);
        }

        /// <summary>
        /// 查找字符串对应信息
        /// </summary>
        /// <param name="input_string_pointer">字符串指针</param>
        /// <returns>字符串对应信息指针</returns>
        public void* Find(void* input_string_pointer)
        {
            if (count != 0) // 如果长度不为0，其实我觉得没必要，但是游戏里面有这个检测，所以直接搬过来了
            {
                byte* input_string_pointer_8_bits = (byte*)input_string_pointer; // 获取输入字符串的指针
                uint* pointer_32_bits = (uint*)pointer; // 获取起始指针
                uint* current_pointer_32_bits = pointer_32_bits; // 获取当前指针
                byte upper_char; // 大写字符临时变量
                uint current_value; // 当前值临时变量
                byte current_char; // 当前字符临时变量
                while (true) // 循环查找
                {
                    if (current_pointer_32_bits == null) // 如果当前指针为空
                    {
                        return null; // 返回空指针
                    }
                    upper_char = CType_H.ToUpper(*input_string_pointer_8_bits); // 获取当前输入字符
                    current_value = *current_pointer_32_bits; // 获取当前值
                    current_char = (byte)current_value; // 获取当前字符，注意不要用指针强转，不然在大端序设备上会出问题
                    if (current_char == upper_char) // 如果字符相等
                    {
                        current_pointer_32_bits++; // 增加当前值指针
                        if (current_char == 0) // 如果当前的值是0，那说明已经查到了
                        {
                            return current_pointer_32_bits; // 现在的指针就是字符串对应信息指针，直接返回
                        }
                        input_string_pointer_8_bits++; // 增加输入字符串指针
                    }
                    else
                    {
                        if (current_char > upper_char) // 如果当前字符比输入字符大，就直接返回空指针（只有当前字符比输入字符小才有必要查找后面的，因为字符串被严格排序）
                        {
                            return null; // 返回空指针
                        }
                        if (current_value >> 8 == 0) // 如果当前值后面的偏移是0
                        {
                            return null; // 返回空指针
                        }
                        current_pointer_32_bits = pointer_32_bits + (current_value >> 8); // 跳转到新的偏移位置继续查找
                    }
                }
            }
            return null; // 返回空指针
        }

        /// <summary>
        /// 查找下一个字符串和数据
        /// </summary>
        /// <param name="offset">数据偏移，是地址差/4</param>
        /// <param name="alreadyString">已经找好的字符数组</param>
        /// <param name="alreadyLength">已经找好的字符长度</param>
        /// <param name="callBack">每次找到数据后的回调函数</param>
        private void FindNextInfo(uint offset, byte* alreadyString, uint alreadyLength, Action<nuint, uint, nuint> callBack)
        {
            uint* pointer_32_bits = (uint*)pointer; // 获取起始指针
            uint* current_pointer_32_bits = pointer_32_bits + offset; // 获取当前指针，offset不是地址差而是地址差/4！
            uint current_value; // 当前值临时变量
            byte current_char; // 当前字符临时变量
            while (true) // 循环查找
            {
                if (current_pointer_32_bits == null) // 如果当前指针为空
                {
                    return; // 直接返回
                }
                current_value = *current_pointer_32_bits; // 获取当前值
                current_char = (byte)current_value; // 获取当前字符，注意不要用指针强转，不然在大端序设备上会出问题
                if (current_value >> 8 != 0) // 如果有尾随地址
                {
                    FindNextInfo(current_value >> 8, alreadyString, alreadyLength, callBack); // 接着找尾随地址对应的信息
                }
                current_pointer_32_bits++; // 增加当前指针
                if (current_char == 0) // 如果当前已经是字符串结尾了 
                {
                    alreadyString[alreadyLength] = 0; // 尾部添加一个0毕竟是c的字符串，长度不变了
                    callBack((nuint)alreadyString, alreadyLength, (nuint)current_pointer_32_bits); // 调用回调函数
                    return; // 直接返回
                }
                alreadyString[alreadyLength++] = current_char; // 增加字符长度，设置字符数组
            }
        }

        /// <summary>
        /// 遍历每一个哈希表中的信息
        /// </summary>
        /// <param name="callBack">回调函数</param>
        public void ForEach(Action<string, nuint> callBack)
        {
            if (count != 0) // 如果长度不为0
            {
                byte* string_buffer = stackalloc byte[string_buffer_len]; // 栈上分配字符串内存
                FindNextInfo(0, string_buffer, 0, (stringPtr, stringLen, dataPtr) => callBack?.Invoke(encoding.GetString((byte*)stringPtr, (int)stringLen), dataPtr)); // 找下一个偏移的字符串
            }
        }
    }
}
