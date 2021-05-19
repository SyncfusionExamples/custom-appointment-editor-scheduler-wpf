using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using Syncfusion.UI.Xaml.Scheduler;
using WpfScheduler.Helper;

namespace WpfScheduler
{
    public class SchedulerBehavior : Behavior<Grid>
    {
        SfScheduler scheduler;
    
        protected override void OnAttached()
        {
            base.OnAttached();
            scheduler = this.AssociatedObject.FindName("Schedule") as SfScheduler;
            this.OnWirEvents();
        }

        private void OnWirEvents()
        {
            this.scheduler.AppointmentEditorOpening += Scheduler_AppointmentEditorOpening;
        }

        private void Scheduler_AppointmentEditorOpening(object sender, AppointmentEditorOpeningEventArgs e)
        {
          e.Cancel = true;
           var editor = new AppointmentEditor(this.scheduler, e.Appointment, e.DateTime);
           editor.ShowDialog();
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.UnWireEvents();
            this.scheduler = null;
        }

        private void UnWireEvents()
        {
            this.scheduler.AppointmentEditorOpening -= Scheduler_AppointmentEditorOpening;
        }
    }
}