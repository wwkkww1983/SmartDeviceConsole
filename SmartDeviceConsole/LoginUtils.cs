using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDeviceConsole
{
    class LoginUtils
    {
        private static readonly LoginUtils instance = new LoginUtils();

        private bool loginStatus;
        private AliIotDeviceInfo deviceInfo;
        private string userName;

        private LoginUtils()
        {

        }

        public static LoginUtils Instance
        {
            get
            {
                return instance;
            }
        }

        public bool LoginStatus
        {
            set { loginStatus = value; }
            get { return loginStatus;  }
        }

        public AliIotDeviceInfo DeviceInfo
        {
            set { deviceInfo = value;  }
            get { return deviceInfo;  }
        }


        public string UserName
        {
            set { userName = value; }
            get { return userName; }
        }

    }
}
