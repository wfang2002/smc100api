using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SMC100API;

namespace smc100api_demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Calling SMC100API");

            string version = SMC100PP.APIVersion();

            Console.WriteLine("API version = {0}", version);

            string smcPort = SMC100PP.Detect();

            if (smcPort.Length < 1)
            {
                Console.WriteLine("Not found SMC100PP Controller. Exit.");
                return;
            }

            Console.WriteLine("Found SMC100PP at port {0}", smcPort);

            if (!SMC100PP.Connect(smcPort))
            {
                Console.WriteLine("Failed to open port. Error message: {0}", SMC100PP.LastErrorString());
                return;
            };

            // SMC100 Controller version
            string ver = SMC100PP.GetControllerVersion();
            Console.WriteLine("Controller version = {0}", ver);

            Console.WriteLine("Set controller to configuration state");
            SMC100PP.EnterConfigurationState();

            // Show SMC parameters
            Console.WriteLine("Show configuration values:");

            Console.WriteLine("GetAcceleration = {0}", SMC100PP.GetAcceleration());
            Console.WriteLine("GetBacklashCompensation = {0}", SMC100PP.GetBacklashCompensation());
            Console.WriteLine("GetHysteresisCompensation = {0}", SMC100PP.GetHysteresisCompensation());
            Console.WriteLine("GetHomeSearchType = {0}", SMC100PP.GetHomeSearchType());
            Console.WriteLine("GetStageIdentifier = {0}", SMC100PP.GetStageIdentifier());
            Console.WriteLine("IsKeypadEnabled = {0}", SMC100PP.IsKeypadEnabled());
            Console.WriteLine("GetJerkTime = {0}", SMC100PP.GetJerkTime());
            Console.WriteLine("IsReadyState = {0}", SMC100PP.IsReadyState());
            Console.WriteLine("GetHomeSearchVelocity = {0}", SMC100PP.GetHomeSearchVelocity());
            Console.WriteLine("GetHomeSearchTimeout = {0}", SMC100PP.GetHomeSearchTimeout());
            Console.WriteLine("GetAbsolutePosition = {0}", SMC100PP.GetAbsolutePosition());
            Console.WriteLine("GetRelativePosition = {0}", SMC100PP.GetRelativePosition());
            Console.WriteLine("IsConfigurationState = {0}", SMC100PP.IsConfigurationState());
            Console.WriteLine("GetCurrentPeakLimit = {0}", SMC100PP.GetCurrentPeakLimit());
            Console.WriteLine("GetAnalogInput = {0}", SMC100PP.GetAnalogInput());
            Console.WriteLine("GetTTLInput = {0}", SMC100PP.GetTTLInput());
            Console.WriteLine("GetRS485Address = {0}", SMC100PP.GetRS485Address());
            Console.WriteLine("GetTTLOutputValue = {0}", SMC100PP.GetTTLOutputValue());
            Console.WriteLine("GetSimulataneousStartedMove = {0}", SMC100PP.GetSimulataneousStartedMove());
            Console.WriteLine("GetNegativeSoftwareLimit = {0}", SMC100PP.GetNegativeSoftwareLimit());
            Console.WriteLine("GetPositiveSoftwareLimit = {0}", SMC100PP.GetPositiveSoftwareLimit());
            Console.WriteLine("GetCommandErrorString = {0}", SMC100PP.GetCommandErrorString());
            Console.WriteLine("GetLastCommandError = {0}", SMC100PP.GetLastCommandError());
            Console.WriteLine("GetSetPointPosition = {0}", SMC100PP.GetSetPointPosition());
            Console.WriteLine("GetCurrentPosition = {0}", SMC100PP.GetCurrentPosition());
            Console.WriteLine("GetPositionerErrorAndControllerState = {0}", SMC100PP.GetPositionerErrorAndControllerState());
            Console.WriteLine("GetVelocity = {0}", SMC100PP.GetVelocity());
            Console.WriteLine("GetAllAxisParameters = {0}", SMC100PP.GetAllAxisParameters());

            // Leave CONFIGURATION state (Change to NOT REFERENCED state)
            Console.WriteLine("Leave configuration state");
            SMC100PP.LeaveConfigurationState();

            Console.WriteLine("Wait 3s to have controller returns to NOT REFERENCED state.");
            System.Threading.Thread.Sleep(3000);

            // Change to READY state.
            Console.WriteLine("Homing (go to READY state)");
            SMC100PP.ExecuteHomeSearch();

            double velocity = 0.5;

            Console.WriteLine("Change velocity = {0}", velocity);
            SMC100PP.SetVelocity(velocity);

            // Get estimated time to move to position 2 (360 degree on a 200-step motor)
            double time = SMC100PP.GetMotionTime(2);
            Console.WriteLine("It'll take {0}s to move to relative position 2 @ velocity {1}", time, velocity);

            // Move to relative position 2
            Console.WriteLine("Moving to relative position 2");
            SMC100PP.SetRelativePosition(2);

            Console.WriteLine("Waiting ...");
            System.Threading.Thread.Sleep((int)(time*1000) + 1000);

            velocity = 5;
            Console.WriteLine("Change velocity = {0}", velocity);
            SMC100PP.SetVelocity(velocity);

            time = SMC100PP.GetMotionTime(2);
            Console.WriteLine("It'll take {0}s to move to relative position -2 @ velocity {1}", time, velocity);

            // Move to relative position -2
            Console.WriteLine("Moving to relative position -2");
            SMC100PP.SetRelativePosition(-2);

            Console.WriteLine("Waiting ...");
            System.Threading.Thread.Sleep((int)(time * 1000) + 1000);

            SMC100PP.Disconnect();

        }
    }
}
