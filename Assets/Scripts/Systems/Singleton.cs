using UnityEngine;

namespace Singleton
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static System.Object _lock = new System.Object();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();

                        if (_instance == null)
                        {
                            Debug.LogWarning("В сцене не найдено синглтона" + typeof(T) + ", поэтому он был создан принудительно");
                            var singleton = new GameObject("[SINGLETON] " + typeof(T));
                            _instance = singleton.AddComponent<T>();
                        }

                    }

                    return _instance;
                }
            }
        }
    }
}
