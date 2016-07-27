using Android.App;
using Android.OS;

namespace Client
{
    public class ServiceBinder<T> : Binder where T : Service
    {
        private readonly T _service;

        public ServiceBinder(T service)
        {
            _service = service;
        }
        public T GetServiceInstance()
        {
            return _service;
        }
    }
}