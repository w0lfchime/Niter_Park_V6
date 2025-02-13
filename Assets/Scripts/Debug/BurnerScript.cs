//using System.Collections.Generic;
//using System;

//[Header("Action Queue")]
//private readonly Queue<(int frame, Action action)> actionQueue = new();
//private readonly Queue<(int frame, Action<object> action, object param)> paramActionQueue = new();


////called in fixed update
//private void ProcessActionQueue()
//{
//    // Execute non-param actions
//    while (actionQueue.Count > 0 && actionQueue.Peek().frame <= currentFixedFrame)
//    {
//        var (_, action) = actionQueue.Dequeue();
//        action?.Invoke();
//    }

//    // Execute param actions
//    while (paramActionQueue.Count > 0 && paramActionQueue.Peek().frame <= currentFixedFrame)
//    {
//        var (_, action, param) = paramActionQueue.Dequeue();
//        action?.Invoke(param);
//    }
//}

//public void ScheduleAction(int framesFromNow, Action action)
//{
//    if (framesFromNow <= 0)
//    {
//        action?.Invoke();
//        return;
//    }

//    actionQueue.Enqueue((currentFixedFrame + framesFromNow, action));
//}
//public void ScheduleAction<T>(int framesFromNow, Action<T> action, T param)
//{
//    if (framesFromNow <= 0)
//    {
//        action?.Invoke(param);
//        return;
//    }

//    paramActionQueue.Enqueue((currentFixedFrame + framesFromNow, (p) => action((T)p), param));
//}