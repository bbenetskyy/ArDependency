﻿using DataSaver;
using DataSaver.Models;
using DevExpress.XtraEditors.Controls;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using ToolsPortable;

namespace DXApplication1.Pages
{
    public partial class FilterPage : Form
    {
        public Filter Filter { get; private set; }

        public LocalSaver LocalSaver { get; private set; }

        private string _userId;

        public bool ToClose { get; set; }

        public FilterPage()
        {
            InitializeComponent();
        }

        public FilterPage(Filter filter)
                : this()
        {
            Filter = filter;
            LocalSaver = new LocalSaver();

            //New Requirements: parser for this two site only
            Filter.MarathonBet = true;
            Filter.PinnacleSports = true;

            FirstBind();
            UserBind();
            InitializeEvents();
        }

        private void UserBind()
        {
            var user = LocalSaver.FindUser();
            textEditLoginPinnacle.Text = user?.LoginPinnacle;
            textEditPasswordPinnacle.Text = user?.PasswordPinnacle;
            textEditLoginMarathon.Text = user?.LoginMarathon;
            textEditPasswordMarathon.Text = user?.PasswordMarathon;
            textEditAntiGateCode.Text = user?.AntiGateCode;
            _userId = user?.Id;
        }

        protected void FirstBind()
        {
            lock (Filter)
            {
                minTextEdit.EditValue = Filter.Min;
                maxTextEdit.EditValue = Filter.Max;
                marathonBetToggleSwitch.EditValue = Filter.MarathonBet;
                pinnacleSportsToggleSwitch.EditValue = Filter.PinnacleSports;
                footballToggleSwitch.EditValue = Filter.Football;
                basketballToggleSwitch.EditValue = Filter.Basketball;
                volleyballToggleSwitch.EditValue = Filter.Volleyball;
                hockeyToggleSwitch.EditValue = Filter.Hockey;
                tennisToggleSwitch.EditValue = Filter.Tennis;
                fasterDateTimePicker.EditValue = Filter.FaterThen;
                longerDateTimePicker.EditValue = Filter.LongerThen;
                textEditAutoUpdate.EditValue = Filter.AutoUpdateTime;
                textEditRate.EditValue = Filter.DefaultRate;
            }
        }

        public void InitializeEvents()
        {
            Closing += FilterPage_Closing;
            fasterDateTimePicker.EditValueChanging += Faster_Changing;
            maxTextEdit.EditValueChanging += Max_Changing;
            minTextEdit.EditValueChanging += Min_Changing;
            pinnacleSportsToggleSwitch.Toggled += PinnacleSports_Toggled;
            marathonBetToggleSwitch.Toggled += MarathonBet_Toggled;
            basketballToggleSwitch.Toggled += Basketball_Toggled;
            footballToggleSwitch.Toggled += Football_Toggled;
            longerDateTimePicker.EditValueChanging += Later_Changing;
            volleyballToggleSwitch.Toggled += Volleyball_Toggled;
            tennisToggleSwitch.Toggled += Tennis_Toggled;
            hockeyToggleSwitch.Toggled += Hockey_Toggled;
            textEditAutoUpdate.EditValueChanging += TextEditAutoUpdate_EditValueChanging;
            textEditAutoUpdate.EditValueChanging += TextEditAutoUpdate_EditValueChanging;
            textEditAutoUpdate.EditValueChanging += TextEditAutoUpdate_EditValueChanging;
            textEditLoginPinnacle.EditValueChanged += User_EditValueChanged;
            textEditPasswordPinnacle.EditValueChanged += User_EditValueChanged;
            textEditLoginMarathon.EditValueChanged += User_EditValueChanged;
            textEditPasswordMarathon.EditValueChanged += User_EditValueChanged;
            textEditAntiGateCode.EditValueChanged += User_EditValueChanged;
            textEditRate.EditValueChanging += SpinEditRate_EditValueChanging;
        }

        private void User_EditValueChanged(object sender, EventArgs e)
        {
            var user = new User
            {
                Id = _userId,
                LoginPinnacle = textEditLoginPinnacle.Text,
                PasswordPinnacle = textEditPasswordPinnacle.Text,
                LoginMarathon = textEditLoginMarathon.Text,
                PasswordMarathon = textEditPasswordMarathon.Text,
                AntiGateCode = textEditAntiGateCode.Text
            };
            LocalSaver.UpdateUser(user);
        }

        public void DeInitializeEvents()
        {
            Closing -= FilterPage_Closing;
            fasterDateTimePicker.EditValueChanging -= Faster_Changing;
            maxTextEdit.EditValueChanging -= Max_Changing;
            minTextEdit.EditValueChanging -= Min_Changing;
            pinnacleSportsToggleSwitch.Toggled -= PinnacleSports_Toggled;
            marathonBetToggleSwitch.Toggled -= MarathonBet_Toggled;
            basketballToggleSwitch.Toggled -= Basketball_Toggled;
            footballToggleSwitch.Toggled -= Football_Toggled;
            longerDateTimePicker.EditValueChanging -= Later_Changing;
            volleyballToggleSwitch.Toggled -= Volleyball_Toggled;
            tennisToggleSwitch.Toggled -= Tennis_Toggled;
            hockeyToggleSwitch.Toggled -= Hockey_Toggled;
            textEditLoginPinnacle.EditValueChanged -= User_EditValueChanged;
            textEditPasswordPinnacle.EditValueChanged -= User_EditValueChanged;
            textEditLoginMarathon.EditValueChanged += User_EditValueChanged;
            textEditPasswordMarathon.EditValueChanged += User_EditValueChanged;
            textEditAntiGateCode.EditValueChanged += User_EditValueChanged;
            textEditRate.EditValueChanging -= SpinEditRate_EditValueChanging;
        }

        private void SpinEditRate_EditValueChanging(object sender, ChangingEventArgs e)
        {
            lock (Filter)
            {
                Filter.DefaultRate = e.NewValue.ConvertToIntOrNull();
            }
        }

        private void TextEditAutoUpdate_EditValueChanging(object sender, ChangingEventArgs e)
        {
            lock (Filter)
            {
                Filter.AutoUpdateTime = e.NewValue.ConvertToIntOrNull();
            }
        }

        private void FilterPage_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !ToClose;
            if (!ToClose)
                Hide();
        }

        private void Min_Changing(object sender, ChangingEventArgs e)
        {
            lock (Filter)
            {
                Filter.Min = e.NewValue.ConvertToIntOrNull();
            }
        }

        private void Max_Changing(object sender, ChangingEventArgs e)
        {
            lock (Filter)
            {
                Filter.Max = e.NewValue.ConvertToIntOrNull();
            }
        }

        private void MarathonBet_Toggled(object sender, EventArgs e)
        {
            lock (Filter)
            {
                Filter.MarathonBet = marathonBetToggleSwitch.EditValue.ConvertToBool();
            }
        }

        private void PinnacleSports_Toggled(object sender, EventArgs e)
        {
            lock (Filter)
            {
                Filter.PinnacleSports = pinnacleSportsToggleSwitch.EditValue.ConvertToBool();
            }
        }

        private void Football_Toggled(object sender, EventArgs e)
        {
            lock (Filter)
            {
                Filter.Football = footballToggleSwitch.EditValue.ConvertToBool();
            }
        }

        private void Basketball_Toggled(object sender, EventArgs e)
        {
            lock (Filter)
            {
                Filter.Basketball = basketballToggleSwitch.EditValue.ConvertToBool();
            }
        }

        private void Volleyball_Toggled(object sender, EventArgs e)
        {
            lock (Filter)
            {
                Filter.Volleyball = volleyballToggleSwitch.EditValue.ConvertToBool();
            }
        }

        private void Hockey_Toggled(object sender, EventArgs e)
        {
            lock (Filter)
            {
                Filter.Hockey = hockeyToggleSwitch.EditValue.ConvertToBool();
            }
        }

        private void Tennis_Toggled(object sender, EventArgs e)
        {
            lock (Filter)
            {
                Filter.Tennis = tennisToggleSwitch.EditValue.ConvertToBool();
            }
        }

        private void Faster_Changing(object sender, ChangingEventArgs e)
        {
            lock (Filter)
            {
                DateTime dateValue;
                if (e.NewValue == null)
                    Filter.FaterThen = null;
                else if (DateTime.TryParse(e.NewValue?.ToString(), out dateValue))
                {
                    Filter.FaterThen = dateValue;
                }
            }
        }

        private void Later_Changing(object sender, ChangingEventArgs e)
        {
            lock (Filter)
            {
                DateTime dateValue;
                if (e.NewValue == null)
                    Filter.LongerThen = null;
                else if (DateTime.TryParse(e.NewValue?.ToString(), out dateValue))
                {
                    Filter.LongerThen = dateValue;
                }
            }
        }

        private void OutCome2_Toggled(object sender, EventArgs e)
        {
            lock (Filter)
            {
                Filter.OutCome2 = !Filter.OutCome2;
            }
        }

        private void OutCome3_Toggled(object sender, EventArgs e)
        {
            lock (Filter)
            {
                Filter.OutCome3 = !Filter.OutCome3;
            }
        }
    }
}