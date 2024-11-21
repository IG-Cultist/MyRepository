using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using UnityEngine;

public class UserModel : BaseModel
{
    private int userID; //ìoò^ÉÜÅ[ÉUID
    public async UniTask<bool> RegistAsync(string name)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(
            ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create <IUserService> (channel);

        try
        {
            userID = await client.RegistUserAsync(name);
            return true;
        }
        catch (RpcException e)
        {
            Debug.Log(e);
            return false;
        }
    }
}
