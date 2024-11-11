using MagicOnion;
using MagicOnion.Server;
using Shared.Interfaces.Services;

namespace Server.Services
{
    public class MyFirstService : ServiceBase <IMyFirstService>, IMyFirstService
    {
        public async UnaryResult<int> SumAsync(int x, int y)
        {
            Console.WriteLine("Recieved:" + x + "," + y);
            return x + y;
        }

        public async UnaryResult<int> SubAsync(int x, int y)
        {
            Console.WriteLine("Recieved:" + x + "," + y);
            return x - y;
        }
    }
}
