using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface ILanguageDetector
    {
        bool IsVietnamese(string text);
    }
}
