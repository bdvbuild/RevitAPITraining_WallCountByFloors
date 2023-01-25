using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;
using Document = Autodesk.Revit.DB.Document;

namespace RevitAPITraining_WallCountByFloors
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<Wall> walls = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .Cast<Wall>()
                .ToList();

            var wallCountByLevel = new Dictionary<string, int>();

            foreach (var wall in walls)
            {
                var level = doc.GetElement(wall.LevelId) as Level;
                if (!wallCountByLevel.ContainsKey(level.Name))
                {
                    wallCountByLevel.Add(level.Name, 0);
                }
                wallCountByLevel[level.Name]++;
            }

            var resultsString = string.Empty;

            foreach (var e in wallCountByLevel)
            {
                resultsString += $"Уровень {e.Key}: {e.Value}\n";
            }

            TaskDialog.Show("Несущие стены", resultsString);


            return Result.Succeeded;
        }
    }
}
