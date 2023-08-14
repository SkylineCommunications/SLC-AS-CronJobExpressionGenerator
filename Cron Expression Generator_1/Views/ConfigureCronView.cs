namespace Cron_Expression_Generator_1.Views
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class ConfigureCronView : Dialog
    {
        public ConfigureCronView(Engine engine) : base(engine)
        {
            this.AllowOverlappingWidgets = true;
            this.Title = "Generate Cron Expression";
            this.NextButton = new Button("Finish");
            this.GenerateButton = new Button("Generate Cron");
            this.CronExpression = new TextBox("* * * * *");
            CronExpression.Width = 300;
            this.CronFeedback = new Label("Configure settings and press Generate Cron");
            CronFeedback.Width = 400;

            this.DaysCheckAllButton = new Button("Check All");
            this.DaysUncheckAllButton = new Button("Unheck All");

            this.MonthsCheckAllButton = new Button("Check All");
            this.MonthsUncheckAllButton = new Button("Uncheck All");

            this.WeekCheckAllButton = new Button("Check All");
            this.WeekUncheckAllButton = new Button("Uncheck All");

            this.MinsSection = GetMinsSection();
            this.HoursSection = GetHoursSection();
            this.DaysSection = GetDaysSection();
            this.MonthsSection = GetMonthsSection();
            this.WeeksSection = GetWeeksSection();
        }

        public TextBox CronExpression { get; set; }

        public Label CronFeedback { get; set; }

        public Button NextButton { get; set; }

        public Button GenerateButton { get; set; }

        public Button DaysCheckAllButton { get; set; }

        public Button DaysUncheckAllButton { get; set; }

        public Button MonthsCheckAllButton { get; set; }

        public Button MonthsUncheckAllButton { get; set; }

        public Button WeekCheckAllButton { get; set; }

        public Button WeekUncheckAllButton { get; set; }

        public Calendar DayCalendar { get; set; }

        public Calendar MonthCalendar { get; set; }

        public Calendar WeekCalendar { get; set; }

        public Numeric MinsStart { get; set; }

        public Numeric MinsEnd { get; set; }

        public Numeric MinsInterval { get; set; }

        public CheckBox MinsIntervalCheckBox { get; set; }

        public Numeric HoursStart { get; set; }

        public Numeric HoursEnd { get; set; }

        public Numeric HoursInterval { get; set; }

        public CheckBox HoursIntervalCheckBox { get; set; }

        public Numeric DaysInterval { get; set; }

        public CheckBox DaysIntervalCheckBox { get; set; }

        public Numeric MonthsInterval { get; set; }

        public CheckBox MonthsIntervalCheckBox { get; set; }

        private Section MinsSection { get; set; }

        private Section HoursSection { get; set; }

        private Section DaysSection { get; set; }

        private Section MonthsSection { get; set; }

        private Section WeeksSection { get; set; }

        public static IEnumerable<int> Range(int min, int max, int step)
        {
            for (int i = min; i <= max; i = checked(i + step))
                yield return i;
        }

        internal void Initialize()
        {
            this.Clear();
            this.NextButton.IsEnabled = true;
            int row = 0;
            int secondRow = 0;
            int firstColumn = 0;
            int secondColumn = 5;

            // First Column
            this.AddSection(MinsSection, row, firstColumn);
            row += MinsSection.RowCount;

            this.AddSection(HoursSection, row, firstColumn);
            row += HoursSection.RowCount;

            this.AddSection(DaysSection, row, firstColumn);
            row += DaysSection.RowCount;

            // Second Column
            this.AddSection(MonthsSection, secondRow, secondColumn);
            secondRow += MonthsSection.RowCount;

            this.AddSection(WeeksSection, secondRow, secondColumn);
            secondRow += WeeksSection.RowCount;

            var cronExpressionLabel = new Label("Cron Expression");
            cronExpressionLabel.Style = TextStyle.Bold;
            this.AddWidget(cronExpressionLabel, secondRow++, secondColumn);
            this.AddWidget(CronExpression, secondRow++, secondColumn, 1, 3);

            var cronFeedbackLabel = new Label("Cron Feedback");
            cronFeedbackLabel.Style = TextStyle.Bold;
            this.AddWidget(cronFeedbackLabel, secondRow++, secondColumn);
            this.AddWidget(CronFeedback, secondRow++, secondColumn, 1, 3);
            this.AddWidget(this.GenerateButton, secondRow++, 7);

            this.AddWidget(this.NextButton, secondRow, 7);

            this.SetColumnWidth(0, 130);
            this.SetColumnWidth(1, 110);
            this.SetColumnWidth(2, 110);
            this.SetColumnWidth(3, 110);
            this.SetColumnWidth(4, 80);
            this.SetColumnWidth(5, 130);
            this.SetColumnWidth(6, 110);
            this.SetColumnWidth(7, 110);
            this.SetColumnWidth(8, 110);
            this.Width = 950;
            this.Height = 900;
        }

        private Section GetMinsSection()
        {
            var section = new Section();
            int row = 0;

            var minsHeader = new Label("Configure Mins");
            minsHeader.Style = TextStyle.Bold;
            section.AddWidget(minsHeader, row++, 0, 1, 3);

            this.MinsStart = GetNumeric(0, 59, 0);
            this.MinsEnd = GetNumeric(0, 59, 60);
            this.MinsInterval = GetNumeric(1, 59, 0);

            section.AddWidget(new Label("Starting from which min?"), row++, 0, 1, 3);
            section.AddWidget(this.MinsStart, row++, 0, 1, 3);
            this.MinsStart.Width = 300;

            section.AddWidget(new Label("Ending at which min?"), row++, 0, 1, 3);
            section.AddWidget(this.MinsEnd, row++, 0, 1, 3);
            this.MinsEnd.Width = 300;

            section.AddWidget(new Label("How many mins between each job?"), row++, 0, 1, 3);
            section.AddWidget(this.MinsInterval, row, 1, 1, 2);
            this.MinsInterval.IsEnabled = false;

            this.MinsIntervalCheckBox = new CheckBox();
            section.AddWidget(this.MinsIntervalCheckBox, row++, 0, 1, 1);

            this.MinsIntervalCheckBox.Changed += MinsIntervalCheckBox_Changed;

            return section;
        }

        private void MinsIntervalCheckBox_Changed(object sender, CheckBox.CheckBoxChangedEventArgs e)
        {
            if (this.MinsIntervalCheckBox.IsChecked)
            {
                this.MinsInterval.IsEnabled = true;
            }
            else
            {
                this.MinsInterval.IsEnabled = false;
            }
        }

        private Section GetHoursSection()
        {
            var section = new Section();
            int row = 0;

            var hoursHeader = new Label("Configure Hours");
            hoursHeader.Style = TextStyle.Bold;
            section.AddWidget(hoursHeader, row++, 0, 1, 3);

            this.HoursStart = GetNumeric(0, 23, 0);
            this.HoursEnd = GetNumeric(0, 23, 23);
            this.HoursInterval = GetNumeric(1, 23, 0);

            section.AddWidget(new Label("Starting from which hour?"), row++, 0, 1, 3);
            section.AddWidget(this.HoursStart, row++, 0, 1, 3);
            this.HoursStart.Width = 300;

            section.AddWidget(new Label("Ending at which hour?"), row++, 0, 1, 3);
            section.AddWidget(this.HoursEnd, row++, 0, 1, 3);
            this.HoursEnd.Width = 300;

            section.AddWidget(new Label("How many hours between each job? (Optional)"), row++, 0, 1, 3);
            section.AddWidget(this.HoursInterval, row, 1, 1, 2);
            this.HoursInterval.IsEnabled = false;

            this.HoursIntervalCheckBox = new CheckBox();
            section.AddWidget(this.HoursIntervalCheckBox, row++, 0, 1, 1);
            this.HoursIntervalCheckBox.Changed += HoursIntervalCheckBox_Changed;

            return section;
        }

        private void HoursIntervalCheckBox_Changed(object sender, CheckBox.CheckBoxChangedEventArgs e)
        {
            if (this.HoursIntervalCheckBox.IsChecked)
            {
                this.HoursInterval.IsEnabled = true;
            }
            else
            {
                this.HoursInterval.IsEnabled = false;
            }
        }

        private Section GetDaysSection()
        {
            var section = new Section();
            int row = 0;

            var daysHeader = new Label("Configure Days");
            daysHeader.Style = TextStyle.Bold;
            section.AddWidget(daysHeader, row++, 0, 1, 3);

            this.DaysInterval = GetNumeric(1, 31, 0);

            section.AddWidget(new Label("Select which days of the month to execute the job:"), row++, 0, 1, 3);
            this.DayCalendar = new Calendar("day", 4);
            section.AddSection(DayCalendar.CalendarWidget, new SectionLayout(row, 0));
            row += DayCalendar.CalendarWidget.RowCount;

            section.AddWidget(DaysCheckAllButton, row, 1, 1, 1);
            section.AddWidget(DaysUncheckAllButton, row++, 2, 1, 1);
            DaysCheckAllButton.Pressed += DaysCheckAllButton_Pressed;
            DaysUncheckAllButton.Pressed += DaysUncheckAllButton_Pressed;

            section.AddWidget(new Label("How many days between each job? (Either select discrete days or interval)"), row++, 0, 1, 3);
            section.AddWidget(this.DaysInterval, row, 1, 1, 2);
            this.DaysInterval.IsEnabled = false;

            this.DaysIntervalCheckBox = new CheckBox();
            section.AddWidget(this.DaysIntervalCheckBox, row++, 0, 1, 1);
            this.DaysIntervalCheckBox.Changed += DaysIntervalCheckBox_Changed;

            return section;
        }

        private void DaysUncheckAllButton_Pressed(object sender, EventArgs e)
        {
            foreach (CheckBoxList checkboxList in DayCalendar.CalendarWidget.Widgets)
            {
                checkboxList.UncheckAll();
            }
        }

        private void DaysCheckAllButton_Pressed(object sender, EventArgs e)
        {
            foreach (CheckBoxList checkboxList in DayCalendar.CalendarWidget.Widgets)
            {
                checkboxList.CheckAll();
            }
        }

        private void DaysIntervalCheckBox_Changed(object sender, CheckBox.CheckBoxChangedEventArgs e)
        {
            if (this.DaysIntervalCheckBox.IsChecked)
            {
                this.DaysInterval.IsEnabled = true;
                this.DayCalendar.CalendarWidget.IsEnabled = false;
                this.DaysCheckAllButton.IsEnabled = false;
                this.DaysUncheckAllButton.IsEnabled = false;
            }
            else
            {
                this.DaysInterval.IsEnabled = false;
                this.DayCalendar.CalendarWidget.IsEnabled = true;
                this.DaysCheckAllButton.IsEnabled = true;
                this.DaysUncheckAllButton.IsEnabled = true;
            }
        }

        private Section GetMonthsSection()
        {
            var section = new Section();
            int row = 0;

            var monthsHeader = new Label("Configure Months");
            monthsHeader.Style = TextStyle.Bold;
            section.AddWidget(monthsHeader, row++, 0, 1, 3);

            this.MonthsInterval = GetNumeric(1, 12, 0);

            section.AddWidget(new Label("Select which months to execute the job:"), row++, 0, 1, 3);
            this.MonthCalendar = new Calendar("month", 4);
            section.AddSection(MonthCalendar.CalendarWidget, new SectionLayout(row, 0));
            row += MonthCalendar.CalendarWidget.RowCount;

            section.AddWidget(MonthsCheckAllButton, row, 1, 1, 1);
            section.AddWidget(MonthsUncheckAllButton, row++, 2, 1, 1);
            MonthsCheckAllButton.Pressed += MonthsCheckAllButton_Pressed;
            MonthsUncheckAllButton.Pressed += MonthsUncheckAllButton_Pressed;

            section.AddWidget(new Label("How many months between each job? (Either select discrete months or interval)"), row++, 0, 1, 3);
            section.AddWidget(this.MonthsInterval, row, 1, 1, 2);
            this.MonthsInterval.IsEnabled = false;

            this.MonthsIntervalCheckBox = new CheckBox();
            section.AddWidget(this.MonthsIntervalCheckBox, row++, 0, 1, 1);
            this.MonthsIntervalCheckBox.Changed += MonthsIntervalCheckBox_Changed;

            return section;
        }

        private void MonthsUncheckAllButton_Pressed(object sender, EventArgs e)
        {
            foreach (CheckBoxList checkboxList in MonthCalendar.CalendarWidget.Widgets)
            {
                checkboxList.UncheckAll();
            }
        }

        private void MonthsCheckAllButton_Pressed(object sender, EventArgs e)
        {
            foreach (CheckBoxList checkboxList in MonthCalendar.CalendarWidget.Widgets)
            {
                checkboxList.CheckAll();
            }
        }

        private void MonthsIntervalCheckBox_Changed(object sender, CheckBox.CheckBoxChangedEventArgs e)
        {
            if (this.MonthsIntervalCheckBox.IsChecked)
            {
                this.MonthsInterval.IsEnabled = true;
                this.MonthCalendar.CalendarWidget.IsEnabled = false;
                this.MonthsCheckAllButton.IsEnabled = false;
                this.MonthsUncheckAllButton.IsEnabled = false;
            }
            else
            {
                this.MonthsInterval.IsEnabled = false;
                this.MonthCalendar.CalendarWidget.IsEnabled = true;
                this.MonthsCheckAllButton.IsEnabled = true;
                this.MonthsUncheckAllButton.IsEnabled = true;
            }
        }

        private Section GetWeeksSection()
        {
            var section = new Section();
            int row = 0;

            var weeksHeader = new Label("Configure Days of the Week");
            weeksHeader.Style = TextStyle.Bold;
            section.AddWidget(weeksHeader, row++, 0, 1, 3);

            section.AddWidget(new Label("Select which days of the week to execute the job:"), row++, 0, 1, 3);
            this.WeekCalendar = new Calendar("week", 4);
            section.AddSection(WeekCalendar.CalendarWidget, new SectionLayout(row, 0));
            row += WeekCalendar.CalendarWidget.RowCount;

            section.AddWidget(WeekCheckAllButton, row, 1, 1, 1);
            section.AddWidget(WeekUncheckAllButton, row++, 2, 1, 1);
            WeekCheckAllButton.Pressed += WeekCheckAllButton_Pressed;
            WeekUncheckAllButton.Pressed += WeekUncheckAllButton_Pressed;

            return section;
        }

        private void WeekUncheckAllButton_Pressed(object sender, EventArgs e)
        {
            foreach (CheckBoxList checkboxList in WeekCalendar.CalendarWidget.Widgets)
            {
                checkboxList.UncheckAll();
            }
        }

        private void WeekCheckAllButton_Pressed(object sender, EventArgs e)
        {
            foreach (CheckBoxList checkboxList in WeekCalendar.CalendarWidget.Widgets)
            {
                checkboxList.CheckAll();
            }
        }

        private Numeric GetNumeric(int min, int max, int init)
        {
            var numeric = new Numeric(init);

            numeric.Maximum = max;
            numeric.Minimum = min;

            return numeric;
        }

        public class Calendar
        {
            private int _width;
            private int _height;
            private string _type;
            private Section _calendarSection;

            public Calendar(string type,int width)
            {
                _width = width;
                _height = (31 / _width) + 1;
                _type = type;

                switch (_type)
                {
                    default:
                    case "day":
                        _calendarSection = GetDayCalendar();
                        break;

                    case "month":
                        _calendarSection = GetMonthCalendar();
                        break;

                    case "week":
                        _calendarSection = GetWeekCalendar();
                        break;
                }
            }

            public string GetCalendarType
            {
                get { return _type; }
            }

            public Section CalendarWidget
            {
				get
                {
                    return _calendarSection;
                }
			}

            public List<int> GetChecked()
            {
                switch (_type)
                {
                    default:
                    case "day":
                        return GetDayChecked();

                    case "month":
                        return GetMonthChecked();

                    case "week":
                        return GetWeekChecked();
                }
            }

            private Section GetDayCalendar()
            {
                var section = new Section();

                for (int i = 1; i <= _width; i++)
                {
                    section.AddWidget(new CheckBoxList(Range(i, 31, _width).Select(x => x.ToString())), 0, i - 1);
                }

                return section;
            }

            private Section GetMonthCalendar()
            {
                var section = new Section();
                var monthDict = new Dictionary<int, string>()
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

                for (int i = 1; i <= _width; i++)
                {
                    section.AddWidget(new CheckBoxList(Range(i, 12, _width).Select(x => monthDict[x])), 0, i - 1);
                }

                return section;
            }

            private Section GetWeekCalendar()
            {
                var section = new Section();
                var weekDict = new Dictionary<int, string>()
                {
                    {1, "mon"},
                    {2, "tue"},
                    {3, "wed"},
                    {4, "thu"},
                    {5, "fri"},
                    {6, "sat"},
                    {7, "sun"},
                };

                for (int i = 1; i <= _width; i++)
                {
                    section.AddWidget(new CheckBoxList(Range(i, 7, _width).Select(x => weekDict[x])), 0, i - 1);
                }

                return section;
            }

            private List<int> GetDayChecked()
            {
                var list = new List<int>();

                foreach (var widget in _calendarSection.Widgets)
                {
                    list.AddRange(((CheckBoxList) widget).Checked.Select(x => Int32.Parse(x)));
                }

                list.Sort();

                return list;
            }

            private List<int> GetMonthChecked()
            {
                var list = new List<int>();
                var monthString2Int = new Dictionary<string, int>()
                {
                    {"jan", 1},
                    {"feb", 2},
                    {"mar" , 3},
                    {"apr" , 4},
                    {"may" , 5},
                    {"jun" , 6},
                    {"jul" , 7},
                    {"aug" , 8},
                    {"sep" , 9},
                    {"oct" , 10},
                    {"nov" , 11},
                    {"dec" , 12},
                };

                foreach (var widget in _calendarSection.Widgets)
                {
                    list.AddRange(((CheckBoxList)widget).Checked.Select(x => monthString2Int[x]));
                }

                list.Sort();

                return list;
            }

            private List<int> GetWeekChecked()
            {
                var list = new List<int>();
                var weekString2Int = new Dictionary<string, int>()
                {
                    {"mon", 1},
                    {"tue", 2},
                    {"wed" , 3},
                    {"thu" , 4},
                    {"fri" , 5},
                    {"sat" , 6},
                    {"sun" , 7},
                };

                foreach (var widget in _calendarSection.Widgets)
                {
                    list.AddRange(((CheckBoxList)widget).Checked.Select(x => weekString2Int[x]));
                }

                list.Sort();

                return list;
            }
        }
    }
}