using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO.Admin
{
    public sealed class ReplicateOptions
    {
        public string BaseUrl { get; init; } = "";
        public string ApiToken { get; init; } = "";
        public string Model { get; init; } = "";
        public int WaitSeconds { get; init; }
    }
}
