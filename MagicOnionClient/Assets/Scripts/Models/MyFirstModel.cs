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

        int[] requestArray = new[] { 50, 100, 200 };
        int resultAll = await SumAll(requestArray);
        Debug.Log("”z—ñ‰ÁŽZ" + resultAll);

        int[] resultCalc = await CalcForOperation(100, 25);
        for (int i = 0; i < resultCalc.Length; i++)
        {
            Debug.Log("‚¢‚ë‚ñ‚ÈŒvŽZ" + resultCalc[i]);
        }

        Number number = new Number();
        number.x = 5.5f;
        number.y = 2.5f;

        float resultNum = await SumAllNumber(number);
        Debug.Log("¬”‰ÁŽZ" + resultNum);

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

    public async UniTask<int> SumAll(int[] num)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(
            ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.SumAllAsync(num);
        return result;
    }

    public async UniTask<int[]> CalcForOperation(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(
            ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.CalcForOperationAsync(x,y);
        return result;
    }

    public async UniTask<float> SumAllNumber(Number numArray)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(
            ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.SumAllNumberAsync(numArray);
        return result;
    }
}
