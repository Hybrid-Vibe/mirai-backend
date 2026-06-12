using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Mirai.Supabase;
using Supabase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Services
{
    public class SupabaseClientService
    {
        public Client Client { get; }

        public SupabaseClientService(IConfiguration configuration)
        {
            Client = new Client(
                configuration["SupabaseImage:Url"],
                configuration["SupabaseImage:Key"]);

            Client.InitializeAsync().Wait();
        }
    }
}
