// using System;
// using System.Threading.Tasks;
// using UnityEngine;
//
// namespace Config {
//     public static class GlobalExceptionHooks
//     {
//         [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//         static void Wire()
//         {
//             AppDomain.CurrentDomain.UnhandledException += (_, unhandledExceptionEventArgs) =>
//             {
//                 if (unhandledExceptionEventArgs.ExceptionObject is Exception ex) Debug.LogException(ex);
//                 else Debug.LogError("UnhandledException: " + unhandledExceptionEventArgs.ExceptionObject);
//             };
//
//             TaskScheduler.UnobservedTaskException += (_, e) =>
//             {
//                 Debug.LogException(e.Exception);
//                 e.SetObserved();
//             };
//         }
//         
//     }
// }