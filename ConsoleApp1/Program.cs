using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("start service...");
       
            Console.WriteLine("Open Service");

            WcfRevitLibrary.IRevitExternalService service;
            try
            {
                System.ServiceModel.ChannelFactory<WcfRevitLibrary.IRevitExternalService> channelFactory =
                    new System.ServiceModel.ChannelFactory<WcfRevitLibrary.IRevitExternalService>("IRevitExternalService");

                service = channelFactory.CreateChannel();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("ready to load family");
            bool isLoad = service.LoadFamily(@"C:\Users\xu.lanhui\Desktop\二次开发\矩形柱帽 斜角 -wcf.rfa");
            Console.WriteLine(isLoad);
            Console.ReadLine();
        }
    }
}
