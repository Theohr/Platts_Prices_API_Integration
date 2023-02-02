using System;
using System.Collections.Generic;
using Danaos.Shared;
using Danaos.Contracts;
using System.Runtime.Serialization;

namespace Danaos.TRD.Models
{


    [Serializable()]
    public class PlattsPricesViewModel : InstanceVariables, ISerializable, IWindow
    {
        public PlattsPricesViewModel()
        {

        }
        public string ViewName { get; set; }
        public string WindowTitleName { get; set; }
        public string RibbonToolbarName { get; set; }
        public bool AllowMultipleFlag { get; set; }
        public string WindowOptionsName { get; set; }

        public string JsComponentName { get; set; }

        public object FrontEndArgs { get; set; }

        public string WindowApplication() { return "TRD"; }
        public string WindowName() { return "PlattsPrices"; }
        public string WindowTitle() { return "Platts Prices"; }
        public string WindowMenu() { return "PlattsPrices"; }
        public string RibbonToolbar() { return null; }
        public bool AllowMultiple() { return AllowMultipleFlag; }
        public string WindowIcon() { return ""; }
        public string WindowOptions() { return WindowOptionsName; }
        public bool CheckTask(string TaskID) { return DanaosApi.CheckTask(TaskID, WindowName(), WindowApplication()); }

        public List<WindowTask> TasksList
        {
            get
            {
                return new List<WindowTask>();
            }
        }
        public PlattsPricesViewModel(SerializationInfo info, StreamingContext ctxt)
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {

        }
    }



}