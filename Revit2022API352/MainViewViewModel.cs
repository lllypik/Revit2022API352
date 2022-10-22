using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Prism.Commands;
using RevitAPILibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Revit2022API352
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand ApplyCommand { get; }

        public DelegateCommand SaveCommand { get; private set; }
        public List<Element> PickedObjects { get; private set; } = new List<Element>();
        public List<WallType> WallSystems { get; private set; } = new List<WallType>();
        public WallType SelectedWallSystems { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            ApplyCommand = new DelegateCommand(OnApplyCommand);
            PickedObjects = SelectionUtils.PickObjects(commandData);
            WallSystems = WallData.GetWallType(commandData);

        }

        private void OnApplyCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uIDocument = uiapp.ActiveUIDocument;
            Document document = uIDocument.Document;

            if (PickedObjects.Count == 0 || SelectedWallSystems == null)
            {
                return;
            }

            using (Transaction transaction = new Transaction(document, "Установить тип системы"))
            {
                transaction.Start();
                foreach (var pickedObject in PickedObjects)
                {
                    if (pickedObject is Wall)
                    {
                        var oWall = pickedObject as Wall;
                        oWall.ChangeTypeId(SelectedWallSystems.Id);
                    }
                }

                transaction.Commit();
            }

            RaiseHideRequest();

        }

        public event EventHandler HideRequest;
        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }


        public event EventHandler ShowRequest;
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CloseReqest;
        private void ReiseCloseReqest()
        {
            CloseReqest?.Invoke(this, EventArgs.Empty);
        }


    }
}
