using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.IO.Ports;

namespace SMC100API
{
    public class SMC100PP
    {
        static SerialPort _serialPort = new SerialPort();
        static string _errorString;
        static private Object rwLock = new Object();
        static private int _device = 1;

        public static string APIVersion()
        {
            return "SMC100API 0.1";
        }

        public static string LastErrorString()
        {
            return _errorString;
        }

        // detects which port is connected to SMC100PP
        public static string Detect()
        {
            string[] theSerialPortNames = System.IO.Ports.SerialPort.GetPortNames();

            foreach(string port in theSerialPortNames)
            {
                //Console.WriteLine("Detecting port {0}", port);
                if (Connect(port))
                {
                    string version = GetControllerVersion();
                    if (version.IndexOf("SMC_PP") >= 0)
                    {
                        // Found SMC 100
                        Disconnect();
                        return port;
                    }
                    Disconnect();
                }
            }

            return ""; 
        }

        public static bool Connect(string port)
        {
            Disconnect();

            _serialPort.PortName = port;
            _serialPort.BaudRate = 57600;
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 500;

            try
            {
                _serialPort.Open();
                return true;
            } 
            catch (Exception e)
            {
                Console.WriteLine("Open port failed: {0}", e.Message);
                _errorString = e.Message;
                return false;
            }

        }

        // Close connection to SMC100 controller
        public static void Disconnect()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }

        }


        // Reference: AC — Set/Get acceleration
        public static void SetAcceleration(double value)
        {
            SendValue("AC", value);
        }

        // Reference: AC — Set/Get acceleration
        public static double GetAcceleration()
        {
            return GetQueryValueDouble("AC");
        }

        // Reference: BA Set/Get backlash compensation
        public static void SetBacklashCompensation(double value)
        {
            SendValue("BA", value);
        }

        // Reference: BA Set/Get backlash compensation
        public static double GetBacklashCompensation()
        {
            return GetQueryValueDouble("BA");
        }

        // Reference: BH Set/Get hysteresis compensation
        public static void SetHysteresisCompensation(double value)
        {
            SendValue("BH", value);
        }

        // Reference: BH Set/Get hysteresis compensation
        public static double GetHysteresisCompensation()
        {
            return GetQueryValueDouble("BH");
        }

        // Reference: HT Set/Get HOME search type
        public static void SetHomeSearchType(double value)
        {
            SendValue("HT", value);
        }

        // Reference: HT Set/Get HOME search type
        public static double GetHomeSearchType()
        {
            return GetQueryValueDouble("HT");
        }

        // Reference: ID Set/Get stage identifier
        public static void SetStageIdentifier(double value)
        {
            SendValue("ID", value);
        }

        // Reference: ID Set/Get stage identifier
        public static string GetStageIdentifier()
        {
            return GetQueryValueString("ID");
        }

        // Reference: JD Leave JOGGING state
        public static void LeaveJoggingState(double value)
        {
            SendValue("JD", value);
        }

        // Reference: JM Enable/disable keypad
        public static void EnableKeypad(bool value)
        {
            SendValue("JM", value);
        }

        // Reference: JM Enable/disable keypad
        public static bool IsKeypadEnabled()
        {
            return GetQueryValueBoolean("JM");
        }

        // Reference: JR Set/Get jerk time
        public static void SetJerkTime(double value)
        {
            SendValue("JR", value);
        }

        // Reference: JR Set/Get jerk time
        public static double GetJerkTime()
        {
            return GetQueryValueDouble("JR");
        }

        // Reference: MM Enter/Leave DISABLE state
        // value = false - changes state from READY to DISABLE
        // value = true - changes state from DISABLE to READY
        public static void EnterDisableState(bool value)
        {
            SendValue("MM", value);
        }

        // Reference: Enter/Leave DISABLE state
        public static bool IsReadyState()
        {
            return GetQueryValueBoolean("MM");
        }

        // Reference: OH Set/Get HOME search velocity
        public static void SetHomeSearchVelocity(double value)
        {
            SendValue("OH", value);
        }

        // Reference: OH Set/Get HOME search velocity
        public static double GetHomeSearchVelocity()
        {
            return GetQueryValueDouble("OH");
        }

        // Reference: OR Execute HOME search
        public static void ExecuteHomeSearch()
        {
            SendValue("OR");
        }

        // Reference: OT Set/Get HOME search time-out
        public static void SetHomeSearchTimeout(double value)
        {
            SendValue("OT", value);
        }

        // Reference: OT Set/Get HOME search time-out
        public static double GetHomeSearchTimeout()
        {
            return GetQueryValueDouble("OT");
        }

        // Reference: PA Move absolute
        public static void SetAbsolutePosition(double value)
        {
            SendValue("PA", value);
        }

        // Reference: PA Move absolute
        public static double GetAbsolutePosition()
        {
            return GetQueryValueDouble("PA");
        }

        // Reference: PR Move relative
        public static void SetRelativePosition(double value)
        {
            SendValue("PR", value);
        }

        // Reference: PR Move relative
        public static double GetRelativePosition()
        {
            return GetQueryValueDouble("PR");
        }

        // Reference: PT Get motion time for a relative move       
        public static double GetMotionTime(double value)
        {
            // Special case - not a regular query with question mark
            string query = String.Format("{0}{1}{2}", _device, "PT", value);
            string result = SendReceive(query);
            string val = result.Length > 3 ? result.Substring(3) : "0";
            val = val.TrimEnd(new char[] { '\r', '\n' });

            try
            {
                return (double)System.Convert.ToDouble(val);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // Reference: PW Enter/Leave CONFIGURATION state
        // value = true - Go from NOT REFERENCED state to CONFIGURATION state
        // value = false - Go from CONFIGURATION state to NOT REFERENCED state.
        public static void EnterConfigurationState()
        {
            SendValue("PW", true);
        }

        public static void LeaveConfigurationState()
        {
            SendValue("PW", false);
        }

        // Reference: PW Enter/Leave CONFIGURATION state
        public static bool IsConfigurationState()
        {
            return GetQueryValueBoolean("PW");
        }

        // Reference: QIL Set/Get motor’s current limits
        public static void SetCurrentPeakLimit(double value)
        {
            SendValue("QIL", value);
        }

        // Reference: QIL Set/Get motor’s current limits
        public static double GetCurrentPeakLimit()
        {
            return GetQueryValueDouble("QIL");
        }

        /*
         *  QIR/QIT not valid for PP version
         *  
        // Reference: QIR Set/Get motor’s current limits
        public static void SetCurrentRMSLimit(double value)
        {
            SendValue("QIR", value);
        }

        // Reference: QIR Set/Get motor’s current limits
        public static double GetCurrentRMSLimit()
        {
            return GetQueryValueDouble("QIR");
        }

        // Reference: QIT Set/Get motor’s current limits
        public static void SetCurrentLimitPeriod(double value)
        {
            SendValue("QIT", value);
        }

        // Reference: QIT Set/Get motor’s current limits
        public static double GetCurrentLimitPeriod()
        {
            return GetQueryValueDouble("QIT");
        }
        */

        // Reference: RA Get analog input value
        public static double GetAnalogInput()
        {
            return GetQueryValueDouble("RA");
        }

        // Reference: RB Get TTL input value
        public static double GetTTLInput()
        {
            return GetQueryValueDouble("RB");
        }

        // Reference: RS Reset controller
        public static void ResetController(double value)
        {
            SendValue("RS");
        }

        // Reference: SA Set/Get controller’s RS-485 address
        public static void SetRS485Address(double value)
        {
            SendValue("SA", value);
        }

        // Reference: SA Set/Get controller’s RS-485 address
        public static double GetRS485Address()
        {
            return GetQueryValueDouble("SA");
        }

        // Reference: SB Set/Get TTL output value
        public static void SetTTLOutputValue(double value)
        {
            SendValue("SB", value);
        }

        // Reference: SB Set/Get TTL output value
        public static double GetTTLOutputValue()
        {
            return GetQueryValueDouble("SB");
        }

        // Reference: SE Configure/Execute simultaneous started move
        public static void SetSimulataneousStartedMove(double value)
        {
            SendValue("SE", value);
        }

        // Reference: SE Configure/Execute simultaneous started move
        public static double GetSimulataneousStartedMove()
        {
            return GetQueryValueDouble("SE");
        }

        // Reference: SL Set/Get negative software limit
        public static void SetNegativeSoftwareLimit(double value)
        {
            SendValue("SL", value);
        }

        // Reference: SL Set/Get negative software limit
        public static double GetNegativeSoftwareLimit()
        {
            return GetQueryValueDouble("SL");
        }

        // Reference: SR Set/Get positive software limit
        public static void SetPositiveSoftwareLimit(double value)
        {
            SendValue("SR", value);
        }

        // Reference: SR Set/Get positive software limit
        public static double GetPositiveSoftwareLimit()
        {
            return GetQueryValueDouble("SR");
        }

        // Reference: ST Stop motion
        public static void StopMotion(double value)
        {
            SendValue("ST");
        }

        // Reference: TB Get command error string
        public static string GetCommandErrorString()
        {
            return GetQueryValueString("TB");
        }

        // Reference: TE Get last command error
        public static string GetLastCommandError()
        {
            return GetQueryValueString("TE");
        }

        // Reference: TH Get set-point position
        public static double GetSetPointPosition()
        {
            return GetQueryValueDouble("TH");
        }

        // Reference: TP Get current position
        public static double GetCurrentPosition()
        {
            return GetQueryValueDouble("TP");
        }

        // Reference: TS Get positioner error and controller state
        public static string GetPositionerErrorAndControllerState()
        {
            return GetQueryValueString("TS");
        }
        
        // Reference: VA — Set/Get velocity
        public static void SetVelocity(double value)
        {
            SendValue("VA", value);
        }

        // Reference: VA — Set/Get velocity
        public static double GetVelocity()
        {
            return GetQueryValueDouble("VA");
        }

        // Reference: VE - Get controller revision information
        public static string GetControllerVersion()
        {
            return GetQueryValueString("VE");
        }

        // Reference: ZT Get all axis parameters
        public static string GetAllAxisParameters()
        {
            return GetQueryValueString("ZT");
        }

        // Reference: ZX Set/Get SmartStage configuration
        public static void SetSmartStateConfiguration(double value)
        {
            SendValue("ZX", value);
        }

        // Reference: ZX Set/Get SmartStage configuration
        public static double GetSmartStateConfiguration()
        {
            return GetQueryValueDouble("ZX");
        }
        
        public static void SetReadTimeout(int timeout)
        {
            _serialPort.ReadTimeout = timeout;
        }

        /**
         * Internal member functions
        **/

        // Helper function: Call Send to send command with double value to controller
        protected static void SendValue(string cmd, bool val)
        {
            string str = String.Format("{0}{1}{2}", _device, cmd, val ? "1" : "0");
            Send(str);
        }

        // Helper function: Call Send to send command with double value to controller
        protected static void SendValue(string cmd, double val)
        {
            string str = String.Format("{0}{1}{2}", _device, cmd, val);
            Send(str);
        }

        // Helper function: Call Send to send command with string value to controller
        protected static void SendValue(string cmd, string val = "")
        {
            string str = String.Format("{0}{1}{2}", _device, cmd, val);
            Send(str);
        }

        // Helper function: Calls GetQueryValueString and returns query result as double
        protected static double GetQueryValueDouble(string cmd)
        {
            string result = GetQueryValueString(cmd);
            try
            {
                return (double)System.Convert.ToDouble(result);
            } 
            catch(Exception)
            {
                return 0;
            }
        }

        // Helper function: Calls GetQueryValueString and returns query result as boolean
        protected static bool GetQueryValueBoolean(string cmd)
        {
            string result = GetQueryValueString(cmd);
            return result == "1" ? true : false;
        }

        // Helper function: Calls SendReceive and return query result as string
        protected static string GetQueryValueString(string cmd)
        {
            string query = String.Format("{0}{1}?", _device, cmd);
            string result = SendReceive(query);
            string val = result.Length > query.Length ? result.Substring(query.Length - 1) : "";
            val = val.Trim(new char[] {' ', '\r', '\n'});
            //Console.WriteLine("query resule value: '{0}'", val);
            return val;
        }

        // Sync call - Send a command to controller and wait for response
        protected static string SendReceive(string cmd)
        {
            if (!_serialPort.IsOpen)
            {
                _errorString = "Port is not open.";
                return "";
            }

            lock (rwLock)
            {
                //cleanup buffer before writing
                if (_serialPort.BytesToRead > 0)
                {
                    _serialPort.ReadExisting();
                }

                _serialPort.Write(cmd + "\r\n");
                //Console.WriteLine("-> {0}", cmd);

                return Receive();
            }

        }

        // Sync call - Send command to controller, not wait for response
        protected static bool Send(string cmd)
        {
            if (!_serialPort.IsOpen)
            {
                _errorString = "Port is not open.";
                return false;
            }

            lock (rwLock)
            {
                //cleanup buffer before writing
                if (_serialPort.BytesToRead > 0)
                {
                    _serialPort.ReadExisting();
                }

                _serialPort.Write(cmd + "\r\n");
                //Console.WriteLine("-> {0}", cmd);                
            }

            return true;

        }

        protected static string Receive()
        {
            try
            {
                string message = _serialPort.ReadLine();
                //Console.WriteLine("<- {0}", message);
                return message;

            }
            catch (TimeoutException e)
            {
                //Console.WriteLine("<-[Exception] {0}", e.Message);
                _errorString = e.Message;
                return "";
            }
        }
    }
}
