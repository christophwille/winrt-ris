using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ris.Client.Models
{
    public class ResultBase
    {
        public ResultBase()
        {
        }

        public ResultBase(string errorMessage)
        {
            Succeeded = false;
            Error = errorMessage;
        }

        public bool Succeeded { get; set; }
        public string Error { get; set; }
    }
}
