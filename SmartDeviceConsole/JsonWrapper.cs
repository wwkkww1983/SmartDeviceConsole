using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDeviceConsole
{
    class StatusWrapper
    {
        private bool status;
        private string errorMsg;


        public bool STATUS
        {
            get { return status; }
            set { status = value; }
        }


        public string ErrorMsg
        {
            get { return errorMsg; }
            set { errorMsg = value; }
        }
    }

    class ValueWrapper<T> : StatusWrapper
    {
        private T value;

        public T VALUE
        {
            get { return value;  }
            set { this.value = value; }
        }
    }

    

}
