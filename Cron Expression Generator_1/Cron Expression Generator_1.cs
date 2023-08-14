namespace CronExpression
{

    /*
    ****************************************************************************
    *  Copyright (c) 2023,  Skyline Communications NV  All Rights Reserved.    *
    ****************************************************************************

    By using this script, you expressly agree with the usage terms and
    conditions set out below.
    This script and all related materials are protected by copyrights and
    other intellectual property rights that exclusively belong
    to Skyline Communications.

    A user license granted for this script is strictly for personal use only.
    This script may not be used in any way by anyone without the prior
    written consent of Skyline Communications. Any sublicensing of this
    script is forbidden.

    Any modifications to this script by the user are only allowed for
    personal use and within the intended purpose of the script,
    and will remain the sole responsibility of the user.
    Skyline Communications will not be responsible for any damages or
    malfunctions whatsoever of the script resulting from a modification
    or adaptation by the user.

    The content of this script is confidential information.
    The user hereby agrees to keep this confidential information strictly
    secret and confidential and not to disclose or reveal it, in whole
    or in part, directly or indirectly to any person, entity, organization
    or administration without the prior written consent of
    Skyline Communications.

    Any inquiries can be addressed to:

        Skyline Communications NV
        Ambachtenstraat 33
        B-8870 Izegem
        Belgium
        Tel.	: +32 51 31 35 69
        Fax.	: +32 51 31 01 29
        E-mail	: info@skyline.be
        Web		: www.skyline.be
        Contact	: Ben Vandenberghe

    ****************************************************************************
    Revision History:

    DATE		VERSION		AUTHOR			COMMENTS

    07/08/2023	1.0.0.1		JCL, Skyline	Initial version
    ****************************************************************************
    */
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Cron_Expression_Generator_1.Controllers;
    using Cron_Expression_Generator_1.Views;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class Script
    {
        /// <summary>
        /// Represents a DataMiner Automation script.
        /// </summary>

        private InteractiveController controller;

        /// <summary>
        /// The script entry point.
        /// </summary>
        /// <param name="engine">Link with SLAutomation process.</param>
        public void Run(Engine engine)
        {
            // engine.ShowUI();
            engine.FindInteractiveClient("Launching Cron Expression Generator", 100, "user:" + engine.UserLoginName, AutomationScriptAttachOptions.AttachImmediately);
            controller = new InteractiveController(engine);
            engine.Timeout = new TimeSpan(1, 0, 0);

            try
            {
                ConfigureCronView configureCronView = new ConfigureCronView(engine);
                ConfigureCronController configureCronController = new ConfigureCronController(engine, configureCronView);

                configureCronController.Next += (sender, args) =>
                {
                    engine.ExitSuccess("Installation Completed.");
                };

                controller.Run(configureCronView);
            }
            catch (ScriptAbortException ex)
            {
                if (ex.Message.Contains("ExitFail"))
                {
                    HandleknownException(engine, ex);
                }
                else
                {
                    // Do nothing as it's an exitsuccess event
                }
            }
            catch (Exception ex)
            {
                HandleUnknownException(engine, ex);
            }
            finally
            {
                engine.AddScriptOutput("status", "success");
            }
        }

        private void HandleUnknownException(Engine engine, Exception ex)
        {
            var message = "ERR| An unexpected error occurred, please contact skyline and provide the following information: \n" + ex;
            try
            {
                controller.Run(new ErrorView(engine, ex));
            }
            catch (Exception ex_two)
            {
                engine.GenerateInformation("ERR| Unable to show error message window: " + ex_two);
            }

            engine.GenerateInformation(message);
        }

        private void HandleknownException(Engine engine, Exception ex)
        {
            var message = "ERR| Script has been canceled because of the following error: \n" + ex;
            try
            {
                controller.Run(new ErrorView(engine, ex));
            }
            catch (Exception ex_two)
            {
                engine.GenerateInformation("ERR| Unable to show error message window: " + ex_two);
            }

            engine.GenerateInformation(message);
        }
    }
}