using System.Data;
using System.Windows;
#region #usings
using System.Data.OleDb;
using DevExpress.XtraScheduler;
#endregion #usings

namespace WpfApplication1 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        CarsDBDataSet dataSet;
        CarsDBDataSetTableAdapters.CarSchedulingTableAdapter adapter;

        public MainWindow() {
            InitializeComponent();
            this.dataSet = new CarsDBDataSet();

            // Bind the scheduler storage to appointment data.
            this.CarSchedulingControl.Storage.AppointmentStorage.DataSource = dataSet.CarScheduling;

            // Load data into the 'CarsDBDataSet.CarScheduling' table. 
            this.adapter = new CarsDBDataSetTableAdapters.CarSchedulingTableAdapter();
            this.adapter.Fill(dataSet.CarScheduling);

            this.CarSchedulingControl.Storage.AppointmentsInserted +=
                new PersistentObjectsEventHandler(Storage_AppointmentsModified);
            this.CarSchedulingControl.Storage.AppointmentsChanged +=
                new PersistentObjectsEventHandler(Storage_AppointmentsModified);
            this.CarSchedulingControl.Storage.AppointmentsDeleted +=
                new PersistentObjectsEventHandler(Storage_AppointmentsModified);

            this.adapter.Adapter.RowUpdated +=
                new System.Data.OleDb.OleDbRowUpdatedEventHandler(adapter_RowUpdated);

            // Bind the scheduler storage to resource data.  
            this.CarSchedulingControl.Storage.ResourceStorage.DataSource = dataSet.Cars;

            // Load data into the 'CarsDBDataSet.Cars' table.  
            CarsDBDataSetTableAdapters.CarsTableAdapter carsAdapter = new CarsDBDataSetTableAdapters.CarsTableAdapter();
            carsAdapter.Fill(dataSet.Cars);
        }
        void Storage_AppointmentsModified(object sender, PersistentObjectsEventArgs e)
        {
            this.adapter.Adapter.Update(this.dataSet);
            this.dataSet.AcceptChanges();
        }

        private void adapter_RowUpdated(object sender, System.Data.OleDb.OleDbRowUpdatedEventArgs e)
        {
            if (e.Status == UpdateStatus.Continue && e.StatementType == StatementType.Insert)
            {
                int id = 0;
                using (OleDbCommand cmd = new OleDbCommand("SELECT @@IDENTITY", adapter.Connection))
                {
                    id = (int)cmd.ExecuteScalar();
                }
                e.Row["ID"] = id;
            }
        }

        #region #editappointmentformshowing
        private void scheduler_EditAppointmentFormShowing(object sender, DevExpress.Xpf.Scheduler.EditAppointmentFormEventArgs e) {
            e.Form = new CustomAppointmentForm(CarSchedulingControl, e.Appointment);
        }
        #endregion #editappointmentformshowing
    }
}
