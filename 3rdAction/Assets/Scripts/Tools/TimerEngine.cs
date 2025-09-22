using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.LowLevel;
using System.Collections.Generic;
using UnityEditor;
internal static class TimerBootstrapper
{
    static PlayerLoopSystem timerSystem;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    internal static void Initialize()
    {
        PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
        if(!InserTimerManager<Update>(ref currentPlayerLoop,0))
        {
            return;
        }
        PlayerLoop.SetPlayerLoop(currentPlayerLoop);
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeState;
        EditorApplication.playModeStateChanged += OnPlayModeState;

#endif
        static void OnPlayModeState(PlayModeStateChange state)
        {
            if(state == PlayModeStateChange.ExitingPlayMode)
            {
                PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                RemoveTimeManager<Update>(ref currentPlayerLoop);
                PlayerLoop.SetPlayerLoop(currentPlayerLoop);

                TimerManager.Clear();
            }
        }
    }

    static void RemoveTimeManager<T>(ref PlayerLoopSystem loop)
    {
        PlayerLoopsUtils.RemoveSystem<T>(ref loop,in timerSystem);
    }



    static bool InserTimerManager<T>(ref PlayerLoopSystem loop,int index)
    {
        timerSystem = new PlayerLoopSystem()
        {
            type = typeof(TimerManager),
            updateDelegate = TimerManager.UpdateTimers,
            subSystemList = null
        };

        return PlayerLoopsUtils.InsertSystem<T>(ref loop,in timerSystem,index);
    }
}