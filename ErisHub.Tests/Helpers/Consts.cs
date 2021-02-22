using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ErisHub.Tests.Helpers
{
    public abstract class TestBase
    {
        private readonly ConcurrentDictionary<Type, object> _mocks;

        protected TestBase()
        {
            _mocks = new();
        }

        protected Mock<T> M<T>() where T: class
        {
            return Get<T>();
        }


        private Mock<T> Get<T>() where T : class
        {
            if (!_mocks.ContainsKey(typeof(T)))
            {
                _mocks[typeof(T)] = new Mock<T>();
            }
            return (Mock<T>)_mocks[typeof(T)];
        }
    }

    class Consts
    {
        public const ulong ChannelId1 = 1;
        public const ulong ChannelId2 = 2;

        public const ulong InvalidMessageId = 0;
        public const ulong MessageId1 = 1;
        public const ulong MessageId2 = 2;
    }

    class Mocks
    {
        static readonly Dictionary<Type, object> _mocks;

        static Mocks()
        {
            _mocks = new Dictionary<Type, object>();
        }

    }
}
