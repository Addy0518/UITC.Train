using Lab.API.DI.Models;

namespace Lab.API.DI.Service
{
    public class CustomerService
    {
        public ISample Transient { get; private set; }
        public ISample Scoped { get; private set; }
        public ISample Singleton { get; private set; }

        public CustomerService(
            ISampleTransient transient,
            ISampleScoped scoped,
            ISampleSingleton singleton
        )
        {
            Transient = transient;
            Scoped = scoped;
            Singleton = singleton;
        }
    }

    // 1. 定義插頭的介面 , 讓他有個方法 , 意指插插座連結到電源
    public interface IElectricalPlug
    {
        void Connect();
    }

    // 2. 再來定義使用插頭的插座

    public class Socket
    {
        private readonly IElectricalPlug _plug;

        public Socket(IElectricalPlug plug)
        {
            if (plug == null)
            {
                throw new ArgumentNullException("plug是空的!");
            }
            _plug = plug;
        }

        public void SendPower()
        {
            this._plug.Connect();
        }
    }

    // 3. 再試試實作 HairDryerPlug 類別

    public class HairDryerPlug : IElectricalPlug
    {
        public void Connect()
        {
            Console.WriteLine("HairDryerPlug connected!\n");
        }
    }
}
