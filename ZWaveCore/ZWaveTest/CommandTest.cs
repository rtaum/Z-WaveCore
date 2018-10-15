using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using ZWaveCore.Commands;
using ZWaveCore.Enums;

namespace ZWaveTest
{
    public class CommandTest
    {
        [Theory]
        [ClassData(typeof(CommandClassProvider))]
        public void CommandClassTest(byte commandClass)
        {
            var command = new Command((byte)commandClass, 1);
            Assert.Equal((byte)commandClass, command.ClassID);
            Assert.Equal(1, command.CommandID);
        }

        private Array GetCommandClass()
        {
            return Enum.GetValues(typeof(CommandClass));
        }
    }

    internal class CommandClassProvider : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var enumValues = Enum.GetValues(typeof(CommandClass));
            foreach (var enumValue in enumValues)
            {
                yield return new object[] { enumValue };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
