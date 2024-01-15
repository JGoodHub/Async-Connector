using System;
using UnityEngine;

namespace Async.Connector
{

    public class Promise<T>
    {
        private bool _resolved;

        public bool Resolved => _resolved;

        private event Action<T> OnResolveCallback;
        private Action<string> OnErrorCallback;

        public Promise<T> Then(Action<T> callback)
        {
            OnResolveCallback += callback;
            return this;
        }

        public void Catch(Action<string> callback)
        {
            OnErrorCallback = callback;
        }

        public void ResolveHandler(T resolveObject)
        {
            OnResolveCallback?.Invoke(resolveObject);
            _resolved = true;
        }

        public void ErrorHandler(string message)
        {
            OnErrorCallback(message);
        }

        public WaitUntil WaitUntilResolved()
        {
            return new WaitUntil(() => Resolved);
        }
    }

}