using Syncfusion.UI.Xaml.Scheduler;
using Syncfusion.Windows.Controls;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfScheduler.Helper
{
    /// <summary>
    /// Interaction logic for AppointmentEditor.xaml
    /// </summary>
    public partial class AppointmentEditor : Window
    {
        private SfScheduler scheduler;
        private ScheduleAppointment appointment;

        public AppointmentEditor(SfScheduler scheduler, ScheduleAppointment appointment, DateTime dateTime)
        {
            InitializeComponent();
            GetTimeZone();
            this.scheduler = scheduler;
            this.appointment = appointment;
            if (appointment != null)
            {
                this.Subject.Text = appointment.Subject;
                this.StartDatePicker.Value = appointment.StartTime.Date;
                this.EndDatePicker.Value = appointment.EndTime.Date;
                this.StartTimePicker.Value = appointment.StartTime;
                this.EndTimePicker.Value = appointment.EndTime;
                this.location.Text = appointment.Location;
                this.description.Text = appointment.Notes;
                this.allDay.IsChecked = appointment.IsAllDay;
                this.ReminderList.ItemsSource = (IList)appointment.Reminders;
                this.ReminderList.ItemContainerGenerator.StatusChanged += this.OnListViewItemGeneratorStatusChanged;
                this.timeZone.IsChecked = (appointment.StartTimeZone != null);
                if((bool)this.timeZone.IsChecked)
                {
                    this.TimeZoneMenu.Text = appointment.StartTimeZone.ToString();
                }
            }
            else
            {
                this.StartDatePicker.Value = dateTime.Date;
                this.EndDatePicker.Value = dateTime.Date;
                this.StartTimePicker.Value = dateTime;
                this.EndTimePicker.Value = dateTime.AddHours(1);
            }
        }

        private void GetTimeZone()
        {
            this.TimeZoneMenu.ItemsSource = new List<string>()
            {
            "Samoa Standard Time",
            "Dateline Standard Time",
            "UTC-11",
            "Hawaiian Standard Time",
            "Alaskan Standard Time",
            "Pacific Standard Time",
            "Pacific Standard Time (Mexico)",
            "Mountain Standard Time",
            "Mountain Standard Time (Mexico)",
            "US Mountain Standard Time",
            "Canada Central Standard Time",
            "Central America Standard Time",
            "Central Standard Time",
            "Eastern Standard Time",
            "SA Pacific Standard Time",
            "US Eastern Standard Time",
            "Venezuela Standard Time",
            "Atlantic Standard Time",
            "Central Brazilian Standard Time",
            "Pacific SA Standard Time",
            "Paraguay Standard Time",
            "SA Western Standard Time",
            "Newfoundland Standard Time",
            "Argentina Standard Time",
            "Bahia Standard Time",
            "Greenland Standard Time",
            "E. South America Standard Time",
            "Montevideo Standard Time",
            "SA Eastern Standard Time",
            "UTC-02",
            "(UTC - 01:00) Azores Standard Time",
            "(UTC - 01:00) Cape Verde Standard Time",
            "(UTC) GMT Standard Time",
            "(UTC) Greenwich Standard Time",
            "(UTC) Morocco Standard Time",
            "(UTC) UTC",
            "Magadan Standard Time",
            "New Zealand Standard Time",
            "Russia Time Zone 11",
            "UTC+12",
            "Line Islands Standard Time",
            "Tonga Standard Time",
            };
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (appointment == null)
            {
                var scheduleAppointment = new ScheduleAppointment();
                scheduleAppointment.Subject = this.Subject.Text;
                scheduleAppointment.StartTime = this.StartDatePicker.Value.Value.Date.Add(this.StartTimePicker.Value.Value.TimeOfDay);
                scheduleAppointment.EndTime = this.EndDatePicker.Value.Value.Date.Add(this.EndTimePicker.Value.Value.TimeOfDay);
                scheduleAppointment.Location = this.location.Text;
                scheduleAppointment.IsAllDay = (bool)this.allDay.IsChecked;
                scheduleAppointment.Notes = this.description.Text;
                scheduleAppointment.Reminders=(ObservableCollection<SchedulerReminder>)this.ReminderList.ItemsSource;
                
                if ((bool)this.timeZone.IsChecked)
                {
                    scheduleAppointment.StartTimeZone = this.TimeZoneMenu.Text;
                    scheduleAppointment.EndTimeZone = this.TimeZoneMenu.Text;
                }
                if (this.scheduler.ItemsSource == null)
                {
                    this.scheduler.ItemsSource = new ScheduleAppointmentCollection();
                }

                (this.scheduler.ItemsSource as ScheduleAppointmentCollection).Add(scheduleAppointment);
            }
            else
            {
                appointment.Subject = this.Subject.Text;
                appointment.StartTime = this.StartDatePicker.Value.Value.Date.Add(this.StartTimePicker.Value.Value.TimeOfDay);
                appointment.EndTime = this.EndDatePicker.Value.Value.Date.Add(this.EndTimePicker.Value.Value.TimeOfDay);
                appointment.Location = this.location.Text;
                appointment.IsAllDay = (bool)this.allDay.IsChecked;
                appointment.Notes = this.description.Text;
                appointment.Reminders = (ObservableCollection<SchedulerReminder>)this.ReminderList.ItemsSource;
                appointment.StartTimeZone = this.TimeZoneMenu.Text;
                appointment.EndTimeZone = this.TimeZoneMenu.Text;
            }
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.Save.Click -= this.Save_Click;
            this.Cancel.Click -= this.Cancel_Click;
            this.scheduler = null;
            this.appointment = null;
        }

        private void addRememainder_Click(object sender, RoutedEventArgs e)
        {

            if (this.ReminderList != null)
            {
                var reminders = this.ReminderList.ItemsSource as IList;
                this.ReminderList.ItemContainerGenerator.StatusChanged += this.OnListViewItemGeneratorStatusChanged;
                if (reminders == null)
                {
                    reminders = new ObservableCollection<SchedulerReminder>();
                }
                else if (reminders.Count == 5)
                {
                    // Only maximum of 5 reminders allowed in editor window.
                    return;
                }
                var newRemainder = new SchedulerReminder();
                reminders.Add(newRemainder);
                this.ReminderList.ItemsSource = reminders;
            }
        }
        private void OnListViewItemGeneratorStatusChanged(object sender, EventArgs e)
        {
            foreach (var reminder in this.ReminderList.Items)
            {
                var listViewItem = this.ReminderList.ItemContainerGenerator.ContainerFromItem(reminder) as ListViewItem;
                if (listViewItem == null)
                {
                    continue;
                }

                //// Sets the reminder interval types.
                var reminderTimeIntervalMenu = VisualUtils.FindDescendant(listViewItem, typeof(ComboBoxAdv)) as ComboBoxAdv;
                if (reminderTimeIntervalMenu != null)
                {
                    reminderTimeIntervalMenu.ItemsSource = new List<string>()
                    {
                        SchedulerLocalizationResourceAccessor.Instance.GetString("MinutesIntervalTypeReminder", CultureInfo.CurrentUICulture),
                        SchedulerLocalizationResourceAccessor.Instance.GetString("HoursIntervalTypeReminder", CultureInfo.CurrentUICulture),
                        SchedulerLocalizationResourceAccessor.Instance.GetString("DaysIntervalTypeReminder", CultureInfo.CurrentUICulture),
                        SchedulerLocalizationResourceAccessor.Instance.GetString("WeeksIntervalTypeReminder", CultureInfo.CurrentUICulture),
                    };
                }
            }
        }
        
    private void RemoveReminder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var reminderCollection = this.ReminderList.ItemsSource as IList;
            reminderCollection.Remove(button.DataContext as SchedulerReminder);
        }

        private void timeZone_Checked(object sender, RoutedEventArgs e)
        {
            if(this.timeZone.IsChecked==true)
            this.TimeZoneMenuPanel.Visibility = Visibility.Visible;
            else
                this.TimeZoneMenuPanel.Visibility = Visibility.Visible;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if(appointment != null)
            (this.scheduler.ItemsSource as ScheduleAppointmentCollection).Remove(appointment);
            this.Close();
        }
    }
}

