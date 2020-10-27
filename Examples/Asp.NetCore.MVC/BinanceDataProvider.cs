﻿using Binance.Net.Interfaces;
using Binance.Net.Objects;
using CryptoExchange.Net.Sockets;
using System;
using System.Threading.Tasks;
using Binance.Net.Enums;
using Binance.Net.Objects.Spot.MarketStream;

namespace WebApplication1
{
    public interface IBinanceDataProvider
    {
        IBinanceStreamKlineData LastKline { get; }
        Action<IBinanceStreamKlineData> OnKlineData { get; set; }

        Task Start();
        Task Stop();
    }

    public class BinanceDataProvider: IBinanceDataProvider
    {
        private IBinanceSocketClient _socketClient;
        private UpdateSubscription _subscription;

        public IBinanceStreamKlineData LastKline { get; private set; }
        public Action<IBinanceStreamKlineData> OnKlineData { get; set; }
       
        public BinanceDataProvider(IBinanceSocketClient socketClient)
        {
            _socketClient = socketClient;

            Start().Wait(); // Probably want to do this in some initialization step at application startup
        }

        public async Task Start()
        {
            var subResult = await _socketClient.Spot.SubscribeToKlineUpdatesAsync("BTCUSDT", KlineInterval.FifteenMinutes, data =>
            {
                LastKline = data;
                OnKlineData?.Invoke(data);
            });
            if (subResult.Success)            
                _subscription = subResult.Data;            
        }

        public async Task Stop()
        {
            await _socketClient.Unsubscribe(_subscription);
        }
    }
}
