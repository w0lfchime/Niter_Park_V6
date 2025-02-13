//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class Character333 : MonoBehaviour
//{
//    private readonly Queue<(int frame, Action action)> actionQueue = new();
//    private readonly Queue<(int frame, Action<object> action, object param)> paramActionQueue = new();
//    private int currentFrame = 0;

//    void FixedUpdate()
//    {
//        currentFrame++;

//        // Execute non-param actions
//        while (actionQueue.Count > 0 && actionQueue.Peek().frame <= currentFrame)
//        {
//            var (_, action) = actionQueue.Dequeue();
//            action?.Invoke();
//        }

//        // Execute param actions
//        while (paramActionQueue.Count > 0 && paramActionQueue.Peek().frame <= currentFrame)
//        {
//            var (_, action, param) = paramActionQueue.Dequeue();
//            action?.Invoke(param);
//        }
//    }

//    /// <summary>
//    /// Schedules an action to run after a set number of frames.
//    /// </summary>
//    public void RunAfterFrames(int frames, Action action)
//    {
//        if (frames <= 0)
//        {
//            action?.Invoke();
//            return;
//        }

//        actionQueue.Enqueue((currentFrame + frames, action));
//    }

//    /// <summary>
//    /// Schedules an action with a parameter to run after a set number of frames.
//    /// </summary>
//    public void RunAfterFrames<T>(int frames, Action<T> action, T param)
//    {
//        if (frames <= 0)
//        {
//            action?.Invoke(param);
//            return;
//        }

//        paramActionQueue.Enqueue((currentFrame + frames, (p) => action((T)p), param));
//    }
//}
