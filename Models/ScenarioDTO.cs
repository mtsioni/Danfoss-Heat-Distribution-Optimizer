using System.Collections.Generic;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class ScenarioDTO
    {
        private List<string> _nameList = new();
        public List<string> NameList
        {
            get {return _nameList;}
            set {_nameList = value;}
        }
        public List<GenericUnit> GetScenario(List<GenericUnit> inputUnits)
        {
            /*
            List<GenericUnit> outputUnits = new();
            bool belongs;
            foreach (GenericUnit unit in inputUnits)
            {
                belongs = false;
                foreach (string name in _nameList)
                {
                    if (unit.Name == name)
                        belongs = true;
                    if (belongs)
                        break;
                }
                if (belongs)
                    outputUnits.Add(unit);
            }
            return outputUnits;
            */
            List<GenericUnit> outputUnits = inputUnits.FindAll(unit => _nameList.Contains(unit.Name));
            return outputUnits;
        }
    }
}