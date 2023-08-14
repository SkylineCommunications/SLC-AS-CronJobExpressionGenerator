namespace Cron_Expression_Generator_1.Controllers
{
    using Cron_Expression_Generator_1.Views;
    using System;
    using Skyline.DataMiner.Automation;
    using System.Linq;
    using System.Collections.Generic;

    public class ConfigureCronController
    {
        private readonly ConfigureCronView view;
        private int minsStart;
        private int minsEnd;
        private int minsInterval;
        private bool minsIntervalCheck;
        private int hoursStart;
        private int hoursEnd;
        private int hoursInterval;
        private bool hoursIntervalCheck;
        private string daysCalendarType;
        private List<int> daysCalendarChecked;
        private int daysInterval;
        private bool daysIntervalCheck;
        private string monthsCalendarType;
        private List<int> monthsCalendarChecked;
        private int monthsInterval;
        private bool monthsIntervalCheck;
        private string weekCalendarType;
        private List<int> weekCalendarCheck;
        private CronStruct completedCron;

        private Dictionary<int, string> monthInt2String = new Dictionary<int, string>()
        {
            {1, "jan"},
            {2, "feb"},
            {3, "mar"},
            {4, "apr"},
            {5, "may"},
            {6, "jun"},
            {7, "jul"},
            {8, "aug"},
            {9, "sep"},
            {10, "oct"},
            {11, "nov"},
            {12, "dec"},
        };

        private Dictionary<int, string> weekInt2String = new Dictionary<int, string>()
        {
            {1, "mon"},
            {2, "tue"},
            {3, "wed"},
            {4, "thu"},
            {5, "fri"},
            {6, "sat"},
            {7, "sun"},
        };

        public ConfigureCronController(Engine engine, ConfigureCronView configureCronView)
        {
            this.view = configureCronView;
            this.Engine = engine;

            this.InitializeView();

            view.NextButton.Pressed += OnNextButtonPressed;
            view.GenerateButton.Pressed += OnGenerateButtonPressed;
        }

        internal event EventHandler<EventArgs> Next;

        public Engine Engine { get; set; }

        internal void InitializeView()
        {
            this.view.Initialize();
        }

        private void OnNextButtonPressed(object sender, EventArgs e)
        {
            if (completedCron.Valid)
            {
                Engine.AddScriptOutput("cron", view.CronExpression.Text);
            }
            Next?.Invoke(this, EventArgs.Empty);
        }

        private void OnGenerateButtonPressed(object sender, EventArgs e)
        {
            SetVariables();
            completedCron = GetCron();

            view.CronExpression.Text = !completedCron.Valid ? "invalid configuration" : completedCron.Expression;
            view.CronFeedback.Text = completedCron.Feedback;
        }

        private void SetVariables()
        {
            minsStart = (int) view.MinsStart.Value;
            minsEnd = (int)view.MinsEnd.Value;
            minsInterval = (int)view.MinsInterval.Value;
            minsIntervalCheck = view.MinsIntervalCheckBox.IsChecked;

            hoursStart = (int)view.HoursStart.Value;
            hoursEnd = (int)view.HoursEnd.Value;
            hoursInterval = (int)view.HoursInterval.Value;
            hoursIntervalCheck = view.HoursIntervalCheckBox.IsChecked;

            daysCalendarType = view.DayCalendar.GetCalendarType;
            daysCalendarChecked = view.DayCalendar.GetChecked();
            daysInterval = (int)view.DaysInterval.Value;
            daysIntervalCheck = view.DaysIntervalCheckBox.IsChecked;

            monthsCalendarType = view.MonthCalendar.GetCalendarType;
            monthsCalendarChecked = view.MonthCalendar.GetChecked();
            monthsInterval = (int)view.MonthsInterval.Value;
            monthsIntervalCheck = view.MonthsIntervalCheckBox.IsChecked;

            weekCalendarType = view.WeekCalendar.GetCalendarType;
            weekCalendarCheck = view.WeekCalendar.GetChecked();
        }

        private CronStruct GetCron()
        {
            var minsExpression = GetMinsExpression();
            var hoursExpression = GetHoursExpression();
            var daysExpression = GetDaysExpression();
            var monthsExpression = GetMonthsExpression();
            var weekExpression = GetWeekExpression();

            var cronValidity = minsExpression.Valid && hoursExpression.Valid && daysExpression.Valid && monthsExpression.Valid && weekExpression.Valid;
            var cronExpression = string.Join(" ", minsExpression.Expression, hoursExpression.Expression, daysExpression.Expression, monthsExpression.Expression, weekExpression.Expression);
            var cronFeedback = string.Join(Environment.NewLine, minsExpression.Feedback, hoursExpression.Feedback, daysExpression.Feedback, monthsExpression.Feedback, weekExpression.Feedback);

            return new CronStruct {Valid = cronValidity, Expression = cronExpression, Feedback = cronFeedback };
        }

        private CronStruct GetMinsExpression()
        {
            if (minsStart > minsEnd)
            {
                return new CronStruct
                {
                    Valid = false,
                    Expression = string.Empty,
                    Feedback = "Starting Mins must be more than ending Mins.",
                };
            }

            string interval = string.Empty;
            string feedback = string.Empty;

            if (minsIntervalCheck)
            {
                interval = "/" + minsInterval.ToString();
                feedback += $"every {minsInterval} minutes, starting ";
            }

            string value;
            if (minsStart == minsEnd)
            {
                value = minsStart.ToString();
                feedback += $"at {minsStart} minutes past the hour";
            }
            else if (minsStart == 0 && minsEnd == 60)
            {
                value = "*";
                feedback = minsIntervalCheck ? $"every {minsInterval} minutes" : "every minute";
            }
            else
            {
                value = minsStart.ToString() + "-" + minsEnd.ToString();
                feedback += $"from minutes {minsStart} through {minsEnd} past the hour";
            }

            return new CronStruct { Valid = true, Expression = value + interval, Feedback = feedback };
        }

        private CronStruct GetHoursExpression()
        {
            if (hoursStart > hoursEnd)
            {
                return new CronStruct
                {
                    Valid = false,
                    Expression = string.Empty,
                    Feedback = "Starting Hours must be more than ending Hours.",
                };
            }

            string interval = string.Empty;
            string feedback = string.Empty;

            if (hoursIntervalCheck)
            {
                interval = "/" + hoursInterval.ToString();
                feedback += $"every {hoursInterval} hours, ";
            }

            string value;
            if (hoursStart == hoursEnd)
            {
                value = hoursStart.ToString();
                feedback += $"at {(hoursStart > 12? (hoursStart-12).ToString()+"PM" : hoursStart.ToString()+"AM")}";
            }
            else if (hoursStart == 0 && hoursEnd == 23)
            {
                value = "*";
                feedback = hoursIntervalCheck ? $"every {hoursInterval} hours" : "every hour";
            }
            else
            {
                value = hoursStart.ToString() + "-" + hoursEnd.ToString();
                feedback += $"hours of {(hoursStart > 12 ? (hoursStart - 12).ToString() + "PM" : hoursStart.ToString() + "AM")} through {(hoursEnd > 12 ? (hoursEnd - 12).ToString() + "PM" : hoursEnd.ToString() + "AM")}";
            }

            return new CronStruct { Valid = true, Expression = value + interval, Feedback = feedback };
        }

        private CronStruct GetDaysExpression()
        {
            if (daysIntervalCheck)
            {
                return GetDaysInterval();
            }
            else
            {
                return GetDaysValue();
            }
        }

        private CronStruct GetDaysInterval()
        {
            return new CronStruct { Valid = true, Expression = $"*/{daysInterval}", Feedback = $"every {daysInterval} days" };
        }

        private CronStruct GetDaysValue()
        {
            if (daysCalendarChecked.Count() == 0 || daysCalendarChecked.Count() == 31)
            {
                return new CronStruct { Valid = true, Expression = "*", Feedback = "every day" };
            }

            string value = GetRangeExpression(daysCalendarType, daysCalendarChecked);
            List<string> feedbackList = new List<string>();

            foreach (var dayRange in value.Split(','))
            {
                if (!dayRange.Contains('-'))
                {
                    feedbackList.Add("day " + dayRange);
                }
                else
                {
                    var daySplit = dayRange.Split('-');
                    var dayStart = daySplit[0];
                    var dayEnd = daySplit[1];
                    feedbackList.Add("between days " + dayStart + " and " + dayEnd);
                }
            }

            string feedback = string.Join(", ", feedbackList) + " of each month";

            return new CronStruct { Valid = true, Expression = value, Feedback = feedback };
        }

        private CronStruct GetMonthsExpression()
        {
            if (monthsIntervalCheck)
            {
                return GetMonthsInterval();
            }
            else
            {
                return GetMonthsValue();
            }
        }

        private CronStruct GetMonthsInterval()
        {
            return new CronStruct { Valid = true, Expression = $"*/{monthsInterval}", Feedback = $"every {monthsInterval} months" };
        }

        private CronStruct GetMonthsValue()
        {

            if (monthsCalendarChecked.Count() == 0 || monthsCalendarChecked.Count() == 12)
            {
                return new CronStruct { Valid = true, Expression = "*", Feedback = "every month" };
            }

            string value = GetRangeExpression(monthsCalendarType, monthsCalendarChecked);
            List<string> feedbackList = new List<string>();

            foreach (var monthRange in value.Split(','))
            {
                if (!monthRange.Contains('-'))
                {
                    feedbackList.Add(monthInt2String[Int32.Parse(monthRange)]);
                }
                else
                {
                    var monthSplit = monthRange.Split('-');
                    var monthStart = monthSplit[0];
                    var monthEnd = monthSplit[1];
                    feedbackList.Add("between " + monthInt2String[Int32.Parse(monthStart)] + " and " + monthInt2String[Int32.Parse(monthEnd)]);
                }
            }

            string feedback = "in the months " + string.Join(", ", feedbackList);

            return new CronStruct { Valid = true, Expression = value, Feedback = feedback };
        }

        private CronStruct GetWeekExpression()
        {
            if (weekCalendarCheck.Count() == 0 || weekCalendarCheck.Count() == 7)
            {
                return new CronStruct { Valid = true, Expression = "*", Feedback = "every day of the week" };
            }

            string value = GetRangeExpression(weekCalendarType, weekCalendarCheck);
            List<string> feedbackList = new List<string>();

            foreach (var weekRange in value.Split(','))
            {
                if (!weekRange.Contains('-'))
                {
                    feedbackList.Add(weekInt2String[Int32.Parse(weekRange)]);
                }
                else
                {
                    var weekSplit = weekRange.Split('-');
                    var weekStart = weekSplit[0];
                    var weekEnd = weekSplit[1];
                    feedbackList.Add("between " + weekInt2String[Int32.Parse(weekStart)] + " and " + weekInt2String[Int32.Parse(weekEnd)]);
                }
            }

            string feedback = string.Join(", ", feedbackList);

            return new CronStruct { Valid = true, Expression = value, Feedback = feedback };
        }

        private string ParseIntList(List<int> inputList)
        {
            List<string> result = new List<string>();
            int currentHead = -1;
            int currentTail = -1;

            foreach (var input in inputList)
            {
                if (currentHead == -1)
                {
                    currentHead = input;
                    currentTail = input;
                    continue;
                }

                if (input == currentTail + 1)
                {
                    currentTail = input;
                    continue;
                }
                else
                {
                    if (currentHead == currentTail)
                    {
                        result.Add(currentHead.ToString());
                    }
                    else
                    {
                        result.Add(currentHead.ToString() + "-" + currentTail.ToString());
                    }

                    currentHead = input;
                    currentTail = input;
                }
            }

            if (currentHead == currentTail)
            {
                result.Add(currentHead.ToString());
            }
            else
            {
                result.Add(currentHead.ToString() + "-" + currentTail.ToString());
            }

            return string.Join(",", result);
        }

        private string GetRangeExpression(string type, List<int> inputList)
        {
            if (inputList.Count == 0)
            {
                return string.Empty;
            }

            return ParseIntList(inputList);
        }

        public struct CronStruct
        {
            public bool Valid;
            public string Expression;
            public string Feedback;
        }
    }
}