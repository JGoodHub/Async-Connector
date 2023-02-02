using System;
using UnityEngine;

namespace AsyncGameServer
{

    public class ScriptableSingleton<T> : ScriptableObject where T : class
    {

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load($"Singletons/{typeof(T).Name}") as T;

                if (_instance == null)
                    throw new Exception($"Exception: No instance could be found for the singleton {typeof(T)}. Check an instance of the scriptable object has been created and has been placed inside a Resources/Singletons folder.");

                return _instance;
            }
        }

    }
    
    

}