using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Extensions;


namespace FirebaseSettings
{
    public class FirebaseInitializer : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            FirebaseApp.CheckAndFixDependenciesAsync().
                ContinueWithOnMainThread(OnDependencyStatusReceived);
        }

        private void OnDependencyStatusReceived(Task<DependencyStatus> task)
        {
            try
            {
                if (!task.IsCompletedSuccessfully)
                {
                    throw new Exception("Could not resolve dependencies", task.Exception);
                }

                if (task.Result != DependencyStatus.Available)
                {
                    throw new Exception($"Could not resolve dependencies {task.Result}");
                }
            }
            catch (Exception exception)
            {
                Debug.Log(exception);
            }
        }
    }   
}