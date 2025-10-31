using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEditor;


#pragma warning disable IDE0005
using Serilog = Meryel.Serilog;
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor
{

    //[InitializeOnLoad]
    public static class MainThreadDispatcher
    {
        readonly static ConcurrentBag<System.Action> actions;

        static MainThreadDispatcher()
        {
            actions = new ConcurrentBag<System.Action>();
            EditorApplication.update += Update;
        }

        /// <summary>
        /// Empty method for invoking static class ctor
        /// </summary>
        public static void Bump() {}

        static void Update()
        {
            while (actions.TryTake(out var action))
            {
                action.Invoke();
            }
        }

        public static void Add(System.Action action) => actions.Add(action);
    }
}