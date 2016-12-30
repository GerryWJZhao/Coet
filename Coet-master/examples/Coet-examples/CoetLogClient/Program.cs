﻿using Coet.LogSDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoetLogClient
{
    class Program
    {
        static void Main(string[] args)
        {
            CoetLogSDK.Start("192.168.1.104:50051");

            while (true)
            {
                Parallel.For(0, 500, (i, j) => {
                    ExamplesClientEntity ec = new ExamplesClientEntity
                    {
                        Id = Guid.NewGuid().ToString(),
                        Action = "test",
                        time = DateTime.Now
                    };
                    CoetLogSDK.Log("info", JsonConvert.SerializeObject(ec), "192.168.1.104", "examplesClient");
                });

                Thread.Sleep(500);
            }

            Console.ReadLine();
        }
    }

    class ExamplesClientEntity
    {
        public string Id { get; set; }
        public string Action { get; set; }
        public DateTime time { get; set; }
    }
}