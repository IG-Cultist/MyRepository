using MagicOnion;
using MagicOnion.Server;
using MessagePack;
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

        public async UnaryResult<int> SumAllAsync(int[] num)
        {
            int result = 0;
            for (int i = 0; i < num.Length; i++)
            {
                result += num[i]; 
                Console.WriteLine("Recieved:" + num[i]);
            }  

            return result;
        }

        public async UnaryResult<int[]> CalcForOperationAsync(int x, int y)
        {
            int[] result = new int[4];

            result[0] = x + y;
            result[1] = x - y;
            result[2] = x * y;
            result[3] = x / y;

            return result;
        }

        public async UnaryResult<float> SumAllNumberAsync(Number numArray)
        {
            return numArray.x + numArray.y;
        }
    }
}
