Imports SMC100API

Module Module1


    Sub Main()

        Dim smcPort = SMC100PP.Detect()

        If (smcPort.Length < 1) Then

            Console.WriteLine("Not found SMC100PP Controller. Exit.")
            Return
        End If

        Console.WriteLine("Found SMC100PP at port {0}", smcPort)

        If (SMC100PP.Connect(smcPort) = False) Then

            Console.WriteLine("Failed to open port. Error message: {0}", SMC100PP.LastErrorString())
            Return

        End If

        REM SMC100 Controller version
        Dim ver = SMC100PP.GetControllerVersion()
        Console.WriteLine("Controller version = {0}", ver)

        Console.WriteLine("Set controller to configuration state")
        SMC100PP.EnterConfigurationState()

        REM Show SMC parameters
        Console.WriteLine("Show configuration values:")

        Console.WriteLine("GetAcceleration = {0}", SMC100PP.GetAcceleration())
        Console.WriteLine("GetBacklashCompensation = {0}", SMC100PP.GetBacklashCompensation())
        Console.WriteLine("GetHysteresisCompensation = {0}", SMC100PP.GetHysteresisCompensation())
        Console.WriteLine("GetHomeSearchType = {0}", SMC100PP.GetHomeSearchType())
        Console.WriteLine("GetStageIdentifier = {0}", SMC100PP.GetStageIdentifier())
        Console.WriteLine("IsKeypadEnabled = {0}", SMC100PP.IsKeypadEnabled())
        Console.WriteLine("GetJerkTime = {0}", SMC100PP.GetJerkTime())
        Console.WriteLine("IsReadyState = {0}", SMC100PP.IsReadyState())
        Console.WriteLine("GetHomeSearchVelocity = {0}", SMC100PP.GetHomeSearchVelocity())
        Console.WriteLine("GetHomeSearchTimeout = {0}", SMC100PP.GetHomeSearchTimeout())
        Console.WriteLine("GetAbsolutePosition = {0}", SMC100PP.GetAbsolutePosition())
        Console.WriteLine("GetRelativePosition = {0}", SMC100PP.GetRelativePosition())
        Console.WriteLine("IsConfigurationState = {0}", SMC100PP.IsConfigurationState())
        Console.WriteLine("GetCurrentPeakLimit = {0}", SMC100PP.GetCurrentPeakLimit())
        Console.WriteLine("GetAnalogInput = {0}", SMC100PP.GetAnalogInput())
        Console.WriteLine("GetTTLInput = {0}", SMC100PP.GetTTLInput())
        Console.WriteLine("GetRS485Address = {0}", SMC100PP.GetRS485Address())
        Console.WriteLine("GetTTLOutputValue = {0}", SMC100PP.GetTTLOutputValue())
        Console.WriteLine("GetSimulataneousStartedMove = {0}", SMC100PP.GetSimulataneousStartedMove())
        Console.WriteLine("GetNegativeSoftwareLimit = {0}", SMC100PP.GetNegativeSoftwareLimit())
        Console.WriteLine("GetPositiveSoftwareLimit = {0}", SMC100PP.GetPositiveSoftwareLimit())
        Console.WriteLine("GetCommandErrorDim = {0}", SMC100PP.GetCommandErrorString())
        Console.WriteLine("GetLastCommandError = {0}", SMC100PP.GetLastCommandError())
        Console.WriteLine("GetSetPointPosition = {0}", SMC100PP.GetSetPointPosition())
        Console.WriteLine("GetCurrentPosition = {0}", SMC100PP.GetCurrentPosition())
        Console.WriteLine("GetPositionerErrorAndControllerState = {0}", SMC100PP.GetPositionerErrorAndControllerState())
        Console.WriteLine("GetVelocity = {0}", SMC100PP.GetVelocity())
        Console.WriteLine("GetAllAxisParameters = {0}", SMC100PP.GetAllAxisParameters())

        REM Leave CONFIGURATION state (Change to NOT REFERENCED state)
        Console.WriteLine("Leave configuration state")
        SMC100PP.LeaveConfigurationState()

        Console.WriteLine("Wait 3s to have controller returns to NOT REFERENCED state.")
        System.Threading.Thread.Sleep(3000)

        REM Change to READY state.
        Console.WriteLine("Homing (go to READY state)")
        SMC100PP.ExecuteHomeSearch()

        REM velocity defines how fast motor can move
        Dim velocity = 0.5

        Console.WriteLine("Change velocity = {0}", velocity)
        SMC100PP.SetVelocity(velocity)

        REM Get estimated time to move to position 2
        Dim time = SMC100PP.GetMotionTime(2)
        Console.WriteLine("It'll take {0}s to move to relative position 2 @ velocity {1}", time, velocity)

        REM Move to relative position 2. 
        Console.WriteLine("Moving to relative position 2")
        SMC100PP.SetRelativePosition(2)

        Console.WriteLine("Waiting ...")
        System.Threading.Thread.Sleep((time * 1000) + 1000)

        velocity = 5
        Console.WriteLine("Change velocity = {0}", velocity)
        SMC100PP.SetVelocity(velocity)

        time = SMC100PP.GetMotionTime(2)
        Console.WriteLine("It'll take {0}s to move to relative position -2 @ velocity {1}", time, velocity)

        REM Move to relative position -2
        Console.WriteLine("Moving to relative position -2")
        SMC100PP.SetRelativePosition(-2)

        Console.WriteLine("Waiting ...")
        System.Threading.Thread.Sleep((time * 1000) + 1000)

        SMC100PP.Disconnect()

    End Sub

End Module
