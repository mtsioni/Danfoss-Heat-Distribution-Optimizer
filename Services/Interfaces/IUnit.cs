namespace Danfoss_Heat_Distribution_Optimizer.Services.Interfaces
{
    public interface IUnit
    {
        int UnitID {get; set;}
        int GetUnitID();
        void SetUnitID(int unitID);
        string Name{get; set;}
        string GetName();
        void SetName(string name);
    }
}