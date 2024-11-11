using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstModel : MonoBehaviour
{
    const string ServerURL = "http://localhost:7000";

    async void Start()
    {
        //Sum(100, 334, result =>
        //{
        //    Debug.Log(result);
        //});

        int resultSum = await Sum(100, 334);
        Debug.Log("‰ÁŽZ" + resultSum);

        int resultSub = await Sub(334, 100);
        Debug.Log("Œ¸ŽZ" + resultSub);
    }

    //public async void Sum(int x, int y,Action<int> callback)
    //{
    //    using var handler = new YetAnotherHttpHandler() { Http2Only = true };
    //    var channel = GrpcChannel.ForAddress(
    //        ServerURL,new GrpcChannelOptions() { HttpHandler = handler });
    //    var client = MagicOnionClient.Create<IMyFirstService>(channel);
    //    var result = await client.SumAsync(x, y);
    //    callback?.Invoke(result);
    //}

    public async UniTask<int> Sum(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(
            ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.SumAsync(x, y);
        return result;
    }

    public async UniTask<int> Sub(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(
            ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.SubAsync(x, y);
        return result;
    }
}
