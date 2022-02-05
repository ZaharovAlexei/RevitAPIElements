using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIElements
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public List<FamilySymbol> ElementsList { get; } = new List<FamilySymbol>();
        public DelegateCommand BuildCommand { get; }
        public int ElementCount { get; set; } = 2;
        public FamilySymbol SelectedElement { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            ElementsList = FamilySymbolUtils.GetFamilySymbols(commandData);
            BuildCommand = new DelegateCommand(OnBuildCommand);
        }

        private void OnBuildCommand()
        {
            Document doc = _commandData.Application.ActiveUIDocument.Document;
            if (ElementCount < 2)
                return;
            Level level = LevelsUtils.GetLevels(_commandData).FirstOrDefault();

            RaiseCloseRequest();
            XYZ firstPoint = GetPoint();
            XYZ secondPoint = GetPoint("Выберите вторую точку");
            XYZ stepPoint = (secondPoint - firstPoint) / (ElementCount - 1);

            for (int i = 0; i < ElementCount; i++)
            {
                XYZ point = firstPoint + stepPoint * i;
                FamilyInstanceUtils.CreateFamilyInstance(_commandData, SelectedElement, point, level);
            }

        }

        private XYZ GetPoint(string message = "Выберите точку")
        {
            UIDocument uidoc = _commandData.Application.ActiveUIDocument;
            XYZ point = uidoc.Selection.PickPoint(ObjectSnapTypes.Endpoints, message);
            return point;
        }

        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
