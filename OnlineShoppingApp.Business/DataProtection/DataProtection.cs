using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingApp.Business.DataProtection
{
    public class DataProtection : IDataProtection
    {
        private readonly IDataProtector _protector; // Instance for data protection.

        // Constructor initializes the IDataProtector with a specific purpose.
        public DataProtection(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("OnlineShoppingApp-security-v1");
        }

        // Protects the input text for secure storage or transmission.
        public string Protect(string text)
        {
            return _protector.Protect(text);
        }

        // Unprotects the protected text, returning it to its original form.
        public string UnProtect(string protectedText)
        {
            return _protector.Unprotect(protectedText);
        }
    }
}
