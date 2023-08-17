# Cron Expression Generator

Cron is a basic utility available on Unix-based systems. 
It enables users to schedule tasks to run periodically at a specified date/time.
Cron expressions generally consist of 5 to  7 fields that describe different parts of the schedule.
The Cron Expression Generator aims to assist you in creating a Cron Expression using a straightforward interface.
More details about using the tool and cron expression can be found in this README.

## 1. Cron Expressions
The cron expressions created by this tool consist of 5 fields.
These fields are separated by whitespace can be any of the characters are described below:

|Field Position | Field | Allowed Values | Allowed Special Characters | Remarks |
|-|-------------------|:----:|---------------|-|
|1| Minutes			  | 0-59 | , - * /       | This tool supports a mix or ranges and intervals |
|2| Hours			  | 0-23 | , - * /       | This tool supports a mix or ranges and intervals |
|3| Days of the Month | 1-31 | , - * or * /  | This tool only supports either ranges OR intervals |
|4| Months of the Year| 1-12 | , - * or * /  | This tool only supports either ranges OR intervals |
|5| Days of the Week  | 1-7  | , - * or * /  | This tool only supports either ranges OR intervals |

|Special Character | Meaning | Example Usage |
|:-:|-|-|
|,| Used to separate discrete values or ranges| * * * * 1,5 to indicate Mondays and Fridays |
|-| Used to indicate a range | * * * 1-4 * to indicate January to April |
|*| Used to indicate every possible allowed value for the field | |
|/| Used to indicate intervals  | * * */2 * * to indicate every other day|

## 2. Running the script
This section will provide 2 examples of how to run the script; from another Automation Script and from a protocol.

### 2.1 From a QAction in a protocol
```cs
using System;
using Skyline.DataMiner.Net.Messages;
using Skyline.DataMiner.Scripting;
using Parameter = Skyline.DataMiner.Scripting.Parameter;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
    /// <summary>
    /// The QAction entry point.
    /// </summary>
    /// <param name="protocol">Link with SLProtocol process.</param>
    public static void Run(SLProtocolExt protocol)
    {
        try
        {
            // Create a string[] with chosen script settings
            string[] scriptOptions = { "DEFER:FALSE" }; // For e.g. DEFER:FALSE will launch the script immediately

            // Create a new ExecuteScriptMessage object to run the script from
            ExecuteScriptMessage message = new ExecuteScriptMessage
            {
                ScriptName = "Cron Expression Generator",
                Options = new SA(scriptOptions),
            };

            // Run the Script
            var res = protocol.ExecuteScript(message);
            
            // Get the final Cron expression from ScriptOutput dictionary with the key "cron"
            var output = res.ScriptOutput;
            string cron = string.Empty;
            var valid = output.TryGetValue("cron", out cron);

            // Automatically fill some parameter with the cron expression
            protocol.SetParameter(Parameter.cronexpression_1, cron);
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }
}
```

### 2.2 From another Automation Script

```cs
using Skyline.DataMiner.Automation;

/// <summary>
/// DataMiner Script Class.
/// </summary>
public class Script
{
	/// <summary>
	/// The Script entry point.
	/// </summary>
	/// <param name="engine">Link with SLAutomation process.</param>
	public void Run(Engine engine)
	{
		// Prepare a subscript
		SubScriptOptions subScript = engine.PrepareSubScript("Cron Expression Generator");

		// Launch the script
		subScript.StartScript();

		// Get the script result
		var scriptResult = subScript.GetScriptResult();
		var valid = scriptResult.TryGetValue("cron", out string cron);
	}
}
```


## 3. Using the Cron Expression Generator
This section will guide you through using the Cron Expression Generator.

### Configuring the Minutes Field
![](/Assets/ConfigureMins.jpg)
The **CONFIGURE MINS** section consists of 2 required values and 1 optional value.
The first 2 sliders are required and represent the starting and ending minutes of the schedule.
Selecting a starting value such as 15 and an ending value such as 30 would result in a *15-30* in the cron expression.
This also means *every minute between 15mins and 30mins past the hour*. 

If the same value is selected for both starting and ending values, such as 45 for both, the result would be a single *45* value.
This also means *only at 45mins past the hour*. 

The last value is optional and can be enabled by first checking the box and then selecting a value.
This value sets the interval rate.
If 2 is selected as the value, the mins field would result in **/2* which indicates *every 2 mins*.

As an example, selecting 15 as the starting mins, 30 as the ending mins and 2 as the interval would result in *15-30/2*.
This means *every 2 mins, starting from 15 to 30mins past the hr*. 

### Configuring the Hours Field
![](/Assets/ConfigureHours.jpg)
The **CONFIGURE HOURS** section is similar to the **CONFIGURE MINS** field above and consists of 2 required values and 1 optional value.
The first 2 sliders are required and represent the starting and ending hours of the schedule.
Selecting a starting value such as 10 and an ending value such as 15 would result in a *10-15* in the cron expression.
The cron expression uses a 24HR clock format also means *every hour between 10:00AM and 3:59PM*. 

If the same value is selected for both starting and ending values, such as 12 for both, the result would be a single *12* value.
This also means *only at 12:00PM*. 

The last value is optional and can be enabled by first checking the box and then selecting a value.
This value sets the interval rate.
If 2 is selected as the value, the hours field would result in **/3* which indicates *every 2 hours*.

As an example, selecting 10 as the starting mins, 15 as the ending mins and 2 as the interval would result in *10-15/2*.
This means *every 2 hours, starting from 10:00AM and ending at 3:59:PM*. 


### Configuring the Days of the Month Field
![](/Assets/ConfigureDays.jpg)
The **CONFIGURE DAYS** section provides two options; selecting the days individually or setting an interval schedule.
Selecting the days individually gives more control over which dates to run the task, e.g. selecting 15 would run the task on the 15th of every month.
Selecting an interval would give more control over the frequency of the task.
This tool does not allow combining both options, although you may write a custom cron expression without the tool and combine both options manually.

As before, check the box to use the interval option.

As an example, selecting 10,11,12 and 20 would result in the expression *10-12,15*.
This means *from 10th to 12th and on the 15th of every month*.

Selecting an interval of 3 would result in **/3* which means *every 3 days*.

### Configuring the Months of the Year Field
![](/Assets/ConfigureMonths.jpg)
The **CONFIGURE MONTHS** section is similar to the **CONFIGURE DAYS** section in that it also provides two options; selecting the months individually or setting an interval schedule.
Selecting the months individually gives more control over which months to run the task, e.g. selecting Jan and Dec would run the task only in January and Decemeber.
Selecting an interval would give more control over the frequency of the task, e.g. selecting 3 would run the task quarterly.
This tool does not allow combining both options, although you may write a custom cron expression without the tool and combine both options manually.

As before, check the box to use the interval option.

As an example, selecting Jan and Dec would result in the expression *1,12*.
This means *in the months Jan and Dec*.

Selecting an interval of 3 would result in **/3* which means *every 3 months* or quarterly frequency.

### Configuring the Days of the Week Field
![](/Assets/ConfigureWeeks.jpg)
The **CONFIGURE DAYS OF THE WEEK** section requires you to select the days in the week that the task should run.

For example, to have a task only run on the weekends, you can select sat and sun. 
This will result in an expression *6,7* that would mean *every sat and sun*.

### Generating the Cron Expression
![](/Assets/GenerateCron.jpg)
Once all fields have been configured, you may press the **Generate Cron** button.
The Cron expression corresponding to the set fields will be generated in the textbox under **CRON EXPRESSION**.
A general explanation of the cron expression will also be generated under the **CRON FEEDBACK** field.

Once satisfied, the **Finish** button can be pressed to end the script. 
The final cron expression will be saved in the script output to be used in other automation scripts or protocols.

