using System;
using System.Collections.Generic;
using System.Text;
using Mirai.Application.Interfaces.Services;
namespace Mirai.Infastructure.Services
{
    
    public class LanguageDetector : ILanguageDetector
    {


        public bool IsVietnamese(string text)
        {
            string vietnameseChars =
                "ăâđêôơưáàảãạấầẩẫậắằẳẵặéèẻẽẹíìỉĩịóòỏõọốồổỗộớờởỡợúùủũụứừửữựýỳỷỹỵ";

            return text
                .ToLower()
                .Any(c => vietnameseChars.Contains(c));
        }
    }
}
